using Microsoft.EntityFrameworkCore;
using Template.Modules.Users.Data;
using Template.Modules.Users.Domain;
using Template.Shared.Errors;

namespace Template.Modules.Users.Features.CreateUser;

public sealed class Handler(
    UsersDbContext dbContext)
{
    public async Task<Response> Handle(
        Request request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail =
            request.Email.Trim().ToLowerInvariant();

        var alreadyExists = await dbContext.Users
            .AnyAsync(
                x => x.Email == normalizedEmail,
                cancellationToken);

        if (alreadyExists)
        {
            throw new ConflictException(
                "users.email_already_exists",
                "A user already exists with this email.");
        }

        var user = new User(
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