using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CRM.Webapp
{
   public class EventStoreContext : DbContext
   {
      public DbSet<AggregateEvent> AggregateEvents { get; set; }

      public EventStoreContext(DbContextOptions<EventStoreContext> options) : base(options)
      {
      }

      protected override void OnModelCreating(ModelBuilder builder)
      {
         builder.Entity<AggregateEvent>()
            .HasKey(x => new { x.RootId, x.AggregateVersion });

         builder.Entity<AggregateEvent>()
            .HasIndex(x => new { x.AggregateName });

         builder.Entity<AggregateEvent>()
            .HasIndex(x => new { x.Timestamp });
      }
   }

   public class AggregateEvent
   {
      public Guid RootId { get; set; }
      
      public int AggregateVersion { get; set; }

      [MaxLength(40)]
      public string AggregateName { get; set; }

      [MaxLength(40)]
      public string EventName { get; set; }

      public int EventVersion { get; set; }

      public string DataType { get; set; }

      public string Data { get; set; }

      public DateTimeOffset Timestamp { get; set; }

      public string Owner { get; set; }
   }
}
