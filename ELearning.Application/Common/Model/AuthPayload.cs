namespace ELearning.Application.Common.Model;

public sealed record AuthPayload(
    string Token,
    Guid UserId,
    string Email,
    string FullName,
    string Role);
