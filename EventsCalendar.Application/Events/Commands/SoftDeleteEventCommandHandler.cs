using EventsCalendar.Application.Common.Interfaces;
using MediatR;
using EventsCalendar.Domain.Exceptions;

namespace EventsCalendar.Application.Events.Commands;

public class SoftDeleteEventByIdCommandHandler : IRequestHandler<SoftDeleteEventByIdCommand, bool>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteEventByIdCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork =  unitOfWork;
    }

    public async Task<bool> Handle(SoftDeleteEventByIdCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetEventByIdAsync(request.EventId, cancellationToken);
        
        if (eventEntity == null)
            throw new NotFoundException($"Event with id '{request.EventId}' was not found.");

        if (eventEntity.OwnerId != request.UserId)
            throw new ForbiddenException("Only the event owner can delete this event.");
        
        _eventRepository.SoftDeleteEventById(eventEntity);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}