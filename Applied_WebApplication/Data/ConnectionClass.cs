using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Data.SQLite;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Applied_WebApplication.Data
{
    public class ConnectionClass : IdentityUser
    {

        public SQLiteConnection AppliedConnection = new();
        private AppliedUsersClass UsersTableClass = new();

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
        }
    }           // END()

    public class AppliedUsersClass
    {
        public string AppliedUsersFile = ".\\wwwroot\\SQLiteDB\\AppliedUsers.db";                   // Applied Users ID & PW File.
        public DataTable UsersTable = new DataTable();
        public readonly DataView UserView = new DataView();
        public SQLiteConnection AppliedUserConnection;
        public SQLiteCommand AppliedUserCommand;
        private string AppliedUserCommandText = string.Empty;

        public AppliedUsersClass()
        {
            AppliedUserConnection = new SQLiteConnection("Data Source=" + AppliedUsersFile);
            AppliedUserConnection.Open();
            AppliedUserCommand = new SQLiteCommand();
            UsersTable = UsersDataTable();
            UserView = UsersTable.AsDataView();
        }


        public DataRow UserRecord(string _UserName)
        {
            UserView.RowFilter = "UserID='" + _UserName + "'";
            if (UserView.Count == 1)
            {
                return UserView[0].Row;
            }
            return UsersTable.NewRow();

        }
        public DataTable UsersDataTable()
        {
            AppliedUserCommandText = "SELECT * FROM [Users]";
            AppliedUserCommand = new(AppliedUserCommandText, AppliedUserConnection);

            SQLiteDataAdapter _Adapter = new(AppliedUserCommand);
            DataSet _DataSet = new();
            _Adapter.Fill(_DataSet, "UsersTable");

            if (_DataSet.Tables.Count == 1)
            {
                UsersTable = _DataSet.Tables[0];
            }
            return UsersTable;
        }

        internal string GetClientDBFile(string _User)
        {
            if (_User == null) { return ".\\wwwroot\\SQLiteDB\\Applied.db"; }

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

        public static string GetUserValue(string UserName, string _Column)
        {
            AppliedUsersClass UserClass = new();
            DataRow _Row = UserClass.UserRecord(UserName);
            return _Row[_Column].ToString();
        }

    }
}



