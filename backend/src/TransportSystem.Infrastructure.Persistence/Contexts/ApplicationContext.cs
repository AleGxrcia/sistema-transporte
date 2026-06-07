using MediatR;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Core.Domain.Common;
using TransportSystem.Core.Domain.Fleet.Aggregates.Driver;
using TransportSystem.Core.Domain.Fleet.Aggregates.Vehicle;
using TransportSystem.Core.Domain.Transportation.Aggregates;

namespace TransportSystem.Infrastructure.Persistence.Contexts
{
    public class ApplicationContext : DbContext
    {
        private readonly IPublisher _publisher;

        public ApplicationContext(DbContextOptions<ApplicationContext> options, IPublisher publisher) 
            : base(options)
        {
            _publisher = publisher;
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TravelRequest> TravelRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            await PublishDomainEventsAsync(cancellationToken);
            return result;
        }

        private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
        {
            var aggregatesWithEvents = ChangeTracker
                .Entries<AggregateRoot<Guid>>()
                .Where(e => e.Entity.DomainEvents.Count != 0)
                .Select(e => e.Entity)
                .ToList();

            var allEvents = aggregatesWithEvents
                .SelectMany(a => a.DomainEvents)
                .ToList();

            // Limpiar antes de publicar para evitar loops si un handler genera más eventos
            aggregatesWithEvents.ForEach(a => a.ClearDomainEvents());

            foreach (var domainEvent in allEvents)
            {
                // IPublisher de MediatR — cada evento puede tener 0 o N handlers
                await _publisher.Publish((INotification)domainEvent, cancellationToken);
            }
        }
    }
}