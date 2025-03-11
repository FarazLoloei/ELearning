using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Enrollments.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

namespace ELearning.Application.Enrollments.Handlers;

public class CreateEnrollmentCommandHandler(
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            IEnrollmentRepository enrollmentRepository,
            ICurrentUserService currentUserService)
    : IRequestHandler<CreateEnrollmentCommand, Result>
{
    public async Task<Result> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId == null)
        {
            throw new ForbiddenAccessException();
        }

        var studentId = currentUserService.UserId.Value;
        var student = await studentRepository.GetByIdAsync(studentId);

        if (student == null)
        {
            throw new NotFoundException(nameof(Student), studentId);
        }

        var course = await courseRepository.GetByIdAsync(request.CourseId);

        if (course == null)
        {
            throw new NotFoundException(nameof(Course), request.CourseId);
        }

        // Check if student is already enrolled
        var existingEnrollment = await enrollmentRepository.GetByStudentAndCourseIdAsync(studentId, request.CourseId);
        if (existingEnrollment != null)
        {
            return Result.Failure("You are already enrolled in this course.");
        }

        // Enroll student in course
        student.EnrollInCourse(course);

        // Get the enrollment that was just created
        var enrollment = student.Enrollments.Last();

        // Save to repository
        await studentRepository.UpdateAsync(student);

        return Result.Success();
    }
}