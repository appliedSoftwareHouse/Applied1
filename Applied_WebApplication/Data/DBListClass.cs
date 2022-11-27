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
                    FileName = ".\\wwwroot\\SQLiteDB\\Applied.db";
                    break;
                case 102:
                    FileName = ".\\wwwroot\\SQLiteDB\\Applied.db";
                    break;
                case 103:
                    FileName = ".\\wwwroot\\SQLiteDB\\Applied.db";
                    break;
                case 104:
                    FileName = ".\\wwwroot\\SQLiteDB\\Applied.db";
                    break;
                case 105:
                    FileName = ".\\wwwroot\\SQLiteDB\\Applied.db";
                    break;
                default:
                    FileName = ".\\wwwroot\\SQLiteDB\\Applied.db";
                    break;
            }
            return FileName;
        }

        public enum DBList
        {
            _default = 101,
            Applied = 102,
            Winmark = 103,
            GTC = 104,
            Amcorp = 105
        }


    }
}
