using Microsoft.EntityFrameworkCore;
using Template.Modules.Users.Data;

namespace Template.Modules.Users.Contracts;

internal sealed class UserReader(
    UsersDbContext dbContext)
    : IUserReader
{
    public Task<bool> ExistsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.Users.AnyAsync(
            x => x.Id == userId,
            cancellationToken);
    }

    public Task<UserSummary?> GetByIdentityIdAsync(
        string identityId,
        CancellationToken cancellationToken)
    {
        return dbContext.Users
            .AsNoTracking()
            .Where(x => x.IdentityId == identityId)
            .Select(x => new UserSummary(
                x.Id,
                x.Email,
                x.DisplayName,
                x.Roles
                    .Select(r => r.Role)
                    .ToArray()))
            .SingleOrDefaultAsync(cancellationToken);
    }
}