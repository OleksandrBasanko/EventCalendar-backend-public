namespace EventsCalendar.Application.Auth;

public class AuthResult
{
    public bool Succeeded { get; set; }
    public string? Token { get; set; }
    public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
}