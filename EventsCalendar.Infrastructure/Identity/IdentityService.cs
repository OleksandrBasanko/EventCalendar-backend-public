using System.IdentityModel.Tokens.Jwt; // Для роботи з JWT токенами
using System.Security.Claims;          // Для створення Claims (даних про юзера)
using System.Text;                     // Для кодування рядків (Encoding)
using Microsoft.AspNetCore.Identity;   // Бібліотека Identity (робота з юзерами)
using Microsoft.Extensions.Options;    // Паттерн Options (читання appsettings.json)
using Microsoft.IdentityModel.Tokens;  // Для створення цифрових підписів
using EventsCalendar.Application.Auth;
using EventsCalendar.Application.Common.Interfaces;

namespace EventsCalendar.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtOptions _jwtOptions;
    
    public IdentityService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager; 
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResult> RegisterAsync(string userName, string email, string phone, string firstName, string secondName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            PhoneNumber = phone,
            FirstName = firstName,
            SecondName = secondName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return new AuthResult { Succeeded = false, Errors = result.Errors.Select(e => e.Description) };

        await _userManager.AddToRoleAsync(user, "user");

        return new AuthResult { Succeeded = true, Token = await GenerateJwtToken(user) };
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
            return new AuthResult { Succeeded = false, Errors = new[] { "Invalid email or password." } };

        return new AuthResult { Succeeded = true, Token = await GenerateJwtToken(user) };
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Any()) roles = new List<string> { "user" };

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()), 
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.UserName!)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim("role", role)); 
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}