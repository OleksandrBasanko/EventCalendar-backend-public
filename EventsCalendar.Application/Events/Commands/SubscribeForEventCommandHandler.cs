using MediatR;
using EventsCalendar.Application.Common.Interfaces;
using EventsCalendar.Domain.Entities;
using EventsCalendar.Domain.Exceptions;

namespace EventsCalendar.Application.Events.Commands;

public class SubscribeForEventCommandHandler : IRequestHandler<SubscribeForEventCommand, bool>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public SubscribeForEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork) 
    {
        _eventRepository = eventRepository;
        _unitOfWork =  unitOfWork;
    }

    public async Task<bool> Handle(SubscribeForEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetEventByIdAsync(request.EventId, cancellationToken);

        if (eventEntity == null)
            throw new NotFoundException($"Event with id '{request.EventId}' was not found.");

        var existingUserEvent = await _eventRepository.GetUserEventByIdAsync(request.EventId, request.UserId, cancellationToken);

        if (existingUserEvent != null)
            throw new InvalidOperationException("User is already subscribed to this event.");

        var userEvent = UserEvent.Create(
            request.UserId,
            request.EventId,
            EventRole.EventSubscriber);
        
        await _eventRepository.SubscribeForEventAsync(userEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}