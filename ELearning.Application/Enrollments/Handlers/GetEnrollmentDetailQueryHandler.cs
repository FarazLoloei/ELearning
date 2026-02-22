using AutoMapper;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using ELearning.Application.Common.Resilience;
using ELearning.Application.Enrollments.Abstractions.ReadModels;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Enrollments.Queries;
using ELearning.Application.Submissions.Dtos;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

namespace ELearning.Application.Enrollments.Handlers;

public class GetEnrollmentDetailQueryHandler(
        IEnrollmentReadService enrollmentReadService,
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IStudentRepository studentRepository,
        IProgressRepository progressRepository,
        ILessonRepository lessonRepository,
        IMapper mapper)
    : IRequestHandler<GetEnrollmentDetailQuery, Result<EnrollmentDetailDto>>
{
    public async Task<Result<EnrollmentDetailDto>> Handle(GetEnrollmentDetailQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Try Dapr read service first
            var enrollmentDto = await enrollmentReadService.GetByIdAsync(request.EnrollmentId);
            return Result.Success(enrollmentDto);
        }
        catch (Exception ex) when (ReadModelFallbackPolicy.ShouldFallback(ex, cancellationToken))
        {
            // Fall back to repository
            var enrollment = await enrollmentRepository.GetByIdAsync(request.EnrollmentId) ??
                throw new NotFoundException(nameof(Enrollment), request.EnrollmentId);

            var course = await courseRepository.GetByIdAsync(enrollment.CourseId)
                ?? throw new NotFoundException("Course", enrollment.CourseId);
            var student = await studentRepository.GetByIdAsync(enrollment.StudentId)
                ?? throw new NotFoundException("Student", enrollment.StudentId);
            var progressRecords = await progressRepository.GetByEnrollmentIdAsync(enrollment.Id);
            var completionPercentage = await progressRepository.GetCourseProgressPercentageAsync(enrollment.Id);

            var lessonProgress = new List<LessonProgressDto>();
            foreach (var progress in progressRecords)
            {
                // Get lesson info
                var lesson = await lessonRepository.GetByIdAsync(progress.LessonId)
                    ?? throw new NotFoundException("Lesson", progress.LessonId);

                lessonProgress.Add(new LessonProgressDto(
                    progress.LessonId,
                    lesson.Title,
                    progress.Status.Name,
                    progress.CompletedDate,
                    progress.TimeSpentSeconds));
            }

            var submissionDtos = mapper.Map<List<SubmissionDto>>(enrollment.Submissions);

            var enrollmentDetailDto = new EnrollmentDetailDto(
                enrollment.Id,
                enrollment.StudentId,
                student.FullName,
                enrollment.CourseId,
                course.Title,
                enrollment.Status.Name,
                enrollment.CreatedAt(),
                enrollment.CompletedDateUTC,
                completionPercentage,
                lessonProgress,
                submissionDtos,
                enrollment.CourseRating?.Value,
                enrollment.Review);

            return Result.Success(enrollmentDetailDto);
        }
    }
}



