// <copyright file="ApiError.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Contracts;

public sealed record ApiError(string Code, string Message);
