namespace EventsCalendar.Application.Common.Interfaces;

public record UserDto(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string SecondName);