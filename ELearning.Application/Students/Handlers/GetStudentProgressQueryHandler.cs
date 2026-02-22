using AutoMapper;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using ELearning.Application.Students.Abstractions.ReadModels;
using ELearning.Application.Students.Dtos;
using ELearning.Application.Students.Queries;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

namespace ELearning.Application.Students.Handlers;

public class GetStudentProgressQueryHandler(
        IStudentReadService studentReadService,
        IStudentRepository studentRepository,
        IEnrollmentRepository enrollmentRepository,
        IProgressRepository progressRepository,
        ICourseRepository courseRepository,
        IMapper mapper)
    : IRequestHandler<GetStudentProgressQuery, Result<StudentProgressDto>>
{
    public async Task<Result<StudentProgressDto>> Handle(GetStudentProgressQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Try to get from Dapr read service first
            var progressDto = await studentReadService.GetStudentProgressAsync(request.StudentId);
            return Result.Success(progressDto);
        }
        catch (Exception)
        {
            // Fall back to repositories if Dapr service fails
            var student = await studentRepository.GetByIdAsync(request.StudentId);

            if (student == null)
            {
                throw new NotFoundException(nameof(Student), request.StudentId);
            }

            var enrollments = await enrollmentRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);
            var completedEnrollments = enrollments.Where(e => e.Status == EnrollmentStatus.Completed).ToList();
            var inProgressEnrollments = enrollments.Where(e => e.Status == EnrollmentStatus.Active).ToList();

            var enrollmentProgressDtos = new List<EnrollmentProgressDto>();

            foreach (var enrollment in enrollments)
            {
                var course = await courseRepository.GetByIdAsync(enrollment.CourseId);
                var completionPercentage = await progressRepository.GetCourseProgressPercentageAsync(enrollment.Id);

                var lessonProgress = await progressRepository.GetByEnrollmentIdAsync(enrollment.Id);
                var totalLessons = course.Modules.Sum(m => m.Lessons.Count);
                var completedLessons = lessonProgress.Count(p => p.Status == ProgressStatus.Completed);

                var assignmentCount = course.Modules.Sum(m => m.Assignments.Count);
                var submissionCount = enrollment.Submissions.Count;

                var enrollmentProgressDto = new EnrollmentProgressDto(
                    enrollment.Id,
                    course.Id,
                    course.Title,
                    enrollment.Status.Name,
                    enrollment.CreatedAt(),
                    enrollment.CompletedDateUTC,
                    completionPercentage,
                    completedLessons,
                    totalLessons,
                    submissionCount,
                    assignmentCount);

                enrollmentProgressDtos.Add(enrollmentProgressDto);
            }

            var progressDto = new StudentProgressDto(
                student.Id,
                student.FullName,
                completedEnrollments.Count,
                inProgressEnrollments.Count,
                enrollmentProgressDtos);

            return Result.Success(progressDto);
        }
    }
}
