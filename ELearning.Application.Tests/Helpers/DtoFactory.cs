using ELearning.Application.Courses.Dtos;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Instructors.Dtos;
using ELearning.Application.Students.Dtos;
using ELearning.Application.Submissions.Dtos;

namespace ELearning.Application.Tests.Helpers;

internal static class DtoFactory
{
    private static readonly Guid FixedId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly DateTime FixedUtc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static AssignmentDto CreateAssignmentDto(
        string title = "Assignment 1",
        int maxPoints = 100) =>
        new(
            FixedId,
            title,
            "Assignment description",
            "Quiz",
            maxPoints,
            FixedUtc.AddDays(7));

    public static LessonDto CreateLessonDto(
        string title = "Lesson 1",
        int order = 1) =>
        new(
            FixedId,
            title,
            "Lesson content",
            "Video",
            "https://cdn.example.com/video.mp4",
            "15m",
            order);

    public static ModuleDto CreateModuleDto(
        string title = "Module 1",
        int order = 1) =>
        new(
            FixedId,
            title,
            "Module description",
            order,
            new[] { CreateLessonDto() },
            new[] { CreateAssignmentDto() });

    public static ReviewDto CreateReviewDto(
        decimal rating = 4.5m) =>
        new(
            FixedId,
            "Student Name",
            rating,
            "Great course",
            FixedUtc);

    public static InstructorDto CreateInstructorDto(
        string fullName = "John Doe") =>
        new(
            FixedId,
            fullName,
            "john@example.com",
            "Senior engineer",
            "Backend",
            "https://cdn.example.com/profile.jpg",
            4.7m,
            120,
            5);

    public static InstructorCourseDto CreateInstructorCourseDto(
        string title = "Domain Modeling") =>
        new(
            FixedId,
            title,
            "Software",
            50,
            "Published",
            FixedUtc.AddMonths(-1),
            FixedUtc.AddDays(-20));

    public static InstructorCoursesDto CreateInstructorCoursesDto() =>
        new(
            FixedId,
            "John Doe",
            "john@example.com",
            "Senior engineer",
            "Backend",
            "https://cdn.example.com/profile.jpg",
            4.7m,
            120,
            5,
            new[] { CreateInstructorCourseDto() });

    public static CourseDto CreateCourseDto(
        string title = "Clean Architecture",
        decimal price = 99,
        decimal averageRating = 4.6m) =>
        new(
            FixedId,
            title,
            "Detailed course description",
            CreateInstructorDto(),
            "Published",
            "Software",
            "Intermediate",
            price,
            "4h",
            FixedUtc.AddDays(-10),
            averageRating,
            40,
            new[] { CreateModuleDto() },
            new[] { CreateReviewDto() });

    public static CourseListDto CreateCourseListDto(
        string title = "Clean Architecture",
        decimal price = 99) =>
        new(
            FixedId,
            title,
            "Summary description",
            "John Doe",
            "Software",
            "Intermediate",
            price,
            4.6m,
            40,
            true,
            "4h",
            100);

    public static LessonProgressDto CreateLessonProgressDto(
        string lessonTitle = "Lesson 1",
        int timeSpentSeconds = 120) =>
        new(
            FixedId,
            lessonTitle,
            "Completed",
            FixedUtc,
            timeSpentSeconds);

    public static SubmissionDto CreateSubmissionDto(
        int? score = 90,
        int maxPoints = 100) =>
        new(
            FixedId,
            FixedId,
            "Assignment 1",
            FixedUtc,
            true,
            score,
            maxPoints);

    public static SubmissionDetailDto CreateSubmissionDetailDto(
        int? score = 90,
        int maxPoints = 100) =>
        new(
            FixedId,
            FixedId,
            "Assignment 1",
            FixedUtc,
            true,
            score,
            maxPoints,
            FixedId,
            "Student Name",
            "Submission content",
            "https://cdn.example.com/submission.pdf",
            "Good work",
            FixedId,
            "Instructor Name",
            FixedUtc);

    public static EnrollmentDto CreateEnrollmentDto(
        string status = "InProgress",
        double completionPercentage = 30) =>
        new(
            FixedId,
            FixedId,
            "Student Name",
            FixedId,
            "Course Title",
            status,
            FixedUtc.AddDays(-5),
            null,
            completionPercentage);

    public static EnrollmentDetailDto CreateEnrollmentDetailDto(
        string status = "InProgress",
        double completionPercentage = 30) =>
        new(
            FixedId,
            FixedId,
            "Student Name",
            FixedId,
            "Course Title",
            status,
            FixedUtc.AddDays(-5),
            null,
            completionPercentage,
            new[] { CreateLessonProgressDto() },
            new[] { CreateSubmissionDto() },
            4.5m,
            "Nice course");

    public static EnrollmentProgressDto CreateEnrollmentProgressDto(
        double completionPercentage = 30,
        int completedLessons = 3,
        int totalLessons = 10) =>
        new(
            FixedId,
            FixedId,
            "Course Title",
            "InProgress",
            FixedUtc.AddDays(-5),
            null,
            completionPercentage,
            completedLessons,
            totalLessons,
            1,
            2);

    public static StudentDto CreateStudentDto(
        string fullName = "Student Name") =>
        new(
            FixedId,
            fullName,
            "student@example.com",
            "https://cdn.example.com/student.jpg",
            FixedUtc.AddDays(-1));

    public static StudentProgressDto CreateStudentProgressDto() =>
        new(
            FixedId,
            "Student Name",
            1,
            2,
            new[] { CreateEnrollmentProgressDto() });
}


