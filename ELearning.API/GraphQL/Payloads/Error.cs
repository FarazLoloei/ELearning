// <copyright file="Error.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.Payloads;

[GraphQLDescription("A GraphQL payload error")]
public sealed record Error(
    [property: GraphQLDescription("Stable error code")]
    string Code,
    [property: GraphQLDescription("Human-readable error message")]
    string Message);
