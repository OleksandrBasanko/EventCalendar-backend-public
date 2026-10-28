using MediatR;
using EventsCalendar.Application.Common.Interfaces;
using EventsCalendar.Domain.Exceptions;

namespace EventsCalendar.Application.Events.Commands;

public class UpdateEventByIdCommandHandler : IRequestHandler<UpdateEventByIdCommand, bool>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEventByIdCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateEventByIdCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetEventByIdAsync(request.EventId, cancellationToken);
        if (eventEntity == null)
            throw new NotFoundException($"Event with id '{request.EventId}' was not found.");

        if (eventEntity.OwnerId != request.CurrentUser)
            throw new ForbiddenException("Only the event owner can update this event.");

        if (request.StartDate >= request.EndDate)
            throw new ValidationException("Start date must be earlier than end date.");

        eventEntity.Update(
            request.EventName,
            request.ShortDescription,
            request.DetailedDescription,
            DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc),
            DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc)
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}