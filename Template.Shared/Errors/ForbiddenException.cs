namespace Template.Shared.Errors;

public sealed class ForbiddenException(
    string code,
    string message)
    : AppException(code, message);