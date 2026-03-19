// <copyright file="ReadModelFallbackPolicy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Resilience;

using ELearning.Application.Common.Exceptions;

internal static class ReadModelFallbackPolicy
{
    public static bool ShouldFallback(Exception exception, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        var isInfrastructureFailure = exception is HttpRequestException
            or TimeoutException
            or TaskCanceledException
            or OperationCanceledException;

        return isInfrastructureFailure && exception is not DomainApplicationException;
    }
}
