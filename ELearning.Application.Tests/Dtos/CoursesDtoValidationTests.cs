using ELearning.Application.Tests.Helpers;
using ELearning.Application.Common.Validators.Dtos;

namespace ELearning.Application.Tests.Dtos;

public class CoursesDtoValidationTests
{
    private static readonly CourseDtoValidator CourseValidator = new();
    private static readonly CourseListDtoValidator CourseListValidator = new();
    private static readonly ModuleDtoValidator ModuleValidator = new();
    private static readonly LessonDtoValidator LessonValidator = new();
    private static readonly AssignmentDtoValidator AssignmentValidator = new();
    private static readonly ReviewDtoValidator ReviewValidator = new();

    [Fact]
    public void CourseDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateCourseDto();
        DtoValidationTestHelper.AssertValid(CourseValidator, dto);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CourseDto_InvalidTitle_FailsValidation(string title)
    {
        var dto = DtoFactory.CreateCourseDto(title: title);
        DtoValidationTestHelper.AssertInvalidFor(CourseValidator, dto, x => x.Title);
    }

    [Theory]
    [InlineData(-1)]
    public void CourseDto_InvalidPrice_FailsValidation(decimal price)
    {
        var dto = DtoFactory.CreateCourseDto(price: price);
        DtoValidationTestHelper.AssertInvalidFor(CourseValidator, dto, x => x.Price);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5.1)]
    public void CourseDto_InvalidRating_FailsValidation(decimal rating)
    {
        var dto = DtoFactory.CreateCourseDto(averageRating: rating);
        DtoValidationTestHelper.AssertInvalidFor(CourseValidator, dto, x => x.AverageRating);
    }

    [Fact]
    public void CourseListDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateCourseListDto();
        DtoValidationTestHelper.AssertValid(CourseListValidator, dto);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CourseListDto_InvalidTitle_FailsValidation(string title)
    {
        var dto = DtoFactory.CreateCourseListDto(title: title);
        DtoValidationTestHelper.AssertInvalidFor(CourseListValidator, dto, x => x.Title);
    }

    [Theory]
    [InlineData(-1)]
    public void CourseListDto_InvalidPrice_FailsValidation(decimal price)
    {
        var dto = DtoFactory.CreateCourseListDto(price: price);
        DtoValidationTestHelper.AssertInvalidFor(CourseListValidator, dto, x => x.Price);
    }

    [Fact]
    public void ModuleDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateModuleDto();
        DtoValidationTestHelper.AssertValid(ModuleValidator, dto);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ModuleDto_InvalidTitle_FailsValidation(string title)
    {
        var dto = DtoFactory.CreateModuleDto(title: title);
        DtoValidationTestHelper.AssertInvalidFor(ModuleValidator, dto, x => x.Title);
    }

    [Theory]
    [InlineData(-1)]
    public void ModuleDto_InvalidOrder_FailsValidation(int order)
    {
        var dto = DtoFactory.CreateModuleDto(order: order);
        DtoValidationTestHelper.AssertInvalidFor(ModuleValidator, dto, x => x.Order);
    }

    [Fact]
    public void LessonDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateLessonDto();
        DtoValidationTestHelper.AssertValid(LessonValidator, dto);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void LessonDto_InvalidTitle_FailsValidation(string title)
    {
        var dto = DtoFactory.CreateLessonDto(title: title);
        DtoValidationTestHelper.AssertInvalidFor(LessonValidator, dto, x => x.Title);
    }

    [Theory]
    [InlineData(-1)]
    public void LessonDto_InvalidOrder_FailsValidation(int order)
    {
        var dto = DtoFactory.CreateLessonDto(order: order);
        DtoValidationTestHelper.AssertInvalidFor(LessonValidator, dto, x => x.Order);
    }

    [Fact]
    public void AssignmentDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateAssignmentDto();
        DtoValidationTestHelper.AssertValid(AssignmentValidator, dto);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void AssignmentDto_InvalidTitle_FailsValidation(string title)
    {
        var dto = DtoFactory.CreateAssignmentDto(title: title);
        DtoValidationTestHelper.AssertInvalidFor(AssignmentValidator, dto, x => x.Title);
    }

    [Theory]
    [InlineData(-1)]
    public void AssignmentDto_InvalidMaxPoints_FailsValidation(int maxPoints)
    {
        var dto = DtoFactory.CreateAssignmentDto(maxPoints: maxPoints);
        DtoValidationTestHelper.AssertInvalidFor(AssignmentValidator, dto, x => x.MaxPoints);
    }

    [Fact]
    public void ReviewDto_ValidModel_PassesValidation()
    {
        var dto = DtoFactory.CreateReviewDto();
        DtoValidationTestHelper.AssertValid(ReviewValidator, dto);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5.1)]
    public void ReviewDto_InvalidRating_FailsValidation(decimal rating)
    {
        var dto = DtoFactory.CreateReviewDto(rating: rating);
        DtoValidationTestHelper.AssertInvalidFor(ReviewValidator, dto, x => x.Rating);
    }
}

