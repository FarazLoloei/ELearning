namespace ELearning.Application.Students;

public readonly record struct StudentProgressDto(
    Guid StudentId,
    string StudentName,
    int CompletedCourses,
    int InProgressCourses,
    List<EnrollmentProgressDto> Enrollments
);