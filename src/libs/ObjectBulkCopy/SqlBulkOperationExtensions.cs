using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ObjectBulkCopy.Internals;

namespace ObjectBulkCopy;



/// <summary>
/// Provides extension methods for performing bulk copy operations.
/// </summary>
public static class SqlBulkOperationExtensions
{
    extension(SqlConnection connection)
    {
        /// <summary>
        /// Asynchronously performs a bulk insert operation into the database using the provided <see cref="SqlConnection"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data to be inserted.</typeparam>
        /// <param name="data">The data to be inserted.</param>
        /// <param name="options">The <see cref="SqlBulkCopyOptions"/> to use for the bulk insert operation.</param>
        /// <param name="timeout">The timeout for the bulk insert operation, in seconds. If <c>null</c>, the default timeout is used.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        public async ValueTask<int> BulkInsertAsync<T>(IEnumerable<T> data, SqlBulkCopyOptions options, int? timeout, CancellationToken cancellationToken = default)
        {
            SqlTransaction? transaction = null;
            var result = await BulkInsertAsyncCore(connection, transaction, data, options, timeout, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }


    extension(SqlTransaction transaction)
    {
        /// <summary>
        /// Asynchronously performs a bulk insert operation into the database using the provided <see cref="SqlTransaction"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data to be inserted.</typeparam>
        /// <param name="data">The data to be inserted.</param>
        /// <param name="options">The <see cref="SqlBulkCopyOptions"/> to use for the bulk insert operation.</param>
        /// <param name="timeout">The timeout for the bulk insert operation, in seconds. If <c>null</c>, the default timeout is used.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A ValueTask representing the asynchronous operation, with the number of rows copied as the result.</returns>
        public async ValueTask<int> BulkInsertAsync<T>(IEnumerable<T> data, SqlBulkCopyOptions options, int? timeout, CancellationToken cancellationToken = default)
        {
            var connection = transaction.Connection;
            var result = await BulkInsertAsyncCore(connection, transaction, data, options, timeout, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }


    #region Helpers
    /// <summary>
    /// Core method for asynchronously performing a bulk insert operation into the database.
    /// </summary>
    /// <typeparam name="T">The type of the data to be inserted.</typeparam>
    /// <param name="connection">The <see cref="SqlConnection"/> to use for the bulk insert operation.</param>
    /// <param name="transaction">The <see cref="SqlTransaction"/> to use for the bulk insert operation, or <c>null</c> if no transaction is used.</param>
    /// <param name="data">The data to be inserted.</param>
    /// <param name="options">The <see cref="SqlBulkCopyOptions"/> to use for the bulk insert operation.</param>
    /// <param name="timeout">The timeout for the bulk insert operation, in seconds. If <c>null</c>, the default timeout is used.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A ValueTask representing the asynchronous operation, with the number of rows copied as the result.</returns>
    private static async ValueTask<int> BulkInsertAsyncCore<T>(SqlConnection connection, SqlTransaction? transaction, IEnumerable<T> data, SqlBulkCopyOptions options, int? timeout, CancellationToken cancellationToken)
    {
        /*
        using (var executor = new SqlBulkCopy(connection, options, transaction))
        {
            var table = TableMapping.Get<T>();
            executor.BulkCopyTimeout = timeout ?? executor.BulkCopyTimeout;
            executor.DestinationTableName = table.GetFullName();

            using (var reader = new SqlBulkCopyDataReader<T>(data))
                await executor.WriteToServerAsync(reader, cancellationToken).ConfigureAwait(false);

            return executor.RowsCopied;
        }
        */

        using (var executor = new SqlBulkCopy(connection, options, transaction))
        {
            var table = TableMapping.Get<T>();
            executor.BulkCopyTimeout = timeout ?? executor.BulkCopyTimeout;
            executor.DestinationTableName = table.GetFullName();
            using (var reader = new SqlBulkCopyDataReader<T>(data))
            {
                const ActivityKind kind = ActivityKind.Client;
                var activityName = $"bulk insert {executor.DestinationTableName}";
                var parentContext = Activity.Current?.Context ?? default;
                using var activity = ActivitySourceCache.Instance.StartActivity(activityName, kind, parentContext);
                try
                {
                    var serverName = getServerName(connection);
                    activity?.SetTag("db.system.name", "microsoft.sql_server");
                    activity?.SetTag("db.query.summary", activityName);
                    activity?.SetTag("db.namespace", $"{serverName} | {connection.Database}");
                    activity?.SetTag("db.operation.timeout", executor.BulkCopyTimeout);

                    await executor.WriteToServerAsync(reader, cancellationToken).ConfigureAwait(false);

                    activity?.SetTag("db.response.returned_rows", executor.RowsCopied);
                }
#if NET9_0_OR_GREATER
                catch (Exception ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error);
                    activity?.AddException(ex);
                    throw;
                }
#else
                catch
                {
                    activity?.SetStatus(ActivityStatusCode.Error);
                    throw;
                }
#endif
            }
            return executor.RowsCopied;
        }


        #region Local Functions
        static ReadOnlySpan<char> getServerName(SqlConnection connection)
        {
            // tcp:<ResourceName>.database.windows.net,1433

            var dataSource = connection.DataSource.AsSpan();
            var from = dataSource.IndexOf(':');
            var to = dataSource.LastIndexOf(',');

            from = (from < 0) ? 0 : (from + 1);
            to = (to < 0) ? dataSource.Length : to;

            if (from >= to)
                return dataSource;

            var length = to - from;
            return dataSource.Slice(from, length);
        }
        #endregion
    }
    #endregion
}
