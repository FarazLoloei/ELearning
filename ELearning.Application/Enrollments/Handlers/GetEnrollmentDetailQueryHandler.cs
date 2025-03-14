using AutoMapper;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
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
        catch (Exception)
        {
            // Fall back to repository
            var enrollment = await enrollmentRepository.GetByIdAsync(request.EnrollmentId);

            if (enrollment == null)
            {
                throw new NotFoundException(nameof(Enrollment), request.EnrollmentId);
            }

            var course = await courseRepository.GetByIdAsync(enrollment.CourseId);
            var student = await studentRepository.GetByIdAsync(enrollment.StudentId);
            var progressRecords = await progressRepository.GetByEnrollmentIdAsync(enrollment.Id);
            var completionPercentage = await progressRepository.GetCourseProgressPercentageAsync(enrollment.Id);

            var lessonProgress = new List<LessonProgressDto>();
            foreach (var progress in progressRecords)
            {
                // Get lesson info
                var lesson = await lessonRepository.GetByIdAsync(progress.LessonId);

                lessonProgress.Add(new LessonProgressDto
                {
                    LessonId = progress.LessonId,
                    LessonTitle = lesson.Title,
                    Status = progress.Status.Name,
                    CompletedDate = progress.CompletedDate,
                    TimeSpentSeconds = progress.TimeSpentSeconds
                });
            }

            var submissionDtos = mapper.Map<List<SubmissionDto>>(enrollment.Submissions);

            var enrollmentDetailDto = new EnrollmentDetailDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                StudentName = student.FullName,
                CourseId = enrollment.CourseId,
                CourseTitle = course.Title,
                Status = enrollment.Status.Name,
                EnrollmentDate = enrollment.CreatedAt,
                CompletedDate = enrollment.CompletedDate,
                CompletionPercentage = completionPercentage,
                LessonProgress = lessonProgress,
                Submissions = submissionDtos,
                CourseRating = enrollment.CourseRating?.Value,
                Review = enrollment.Review
            };

            return Result.Success(enrollmentDetailDto);
        }
    }
}