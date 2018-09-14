using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SimpleMigrator
{
    class Program
    {
        static void Main(string[] args)
        {
            Write("--Start--");

            using (var connectionHolder = new ConnectionHolder(Settings.ConnectionStringFrom, Settings.ConnectionStringTo))
            {
                var migrator = new Migrator(connectionHolder);
                DataTable sourceTablesDataTable = connectionHolder.Source.GetSchema("Tables", new string[] { null, null, null, "BASE TABLE" });
                DataTable destinationDataTable = connectionHolder.Destination.GetSchema("Tables", new string[] { null, null, null, "BASE TABLE" });
                var sourceTables = new List<string>();
                foreach (DataRow row in sourceTablesDataTable.Rows)
                {
                    string tablename = (string)row[2];
                    sourceTables.Add(tablename);
                }
                foreach (DataRow row in destinationDataTable.Rows)
                {
                    string tableName = (string)row[2];
                    if (sourceTables.Any(x => x == tableName))
                    {
                        Write(tableName);
                        migrator.Copy(tableName);
                    }
                }
            }

            Write("--Finish--");
        }

        public static void Write(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
