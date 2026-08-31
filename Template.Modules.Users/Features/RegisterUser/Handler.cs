using Microsoft.EntityFrameworkCore;
using Template.Modules.Users.Contracts;
using Template.Modules.Users.Data;
using Template.Modules.Users.Domain;
using Template.Shared.Errors;

namespace Template.Modules.Users.Features.RegisterUser;

public sealed class Handler(
    UsersDbContext dbContext,
    IIdentityProvider identityProvider)
{
    public async Task<Response> Handle(
        Request request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailAlreadyExists = await dbContext.Users
            .AnyAsync(x => x.Email == normalizedEmail, cancellationToken);

        if (emailAlreadyExists)
        {
            throw new ConflictException(
                "users.email_already_exists",
                "A user already exists with this email.");
        }

        var identityId = await identityProvider.CreateUserAsync(
            normalizedEmail,
            request.DisplayName,
            request.Password,
            cancellationToken);

        var user = new User(identityId, normalizedEmail, request.DisplayName);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new Response(user.Id, user.Email, user.DisplayName, user.CreatedAt);
    }
}
