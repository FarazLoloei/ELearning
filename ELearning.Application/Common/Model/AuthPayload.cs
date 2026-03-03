namespace ELearning.Application.Common.Model;

public sealed record AuthPayload(
    string Token,
    string? RefreshToken,
    Guid UserId,
    string Email,
    string FullName,
    string Role);
