using ELearning.Application.Students.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class EnrollmentProgressDtoValidator : AbstractValidator<EnrollmentProgressDto>
{
    public EnrollmentProgressDtoValidator()
    {
        RuleFor(x => x.EnrollmentId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.CourseTitle).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
        RuleFor(x => x.CompletionPercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.CompletedLessons).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TotalLessons).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CompletedAssignments).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TotalAssignments).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CompletedLessons).LessThanOrEqualTo(x => x.TotalLessons);
        RuleFor(x => x.CompletedAssignments).LessThanOrEqualTo(x => x.TotalAssignments);
    }
}
