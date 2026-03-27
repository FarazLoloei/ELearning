// <copyright file="RegisterInstructorInput.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.InputTypes;

public sealed class RegisterInstructorInput : RegisterStudentInput
{
    public string Bio { get; init; } = string.Empty;

    public string Expertise { get; init; } = string.Empty;
}
