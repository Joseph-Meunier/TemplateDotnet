using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Template.Shared.Auth;

namespace Template.Api.Authentication;

public sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor)
    : ICurrentUser
{
    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException(
            "No active HTTP context.");

    public bool IsAuthenticated =>
        User.Identity?.IsAuthenticated == true;

    public string IdentityId =>
        User.FindFirstValue("sub")
        ?? throw new InvalidOperationException(
            "Authenticated user has no 'sub' claim.");
}