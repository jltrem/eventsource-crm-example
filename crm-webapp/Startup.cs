using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SimpleCQRS;
using CRM.Domain;

namespace CRM.Webapp
{
   public class Startup
   {
      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public IConfiguration Configuration { get; }

      private Domain.ContactCommandHandlers ContactCommandHandlers { get; set; }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddControllers();

         services.AddDbContext<EventStoreContext>(x => x.UseSqlServer(Configuration["ConnectionStrings:AggregateEventStore"]));

         services.AddSingleton<ICommandBus>(_ => new CommandBus());
         services.AddSingleton<Domain.ContactCommandHandlers>();
         services.AddSingleton<IUseRepo<Domain.Contact>>(x => new UseContactRepo(x));

         services.AddTransient<ITimeService>(_ => new TimeService(() => DateTimeOffset.UtcNow));
         services.AddTransient<ISecurityPrincipalService>(_ => new SecurityPrincipalService());
         
         services.AddScoped<IEventStore>(x => new EventStore(x.GetService<EventStoreContext>()));
         services.AddScoped<IRepository<Domain.Contact>>(x => new Repository<Domain.Contact>(x.GetService<IEventStore>()));
      }

      public class UseContactRepo : IUseRepo<Domain.Contact>
      {
         private readonly IServiceProvider _serviceProvider;

         public UseContactRepo(IServiceProvider serviceProvider)
         {
            _serviceProvider = serviceProvider;
         }

         public void UseRepo(Action<IRepository<Contact>> action)
         {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRepository<Domain.Contact>>();
            action(repo);
         }
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ContactCommandHandlers contactHandlers)
      {
         ContactCommandHandlers = contactHandlers;

         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }

         app.UseHttpsRedirection();

         app.UseRouting();

         app.UseAuthorization();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapControllers();
         });
      }
   }
}
