using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SimpleMigrator
{
    class TableDescriptor
    {
        public TableDescriptor(string schema, string name)
        {
            Schema = schema;
            Name = name;
        }

        public string Schema { get; set; }
        public string Name { get; set; }
    }

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
                var sourceTables = new List<TableDescriptor>();
                foreach (DataRow row in sourceTablesDataTable.Rows)
                {
                    sourceTables.Add(new TableDescriptor((string)row[1], (string)row[2]));
                }
                foreach (DataRow row in destinationDataTable.Rows)
                {
                    string schema = (string)row[1];
                    string tableName = (string)row[2];
                    if (sourceTables.Any(x => x.Name == tableName && x.Schema == schema))
                    {
                        Write(tableName);
                        migrator.Copy(schema, tableName);
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
