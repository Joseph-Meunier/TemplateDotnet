using Microsoft.EntityFrameworkCore;
using Template.Modules.Users.Data;
using Template.Modules.Users.Domain;
using Template.Shared.Auth;
using Template.Shared.Errors;

namespace Template.Modules.Users.Features.CreateUser;

public sealed class Handler(
    UsersDbContext dbContext,
    ICurrentUser currentUser)
{
    public async Task<Response> Handle(
        Request request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail =
            request.Email.Trim().ToLowerInvariant();

        var emailAlreadyExists = await dbContext.Users
            .AnyAsync(
                x => x.Email == normalizedEmail,
                cancellationToken);

        if (emailAlreadyExists)
        {
            throw new ConflictException(
                "users.email_already_exists",
                "A user already exists with this email.");
        }

        var identityAlreadyExists = await dbContext.Users
            .AnyAsync(
                x => x.IdentityId == currentUser.IdentityId,
                cancellationToken);

        if (identityAlreadyExists)
        {
            throw new ConflictException(
                "users.identity_already_exists",
                "A user profile already exists for this identity.");
        }

        var user = new User(
            currentUser.IdentityId,
            normalizedEmail,
            request.DisplayName);

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return new Response(
            user.Id,
            user.Email,
            user.DisplayName,
            user.CreatedAt);
    }
}