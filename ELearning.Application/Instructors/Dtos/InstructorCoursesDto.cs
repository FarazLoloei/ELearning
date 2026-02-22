using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Instructors.Dtos;

public sealed record InstructorCoursesDto(
    Guid Id,
    string FullName,
    string Email,
    string Bio,
    string Expertise,
    string ProfilePictureUrl,
    decimal AverageRating,
    int TotalStudents,
    int TotalCourses,
    IReadOnlyList<InstructorCourseDto> Courses
) : IDto;
