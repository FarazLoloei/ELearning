namespace ELearning.Application.Common.Model;

public sealed record AuthResult
{
    public bool Success { get; }
    public string Token { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public string FullName { get; }
    public string Role { get; }
    public string ErrorMessage { get; }

    private AuthResult(
        bool success,
        string token,
        Guid userId,
        string email,
        string fullName,
        string role,
        string errorMessage)
    {
        if (success)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token is required for successful auth.", nameof(token));
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required for successful auth.", nameof(userId));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required for successful auth.", nameof(email));
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("FullName is required for successful auth.", nameof(fullName));
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role is required for successful auth.", nameof(role));
            if (!string.IsNullOrEmpty(errorMessage))
                throw new ArgumentException("ErrorMessage must be empty for successful auth.", nameof(errorMessage));
        }
        else
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException("ErrorMessage is required for failed auth.", nameof(errorMessage));
        }

        Success = success;
        Token = token;
        UserId = userId;
        Email = email;
        FullName = fullName;
        Role = role;
        ErrorMessage = errorMessage;
    }

    public static AuthResult Succeeded(
        string token,
        Guid userId,
        string email,
        string fullName,
        string role)
        => new(true, token, userId, email, fullName, role, string.Empty);

    public static AuthResult Failed(string errorMessage)
        => new(false, string.Empty, Guid.Empty, string.Empty, string.Empty, string.Empty, errorMessage);
}
