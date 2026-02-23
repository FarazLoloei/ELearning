using ELearning.Application.Enrollments.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class EnrollmentDetailDtoValidator : AbstractValidator<EnrollmentDetailDto>
{
    public EnrollmentDetailDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.StudentName).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.CourseTitle).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
        RuleFor(x => x.CompletionPercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.LessonProgress).NotNull();
        RuleFor(x => x.Submissions).NotNull();
    }
}
