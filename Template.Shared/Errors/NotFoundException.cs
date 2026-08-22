namespace Template.Shared.Errors;

public sealed class NotFoundException(
    string code,
    string message)
    : AppException(code, message);