using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.CourseAggregate;

public class Lesson : BaseEntity
{
    /// <summary>
    /// Name of the lesson
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// The actual lesson content (text/HTML)
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// Format of lesson (Video, Text, Presentation, Interactive)
    /// </summary>
    public LessonType Type { get; private set; }

    /// <summary>
    /// Link to video content (for video-type lessons)
    /// </summary>
    public string VideoUrl { get; private set; }

    /// <summary>
    /// Estimated time to complete the lesson
    /// </summary>
    public Duration Duration { get; private set; }

    /// <summary>
    /// Sequence number within the module
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// Reference to the parent module
    /// </summary>
    public Guid ModuleId { get; private set; }

    private Lesson()
    { }

    public Lesson(
        string title,
        string content,
        LessonType type,
        int order,
        Guid moduleId,
        Duration duration = null,
        string videoUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Lesson title cannot be empty", nameof(title));

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
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateVideo(string videoUrl, Duration duration)
    {
        VideoUrl = videoUrl;
        Duration = duration;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateOrder(int order)
    {
        Order = order;
        UpdatedAt = DateTime.UtcNow;
    }
}