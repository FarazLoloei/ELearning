// <copyright file="IApiFacade.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Facades;

using ELearning.API.Models;
using ELearning.Application.Common.Model;
using MediatR;
using ApplicationModel = ELearning.Application.Common.Model;
using Result = ELearning.Application.Common.Model.Result;

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
