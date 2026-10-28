using MediatR;
using EventsCalendar.Application.Common.Interfaces;
using EventsCalendar.Domain.Exceptions;

namespace EventsCalendar.Application.Events.Queries;

public record GetEventByIdQuery(Guid Id, Guid? UserId) : IRequest<EventDto>;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto>
{
    private readonly IEventRepository _eventRepository;

    public GetEventByIdQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetEventByIdAsync(request.Id, cancellationToken);

        if (eventEntity == null) { throw new NotFoundException($"Event with id '{request.Id}' was not found."); }

        return new EventDto(
            eventEntity.Id,
            eventEntity.EventName,
            eventEntity.ShortDescription,
            eventEntity.DetailedDescription,
            eventEntity.StartDate,
            eventEntity.EndDate,
            eventEntity.CreateAt,
            eventEntity.OwnerId,
            IsSubscribed: request.UserId.HasValue && eventEntity.UserEvents.Any(ue => ue.UserId == request.UserId.Value)
        );
    }
}
