using MediatR;
using EventsCalendar.Application.Common.Interfaces;
using EventsCalendar.Domain.Entities;
using EventsCalendar.Domain.Exceptions;

namespace EventsCalendar.Application.Events.Commands;

public class CreateNewEventCommandHandler : IRequestHandler<CreateNewEventCommand, Guid>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateNewEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork =  unitOfWork;
    }

    public async Task<Guid> Handle(CreateNewEventCommand request, CancellationToken cancellationToken)
    {
        if (request.StartDate >= request.EndDate)
            throw new ValidationException("Start date must be earlier than end date.");
        
        var newEvent = Event.Create(
            request.EventName,
            request.ShortDescription,
            request.DetailedDescription,
            DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc),
            DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc),
            request.CurrentUserId
        );

        var userEvent = UserEvent.Create(
            request.CurrentUserId,
            newEvent.Id,
            EventRole.EventOwner);
        
        await _eventRepository.AddNewEventAsync(newEvent, userEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return newEvent.Id;
    }
}