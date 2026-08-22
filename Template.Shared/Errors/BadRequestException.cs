namespace Template.Shared.Errors;

public class BadRequestException(
    string code,
    string message)
    : AppException(code, message);