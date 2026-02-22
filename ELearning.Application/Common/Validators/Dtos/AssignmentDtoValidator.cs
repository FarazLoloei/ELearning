using ELearning.Application.Courses.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class AssignmentDtoValidator : AbstractValidator<AssignmentDto>
{
    public AssignmentDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.MaxPoints).GreaterThanOrEqualTo(0);
    }
}
