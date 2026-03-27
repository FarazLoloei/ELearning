// <copyright file="AuthenticateUserCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Auth.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record AuthenticateUserCommand(string Email, string Password) : IRequest<AuthResult>;
