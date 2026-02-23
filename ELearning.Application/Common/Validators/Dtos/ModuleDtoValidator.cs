using ELearning.Application.Courses.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class ModuleDtoValidator : AbstractValidator<ModuleDto>
{
    public ModuleDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Lessons).NotNull();
        RuleFor(x => x.Assignments).NotNull();
    }
}
