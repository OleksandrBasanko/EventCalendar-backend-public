using MediatR;
using EventsCalendar.Application.Common.Interfaces;
using EventsCalendar.Domain.Exceptions;

namespace EventsCalendar.Application.Events.Commands;

public class UnsubscribeFromEventCommandHandler : IRequestHandler<UnsubscribeFromEventCommand, bool>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UnsubscribeFromEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork =  unitOfWork;
    }
    
    public async Task<bool> Handle(UnsubscribeFromEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetEventByIdAsync(request.EventId, cancellationToken);

        if (eventEntity == null)
            throw new NotFoundException($"Event with id '{request.EventId}' was not found.");

        if (request.UserId == eventEntity.OwnerId)
            throw new InvalidOperationException("The event owner cannot unsubscribe from their own event.");
        
        var userEvent = await _eventRepository.GetUserEventByIdAsync(request.EventId, request.UserId, cancellationToken);
    
        if (userEvent == null)
            throw new NotFoundException("User is not subscribed to this event.");

        _eventRepository.UnsubscribeFromEventAsync(userEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}