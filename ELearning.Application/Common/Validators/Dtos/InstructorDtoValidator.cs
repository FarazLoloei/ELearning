using ELearning.Application.Instructors.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class InstructorDtoValidator : AbstractValidator<InstructorDto>
{
    public InstructorDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Bio).NotEmpty();
        RuleFor(x => x.Expertise).NotEmpty();
        RuleFor(x => x.ProfilePictureUrl).NotEmpty();
        RuleFor(x => x.AverageRating).InclusiveBetween(0, 5);
        RuleFor(x => x.TotalStudents).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TotalCourses).GreaterThanOrEqualTo(0);
    }
}
