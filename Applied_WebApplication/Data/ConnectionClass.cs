using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Data.SQLite;
using System.IO;
using System.Xml.Schema;

namespace Applied_WebApplication.Data
{
    public class ConnectionClass
    {
        public SQLiteConnection AppliedConnection = new();
        //public string DBFile_Path { get; set; } = ".\\wwwroot\\SQLiteDB\\Applied.DB";

        public string DBFile_Path { get; set; }
        public string DBFile_Name ="";

        public bool DBFile_Exist { get => File.Exists(DBFile_Path); }
                
        public ConnectionClass()
        {
            DBFile_Path = DBListClass.GetSQLiteFile((int)DBListClass.DBList.Applied);

            if (!DBFile_Exist) { CreateAppliedDataBase(); }
            AppliedConnection = new("Data Source=" + DBFile_Path);
        }

        private void CreateAppliedDataBase()
        {
            CreateTablesClass TableClass = new();
            SQLiteConnection.CreateFile(DBFile_Path);
            SQLiteCommand _Command = new();
            SQLiteConnection _Connection = new SQLiteConnection("Data Source=" + DBFile_Path); _Connection.Open();

            _Command = new SQLiteCommand(TableClass.tb_Users, _Connection); _Command.ExecuteNonQuery();
            _Command = new SQLiteCommand(TableClass.AddAdmin, _Connection); _Command.ExecuteNonQuery();
            _Command = new SQLiteCommand(TableClass.tb_Profile, _Connection); _Command.ExecuteNonQuery();
            _Command = new SQLiteCommand(TableClass.tb_Roles, _Connection); _Command.ExecuteNonQuery();
            _Command = new SQLiteCommand(TableClass.AddRole1, _Connection); _Command.ExecuteNonQuery();
        }

        
    }           // END()
}
