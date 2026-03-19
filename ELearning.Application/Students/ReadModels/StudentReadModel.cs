// <copyright file="StudentReadModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Students.ReadModels;

public sealed record StudentReadModel(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? ProfilePictureUrl,
    DateTime? LastLoginDate)
{
    public string FullName => $"{this.FirstName} {this.LastName}".Trim();
}
