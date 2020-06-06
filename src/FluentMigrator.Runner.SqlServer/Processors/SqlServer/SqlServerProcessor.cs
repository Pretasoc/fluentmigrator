#region License
// Copyright (c) 2018, Fluent Migrator Project
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentMigrator.Expressions;
using FluentMigrator.Runner.BatchParser;
using FluentMigrator.Runner.BatchParser.Sources;
using FluentMigrator.Runner.BatchParser.SpecialTokenSearchers;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Helpers;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Server;

namespace FluentMigrator.Runner.Processors.SqlServer
{
    public class SqlServerProcessor : GenericProcessorBase
    {
        [CanBeNull]
        private readonly IServiceProvider _serviceProvider;

        private const string SqlSchemaExists = "SELECT 1 WHERE EXISTS (SELECT * FROM sys.schemas WHERE NAME = '{0}') ";
        private const string TABLE_EXISTS = "SELECT 1 WHERE EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}')";
        private const string COLUMN_EXISTS = "SELECT 1 WHERE EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}' AND COLUMN_NAME = '{2}')";
        private const string CONSTRAINT_EXISTS = "SELECT 1 WHERE EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_CATALOG = DB_NAME() AND TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}' AND CONSTRAINT_NAME = '{2}')";
        private const string INDEX_EXISTS = "SELECT 1 WHERE EXISTS (SELECT * FROM sys.indexes WHERE name = '{0}' and object_id=OBJECT_ID('{1}.{2}'))";
        private const string SEQUENCES_EXISTS = "SELECT 1 WHERE EXISTS (SELECT * FROM INFORMATION_SCHEMA.SEQUENCES WHERE SEQUENCE_SCHEMA = '{0}' AND SEQUENCE_NAME = '{1}' )";
        private const string DEFAULTVALUE_EXISTS = "SELECT 1 WHERE EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}' AND COLUMN_NAME = '{2}' AND COLUMN_DEFAULT LIKE '{3}')";

        public override string DatabaseType { get; }

        public override IList<string> DatabaseTypeAliases { get; }

        public IQuoter Quoter { get; }

        protected SqlServerProcessor(
            [NotNull, ItemNotNull] IEnumerable<string> databaseTypes,
            [NotNull] IMigrationGenerator generator,
            [NotNull] IQuoter quoter,
            [NotNull] ILogger logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor,
            [NotNull] IServiceProvider serviceProvider)
            : this(databaseTypes, SqlClientFactory.Instance, generator, quoter, logger, options, connectionStringAccessor, serviceProvider)
        {
        }

        protected SqlServerProcessor(
            [NotNull, ItemNotNull] IEnumerable<string> databaseTypes,
            [NotNull] DbProviderFactory factory,
            [NotNull] IMigrationGenerator generator,
            [NotNull] IQuoter quoter,
            [NotNull] ILogger logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor,
            [NotNull] IServiceProvider serviceProvider)
            : base(() => factory, generator, logger, options.Value, connectionStringAccessor)
        {
            _serviceProvider = serviceProvider;
            var dbTypes = databaseTypes.ToList();
            DatabaseType = dbTypes.First();
            DatabaseTypeAliases = dbTypes.Skip(1).ToList();
            Quoter = quoter;
        }

        private static string SafeSchemaName(string schemaName)
        {
            return string.IsNullOrEmpty(schemaName) ? "dbo" : FormatHelper.FormatSqlEscape(schemaName);
        }

        public override void BeginTransaction()
        {
            base.BeginTransaction();
            Logger.LogSql("BEGIN TRANSACTION");
        }

        public override void CommitTransaction()
        {
            base.CommitTransaction();
            Logger.LogSql("COMMIT TRANSACTION");
        }

        public override void RollbackTransaction()
        {
            if (Transaction == null)
            {
                return;
            }

            base.RollbackTransaction();
            Logger.LogSql("ROLLBACK TRANSACTION");
        }

        public override bool SchemaExists(string schemaName)
        {
            return Exists(SqlSchemaExists, SafeSchemaName(schemaName));
        }

        /// <inheritdoc />
        public override Task<bool> SchemaExistsAsync(string schemaName, CancellationToken cancellationToken)
        {
            try
            {
                return ExistsAsync(SqlSchemaExists, cancellationToken, SafeSchemaName(schemaName));
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        public override bool TableExists(string schemaName, string tableName)
        {
            try
            {
                return Exists(TABLE_EXISTS, SafeSchemaName(schemaName),
                    FormatHelper.FormatSqlEscape(tableName));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        public override async Task<bool> TableExistsAsync(string schemaName, string tableName, CancellationToken cancellationToken = default)
        {
            try
            {
                return await ExistsAsync(
                        TABLE_EXISTS,
                        cancellationToken,
                        SafeSchemaName(schemaName),
                        FormatHelper.FormatSqlEscape(tableName))
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        public override bool ColumnExists(string schemaName, string tableName, string columnName)
        {
            return Exists(COLUMN_EXISTS, SafeSchemaName(schemaName),
                FormatHelper.FormatSqlEscape(tableName), FormatHelper.FormatSqlEscape(columnName));
        }

        /// <inheritdoc />
        public override Task<bool> ColumnExistsAsync(
            string schemaName,
            string tableName,
            string columnName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return ExistsAsync(
                    COLUMN_EXISTS,
                    cancellationToken,
                    SafeSchemaName(schemaName),
                    FormatHelper.FormatSqlEscape(tableName),
                    FormatHelper.FormatSqlEscape(columnName));
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        public override bool ConstraintExists(string schemaName, string tableName, string constraintName)
        {
            return Exists(CONSTRAINT_EXISTS, SafeSchemaName(schemaName),
                FormatHelper.FormatSqlEscape(tableName), FormatHelper.FormatSqlEscape(constraintName));
        }

        /// <inheritdoc />
        public override Task<bool> ConstraintExistsAsync(
            string schemaName,
            string tableName,
            string constraintName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return ExistsAsync(
                    CONSTRAINT_EXISTS,
                    cancellationToken,
                    SafeSchemaName(schemaName),
                    FormatHelper.FormatSqlEscape(tableName),
                    FormatHelper.FormatSqlEscape(constraintName));
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        public override bool IndexExists(string schemaName, string tableName, string indexName)
        {
            return Exists(INDEX_EXISTS,
                FormatHelper.FormatSqlEscape(indexName), SafeSchemaName(schemaName), FormatHelper.FormatSqlEscape(tableName));
        }

        /// <inheritdoc />
        public override Task<bool> IndexExistsAsync(
            string schemaName,
            string tableName,
            string indexName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return ExistsAsync(
                    INDEX_EXISTS,
                    cancellationToken,
                    FormatHelper.FormatSqlEscape(indexName),
                    SafeSchemaName(schemaName),
                    FormatHelper.FormatSqlEscape(tableName));
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        public override bool SequenceExists(string schemaName, string sequenceName)
        {
            return Exists(SEQUENCES_EXISTS, SafeSchemaName(schemaName),
                FormatHelper.FormatSqlEscape(sequenceName));
        }

        /// <inheritdoc />
        public override Task<bool> SequenceExistsAsync(string schemaName, string sequenceName, CancellationToken cancellationToken = default)
        {
            try
            {
                return ExistsAsync(
                    SEQUENCES_EXISTS,
                    cancellationToken,
                    SafeSchemaName(schemaName),
                    FormatHelper.FormatSqlEscape(sequenceName));
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        public override bool DefaultValueExists(string schemaName, string tableName, string columnName, object defaultValue)
        {
            var defaultValueAsString = string.Format("%{0}%", FormatHelper.FormatSqlEscape(defaultValue.ToString()));
            return Exists(DEFAULTVALUE_EXISTS, SafeSchemaName(schemaName),
                FormatHelper.FormatSqlEscape(tableName),
                FormatHelper.FormatSqlEscape(columnName), defaultValueAsString);
        }

        /// <inheritdoc />
        public override Task<bool> DefaultValueExistsAsync(
            string schemaName,
            string tableName,
            string columnName,
            object defaultValue,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var defaultValueAsString = string.Format("%{0}%", FormatHelper.FormatSqlEscape(defaultValue.ToString()));
                return ExistsAsync(
                    DEFAULTVALUE_EXISTS,
                    cancellationToken,
                    SafeSchemaName(schemaName),
                    FormatHelper.FormatSqlEscape(tableName),
                    FormatHelper.FormatSqlEscape(columnName),
                    defaultValueAsString);
            }
            catch (Exception e)
            {
                return Task.FromException<bool>(e);
            }
        }

        public override void Execute(string template, params object[] args)
        {
            Process(string.Format(template, args));
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(string template, CancellationToken cancellationToken = default, params object[] args)
        {
            try
            {
                return ProcessAsync(string.Format(template, args), cancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        public override bool Exists(string template, params object[] args)
        {
            EnsureConnectionIsOpen();

            using (var command = CreateCommand(string.Format(template, args)))
            {
                var result = command.ExecuteScalar();
                return DBNull.Value != result && Convert.ToInt32(result) == 1;
            }
        }

        /// <inheritdoc />
        public override async Task<bool> ExistsAsync(string template, CancellationToken cancellationToken = default, params object[] args)
        {
            await EnsureConnectionIsOpenAsync(cancellationToken).ConfigureAwait(false);

            using (var command = CreateCommand(string.Format(template, args)))
            {
                var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                return DBNull.Value != result && Convert.ToInt32(result) == 1;
            }
        }

        public override DataSet ReadTableData(string schemaName, string tableName)
        {
            return Read("SELECT * FROM [{0}].[{1}]", SafeSchemaName(schemaName), tableName);
        }

        /// <inheritdoc />
        public override Task<DataSet> ReadTableDataAsync(string schemaName, string tableName, CancellationToken cancellationToken = default)
        {
            try
            {
                return ReadAsync("SELECT * FROM [{0}].[{1}]", cancellationToken, SafeSchemaName(schemaName), tableName);
            }
            catch (Exception e)
            {
                return Task.FromException<DataSet>(e);
            }
        }

        public override DataSet Read(string template, params object[] args)
        {
            EnsureConnectionIsOpen();

            using (var command = CreateCommand(string.Format(template, args)))
            using (var reader = command.ExecuteReader())
            {
                return reader.ReadDataSet();
            }
        }

        /// <inheritdoc />
        public override async Task<DataSet> ReadAsync(string template, CancellationToken cancellationToken = default, params object[] args)
        {
            await EnsureConnectionIsOpenAsync(cancellationToken).ConfigureAwait(false);

            using (var command = CreateCommand(string.Format(template, args)))
            using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
                return reader.ReadDataSet();
            }
        }

        protected override void Process(string sql)
        {
            Logger.LogSql(sql);

            if (Options.PreviewOnly || string.IsNullOrEmpty(sql))
            {
                return;
            }

            EnsureConnectionIsOpen();

            if (ContainsGo(sql))
            {
                ExecuteBatchNonQuery(sql, CancellationToken.None, sync: true).GetAwaiter().GetResult();
            }
            else
            {
                ExecuteNonQuery(sql, CancellationToken.None, sync: true).GetAwaiter().GetResult();
            }
        }

        protected override async Task ProcessAsync(string sql, CancellationToken cancellationToken = default)
        {
            Logger.LogSql(sql);

            if (Options.PreviewOnly || string.IsNullOrEmpty(sql))
            {
                return;
            }

            await EnsureConnectionIsOpenAsync(cancellationToken).ConfigureAwait(false);

            if (ContainsGo(sql))
            {
                await ExecuteBatchNonQuery(sql, cancellationToken, false).ConfigureAwait(false);
            }
            else
            {
                await ExecuteNonQuery(sql, cancellationToken, sync: false);
            }
        }

        private bool ContainsGo(string sql)
        {
            var containsGo = false;
            var parser = _serviceProvider?.GetService<SqlServerBatchParser>() ?? new SqlServerBatchParser();
            parser.SpecialToken += (sender, args) => containsGo = true;
            using (var source = new TextReaderSource(new StringReader(sql), true))
            {
                parser.Process(source);
            }

            return containsGo;
        }

        private async Task ExecuteNonQuery(string sql, CancellationToken cancellationToken, bool sync)
        {
            using (var command = CreateCommand(sql))
            {
                try
                {
                    if (sync)
                    {
                        // If the sync flag is given, we must execute all operation synchronously
                        // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    using (var message = new StringWriter())
                    {
                        message.WriteLine("An error occured executing the following sql:");
                        message.WriteLine(sql);
                        message.WriteLine("The error was {0}", ex.Message);

                        throw new Exception(message.ToString(), ex);
                    }
                }
            }
        }

        private async Task ExecuteBatchNonQuery(string sql, CancellationToken cancellationToken, bool sync)
        {
            var sqlBatch = string.Empty;
            ;
            try
            {
                GoSearcher.GoSearcherParameters goParameters = null;
                var parser = _serviceProvider?.GetService<SqlServerBatchParser>() ?? new SqlServerBatchParser();
                parser.SqlText += (sender, args) => sqlBatch = args.SqlText.Trim();
                parser.SpecialToken += (sender, args) =>
                {
                    if (goParameters != null)
                    {
                        return;
                    }

                    if (args.Opaque is GoSearcher.GoSearcherParameters goParams)
                    {
                        goParameters = goParams;
                    }
                };

                using (var source = new TextReaderSource(new StringReader(sql), true))
                {
                    parser.Process(source, stripComments: Options.StripComments);
                }

                using (var command = CreateCommand(sqlBatch))
                {
                    for (var i = 0; i != (goParameters?.Count ?? 1); ++i)
                    {
                        if (sync)
                        {
                            // If the sync flag is given, we must execute all operation synchronously
                            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (var message = new StringWriter())
                {
                    message.WriteLine("An error occured executing the following sql:");
                    message.WriteLine(string.IsNullOrEmpty(sqlBatch) ? sql : sqlBatch);
                    message.WriteLine("The error was {0}", ex.Message);

                    throw new Exception(message.ToString(), ex);
                }

            }
        }

        public override void Process(PerformDBOperationExpression expression)
        {
            Logger.LogSay("Performing DB Operation");

            if (Options.PreviewOnly)
            {
                return;
            }

            EnsureConnectionIsOpen();

            expression.Operation?.Invoke(Connection, Transaction);
        }

        /// <inheritdoc />
        public override async Task ProcessAsync(PerformDBOperationExpression expression, CancellationToken cancellationToken = default)
        {
            Logger.LogSay("Performing DB Operation");

            if (Options.PreviewOnly)
            {
                return;
            }

            await EnsureConnectionIsOpenAsync(cancellationToken).ConfigureAwait(false);

            expression.Operation?.Invoke(Connection, Transaction);
        }
    }
}
