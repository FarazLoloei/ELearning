namespace ELearning.Application.Instructors.ReadModels;

public sealed record InstructorWithCoursesReadModel(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Bio,
    string Expertise,
    string? ProfilePictureUrl,
    decimal AverageRating,
    int TotalStudents,
    int TotalCourses,
    IReadOnlyList<InstructorCourseReadModel> Courses)
{
    public string FullName => $"{FirstName} {LastName}".Trim();
}
