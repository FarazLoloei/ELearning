// <copyright file="AssignmentType.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate.Enums;

using ELearning.SharedKernel;

public sealed class AssignmentType : Enumeration
{
    public static AssignmentType Quiz = new AssignmentType(1, nameof(Quiz));
    public static AssignmentType Essay = new AssignmentType(2, nameof(Essay));
    public static AssignmentType Project = new AssignmentType(3, nameof(Project));
    public static AssignmentType Exam = new AssignmentType(4, nameof(Exam));

    private AssignmentType(int id, string name)
        : base(id, name)
    {
    }
}