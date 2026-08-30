using Template.Modules.Users.Authorization;
using Template.Modules.Users.Contracts;
using Template.Modules.Users.Data;
using Template.Shared.Errors;

namespace Template.Modules.Users.Features.DeleteUserRole;


public class Handler(
    UsersDbContext dbContext,
    UsersAuthorizationService authorizationService)
{
    public async Task Handle(
        Guid id,
        UserRole role,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FindAsync(id, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException(
                "users.not_found",
                "The user was not found.");
        }
        
        await authorizationService.RequireAdminAsync(cancellationToken);
        
        user.RemoveRole(role);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}