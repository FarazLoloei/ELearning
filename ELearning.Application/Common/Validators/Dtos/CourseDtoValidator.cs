using ELearning.Application.Courses.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class CourseDtoValidator : AbstractValidator<CourseDto>
{
    public CourseDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Instructor).NotNull();
        RuleFor(x => x.Status).NotEmpty();
        RuleFor(x => x.Category).NotEmpty();
        RuleFor(x => x.Level).NotEmpty();
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AverageRating).InclusiveBetween(0, 5);
        RuleFor(x => x.NumberOfRatings).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Modules).NotNull();
        RuleFor(x => x.Reviews).NotNull();
    }
}
