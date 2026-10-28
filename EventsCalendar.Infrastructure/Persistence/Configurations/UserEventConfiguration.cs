using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EventsCalendar.Domain.Entities;
using EventsCalendar.Infrastructure.Identity;

namespace EventsCalendar.Infrastructure.Persistence.Configurations;

public class UserEventConfiguration : IEntityTypeConfiguration<UserEvent>
{
    public void Configure(EntityTypeBuilder<UserEvent> builder)
    {
        builder.HasKey(ue => new { ue.UserId, ue.EventId });
        
        builder
            .HasOne<ApplicationUser>()
            .WithMany(u => u.UserEvents)
            .HasForeignKey(ue => ue.UserId);
            
        builder
            .HasOne(ue => ue.Event)
            .WithMany(e => e.UserEvents)
            .HasForeignKey(ue => ue.EventId);
    }
}
