using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using EventsCalendar.Application.Events.Queries;
using EventsCalendar.Application.Events.Commands;
using System.Security.Claims;
using EventsCalendar.API.Controllers.Requests.EventsRequests;

namespace EventsCalendar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    private Guid? CurrentUserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : null;

    /// <summary>
    /// Get the list of events for the specified month and year
    /// </summary>
    /// <response code="200">Returns the list of events for the specified month</response>
    [HttpGet]
    public async Task<IActionResult> GetEventsByMonth(int year, int month, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEventsByMonthQuery(year, month, CurrentUserId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get detailed information about an event by id
    /// </summary>
    /// <param name="id">Event id</param>
    /// <response code="200">Returns the event details</response>
    /// <response code="404">Event was not found</response>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEventById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEventByIdQuery(id, CurrentUserId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get a list of users subscribed to a specific event by id
    /// </summary>
    /// <param name="id">The unique id of the event</param>
    /// <response code="200">Returns List UserEvent of subscribers for the specified event</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not an admin</response>
    /// <response code="404">Event was not found</response>
    [Authorize(Roles = "admin")]
    [HttpGet("{id:guid}/subscribers")]
    public async Task<IActionResult> GetEventSubscribersById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEventSubscribersByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create a new event (admin only)
    /// </summary>
    /// <param name="request">New event data</param>
    /// <response code="400">Start date is not earlier than the end date</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not an admin</response>
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateNewEventCommand(CurrentUserId!.Value, request.EventName, request.ShortDescription, 
            request.DetailedDescription, request.StartDate, request.EndDate), 
            cancellationToken);
        return CreatedAtAction(nameof(GetEventById), new {id = result}, result);
    }

    /// <summary>
    /// Soft Delete the event by id (owner only)
    /// </summary>
    /// <param name="id">The unique id of the event</param>
    /// <response code="200">Return true (Event deleted successfully)</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not the event owner</response>
    /// <response code="404">Event was not found</response>
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> SoftDeleteEventById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SoftDeleteEventByIdCommand(id, CurrentUserId!.Value), cancellationToken);
        return Ok(result);
    }
    
    /// <summary>
    /// Subscribe the current user to an event by event id
    /// </summary>
    /// <param name="id"> id of the event to subscribe to</param>
    /// <response code="200">Return true (Subscribed successfully)</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Event was not found</response>
    /// <response code="409">User is already subscribed to this event</response>
    [Authorize]
    [HttpPost("{id:guid}/subscribe")]
    public async Task<IActionResult> SubscribeForEvent(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SubscribeForEventCommand(id, CurrentUserId!.Value), cancellationToken);
        return Ok(result);
    }
    
    /// <summary>
    /// Unsubscribe the current user from an event by event id
    /// </summary>
    /// <param name="id">Identifier of the event to unsubscribe from</param>
    /// <response code="200">Return true (Unsubscribed successfully)</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Event was not found, or the user is not subscribed to it</response>
    [Authorize]
    [HttpDelete("{id:guid}/unsubscribe")]
    public async Task<IActionResult> UnsubscribeFromEvent(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UnsubscribeFromEventCommand(id, CurrentUserId!.Value), cancellationToken);
        return Ok(result);
    }
    
    /// <summary>
    /// Remove a user (by user id) from an event (by event id)
    /// </summary>
    /// <param name="eventId">Event identifier</param>
    /// <param name="targetUserId">Identifier of the user to remove from the event</param>
    /// <response code="200">Return true (Participant removed successfully)</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not the event owner</response>
    /// <response code="404">Event was not found, or the participant was not found in this event</response>
    [Authorize]
    [HttpDelete("{eventId:guid}/participants/{targetUserId:guid}")]
    public async Task<IActionResult> RemoveUserFromEvent(Guid eventId, Guid targetUserId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RemoveUserFromEventCommand(eventId, targetUserId, CurrentUserId!.Value), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Completely update information about event by id
    /// </summary>
    /// <param name="request">Updated event data</param>
    /// <response code="200">Return true (Event updated successfully)</response>
    /// <response code="400">Start date is not earlier than the end date</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not the event owner</response>
    /// <response code="404">Event was not found</response>
    [Authorize]
    [HttpPut("{id:guid}/edit_event")]
    public async Task<IActionResult> UpdateEventById([FromBody] UpdateEventRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateEventByIdCommand(
                CurrentUserId!.Value, request.EventId, request.EventName, request.ShortDescription, request.DetailedDescription,
                request.StartDate, request.EndDate),
            cancellationToken);
        return Ok(result);
    }
}