// <copyright file="EnrollmentStatus.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate.Enums;

using ELearning.SharedKernel;

public sealed class EnrollmentStatus : Enumeration
{
    public static EnrollmentStatus Active = new EnrollmentStatus(1, nameof(Active));
    public static EnrollmentStatus Paused = new EnrollmentStatus(2, nameof(Paused));
    public static EnrollmentStatus Completed = new EnrollmentStatus(3, nameof(Completed));
    public static EnrollmentStatus Abandoned = new EnrollmentStatus(4, nameof(Abandoned));

    private EnrollmentStatus(int id, string name)
        : base(id, name)
    {
    }
}