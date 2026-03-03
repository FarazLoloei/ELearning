using ELearning.API.Models;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using MediatR;
using ApplicationModel = ELearning.Application.Common.Model;

namespace ELearning.API.Facades;

public sealed class ApiFacade(IMediator mediator, IAuthService authService) : IApiFacade
{
    public Task<ApplicationModel.Result<T>> SendAsync<T>(IRequest<ApplicationModel.Result<T>> request, CancellationToken cancellationToken) =>
        mediator.Send(request, cancellationToken);

    public Task<Result> SendAsync(IRequest<Result> request, CancellationToken cancellationToken) =>
        mediator.Send(request, cancellationToken);

    public Task<AuthResult> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken) =>
        authService.AuthenticateAsync(request.Email, request.Password, cancellationToken);

    public Task<AuthResult> RegisterStudentAsync(RegisterStudentRequest request, CancellationToken cancellationToken) =>
        authService.RegisterStudentAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            cancellationToken);

    public Task<AuthResult> RegisterInstructorAsync(RegisterInstructorRequest request, CancellationToken cancellationToken) =>
        authService.RegisterInstructorAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.Bio,
            request.Expertise,
            cancellationToken);
}
