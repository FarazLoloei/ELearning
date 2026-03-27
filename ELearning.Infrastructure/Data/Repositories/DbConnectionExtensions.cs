// <copyright file="DbConnectionExtensions.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using System.Data;
using System.Data.Common;

internal static class DbConnectionExtensions
{
    public static async Task EnsureOpenAsync(this IDbConnection connection, CancellationToken cancellationToken = default)
    {
        if (connection.State == ConnectionState.Open)
        {
            return;
        }

        if (connection is DbConnection dbConnection)
        {
            await dbConnection.OpenAsync(cancellationToken);
            return;
        }

        connection.Open();
    }
}
