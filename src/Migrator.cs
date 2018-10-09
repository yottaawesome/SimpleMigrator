using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SimpleMigrator
{
    class Migrator
    {
        private ConnectionHolder _connectionHolder;

        public Migrator(ConnectionHolder connectionHolder)
        {
            _connectionHolder = connectionHolder;
        }

        public void Copy(string schema, string tableName)
        {
            var sourceCommand = new SqlCommand($"select * from [{schema}].[{tableName}]", _connectionHolder.Source);
            //using (TransactionScope transaction = new TransactionScope())
            using (SqlDataReader dr = sourceCommand.ExecuteReader())
            {
                using (var bulkCopy = new SqlBulkCopy(_connectionHolder.Destination.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
                {
                    foreach (string columnName in GetMapping(tableName))
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(columnName, columnName));
                    bulkCopy.DestinationTableName = $"[{schema}].[{tableName}]";
                    bulkCopy.BatchSize = 1000;
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.WriteToServer(dr);
                    bulkCopy.Close();
                }
                //transaction.Complete();
            }
        }

        private IEnumerable<string> GetMapping(string tableName)
        {
            return Enumerable.Intersect(
                GetSchema(_connectionHolder.Source, tableName),
                GetSchema(_connectionHolder.Destination, tableName),
                StringComparer.Ordinal); // or StringComparer.OrdinalIgnoreCase
        }

        private IEnumerable<string> GetSchema(SqlConnection connection, string tableName)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "sp_Columns";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@table_name", SqlDbType.NVarChar, 384).Value = tableName;

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
