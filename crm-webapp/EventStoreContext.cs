using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LanguageExt;
using static LanguageExt.Prelude;

namespace CRM.Webapp
{
   public class EventStoreContext : DbContext
   {
      public DbSet<AggregateEvent> AggregateEvents { get; set; }

      private readonly Map<string, int> _nameMaxLengthMap;

      public EventStoreContext(DbContextOptions<EventStoreContext> options) : base(options)
      {
         _nameMaxLengthMap = 
            new[]
            {
               (nameof(AggregateEvent.AggregateName), 40),
               (nameof(AggregateEvent.EventName), 40),
               (nameof(AggregateEvent.Owner), 40)
            }
            .Apply(toMap);
      }

      public Option<int> MaxLength(string propertyName) =>
         _nameMaxLengthMap.ContainsKey(propertyName)
            ? Some(_nameMaxLengthMap[propertyName])
            : None;

      public int UnsafeMaxLength(string propertyName) =>
         _nameMaxLengthMap[propertyName];

      protected override void OnModelCreating(ModelBuilder builder)
      {
         builder
            .Entity<AggregateEvent>()
            .HasKey(x => x.Id);

         builder
            .Entity<AggregateEvent>()
            .Property(p => p.Id)
            .HasColumnType("bigint");

         builder
            .Entity<AggregateEvent>()
            .HasIndex(x => new { x.RootId, x.AggregateVersion });

         builder.Entity<AggregateEvent>()
            .HasIndex(x => new { x.AggregateName });

         builder.Entity<AggregateEvent>()
            .HasIndex(x => new { x.Timestamp });

         builder.Entity<AggregateEvent>()
            .Property(x => x.AggregateName)
            .HasMaxLength(UnsafeMaxLength(nameof(AggregateEvent.AggregateName)));

         builder.Entity<AggregateEvent>()
            .Property(x => x.EventName)
            .HasMaxLength(UnsafeMaxLength(nameof(AggregateEvent.EventName)));

         builder.Entity<AggregateEvent>()
            .Property(x => x.Owner)
            .HasMaxLength(UnsafeMaxLength(nameof(AggregateEvent.Owner)));
      }
   }

   public class AggregateEvent
   {
      public long Id { get; set; }
      public Guid RootId { get; set; }     
      public int AggregateVersion { get; set; }
      public string AggregateName { get; set; }
      public string EventName { get; set; }
      public int EventVersion { get; set; }
      public string EventData { get; set; }
      public DateTimeOffset Timestamp { get; set; }
      public string Owner { get; set; }
   }
}
