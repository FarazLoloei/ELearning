// <copyright file="Lesson.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate;

using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;

public class Lesson : BaseEntity
{
    public string Title { get; private set; } = string.Empty;

    public string Content { get; private set; } = string.Empty;

    public LessonType Type { get; private set; } = null!;

    public string? VideoUrl { get; private set; }

    public Duration Duration { get; private set; } = null!;

    public int Order { get; private set; }

    public Guid ModuleId { get; private set; }

    private Lesson()
    {
    }

    public Lesson(
        string title,
        string content,
        LessonType type,
        int order,
        Guid moduleId,
        Duration? duration = null,
        string? videoUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Lesson title cannot be empty", nameof(title));
        }

        if (type == LessonType.Video)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                throw new ArgumentException("Video URL must be provided for video lessons", nameof(videoUrl));
            }

            if (duration == null)
            {
                throw new ArgumentException("Duration must be provided for video lessons", nameof(duration));
            }
        }

        this.Title = title;
        this.Content = content;
        this.Type = type;
        this.Order = order;
        this.ModuleId = moduleId;
        this.Duration = duration ?? Duration.CreateDefault();
        this.VideoUrl = videoUrl;
    }

    public void UpdateDetails(string title, string content, LessonType type)
    {
        this.Title = title;
        this.Content = content;
        this.Type = type;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void UpdateVideo(string? videoUrl, Duration duration)
    {
        if (this.Type == LessonType.Video)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                throw new ArgumentException("Video URL must be provided for video lessons", nameof(videoUrl));
            }

            if (duration == null)
            {
                throw new ArgumentException("Duration must be provided for video lessons", nameof(duration));
            }
        }

        this.VideoUrl = videoUrl;
        this.Duration = duration;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void UpdateOrder(int order)
    {
        this.Order = order;
        this.UpdatedAt(DateTime.UtcNow);
    }
}
