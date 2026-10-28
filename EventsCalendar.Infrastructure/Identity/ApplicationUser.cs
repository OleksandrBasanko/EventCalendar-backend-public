using Microsoft.AspNetCore.Identity;
using EventsCalendar.Domain.Entities;

namespace EventsCalendar.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string SecondName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Event> OwnedEvents { get; set; } = new List<Event>();
    public ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
}