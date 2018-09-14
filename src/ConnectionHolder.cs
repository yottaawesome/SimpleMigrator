using System;
using System.Data.SqlClient;

namespace SimpleMigrator
{
    class ConnectionHolder : IDisposable
    {
        public ConnectionHolder(string sourceConStr, string destinationConStr)
        {
            Source = new SqlConnection(sourceConStr);
            Destination = new SqlConnection(destinationConStr);
            Source.Open();
            Destination.Open();
        }

        public SqlConnection Source { get; private set; }
        public SqlConnection Destination { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Source.Close();
                Destination.Close();
            }
        }
    }
}
