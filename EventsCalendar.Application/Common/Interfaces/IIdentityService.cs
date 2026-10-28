using EventsCalendar.Application.Auth;

namespace EventsCalendar.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<AuthResult> RegisterAsync(string userName, string email, string phone, string firstName, string secondName, string password);
    Task<AuthResult> LoginAsync(string email, string password);
}