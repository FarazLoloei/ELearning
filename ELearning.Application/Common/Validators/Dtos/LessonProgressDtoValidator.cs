using ELearning.Application.Enrollments.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class LessonProgressDtoValidator : AbstractValidator<LessonProgressDto>
{
    public LessonProgressDtoValidator()
    {
        RuleFor(x => x.LessonId).NotEmpty();
        RuleFor(x => x.LessonTitle).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
        RuleFor(x => x.TimeSpentSeconds).GreaterThanOrEqualTo(0);
    }
}
