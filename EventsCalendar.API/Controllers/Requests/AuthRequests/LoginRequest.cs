namespace EventsCalendar.API.Controllers.Requests.AuthRequests;

public record LoginRequest(
    string Email, 
    string Password);