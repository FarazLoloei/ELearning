namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Services;

public interface IUserService
{
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);

    string HashPassword(string password);

    Task<bool> VerifyPasswordAsync(string hashedPassword, string providedPassword);
}