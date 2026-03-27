// <copyright file="RefreshAuthTokenCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Auth.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record RefreshAuthTokenCommand(string RefreshToken) : IRequest<AuthResult>;
