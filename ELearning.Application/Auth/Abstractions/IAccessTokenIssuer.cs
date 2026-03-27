// <copyright file="IAccessTokenIssuer.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Auth.Abstractions;

using ELearning.Domain.Entities.UserAggregate;

public interface IAccessTokenIssuer
{
    string IssueToken(User user);
}
