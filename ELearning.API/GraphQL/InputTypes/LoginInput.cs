// <copyright file="LoginInput.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.InputTypes;

public sealed class LoginInput
{
    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
