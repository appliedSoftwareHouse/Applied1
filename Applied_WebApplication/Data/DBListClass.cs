using Microsoft.AspNetCore.Routing.Constraints;
using System.Data;

namespace Applied_WebApplication.Data
{
    public class DBListClass
    {
        
        public static string GetSQLiteFile(int DBID)
        {
            string FileName = string.Empty;

            switch (DBID)
            {
                case 101:
                    FileName = ".\\wwwroot\\SQLiteDB\\Guest.db";
                    break;
                case 102:
                    FileName = ".\\wwwroot\\SQLiteDB\\Applied.db";
                    break;
                case 103:
                    FileName = ".\\wwwroot\\SQLiteDB\\Altamash.db";
                    break;
                case 104:
                    FileName = ".\\wwwroot\\SQLiteDB\\Winmark.db";
                    break;
                case 105:
                    FileName = ".\\wwwroot\\SQLiteDB\\Amcorp.db";
                    break;
                default:
                    FileName = ".\\wwwroot\\SQLiteDB\\Guest.db";
                    break;
            }
            return FileName;
        }

        public enum DBList
        {
            Guest = 101,
            Applied = 102,
            Altamash = 103,
            Winmark = 104,
            Amcorp = 105
        }


    }
}
