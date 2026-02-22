using ELearning.Application.Courses.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class CourseListDtoValidator : AbstractValidator<CourseListDto>
{
    public CourseListDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.InstructorName).NotEmpty();
        RuleFor(x => x.Category).NotEmpty();
        RuleFor(x => x.Level).NotEmpty();
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AverageRating).InclusiveBetween(0, 5);
        RuleFor(x => x.NumberOfRatings).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EnrollmentsCount).GreaterThanOrEqualTo(0);
    }
}
