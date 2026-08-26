using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Template.Modules.Users.Contracts;
using Template.Modules.Users.Data;

namespace Template.Modules.Users.Bootstrap;

public sealed class BootstrapAdminService(
    UsersDbContext dbContext,
    IOptions<BootstrapAdminOptions> options,
    ILogger<BootstrapAdminService> logger)
{
    public async Task RunAsync(
        CancellationToken cancellationToken = default)
    {
        var settings = options.Value;

        if (!settings.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(settings.IdentityId))
        {
            throw new InvalidOperationException(
                "BootstrapAdmin is enabled but IdentityId is missing.");
        }

        var user = await dbContext.Users
            .Include(x => x.Roles)
            .SingleOrDefaultAsync(
                x => x.IdentityId == settings.IdentityId,
                cancellationToken);

        if (user is null)
        {
            logger.LogWarning(
                "Bootstrap admin skipped: no user found for IdentityId {IdentityId}",
                settings.IdentityId);

            return;
        }

        if (!user.HasRole(UserRole.Admin))
        {
            user.AddRole(UserRole.Admin);
        }

        if (!user.HasRole(UserRole.Creator))
        {
            user.AddRole(UserRole.Creator);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Bootstrap admin role assigned to user {UserId}",
            user.Id);
    }
}