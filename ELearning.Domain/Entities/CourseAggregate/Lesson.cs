using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.CourseAggregate;

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
    { }

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
            throw new ArgumentException("Lesson title cannot be empty", nameof(title));

        if (type == LessonType.Video)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
                throw new ArgumentException("Video URL must be provided for video lessons", nameof(videoUrl));

            if (duration == null)
                throw new ArgumentException("Duration must be provided for video lessons", nameof(duration));
        }

        Title = title;
        Content = content;
        Type = type;
        Order = order;
        ModuleId = moduleId;
        Duration = duration ?? Duration.CreateDefault();
        VideoUrl = videoUrl;
    }

    public void UpdateDetails(string title, string content, LessonType type)
    {
        Title = title;
        Content = content;
        Type = type;
        UpdatedAt(DateTime.UtcNow);
    }

    public void UpdateVideo(string? videoUrl, Duration duration)
    {
        if (Type == LessonType.Video)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
                throw new ArgumentException("Video URL must be provided for video lessons", nameof(videoUrl));

            if (duration == null)
                throw new ArgumentException("Duration must be provided for video lessons", nameof(duration));
        }

        VideoUrl = videoUrl;
        Duration = duration;
        UpdatedAt(DateTime.UtcNow);
    }

    public void UpdateOrder(int order)
    {
        Order = order;
        UpdatedAt(DateTime.UtcNow);
    }
}
