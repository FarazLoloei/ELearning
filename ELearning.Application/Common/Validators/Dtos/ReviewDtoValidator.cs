using ELearning.Application.Courses.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class ReviewDtoValidator : AbstractValidator<ReviewDto>
{
    public ReviewDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.StudentName).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(0, 5);
        RuleFor(x => x.Comment).NotEmpty();
    }
}
