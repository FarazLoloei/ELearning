namespace ELearning.API.Models;

public class RegisterInstructorRequest : RegisterStudentRequest
{
    public string Bio { get; set; }

    public string Expertise { get; set; }
}