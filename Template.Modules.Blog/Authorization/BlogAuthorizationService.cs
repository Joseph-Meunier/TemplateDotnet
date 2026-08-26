using Template.Modules.Blog.Domain;
using Template.Modules.Users.Contracts;
using Template.Modules.Users.Domain;
using Template.Shared.Auth;
using Template.Shared.Errors;

namespace Template.Modules.Blog.Authorization;

public sealed class BlogAuthorizationService(
    ICurrentUser currentUser,
    IUserReader userReader)
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

    public async Task<UserSummary> RequireCreatorAsync(
        CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        
        var isCreator = user.Roles.Contains(UserRole.Creator);

        if (!isCreator)
        {
            throw new ForbiddenException(
                "blog.creator_required",
                "Creator role is required.");
        }

        return user;
    }

    public async Task RequireCanEditAsync(
        Post post,
        CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        
        var isCreator = user.Roles.Contains(UserRole.Creator);
        var canEdit = (isCreator &&
                       post.AuthorUserId == user.Id);
        if (!canEdit)
        {
            throw new ForbiddenException(
                "blog.post_edit_forbidden",
                "You cannot edit this post.");
        }
    }

    public async Task RequireCanDeleteAsync(
        Post post,
        CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);

        var canDelete =
            user.Roles.Contains(UserRole.Admin) ||
            (
                user.Roles.Contains(UserRole.Creator) &&
                post.AuthorUserId == user.Id
            );

        if (!canDelete)
        {
            throw new ForbiddenException(
                "blog.post_delete_forbidden",
                "You cannot delete this post.");
        }
    }

    public async Task RequireCanPublishAsync(
        Post post,
        CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);

        var canPublish =
            user.Roles.Contains(UserRole.Admin) ||
            (
                user.Roles.Contains(UserRole.Creator) &&
                post.AuthorUserId == user.Id
            );

        if (!canPublish)
        {
            throw new ForbiddenException(
                "blog.post_publish_forbidden",
                "You cannot change the publication state of this post.");
        }
    }
}