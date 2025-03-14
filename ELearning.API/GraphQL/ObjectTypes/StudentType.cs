using ELearning.Application.Students.Dtos;

namespace ELearning.API.GraphQL.ObjectTypes;

public class StudentType : ObjectType<StudentDto>
{
    protected override void Configure(IObjectTypeDescriptor<StudentDto> descriptor)
    {
        descriptor.Description("A student user in the e-learning platform");

        descriptor
            .Field(s => s.Id)
            .Description("The unique identifier of the student");

        descriptor
            .Field(s => s.FullName)
            .Description("The full name of the student");

        descriptor
            .Field(s => s.Email)
            .Description("The email address of the student");

        descriptor
            .Field(s => s.ProfilePictureUrl)
            .Description("The URL to the student's profile picture");

        descriptor
            .Field(s => s.LastLoginDate)
            .Description("The date and time of the student's last login");
    }
}