// <copyright file="LessonType.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate.Enums;

using ELearning.SharedKernel;

public sealed class LessonType : Enumeration
{
    public static LessonType Video = new LessonType(1, nameof(Video));

    public static LessonType Text = new LessonType(2, nameof(Text));

    public static LessonType Presentation = new LessonType(3, nameof(Presentation));

    public static LessonType Interactive = new LessonType(4, nameof(Interactive));

    private LessonType(int id, string name)
        : base(id, name)
    {
    }
}