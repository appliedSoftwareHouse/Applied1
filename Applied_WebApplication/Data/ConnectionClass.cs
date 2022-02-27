using System.Data.SQLite;
using System.IO;

namespace Applied_WebApplication.Data
{
    public class ConnectionClass
    {
        public SQLiteConnection AppliedConnection = new();
        public string DBFile_Path { get; set; } = ".\\Data\\Applied.DB";
        public bool DBFile_Exist { get => File.Exists(DBFile_Path); }


        public ConnectionClass()
        {
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
    }
}
