// <copyright file="LoginInput.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.InputTypes;

public sealed class LoginInput
{
    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
