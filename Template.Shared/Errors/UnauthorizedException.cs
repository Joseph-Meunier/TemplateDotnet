namespace Template.Shared.Errors;

public class UnauthorizedException(
    string code,
    string message)
    : AppException(code, message);