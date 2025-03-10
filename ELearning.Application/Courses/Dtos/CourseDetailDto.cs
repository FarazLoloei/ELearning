namespace ELearning.Application.Courses.Dtos;

public readonly record struct CourseDetailDto(
    Guid Id,
    string Title,
    string Description,
    InstructorDto Instructor,
    string Status,
    string Category,
    string Level,
    decimal Price,
    string Duration,
    DateTime? PublishedDate,
    decimal AverageRating,
    int NumberOfRatings,
    List<ModuleDto> Modules,
    List<ReviewDto> Reviews
);