using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace Applied_WebApplication.Data
{
    public class ConnectionClass : IdentityUser
    {
        
        public SQLiteConnection AppliedConnection = new();
        private AppliedUsersClass UsersTableClass= new();

        public string DBFile_Path { get; set; }
        public string DBFile_Name = "";

        public bool DBFile_Exist { get => File.Exists(DBFile_Path); }

        public ConnectionClass()
        {
            DBFile_Path = UsersTableClass.GetClientDBFile(UserName);

            if (!DBFile_Exist) { CreateAppliedDataBase(); }
            AppliedConnection = new("Data Source=" + DBFile_Path);
        }

        public ConnectionClass(string _UserName)
        {
            DBFile_Path = UsersTableClass.GetClientDBFile(_UserName);

            if (!DBFile_Exist) { CreateAppliedDataBase(); }
            AppliedConnection = new("Data Source=" + DBFile_Path);                      // Established a Connection with Database File
            AppliedConnection.Open();                                                                   

        }

        private void CreateAppliedDataBase()
        {
            CreateTablesClass TableClass = new();
            SQLiteConnection.CreateFile(DBFile_Path);
            SQLiteCommand _Command = new();
            SQLiteConnection _Connection = new SQLiteConnection("Data Source=" + DBFile_Path); _Connection.Open();

            //_Command = new SQLiteCommand(TableClass.tb_Users, _Connection); _Command.ExecuteNonQuery();
            //_Command = new SQLiteCommand(TableClass.AddAdmin, _Connection); _Command.ExecuteNonQuery();
            //_Command = new SQLiteCommand(TableClass.tb_Profile, _Connection); _Command.ExecuteNonQuery();
            //_Command = new SQLiteCommand(TableClass.tb_Roles, _Connection); _Command.ExecuteNonQuery();
            //_Command = new SQLiteCommand(TableClass.AddRole1, _Connection); _Command.ExecuteNonQuery();
        }
    }           // END()

    public class AppliedUsersClass
    {
        public string AppliedUsersFile = ".\\wwwroot\\SQLiteDB\\AppliedUsers.db";                   // Applied Users ID & PW File.
        public DataTable UsersTable = new DataTable();                                  
        public readonly DataView UserView = new DataView();
        public AppliedUsersClass()
        {
            UsersTable = UsersDataTable();
            UserView = UsersTable.AsDataView();
        }

        public DataTable UsersDataTable()
        {
            SQLiteConnection UsersConnection = new SQLiteConnection();

            if (File.Exists(AppliedUsersFile))
            {
                UsersConnection = new SQLiteConnection("Data Source=" + AppliedUsersFile);
                UsersConnection.Open();

                SQLiteCommand _Command = new("SELECT * FROM [Users]", UsersConnection);
                SQLiteDataAdapter _Adapter = new(_Command);
                DataSet _DataSet = new();
                _Adapter.Fill(_DataSet, "UsersTable");

                if (_DataSet.Tables.Count == 1)
                {
                    UsersTable = _DataSet.Tables[0];
                }
            }
            return UsersTable;
        }

        internal string GetClientDBFile(string _User)
        {
            if(_User == null) { return ".\\wwwroot\\SQLiteDB\\Applied.db"; }

            switch (_User.ToUpper())
            {
                case "ADMIN":
                    return ".\\wwwroot\\SQLiteDB\\Applied.db";

                case "AMCORP":
                    return ".\\wwwroot\\SQLiteDB\\Amcorp.db";

                case "ALTAMASH":
                    return ".\\wwwroot\\SQLiteDB\\Altamash.db";

                case "GUEST":
                    return ".\\wwwroot\\SQLiteDB\\Guest.db";

                case "WINMARK":
                    return ".\\wwwroot\\SQLiteDB\\Winmark.db";

                default:
                    return ".\\wwwroot\\SQLiteDB\\Applied.db";
            }
        }
    }
}



