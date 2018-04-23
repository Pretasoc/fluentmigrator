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

using System.Data;
using System.IO;

using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Generators.SQLite;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SQLite;

using Moq;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.SQLite
{
    [TestFixture]
    [Category("Integration")]
    [Category("SQLite")]
    // ReSharper disable once InconsistentNaming
    public class SQLiteProcessorTests
    {
        private IDbFactory _dbFactory;
        private IDbConnection _connection;
        private SQLiteProcessor _processor;
        private Mock<ColumnDefinition> _column;
        private IDbCommand _command;
        private string _columnName;
        private string _tableName;
        private string _tableNameThanMustBeEscaped;

        [SetUp]
        public void SetUp()
        {
            // This connection used in the tests
            _dbFactory = new SQLiteDbFactory(serviceProvider: null);
            _connection = _dbFactory.CreateConnection(IntegrationTestOptions.SqlLite.ConnectionString);
            _connection.Open();
            _command = _connection.CreateCommand();

            // SUT
            _processor = new SQLiteProcessor(_connection, new SQLiteGenerator(), new TextWriterAnnouncer(TestContext.Out), new ProcessorOptions(), _dbFactory);

            _column = new Mock<ColumnDefinition>();
            _tableName = "NewTable";
            _tableNameThanMustBeEscaped = "123NewTable";
            _columnName = "ColumnName";
            _column.SetupGet(c => c.Name).Returns(_columnName);
            _column.SetupGet(c => c.IsNullable).Returns(true);
            _column.SetupGet(c => c.Type).Returns(DbType.Int32);
        }

        [Test]
        public void CanDefaultAutoIncrementColumnTypeToInteger()
        {
            var column = new ColumnDefinition
                             {
                                 Name = "Id",
                                 IsIdentity = true,
                                 IsPrimaryKey = true,
                                 Type = DbType.Int64,
                                 IsNullable = false
                             };

            var expression = new CreateTableExpression { TableName = _tableName };
            expression.Columns.Add(column);

            using (_command)
            {
                _processor.Process(expression);
                _command.CommandText = string.Format("SELECT name FROM sqlite_master WHERE type='table' and name='{0}'", _tableName);
                _command.ExecuteReader().Read().ShouldBeTrue();
            }
        }

        [Test]
        public void CanCreateTableExpression()
        {
            var expression = new CreateTableExpression { TableName = _tableName };
            expression.Columns.Add(_column.Object);

            using (_command)
            {
                _processor.Process(expression);
                _command.CommandText = string.Format("SELECT name FROM sqlite_master WHERE type='table' and name='{0}'", _tableName);
                _command.ExecuteReader().Read().ShouldBeTrue();
            }
        }

        [Test]
        public void IsEscapingTableNameCorrectlyOnTableCreate()
        {
            var expression = new CreateTableExpression { TableName = _tableNameThanMustBeEscaped };
            expression.Columns.Add(_column.Object);
            _processor.Process(expression);
        }

        [Test]
        public void IsEscapingTableNameCorrectlyOnReadTableData()
        {
            var expression = new CreateTableExpression { TableName = _tableNameThanMustBeEscaped };
            expression.Columns.Add(_column.Object);
            _processor.Process(expression);
            _processor.ReadTableData(null, _tableNameThanMustBeEscaped).Tables.Count.ShouldBe(1);
        }

        [Test]
        public void IsEscapingTableNameCorrectlyOnTableExists()
        {
            var expression = new CreateTableExpression { TableName = _tableNameThanMustBeEscaped };
            expression.Columns.Add(_column.Object);
            _processor.Process(expression);
            _processor.TableExists(null, _tableNameThanMustBeEscaped).ShouldBeTrue();
        }

        [Test]
        public void IsEscapingTableNameCorrectlyOnColumnExists()
        {
            const string columnName = "123ColumnName";

            var expression = new CreateTableExpression { TableName = _tableNameThanMustBeEscaped };
            expression.Columns.Add(new ColumnDefinition() { Name = "123ColumnName", Type = DbType.AnsiString, IsNullable = true });
            _processor.Process(expression);
            _processor.ColumnExists(null, _tableNameThanMustBeEscaped, columnName).ShouldBeTrue();
        }

        [Test]
        public void CallingProcessWithPerformDBOperationExpressionWhenInPreviewOnlyModeWillNotMakeDbChanges()
        {
            var output = new StringWriter();

            var connection = _dbFactory.CreateConnection(IntegrationTestOptions.SqlLite.ConnectionString);

            var processor = new SQLiteProcessor(
                connection,
                new SQLiteGenerator(),
                new TextWriterAnnouncer(output),
                new ProcessorOptions { PreviewOnly = true },
                new SQLiteDbFactory(serviceProvider: null));

            bool tableExists;

            try
            {
                var expression =
                    new PerformDBOperationExpression
                    {
                        Operation = (con, trans) =>
                        {
                            var command = con.CreateCommand();
                            command.CommandText = "CREATE TABLE ProcessTestTable (test int NULL) ";
                            command.Transaction = trans;

                            command.ExecuteNonQuery();
                        }
                    };

                processor.Process(expression);

                tableExists = processor.TableExists("", "ProcessTestTable");
            }
            finally
            {
                processor.RollbackTransaction();

            }

            tableExists.ShouldBeFalse();

            Assert.That(output.ToString(), Does.Contain(@"/* Performing DB Operation */"));
        }
    }
}