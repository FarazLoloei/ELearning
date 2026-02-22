using ELearning.Application.Students.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class StudentProgressDtoValidator : AbstractValidator<StudentProgressDto>
{
    public StudentProgressDtoValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.StudentName).NotEmpty();
        RuleFor(x => x.CompletedCourses).GreaterThanOrEqualTo(0);
        RuleFor(x => x.InProgressCourses).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Enrollments).NotNull();
    }
}
