using ELearning.Application.Courses.Dtos;
using HotChocolate.Types;

namespace ELearning.API.GraphQL.ObjectTypes;

public class CourseType : ObjectType<CourseDetailDto>
{
    protected override void Configure(IObjectTypeDescriptor<CourseDetailDto> descriptor)
    {
        descriptor.Description("A course in the e-learning platform");

        descriptor
            .Field(c => c.Id)
            .Description("The unique identifier of the course");

        descriptor
            .Field(c => c.Title)
            .Description("The title of the course");

        descriptor
            .Field(c => c.Description)
            .Description("The description of the course");

        descriptor
            .Field(c => c.Status)
            .Description("The current status of the course");

        descriptor
            .Field(c => c.Category)
            .Description("The category of the course");

        descriptor
            .Field(c => c.Level)
            .Description("The difficulty level of the course");

        descriptor
            .Field(c => c.Price)
            .Description("The price of the course");

        descriptor
            .Field(c => c.Duration)
            .Description("The estimated duration to complete the course");

        descriptor
            .Field(c => c.PublishedDate)
            .Description("The date when the course was published");

        descriptor
            .Field(c => c.AverageRating)
            .Description("The average rating of the course");

        descriptor
            .Field(c => c.NumberOfRatings)
            .Description("The number of ratings received");

        descriptor
            .Field(c => c.Modules)
            .Description("The modules included in the course");

        descriptor
            .Field(c => c.Reviews)
            .Description("The reviews provided by students");
    }
}