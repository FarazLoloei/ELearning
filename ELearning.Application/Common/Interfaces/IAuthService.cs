using ELearning.Application.Common.Model;
using ELearning.Domain.Entities.UserAggregate;

namespace ELearning.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(string email, string password);

    Task<AuthResult> RegisterStudentAsync(string firstName, string lastName, string email, string password);

    Task<AuthResult> RegisterInstructorAsync(string firstName, string lastName, string email, string password, string bio, string expertise);

    Task<string> GenerateJwtToken(User user);
}