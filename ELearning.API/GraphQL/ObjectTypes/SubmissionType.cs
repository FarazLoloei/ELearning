using ELearning.Application.Submissions.Dtos;

namespace ELearning.API.GraphQL.ObjectTypes;

public class SubmissionType : ObjectType<SubmissionDetailDto>
{
    protected override void Configure(IObjectTypeDescriptor<SubmissionDetailDto> descriptor)
    {
        descriptor.Description("A submission for an assignment");

        descriptor
            .Field(s => s.Id)
            .Description("The unique identifier of the submission");

        descriptor
            .Field(s => s.AssignmentId)
            .Description("The ID of the assignment");

        descriptor
            .Field(s => s.AssignmentTitle)
            .Description("The title of the assignment");

        descriptor
            .Field(s => s.StudentId)
            .Description("The ID of the student who submitted");

        descriptor
            .Field(s => s.StudentName)
            .Description("The name of the student who submitted");

        descriptor
            .Field(s => s.SubmittedDate)
            .Description("The date when the submission was made");

        descriptor
            .Field(s => s.Content)
            .Description("The textual content of the submission");

        descriptor
            .Field(s => s.FileUrl)
            .Description("The URL to the submitted file");

        descriptor
            .Field(s => s.IsGraded)
            .Description("Whether the submission has been graded");

        descriptor
            .Field(s => s.Score)
            .Description("The score given to the submission");

        descriptor
            .Field(s => s.MaxPoints)
            .Description("The maximum possible score for the assignment");

        descriptor
            .Field(s => s.Feedback)
            .Description("The feedback provided by the instructor");

        descriptor
            .Field(s => s.GradedById)
            .Description("The ID of the instructor who graded the submission");

        descriptor
            .Field(s => s.GradedByName)
            .Description("The name of the instructor who graded the submission");

        descriptor
            .Field(s => s.GradedDate)
            .Description("The date when the submission was graded");
    }
}