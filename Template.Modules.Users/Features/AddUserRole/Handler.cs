using Template.Modules.Users.Authorization;
using Template.Modules.Users.Data;
using Template.Shared.Auth;
using Template.Shared.Errors;

namespace Template.Modules.Users.Features.AddUserRole;

public class Handler(
    UsersDbContext dbContext,
    ICurrentUser currentUser,
    UsersAuthorizationService authorizationService)
{
    public async Task Handle(
        Guid id,
        Request request,
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
        
        user.AddRole(request.Role);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}