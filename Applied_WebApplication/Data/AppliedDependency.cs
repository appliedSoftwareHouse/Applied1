namespace Applied_WebApplication.Data
{
    public class AppliedDependency : IAppliedDependency
    {
        public string UserDBPath { get; set; }
        public string DefaultDB { get; set; }
        public string DefaultPath { get; set; }

        public AppliedDependency()
        {
            DefaultDB = ".\\wwwroot\\SQLiteDB\\";
            UserDBPath = string.Concat(DefaultPath, "AppliedUsers.db");
            DefaultDB = string.Concat(DefaultPath, "Applied.db");
        }
    }
}
