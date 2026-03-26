// <copyright file="CourseStatus.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate.Enums;

using ELearning.SharedKernel;

public sealed class CourseStatus : Enumeration
{
    public static CourseStatus Draft = new CourseStatus(1, nameof(Draft));

    public static CourseStatus Published = new CourseStatus(2, nameof(Published));

    public static CourseStatus Unpublished = new CourseStatus(3, nameof(Unpublished));

    public static CourseStatus Archived = new CourseStatus(4, nameof(Archived));

    private CourseStatus(int id, string name)
        : base(id, name)
    {
    }
}