using ELearning.Application.Enrollments.Dtos;

namespace ELearning.API.GraphQL.ObjectTypes;

public class EnrollmentType : ObjectType<EnrollmentDetailDto>
{
    protected override void Configure(IObjectTypeDescriptor<EnrollmentDetailDto> descriptor)
    {
        descriptor.Description("A student's enrollment in a course");

        descriptor
            .Field(e => e.Id)
            .Description("The unique identifier of the enrollment");

        descriptor
            .Field(e => e.StudentId)
            .Description("The ID of the enrolled student");

        descriptor
            .Field(e => e.StudentName)
            .Description("The name of the enrolled student");

        descriptor
            .Field(e => e.CourseId)
            .Description("The ID of the course");

        descriptor
            .Field(e => e.CourseTitle)
            .Description("The title of the course");

        descriptor
            .Field(e => e.Status)
            .Description("The current status of the enrollment");

        descriptor
            .Field(e => e.EnrollmentDate)
            .Description("The date when the student enrolled in the course");

        descriptor
            .Field(e => e.CompletedDate)
            .Description("The date when the student completed the course");

        descriptor
            .Field(e => e.CompletionPercentage)
            .Description("The percentage of course completion");

        descriptor
            .Field(e => e.LessonProgress)
            .Description("The progress records for individual lessons");

        descriptor
            .Field(e => e.Submissions)
            .Description("The student's assignment submissions");

        descriptor
            .Field(e => e.CourseRating)
            .Description("The rating given by the student to the course");

        descriptor
            .Field(e => e.Review)
            .Description("The review provided by the student for the course");
    }
}