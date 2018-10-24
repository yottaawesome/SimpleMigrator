namespace SimpleMigrator
{
    public static class Settings
    {
        public static string ConnectionStringFrom
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["Source"].ConnectionString;
            }
        }

        public static string ConnectionStringTo
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["Destination"].ConnectionString;
            }
        }

        public static int BatchSize
        {
            get
            {
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["BatchSize"]);
            }
        }
    }
}
