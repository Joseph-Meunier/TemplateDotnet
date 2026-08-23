namespace Template.Modules.Users.Contracts;

public interface IUserReader
{
    Task<bool> ExistsAsync(
        Guid userId,
        CancellationToken cancellationToken);
}