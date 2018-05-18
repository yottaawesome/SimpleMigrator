using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace SimpleMigrator
{
    public class Migrator
    {
        private readonly string _connectionFromString;
        private readonly string _connectionToString;

        public Migrator(string connectionFrom, string connectionTo)
        {
            _connectionFromString = connectionFrom;
            _connectionToString = connectionTo;
        }

        public void Copy(string tableName)
        {
            using (var connectionFrom = new SqlConnection(_connectionFromString))
            using (var connectionTo = new SqlConnection(_connectionToString))
            {
                connectionTo.Open();
                connectionFrom.Open();
                var sourceCommand = new SqlCommand($"select * from [{tableName}]", connectionFrom);
                //using (TransactionScope transaction = new TransactionScope())
                using (SqlDataReader dr = sourceCommand.ExecuteReader())
                {
                    using (var bulkCopy = new SqlBulkCopy(connectionTo.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
                    {
                        foreach (string columnName in GetMapping(_connectionFromString, _connectionToString, tableName))
                            bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(columnName, columnName));
                        bulkCopy.DestinationTableName = $"[{tableName}]";
                        bulkCopy.BatchSize = 1000;
                        bulkCopy.BulkCopyTimeout = 0;
                        bulkCopy.WriteToServer(dr);
                        bulkCopy.Close();
                    }
                    //transaction.Complete();
                }
            }
        }

        private IEnumerable<string> GetMapping(string stringSource, string stringTarget, string tableName)
        {
            return Enumerable.Intersect(
                GetSchema(stringSource, tableName),
                GetSchema(stringTarget, tableName),
                StringComparer.Ordinal); // or StringComparer.OrdinalIgnoreCase
        }

        private IEnumerable<string> GetSchema(string connectionString, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "sp_Columns";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@table_name", SqlDbType.NVarChar, 384).Value = tableName;

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return (string)reader["column_name"];
                    }
                }
            }
        }
    }
}
