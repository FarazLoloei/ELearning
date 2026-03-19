// <copyright file="CustomError.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.SharedKernel.Models;

public record CustomError(string Code, string Message)
{
    public static readonly CustomError None = new(string.Empty, string.Empty);

    public override string ToString() => $"{this.Code}: {this.Message}";
}