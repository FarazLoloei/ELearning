namespace ELearning.API.GraphQL.InputTypes;

public record CreateSubmissionInput(
    Guid AssignmentId,
    string Content,
    string FileUrl);