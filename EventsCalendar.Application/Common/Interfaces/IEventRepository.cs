using EventsCalendar.Domain.Entities;

namespace EventsCalendar.Application.Common.Interfaces;

public interface IEventRepository
{
    Task<IReadOnlyList<Event>> GetEventsByMonthAsync(int year, int month, CancellationToken cancellationToken);
    Task AddNewEventAsync(Event eventEntity, UserEvent userEvent,CancellationToken cancellationToken);
    Task<Event?> GetEventByIdAsync(Guid id, CancellationToken cancellationToken);
    void SoftDeleteEventById(Event eventEntity);
    Task SubscribeForEventAsync(UserEvent userEvent, CancellationToken cancellationToken); 
    void UnsubscribeFromEventAsync(UserEvent userEvent, CancellationToken cancellationToken);
    void RemoveUserFromEventAsync(UserEvent userEvent, CancellationToken cancellationToken);
    Task<UserEvent> GetUserEventByIdAsync(Guid eventId, Guid userId, CancellationToken cancellationToken);
    Task<List<UserEvent>> GetEventSubscribersByIdAsync(Guid id, CancellationToken cancellationToken);
}