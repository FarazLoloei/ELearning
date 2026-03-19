// <copyright file="ApiError.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.API.Contracts;

public sealed record ApiError(string Code, string Message);
