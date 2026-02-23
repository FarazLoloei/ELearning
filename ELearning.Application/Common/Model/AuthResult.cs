namespace ELearning.Application.Common.Model;

public sealed record AuthResult
{
    public bool Success { get; }
    public AuthPayload? Data { get; }
    public string? ErrorMessage { get; }

    public string? Token => Data?.Token;
    public Guid? UserId => Data?.UserId;
    public string? Email => Data?.Email;
    public string? FullName => Data?.FullName;
    public string? Role => Data?.Role;

    private AuthResult(bool success, AuthPayload? data, string? errorMessage)
    {
        var isValidSuccess = success && data is not null && string.IsNullOrWhiteSpace(errorMessage);
        var isValidFailure = !success && data is null && !string.IsNullOrWhiteSpace(errorMessage);

        if (!isValidSuccess && !isValidFailure)
        {
            throw new InvalidOperationException("AuthResult must be either a success with data or a failure with error.");
        }

        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
    }

    public static AuthResult Succeeded(AuthPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return new AuthResult(true, payload, null);
    }

    public static AuthResult Succeeded(
        string token,
        Guid userId,
        string email,
        string fullName,
        string role)
        => Succeeded(new AuthPayload(token, userId, email, fullName, role));

    public static AuthResult Failed(string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);
        return new AuthResult(false, null, errorMessage);
    }
}