using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Instructors.Dtos;

public readonly record struct InstructorCoursesDto(
    Guid Id,
    string FullName,
    string Email,
    string Bio,
    string Expertise,
    string ProfilePictureUrl,
    decimal AverageRating,
    int TotalStudents,
    int TotalCourses,
    List<InstructorCourseDto> Courses
) : IDto;