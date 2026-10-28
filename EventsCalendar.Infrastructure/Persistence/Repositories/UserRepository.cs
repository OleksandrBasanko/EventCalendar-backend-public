using Microsoft.EntityFrameworkCore;
using EventsCalendar.Application.Common.Interfaces;

namespace EventsCalendar.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return new UserDto(user.Id, user.UserName, user.Email, user.FirstName, user.SecondName);
    }

    public async Task<List<UserDto>> GetUsersByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var idList = ids.ToList();

        return await _context.Users
            .Where(u => idList.Contains(u.Id))
            .Select(u => new UserDto(u.Id, u.UserName, u.Email, u.FirstName, u.SecondName))
            .ToListAsync(cancellationToken);
    }
}