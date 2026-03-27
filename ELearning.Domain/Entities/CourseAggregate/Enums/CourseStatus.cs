// <copyright file="CourseStatus.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate.Enums;

using ELearning.SharedKernel;

public sealed class CourseStatus : Enumeration
{
    public static readonly CourseStatus Draft = new CourseStatus(1, nameof(Draft));

    public static readonly CourseStatus Published = new CourseStatus(2, nameof(Published));

    public static readonly CourseStatus ReadyForReview = new CourseStatus(3, nameof(ReadyForReview));

    public static readonly CourseStatus Archived = new CourseStatus(4, nameof(Archived));

    public static readonly CourseStatus Rejected = new CourseStatus(5, nameof(Rejected));

    private CourseStatus(int id, string name)
        : base(id, name)
    {
    }
}
