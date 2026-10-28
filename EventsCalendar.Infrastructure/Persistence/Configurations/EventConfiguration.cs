using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EventsCalendar.Domain.Entities;
using EventsCalendar.Infrastructure.Identity;

namespace EventsCalendar.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder
            .HasOne<ApplicationUser>() 
            .WithMany(u => u.OwnedEvents)
            .HasForeignKey(e => e.OwnerId) 
            .OnDelete(DeleteBehavior.Restrict);
    }
}