namespace ELearning.Application.Common.Model;

public record AuthResult
{
    public bool Success { get; set; }
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
    public string ErrorMessage { get; set; }
}