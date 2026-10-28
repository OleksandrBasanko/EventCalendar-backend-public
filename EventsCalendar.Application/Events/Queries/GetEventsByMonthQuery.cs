using MediatR;
using EventsCalendar.Application.Common.Interfaces;

namespace EventsCalendar.Application.Events.Queries;

public record GetEventsByMonthQuery(int Year, int Month, Guid? UserId) : IRequest<List<EventDto>>;
public class GetEventsByMonthQueryHandler : IRequestHandler<GetEventsByMonthQuery, List<EventDto>>
{
    private readonly IEventRepository _eventRepository;

    public GetEventsByMonthQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }
    
    public async Task<List<EventDto>> Handle(GetEventsByMonthQuery request, CancellationToken cancellationToken)
    {
        var events = await _eventRepository.GetEventsByMonthAsync(request.Year, request.Month, cancellationToken);

        var dtos = events.Select(e => new EventDto(
            e.Id,
            e.EventName,
            e.ShortDescription,
            e.DetailedDescription,
            e.StartDate,
            e.EndDate,
            e.CreateAt,
            e.OwnerId,
            IsSubscribed: request.UserId.HasValue && e.UserEvents.Any(ue => ue.UserId == request.UserId.Value)
        )).ToList();

        return dtos;
    }
}