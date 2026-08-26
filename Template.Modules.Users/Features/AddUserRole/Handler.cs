using Microsoft.EntityFrameworkCore;
using Template.Modules.Users.Authorization;
using Template.Modules.Users.Contracts;
using Template.Modules.Users.Data;
using Template.Modules.Users.Domain;
using Template.Shared.Errors;

namespace Template.Modules.Users.Features.AddUserRole;

public class Handler(
    UsersDbContext dbContext,
    UsersAuthorizationService authorizationService)
{
        public async Task Handle(
        Guid id,
        UserRole role,
        CancellationToken cancellationToken)
    {
        await authorizationService.RequireAdminAsync(
            cancellationToken);

        var user = await dbContext.Users
            .Include(x => x.Roles)
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(
                "users.not_found",
                "The user was not found.");
        }

        user.AddRole(role);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}