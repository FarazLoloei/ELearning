using ELearning.Application.Instructors.Dtos;
using HotChocolate.Types;

namespace ELearning.API.GraphQL.ObjectTypes;

public class InstructorType : ObjectType<InstructorDto>
{
    protected override void Configure(IObjectTypeDescriptor<InstructorDto> descriptor)
    {
        descriptor.Description("An instructor user in the e-learning platform");

        descriptor
            .Field(i => i.Id)
            .Description("The unique identifier of the instructor");

        descriptor
            .Field(i => i.FullName)
            .Description("The full name of the instructor");

        descriptor
            .Field(i => i.Email)
            .Description("The email address of the instructor");

        descriptor
            .Field(i => i.Bio)
            .Description("The biography of the instructor");

        descriptor
            .Field(i => i.Expertise)
            .Description("The areas of expertise of the instructor");

        descriptor
            .Field(i => i.ProfilePictureUrl)
            .Description("The URL to the instructor's profile picture");

        descriptor
            .Field(i => i.AverageRating)
            .Description("The average rating of the instructor's courses");

        descriptor
            .Field(i => i.TotalStudents)
            .Description("The total number of students enrolled in the instructor's courses");

        descriptor
            .Field(i => i.TotalCourses)
            .Description("The total number of courses created by the instructor");
    }
}
