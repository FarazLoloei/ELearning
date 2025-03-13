namespace ELearning.API.GraphQL.InputTypes;

public record GradeSubmissionInput(
    Guid SubmissionId,
    int Score,
    string Feedback);