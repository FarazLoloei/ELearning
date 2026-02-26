using ELearning.API.GraphQL.ObjectTypes;

namespace ELearning.API.GraphQL;

public static class GraphQLConfiguration
{
    public static IServiceCollection AddGraphQLServices(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddType<CourseType>()
            .AddType<StudentType>()
            .AddType<InstructorType>()
            .AddType<EnrollmentType>()
            .AddType<SubmissionType>()
            .AddFiltering()
            .AddSorting()
            .AddProjections();

        return services;
    }

    public static void UseGraphQLEndpoint(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGraphQL();
        });
    }
}
