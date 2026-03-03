using ELearning.Application.Common.Model;
using ELearning.API.Models;
using MediatR;
using ApplicationModel = ELearning.Application.Common.Model;
using Result = ELearning.Application.Common.Model.Result;

namespace ELearning.API.Facades;

public interface IApiFacade
{
    Task<ApplicationModel.Result<T>> SendAsync<T>(IRequest<ApplicationModel.Result<T>> request, CancellationToken cancellationToken);

    Task<Result> SendAsync(IRequest<Result> request, CancellationToken cancellationToken);

    Task<AuthResult> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken);

    Task<AuthResult> RegisterStudentAsync(RegisterStudentRequest request, CancellationToken cancellationToken);

    Task<AuthResult> RegisterInstructorAsync(RegisterInstructorRequest request, CancellationToken cancellationToken);

    Task<AuthResult> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken);

    Task<Result> RevokeTokenAsync(RevokeTokenRequest request, CancellationToken cancellationToken);
}
