
namespace EventsCalendar.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<UserDto>> GetUsersByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
}