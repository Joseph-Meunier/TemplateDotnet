using Microsoft.EntityFrameworkCore;
using Template.Modules.Users.Data;
using Template.Shared.Errors;

namespace Template.Modules.Users.Features.GetUser;

public sealed class Handler(
    UsersDbContext dbContext)
{
    public async Task<Response> Handle(
        Guid id,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new Response(
                x.Id,
                x.Email,
                x.DisplayName,
                x.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(
                "users.not_found",
                "The requested user does not exist.");
        }

        return user;
    }
}