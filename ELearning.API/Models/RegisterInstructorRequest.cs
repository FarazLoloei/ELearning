namespace ELearning.API.Models;

public class RegisterInstructorRequest : RegisterStudentRequest
{
    public string Bio { get; set; } = string.Empty;

    public string Expertise { get; set; } = string.Empty;
}