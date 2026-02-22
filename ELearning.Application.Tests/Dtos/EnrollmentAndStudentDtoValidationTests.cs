using ELearning.Application.Tests.Helpers;
using ELearning.Application.Common.Validators.Dtos;

namespace ELearning.Application.Tests.Dtos;

public class EnrollmentAndStudentDtoValidationTests
{
    private static readonly EnrollmentDtoValidator EnrollmentValidator = new();
    private static readonly EnrollmentDetailDtoValidator EnrollmentDetailValidator = new();
    private static readonly LessonProgressDtoValidator LessonProgressValidator = new();
    private static readonly EnrollmentProgressDtoValidator EnrollmentProgressValidator = new();
    private static readonly StudentDtoValidator StudentValidator = new();
    private static readonly StudentProgressDtoValidator StudentProgressValidator = new();

    [Fact]
    public void EnrollmentDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateEnrollmentDto();
        DtoValidationTestHelper.AssertValid(EnrollmentValidator, dto);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void EnrollmentDto_InvalidCompletionPercentage_FailsValidation(double completionPercentage)
    {
        var dto = DtoFactory.CreateEnrollmentDto(completionPercentage: completionPercentage);
        DtoValidationTestHelper.AssertInvalidFor(EnrollmentValidator, dto, x => x.CompletionPercentage);
    }

    [Fact]
    public void EnrollmentDetailDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateEnrollmentDetailDto();
        DtoValidationTestHelper.AssertValid(EnrollmentDetailValidator, dto);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void EnrollmentDetailDto_InvalidCompletionPercentage_FailsValidation(double completionPercentage)
    {
        var dto = DtoFactory.CreateEnrollmentDetailDto(completionPercentage: completionPercentage);
        DtoValidationTestHelper.AssertInvalidFor(EnrollmentDetailValidator, dto, x => x.CompletionPercentage);
    }

    [Fact]
    public void EnrollmentDetailDto_NullLessonProgress_FailsValidation()
    {
        var dto = DtoFactory.CreateEnrollmentDetailDto() with { LessonProgress = null! };
        DtoValidationTestHelper.AssertInvalidFor(EnrollmentDetailValidator, dto, x => x.LessonProgress);
    }

    [Fact]
    public void EnrollmentDetailDto_NullSubmissions_FailsValidation()
    {
        var dto = DtoFactory.CreateEnrollmentDetailDto() with { Submissions = null! };
        DtoValidationTestHelper.AssertInvalidFor(EnrollmentDetailValidator, dto, x => x.Submissions);
    }

    [Fact]
    public void LessonProgressDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateLessonProgressDto();
        DtoValidationTestHelper.AssertValid(LessonProgressValidator, dto);
    }

    [Theory]
    [InlineData(-1)]
    public void LessonProgressDto_InvalidTimeSpent_FailsValidation(int timeSpentSeconds)
    {
        var dto = DtoFactory.CreateLessonProgressDto(timeSpentSeconds: timeSpentSeconds);
        DtoValidationTestHelper.AssertInvalidFor(LessonProgressValidator, dto, x => x.TimeSpentSeconds);
    }

    [Fact]
    public void EnrollmentProgressDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateEnrollmentProgressDto();
        DtoValidationTestHelper.AssertValid(EnrollmentProgressValidator, dto);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void EnrollmentProgressDto_InvalidCompletionPercentage_FailsValidation(double completionPercentage)
    {
        var dto = DtoFactory.CreateEnrollmentProgressDto(completionPercentage: completionPercentage);
        DtoValidationTestHelper.AssertInvalidFor(EnrollmentProgressValidator, dto, x => x.CompletionPercentage);
    }

    [Theory]
    [InlineData(11, 10)]
    public void EnrollmentProgressDto_CompletedLessonsGreaterThanTotal_FailsValidation(
        int completedLessons,
        int totalLessons)
    {
        var dto = DtoFactory.CreateEnrollmentProgressDto(
            completedLessons: completedLessons,
            totalLessons: totalLessons);
        DtoValidationTestHelper.AssertInvalidFor(EnrollmentProgressValidator, dto, x => x.CompletedLessons);
    }

    [Fact]
    public void StudentDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateStudentDto();
        DtoValidationTestHelper.AssertValid(StudentValidator, dto);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void StudentDto_InvalidFullName_FailsValidation(string fullName)
    {
        var dto = DtoFactory.CreateStudentDto(fullName: fullName);
        DtoValidationTestHelper.AssertInvalidFor(StudentValidator, dto, x => x.FullName);
    }

    [Fact]
    public void StudentProgressDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateStudentProgressDto();
        DtoValidationTestHelper.AssertValid(StudentProgressValidator, dto);
    }

    [Theory]
    [InlineData(-1)]
    public void StudentProgressDto_InvalidCompletedCourses_FailsValidation(int completedCourses)
    {
        var dto = DtoFactory.CreateStudentProgressDto() with { CompletedCourses = completedCourses };
        DtoValidationTestHelper.AssertInvalidFor(StudentProgressValidator, dto, x => x.CompletedCourses);
    }

    [Fact]
    public void StudentProgressDto_NullEnrollments_FailsValidation()
    {
        var dto = DtoFactory.CreateStudentProgressDto() with { Enrollments = null! };
        DtoValidationTestHelper.AssertInvalidFor(StudentProgressValidator, dto, x => x.Enrollments);
    }
}

