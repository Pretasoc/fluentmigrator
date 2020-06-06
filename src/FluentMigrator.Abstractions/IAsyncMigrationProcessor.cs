#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using FluentMigrator.Expressions;

namespace FluentMigrator
{
    /// <summary>
    ///     Interface for a migration processor
    /// </summary>
    /// <remarks>
    ///     A migration processor generates the SQL statements using a <see cref="IMigrationGenerator"/>
    ///     and executes it using the given connection string.
    /// </remarks>
    public interface IAsyncMigrationProcessor : IAsyncQuerySchema, IDisposable
    {
        /// <summary>
        ///     Begins a transaction
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Commits a transaction
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Execute an SQL statement (escaping not needed)
        /// </summary>
        /// <param name="sql">The SQL statement</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ExecuteAsync(string sql, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Execute an SQL statement
        /// </summary>
        /// <param name="template">The SQL statement</param>
        /// <param name="args">The arguments to replace in the SQL statement</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ExecuteAsync(string template, CancellationToken cancellationToken = default, params object[] args);

        /// <summary>
        ///     Returns <c>true</c> if data could be found for the given SQL query
        /// </summary>
        /// <param name="template">The SQL query</param>
        /// <param name="args">The arguments of the SQL query</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>
        ///     A <see cref="Task"/>, that represents the asynchronous operation. The <see cref="Task{T}.Result"/> property
        ///     will yield <see langword="true"/> when the SQL query returned data
        /// </returns>
        Task<bool> ExistsAsync(string template, CancellationToken cancellationToken = default, params object[] args);

        /// <summary>
        ///     Executes a <c>CREATE SCHEMA</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(CreateSchemaExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes a <c>DROP SCHEMA</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(DeleteSchemaExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes a <c>ALTER TABLE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(AlterTableExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes a <c>ALTER TABLE ALTER COLUMN</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(AlterColumnExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes a <c>CREATE TABLE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(CreateTableExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes a <c>ALTER TABLE ADD COLUMN</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(CreateColumnExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes a <c>DROP TABLE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(DeleteTableExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes a <c>ALTER TABLE DROP COLUMN</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(DeleteColumnExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes an SQL expression to create a foreign key
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(CreateForeignKeyExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes an SQL expression to drop a foreign key
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(DeleteForeignKeyExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes an SQL expression to create an index
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(CreateIndexExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes an SQL expression to drop an index
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(DeleteIndexExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes an SQL expression to rename a table
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(RenameTableExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes an SQL expression to rename a column
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(RenameColumnExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes an SQL expression to INSERT data
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(InsertDataExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes an SQL expression to alter a default constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(AlterDefaultConstraintExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes a DB operation
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(PerformDBOperationExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes an SQL expression to DELETE data
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(DeleteDataExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes an SQL expression to UPDATE data
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(UpdateDataExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes a <c>ALTER SCHEMA</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(AlterSchemaExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes a <c>CREATE SEQUENCE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(CreateSequenceExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes a <c>DROP SEQUENCE</c> SQL expression
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(DeleteSequenceExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes an SQL expression to create a constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(CreateConstraintExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes an SQL expression to drop a constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(DeleteConstraintExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes an SQL expression to drop a default constraint
        /// </summary>
        /// <param name="expression">The expression to execute</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task ProcessAsync(DeleteDefaultConstraintExpression expression, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Executes and returns the result of an SQL query
        /// </summary>
        /// <param name="template">The SQL query</param>
        /// <param name="args">The arguments of the SQL query</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>
        ///     A <see cref="Task"/>, that represents the asynchronous operation. The <see cref="Task{T}.Result"/> property
        ///     will yield data from the specified SQL query
        /// </returns>
        Task<DataSet> ReadAsync(string template, CancellationToken cancellationToken = default, params object[] args);

        /// <summary>
        ///     Reads all data from all rows from a table
        /// </summary>
        /// <param name="schemaName">The schema name of the table</param>
        /// <param name="tableName">The table name</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>
        ///     A <see cref="Task"/>, that represents the asynchronous operation. The <see cref="Task{T}.Result"/> property
        ///     will yield the data from the specified table
        /// </returns>
        Task<DataSet> ReadTableDataAsync(string schemaName, string tableName, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Rollback of a transaction
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/>, that represents the asynchronous operation.</returns>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
