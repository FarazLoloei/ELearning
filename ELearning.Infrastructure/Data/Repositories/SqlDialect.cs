// <copyright file="SqlDialect.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using ELearning.Infrastructure.Options;

public interface ISqlDialect
{
    string Page(string pageSizeParameter = "@PageSize", string offsetParameter = "@Offset");

    string FetchFirst(string countExpression);

    string Concatenate(params string[] expressions);
}

internal static class SqlDialectFactory
{
    public static ISqlDialect Create(string provider)
    {
        if (DatabaseProviderNames.IsSqlServer(provider))
        {
            return new SqlServerSqlDialect();
        }

        if (DatabaseProviderNames.IsSqliteInMemory(provider))
        {
            return new SqliteSqlDialect();
        }

        throw new InvalidOperationException($"Unsupported database provider '{provider}'.");
    }
}

internal sealed class SqliteSqlDialect : ISqlDialect
{
    public string Page(string pageSizeParameter = "@PageSize", string offsetParameter = "@Offset") =>
        $"LIMIT {pageSizeParameter} OFFSET {offsetParameter}";

    public string FetchFirst(string countExpression) =>
        $"LIMIT {countExpression}";

    public string Concatenate(params string[] expressions) =>
        string.Join(" || ", expressions);
}

internal sealed class SqlServerSqlDialect : ISqlDialect
{
    public string Page(string pageSizeParameter = "@PageSize", string offsetParameter = "@Offset") =>
        $"OFFSET {offsetParameter} ROWS FETCH NEXT {pageSizeParameter} ROWS ONLY";

    public string FetchFirst(string countExpression) =>
        $"OFFSET 0 ROWS FETCH NEXT {countExpression} ROWS ONLY";

    public string Concatenate(params string[] expressions) =>
        $"CONCAT({string.Join(", ", expressions)})";
}
