namespace ELearning.Application.Instructors.Dtos;

public readonly record struct InstructorDto(
    Guid Id,
    string FullName,
    string Email,
    string Bio,
    string Expertise,
    string ProfilePictureUrl,
    decimal AverageRating,
    int TotalStudents,
    int TotalCourses
);