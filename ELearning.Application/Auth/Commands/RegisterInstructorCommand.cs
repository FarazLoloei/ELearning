// <copyright file="RegisterInstructorCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Auth.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record RegisterInstructorCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Bio,
    string Expertise) : IRequest<AuthResult>;
