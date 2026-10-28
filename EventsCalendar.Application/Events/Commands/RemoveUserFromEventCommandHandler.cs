using MediatR;
using EventsCalendar.Application.Common.Interfaces;
using EventsCalendar.Domain.Exceptions;

namespace EventsCalendar.Application.Events.Commands;

public class RemoveUserFromEventCommandHandler : IRequestHandler<RemoveUserFromEventCommand, bool>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public RemoveUserFromEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork =  unitOfWork;
    }

    public async Task<bool> Handle(RemoveUserFromEventCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetEventByIdAsync(request.EventId, cancellationToken);
        
        if (eventEntity == null)
            throw new NotFoundException($"Event with id '{request.EventId}' was not found.");

        if (eventEntity.OwnerId != request.CurrentUserId)
            throw new ForbiddenException("Only the event owner can remove participants from this event.");
        
        if (request.TargetUserId == eventEntity.OwnerId)
            throw new InvalidOperationException("The event owner cannot remove themselves from the event.");
        
        var userEvent = await _eventRepository.GetUserEventByIdAsync(request.EventId, request.TargetUserId, cancellationToken);
        
        if (userEvent == null)
            throw new KeyNotFoundException("Participant not found in this event.");
        
        _eventRepository.RemoveUserFromEventAsync(userEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}