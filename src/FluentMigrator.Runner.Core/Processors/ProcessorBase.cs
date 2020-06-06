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
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using FluentMigrator.Expressions;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace FluentMigrator.Runner.Processors
{
    /// <summary>
    /// Minimalist base class for a processor.
    /// </summary>
    public abstract class ProcessorBase : IMigrationProcessor, IAsyncMigrationProcessor
    {
        protected internal readonly IMigrationGenerator Generator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorBase"/> class.
        /// </summary>
        /// <param name="generator">The migration generator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        protected ProcessorBase(
            [NotNull] IMigrationGenerator generator,
            [NotNull] ILogger logger,
            [NotNull] ProcessorOptions options)
        {
            Generator = generator;
            Options = options;
            Logger = logger;
        }

        /// <summary>
        /// Gets the default database type identifier.
        /// </summary>
        public abstract string DatabaseType { get; }

        /// <summary>
        /// Gets the database type aliases.
        /// </summary>
        public abstract IList<string> DatabaseTypeAliases { get; }

        /// <summary>
        /// Gets or sets a value indicating whether a transaction was committed or rolled back.
        /// </summary>
        public bool WasCommitted { get; protected set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected internal ILogger Logger { get; }

        /// <summary>
        /// Gets the processor options.
        /// </summary>
        [NotNull]
        protected ProcessorOptions Options { get; }

        /// <inheritdoc/>
        public virtual Task<bool> ExistsAsync(
            string template,
            CancellationToken cancellationToken = default,
            params object[] args)
        {
            try
            {
                var result = Exists(template, args);
                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                BeginTransaction();
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                CommitTransaction();
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ExecuteAsync(string sql, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(
                    sql.Replace("{", "{{").Replace("}", "}}"),
                    cancellationToken,
                    Array.Empty<object>());
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task ExecuteAsync(
            string template,
            CancellationToken cancellationToken = default,
            params object[] args)
        {
            try
            {
                Execute(template, args);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(CreateSchemaExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(DeleteSchemaExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(AlterTableExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(AlterColumnExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(CreateTableExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(CreateColumnExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(DeleteTableExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(DeleteColumnExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(CreateForeignKeyExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(DeleteForeignKeyExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(CreateIndexExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(DeleteIndexExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(RenameTableExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(RenameColumnExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(InsertDataExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(
            AlterDefaultConstraintExpression expression,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task ProcessAsync(PerformDBOperationExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                Process(expression);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(DeleteDataExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(UpdateDataExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(AlterSchemaExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(CreateSequenceExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(DeleteSequenceExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(CreateConstraintExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(DeleteConstraintExpression expression, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public Task ProcessAsync(
            DeleteDefaultConstraintExpression expression,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return ExecuteAsync(Generator.Generate(expression), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        protected virtual Task ProcessAsync(string sql, CancellationToken cancellationToken = default)
        {
            try
            {
                Process(sql);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task<DataSet> ReadAsync(
            string template,
            CancellationToken cancellationToken = default,
            params object[] args)
        {
            try
            {
                DataSet result = Read(template, args);
                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                return Task.FromException<DataSet>(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task<DataSet> ReadTableDataAsync(
            string schemaName,
            string tableName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                DataSet result = ReadTableData(schemaName, tableName);
                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                return Task.FromException<DataSet>(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                RollbackTransaction();
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task<bool> ColumnExistsAsync(
            string schemaName,
            string tableName,
            string columnName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = ColumnExists(schemaName, tableName, columnName);
                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task<bool> ConstraintExistsAsync(
            string schemaName,
            string tableName,
            string constraintName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = ConstraintExists(schemaName, tableName, constraintName);
                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task<bool> DefaultValueExistsAsync(
            string schemaName,
            string tableName,
            string columnName,
            object defaultValue,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = DefaultValueExists(schemaName, tableName, columnName, defaultValue);
                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task<bool> IndexExistsAsync(
            string schemaName,
            string tableName,
            string indexName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = IndexExists(schemaName, tableName, indexName);
                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task<bool> SchemaExistsAsync(string schemaName, CancellationToken cancellationToken)
        {
            try
            {
                var result = SchemaExists(schemaName);
                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task<bool> SequenceExistsAsync(
            string schemaName,
            string sequenceName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = SequenceExists(schemaName, sequenceName);
                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        /// <inheritdoc/>
        public virtual Task<bool> TableExistsAsync(
            string schemaName,
            string tableName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = TableExists(schemaName, tableName);
                return Task.FromResult(result);
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        public virtual void Process(CreateSchemaExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(DeleteSchemaExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(CreateTableExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(AlterTableExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(AlterColumnExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(CreateColumnExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(DeleteTableExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(DeleteColumnExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(CreateForeignKeyExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(DeleteForeignKeyExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(CreateIndexExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(DeleteIndexExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(RenameTableExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(RenameColumnExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(InsertDataExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(DeleteDataExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(AlterDefaultConstraintExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(UpdateDataExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public abstract void Process(PerformDBOperationExpression expression);

        public virtual void Process(AlterSchemaExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(CreateSequenceExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(DeleteSequenceExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(CreateConstraintExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(DeleteConstraintExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        public virtual void Process(DeleteDefaultConstraintExpression expression)
        {
            Process(Generator.Generate(expression));
        }

        protected abstract void Process(string sql);

        public virtual void BeginTransaction()
        {
        }

        public virtual void CommitTransaction()
        {
        }

        public virtual void RollbackTransaction()
        {
        }

        public abstract DataSet ReadTableData(string schemaName, string tableName);

        public abstract DataSet Read(string template, params object[] args);

        public abstract bool Exists(string template, params object[] args);

        /// <inheritdoc />
        public virtual void Execute(string sql)
        {
            Execute(sql.Replace("{", "{{").Replace("}", "}}"), Array.Empty<object>());
        }

        public abstract void Execute(string template, params object[] args);

        public abstract bool SchemaExists(string schemaName);

        public abstract bool TableExists(string schemaName, string tableName);

        public abstract bool ColumnExists(string schemaName, string tableName, string columnName);

        public abstract bool ConstraintExists(string schemaName, string tableName, string constraintName);

        public abstract bool IndexExists(string schemaName, string tableName, string indexName);

        public abstract bool SequenceExists(string schemaName, string sequenceName);

        public abstract bool DefaultValueExists(string schemaName, string tableName, string columnName, object defaultValue);

        public void Dispose()
        {
            Dispose(true);
        }

        protected abstract void Dispose(bool isDisposing);
    }
}
