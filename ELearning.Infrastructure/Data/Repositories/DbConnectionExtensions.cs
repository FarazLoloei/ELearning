using System.Data;
using System.Data.Common;

namespace ELearning.Infrastructure.Data.Repositories;

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
