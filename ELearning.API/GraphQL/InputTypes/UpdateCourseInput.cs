namespace ELearning.API.GraphQL.InputTypes;

public sealed class UpdateCourseInput
{
    public Guid CourseId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int CategoryId { get; init; }

    public int LevelId { get; init; }

    public decimal Price { get; init; }

    public int DurationHours { get; init; }

    public int DurationMinutes { get; init; }

    public bool IsFeatured { get; init; }
}
