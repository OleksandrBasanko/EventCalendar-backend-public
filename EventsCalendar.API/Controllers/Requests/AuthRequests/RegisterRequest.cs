namespace EventsCalendar.API.Controllers.Requests.AuthRequests;

public record RegisterRequest(
    string UserName, 
    string Email, 
    string Phone, 
    string FirstName, 
    string SecondName, 
    string Password);