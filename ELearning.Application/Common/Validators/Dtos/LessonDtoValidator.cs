using ELearning.Application.Courses.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class LessonDtoValidator : AbstractValidator<LessonDto>
{
    public LessonDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.VideoUrl).NotEmpty();
        RuleFor(x => x.Duration).NotEmpty();
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}
