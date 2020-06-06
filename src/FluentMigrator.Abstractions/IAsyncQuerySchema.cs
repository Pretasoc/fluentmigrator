#region License
// Copyright (c) 2020, FluentMigrator Project
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace FluentMigrator
{
    /// <summary>
    ///     Gets the interface to query a database
    /// </summary>
    public interface IAsyncQuerySchema
    {
        /// <summary>
        ///     Gets the database type
        /// </summary>
        [NotNull]
        string DatabaseType { get; }

        /// <summary>
        ///     Gets the database type aliases
        /// </summary>
        [NotNull]
        [ItemNotNull]
        IList<string> DatabaseTypeAliases { get; }

        /// <summary>
        ///     Tests if a column exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>
        ///     A <see cref="Task"/>, that represents the asynchronous operation. When the <see cref="Task"/> completed the
        ///     <see cref="Task{T}.Result"/> property will yield <see langword="true"/> when it exists.
        /// </returns>
        Task<bool> ColumnExistsAsync(
            [CanBeNull] string schemaName,
            [NotNull] string tableName,
            [NotNull] string columnName,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Tests if a constraint exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="constraintName">The constraint name</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>
        ///     A <see cref="Task"/>, that represents the asynchronous operation. When the <see cref="Task"/> completed the
        ///     <see cref="Task{T}.Result"/> property will yield <see langword="true"/> when it exists.
        /// </returns>
        Task<bool> ConstraintExistsAsync(
            [CanBeNull] string schemaName,
            [NotNull] string tableName,
            [NotNull] string constraintName,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Tests if a default value for a column exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="columnName">The column name</param>
        /// <param name="defaultValue">The default value</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>
        ///     A <see cref="Task"/>, that represents the asynchronous operation. When the <see cref="Task"/> completed the
        ///     <see cref="Task{T}.Result"/> property will yield <see langword="true"/> when it exists.
        /// </returns>
        Task<bool> DefaultValueExistsAsync(
            [CanBeNull] string schemaName,
            [NotNull] string tableName,
            [NotNull] string columnName,
            object defaultValue,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Tests if an index exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="indexName">The index name</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>
        ///     A <see cref="Task"/>, that represents the asynchronous operation. When the <see cref="Task"/> completed the
        ///     <see cref="Task{T}.Result"/> property will yield <see langword="true"/> when it exists.
        /// </returns>
        Task<bool> IndexExistsAsync(
            [CanBeNull] string schemaName,
            [NotNull] string tableName,
            [NotNull] string indexName,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Tests if the schema exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>
        ///     A <see cref="Task"/>, that represents the asynchronous operation. When the <see cref="Task"/> completed the
        ///     <see cref="Task{T}.Result"/> property will yield <see langword="true"/> when the schema exists.
        /// </returns>
        Task<bool> SchemaExistsAsync([NotNull] string schemaName, CancellationToken cancellationToken);

        /// <summary>
        ///     Tests if a sequence exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="sequenceName">The sequence name</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>
        ///     A <see cref="Task"/>, that represents the asynchronous operation. When the <see cref="Task"/> completed the
        ///     <see cref="Task{T}.Result"/> property will yield <see langword="true"/> when it exists.
        /// </returns>
        Task<bool> SequenceExistsAsync(
            [CanBeNull] string schemaName,
            [NotNull] string sequenceName,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Tests if the table exists
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="tableName">The table name</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
        /// <returns>
        ///     A <see cref="Task"/>, that represents the asynchronous operation. When the <see cref="Task"/> completed the
        ///     <see cref="Task{T}.Result"/> property will yield <see langword="true"/> when it exists.
        /// </returns>
        Task<bool> TableExistsAsync(
            [CanBeNull] string schemaName,
            [NotNull] string tableName,
            CancellationToken cancellationToken = default);
    }
}
