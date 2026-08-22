namespace Template.Shared.Errors;

public sealed class ConflictException(
    string code,
    string message)
    : AppException(code, message);