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
            Console.WriteLine($"Migrating [{schema}].[{tableName}]...");

            int tableRowTotal = 
                (int)
                new SqlCommand($"select count(*) from [{schema}].[{tableName}]", _connectionHolder.Source)
                    .ExecuteScalar();

            var sourceCommand = new SqlCommand($"select * from [{schema}].[{tableName}]", _connectionHolder.Source);

            //using (TransactionScope transaction = new TransactionScope())
            using (SqlDataReader dr = sourceCommand.ExecuteReader())
            {
                using (var bulkCopy = new SqlBulkCopy(_connectionHolder.Destination.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
                {
                    bulkCopy.DestinationTableName = $"[{schema}].[{tableName}]";
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.BatchSize = Settings.BatchSize;
                    bulkCopy.NotifyAfter = Settings.BatchSize;
                    bulkCopy.SqlRowsCopied += (s, e) => WriteProgress(e.RowsCopied, tableRowTotal);
                    foreach (string columnName in GetMapping(tableName))
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(columnName, columnName));

                    bulkCopy.WriteToServer(dr);
                    bulkCopy.Close();

                    WriteProgress(tableRowTotal, tableRowTotal);
                    Console.WriteLine("");
                    Console.WriteLine($"Finished [{schema}].[{tableName}].");
                    Console.WriteLine("");
                }
                //transaction.Complete();
            }
        }

        private void WriteProgress(long rowsCopied, int rowTotal)
        {
            Console.CursorLeft = 0;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"Copied {rowsCopied}/{rowTotal} records...                     ");
            Console.ResetColor();
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
