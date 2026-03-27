// <copyright file="GraphQLConfiguration.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL;

using ELearning.API.GraphQL.ObjectTypes;

public static class GraphQLConfiguration
{
    public static IServiceCollection AddGraphQLServices(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddType<CourseType>()
            .AddType<CertificateType>()
            .AddType<StudentType>()
            .AddType<InstructorType>()
            .AddType<EnrollmentType>()
            .AddType<SubmissionType>()
            .AddFiltering()
            .AddSorting()
            .AddProjections();

        return services;
    }
}
