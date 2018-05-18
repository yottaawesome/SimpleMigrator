using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SimpleMigrator
{
    class Program
    {
        static void Main(string[] args)
        {
            Write("--Start--");

            var migrator = new Migrator(Settings.ConnectionStringFrom, Settings.ConnectionStringTo);

            using (var connectionFrom = new SqlConnection(Settings.ConnectionStringFrom))
            using (var connectionTo = new SqlConnection(Settings.ConnectionStringTo))
            {
                connectionFrom.Open();
                connectionTo.Open();
                DataTable sourceTablesDataTable = connectionFrom.GetSchema("Tables", new string[] { null, null, null, "BASE TABLE" });
                DataTable destinationDataTable = connectionTo.GetSchema("Tables", new string[] { null, null, null, "BASE TABLE" });
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
            System.Console.WriteLine(msg);
        }
    }
}
