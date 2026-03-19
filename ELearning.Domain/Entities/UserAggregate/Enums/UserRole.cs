// <copyright file="UserRole.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.UserAggregate.Enums;

using ELearning.SharedKernel;

public sealed class UserRole : Enumeration
{
    public static UserRole Student = new UserRole(1, nameof(Student));

    public static UserRole Instructor = new UserRole(2, nameof(Instructor));

    public static UserRole Admin = new UserRole(3, nameof(Admin));

    private UserRole(int id, string name)
        : base(id, name)
    {
    }
}