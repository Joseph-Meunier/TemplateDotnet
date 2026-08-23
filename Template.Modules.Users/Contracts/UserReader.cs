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
}