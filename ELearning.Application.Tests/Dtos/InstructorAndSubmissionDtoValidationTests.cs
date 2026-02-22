using ELearning.Application.Tests.Helpers;
using ELearning.Application.Common.Validators.Dtos;

namespace ELearning.Application.Tests.Dtos;

public class InstructorAndSubmissionDtoValidationTests
{
    private static readonly InstructorDtoValidator InstructorValidator = new();
    private static readonly InstructorCourseDtoValidator InstructorCourseValidator = new();
    private static readonly InstructorCoursesDtoValidator InstructorCoursesValidator = new();
    private static readonly SubmissionDtoValidator SubmissionValidator = new();
    private static readonly SubmissionDetailDtoValidator SubmissionDetailValidator = new();

    [Fact]
    public void InstructorDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateInstructorDto();
        DtoValidationTestHelper.AssertValid(InstructorValidator, dto);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void InstructorDto_InvalidFullName_FailsValidation(string fullName)
    {
        var dto = DtoFactory.CreateInstructorDto(fullName: fullName);
        DtoValidationTestHelper.AssertInvalidFor(InstructorValidator, dto, x => x.FullName);
    }

    [Fact]
    public void InstructorCourseDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateInstructorCourseDto();
        DtoValidationTestHelper.AssertValid(InstructorCourseValidator, dto);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void InstructorCourseDto_InvalidTitle_FailsValidation(string title)
    {
        var dto = DtoFactory.CreateInstructorCourseDto(title: title);
        DtoValidationTestHelper.AssertInvalidFor(InstructorCourseValidator, dto, x => x.Title);
    }

    [Theory]
    [InlineData(-1)]
    public void InstructorCourseDto_InvalidEnrollmentsCount_FailsValidation(int enrollmentsCount)
    {
        var dto = DtoFactory.CreateInstructorCourseDto() with { EnrollmentsCount = enrollmentsCount };
        DtoValidationTestHelper.AssertInvalidFor(InstructorCourseValidator, dto, x => x.EnrollmentsCount);
    }

    [Fact]
    public void InstructorCoursesDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateInstructorCoursesDto();
        DtoValidationTestHelper.AssertValid(InstructorCoursesValidator, dto);
    }

    [Fact]
    public void InstructorCoursesDto_NullCourses_FailsValidation()
    {
        var dto = DtoFactory.CreateInstructorCoursesDto() with { Courses = null! };
        DtoValidationTestHelper.AssertInvalidFor(InstructorCoursesValidator, dto, x => x.Courses);
    }

    [Fact]
    public void SubmissionDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateSubmissionDto();
        DtoValidationTestHelper.AssertValid(SubmissionValidator, dto);
    }

    [Theory]
    [InlineData(-1, 100)]
    [InlineData(110, 100)]
    public void SubmissionDto_InvalidScore_FailsValidation(int score, int maxPoints)
    {
        var dto = DtoFactory.CreateSubmissionDto(score: score, maxPoints: maxPoints);
        DtoValidationTestHelper.AssertInvalidFor(SubmissionValidator, dto, x => x.Score);
    }

    [Theory]
    [InlineData(-1)]
    public void SubmissionDto_InvalidMaxPoints_FailsValidation(int maxPoints)
    {
        var dto = DtoFactory.CreateSubmissionDto(score: 10, maxPoints: maxPoints);
        DtoValidationTestHelper.AssertInvalidFor(SubmissionValidator, dto, x => x.MaxPoints);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void SubmissionDto_InvalidAssignmentTitle_FailsValidation(string assignmentTitle)
    {
        var dto = DtoFactory.CreateSubmissionDto() with { AssignmentTitle = assignmentTitle };
        DtoValidationTestHelper.AssertInvalidFor(SubmissionValidator, dto, x => x.AssignmentTitle);
    }

    [Fact]
    public void SubmissionDetailDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateSubmissionDetailDto();
        DtoValidationTestHelper.AssertValid(SubmissionDetailValidator, dto);
    }

    [Theory]
    [InlineData(-1, 100)]
    [InlineData(110, 100)]
    public void SubmissionDetailDto_InvalidScore_FailsValidation(int score, int maxPoints)
    {
        var dto = DtoFactory.CreateSubmissionDetailDto(score: score, maxPoints: maxPoints);
        DtoValidationTestHelper.AssertInvalidFor(SubmissionDetailValidator, dto, x => x.Score);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void SubmissionDetailDto_InvalidGradedByName_FailsValidation(string gradedByName)
    {
        var dto = DtoFactory.CreateSubmissionDetailDto() with { GradedByName = gradedByName };
        DtoValidationTestHelper.AssertInvalidFor(SubmissionDetailValidator, dto, x => x.GradedByName);
    }
}

