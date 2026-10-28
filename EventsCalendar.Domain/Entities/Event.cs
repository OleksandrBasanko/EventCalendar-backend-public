namespace EventsCalendar.Domain.Entities;

public class Event
{
    public Guid Id { get; private set; }
    public string EventName { get; private set; } = string.Empty;
    public string ShortDescription { get; private set; } = string.Empty;
    public string DetailedDescription { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime CreateAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public Guid OwnerId { get; private set; }

    public ICollection<UserEvent> UserEvents { get; private set; } = new List<UserEvent>();

    private Event(Guid id, string eventName, string shortDescription,
        string detailedDescription, DateTime startDate, DateTime endDate, Guid ownerId)
    {
        Id = id;
        EventName = eventName;
        ShortDescription = shortDescription;
        DetailedDescription = detailedDescription;
        StartDate = startDate;
        EndDate = endDate;
        CreateAt = DateTime.UtcNow;
        OwnerId = ownerId;
        IsDeleted = false;
    }

    public static Event Create(string eventName, string shortDescription,
        string detailedDescription, DateTime startDate, DateTime endDate, Guid ownerId)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            throw new ArgumentException("Event name can't be empty.");
        if (string.IsNullOrWhiteSpace(shortDescription))
            throw new ArgumentException("Short description can't be empty.");
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be earlier than end date.");

        return new Event(Guid.NewGuid(), eventName, shortDescription,
            detailedDescription, startDate, endDate, ownerId);
    }

    public void Update(string eventName, string shortDescription,
        string detailedDescription, DateTime startDate, DateTime endDate)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            throw new ArgumentException("Event name can't be empty.");
        if (string.IsNullOrWhiteSpace(shortDescription))
            throw new ArgumentException("Short description can't be empty.");
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be earlier than end date.");

        EventName = eventName;
        ShortDescription = shortDescription;
        DetailedDescription = detailedDescription;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}