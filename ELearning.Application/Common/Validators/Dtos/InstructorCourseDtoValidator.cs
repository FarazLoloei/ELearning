using ELearning.Application.Instructors.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class InstructorCourseDtoValidator : AbstractValidator<InstructorCourseDto>
{
    public InstructorCourseDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Category).NotEmpty();
        RuleFor(x => x.EnrollmentsCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).NotEmpty();
    }
}
