using EventsCalendar.Application.Common.Interfaces;
using EventsCalendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventsCalendar.Infrastructure.Persistence.Repositories;

public class EventRepository : IEventRepository
{
    private readonly AppDbContext _context;

    public EventRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Event>> GetEventsByMonthAsync(int year, int month, CancellationToken cancellationToken)
    {
        return await _context.Events
            .Include(e => e.UserEvents)
            .Where(e => e.StartDate.Year == year && e.StartDate.Month == month && !e.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserEvent> GetUserEventByIdAsync(Guid eventId, Guid userId, CancellationToken cancellationToken)
    {
        return await _context.UserEvents.FirstOrDefaultAsync(e => e.EventId == eventId && e.UserId == userId, cancellationToken);;
    }

    public async Task<List<UserEvent>> GetEventSubscribersByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.UserEvents
            .Where(e => e.EventId == id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddNewEventAsync(Event eventEntity, UserEvent userEvent, CancellationToken cancellationToken)
    {
        await _context.Events.AddAsync(eventEntity, cancellationToken);
        await _context.UserEvents.AddAsync(userEvent, cancellationToken);
    }

    public async Task<Event?> GetEventByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Events
            .Include(e => e.UserEvents)
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    }

    public void SoftDeleteEventById (Event eventEntity)
    {
        eventEntity.SoftDelete();
        _context.Events.Update(eventEntity);
    }

    public async Task SubscribeForEventAsync(UserEvent userEvent, CancellationToken cancellationToken)
    {
        await _context.UserEvents.AddAsync(userEvent, cancellationToken);
    }

    public void UnsubscribeFromEventAsync(UserEvent userEvent, CancellationToken cancellationToken)
    {
        _context.UserEvents.Remove(userEvent);
    }

    public void RemoveUserFromEventAsync(UserEvent userEvent, CancellationToken cancellationToken) 
    {
        _context.UserEvents.Remove(userEvent);
    }
}