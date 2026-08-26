using Template.Modules.Users.Contracts;
using Template.Modules.Users.Domain;
using Template.Shared.Auth;
using Template.Shared.Errors;

namespace Template.Modules.Users.Authorization;

public sealed class UsersAuthorizationService(
        ICurrentUser currentUser,
        IUserReader userReader
)
{
        private async Task<UserSummary> GetCurrentUserAsync(
                CancellationToken cancellationToken)
        {
                var user = await userReader.GetByIdentityIdAsync(
                        currentUser.IdentityId,
                        cancellationToken);

                if (user is null)
                {
                        throw new ForbiddenException(
                                "users.profile_required",
                                "An application user profile is required.");
                }

                return user;
        }

        public async Task RequireAdminAsync(
                CancellationToken cancellationToken)
        {
                var user = await GetCurrentUserAsync(
                        cancellationToken);

                if (!user.Roles.Contains(UserRole.Admin))
                {
                        throw new ForbiddenException(
                                "users.admin_required",
                                "Admin role is required.");
                }
        }
}
