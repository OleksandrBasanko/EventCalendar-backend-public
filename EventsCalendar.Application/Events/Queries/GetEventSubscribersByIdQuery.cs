using MediatR;
using EventsCalendar.Application.Common.Interfaces;
using EventsCalendar.Domain.Exceptions;

namespace EventsCalendar.Application.Events.Queries;

public record GetEventSubscribersByIdQuery(Guid EventId) : IRequest<List<EventSubscriberDto>>;

public class GetEventSubscribersByIdQueryQueryHandler : IRequestHandler<GetEventSubscribersByIdQuery, List<EventSubscriberDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;

    public GetEventSubscribersByIdQueryQueryHandler(IEventRepository eventRepository, IUserRepository userRepository)
    {
        _eventRepository = eventRepository;
        _userRepository = userRepository;
    }

    public async Task<List<EventSubscriberDto>> Handle(GetEventSubscribersByIdQuery request, CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetEventByIdAsync(request.EventId, cancellationToken);

        if (eventEntity == null)
            throw new NotFoundException($"Event with id '{request.EventId}' was not found.");

        var eventSubscribers = await _eventRepository.GetEventSubscribersByIdAsync(request.EventId, cancellationToken);

        var userIds = eventSubscribers.Select(ue => ue.UserId).Distinct();
        var users = await _userRepository.GetUsersByIdsAsync(userIds, cancellationToken);
        var usersById = users.ToDictionary(u => u.Id);

        return eventSubscribers
            .Where(ue => usersById.ContainsKey(ue.UserId))
            .Select(ue => new EventSubscriberDto(
                ue.UserId,
                usersById[ue.UserId].UserName,
                usersById[ue.UserId].FirstName,
                usersById[ue.UserId].SecondName,
                ue.EventId,
                ue.EventRole))
            .ToList();
    }
}