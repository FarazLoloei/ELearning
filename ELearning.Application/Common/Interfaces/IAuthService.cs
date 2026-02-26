using ELearning.Application.Common.Model;
using ELearning.Domain.Entities.UserAggregate;

namespace ELearning.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<AuthResult> RegisterStudentAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken = default);

    Task<AuthResult> RegisterInstructorAsync(string firstName, string lastName, string email, string password, string bio, string expertise, CancellationToken cancellationToken = default);

    Task<string> GenerateJwtToken(User user);
}
