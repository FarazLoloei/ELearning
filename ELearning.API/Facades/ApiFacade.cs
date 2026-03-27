// <copyright file="ApiFacade.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Facades;

using MediatR;

public sealed class ApiFacade(IMediator mediator) : IApiFacade
{
    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) =>
        mediator.Send(request, cancellationToken);
}
