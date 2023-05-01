using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Data.SQLite;
using System.Text;
using static Applied_WebApplication.Data.AppFunctions;
using DataSet = System.Data.DataSet;

namespace Applied_WebApplication.Data
{
    public class ConnectionClass : IdentityUser
    {
        public AppliedDependency AppGlobal = new();
        public SQLiteConnection AppliedConnection = new();
        public SQLiteConnection TempConnection = new();
        private readonly AppliedUsersClass UsersTableClass = new();
        private UserProfile uProfile = new();
        public string DBFile_Path { get; set; }
        
        public string DBFile_Name = "";

        public bool DBFile_Exist => File.Exists(DBFile_Path); 

        public ConnectionClass(string _UserName)
        {
            uProfile = new(_UserName);
            DBFile_Path = uProfile.

            //if (!DBFile_Exist) { CreateAppliedDataBase(); }
            AppliedConnection = new("Data Source=" + DBFile_Path);                      // Established a Connection with Database File
            AppliedConnection.Open();

            TempConnection = new("DataSource=" + AppGlobal.AppDBTempPath);
            TempConnection.Open();

        }

        private void CreateAppliedDataBase()
        {
            //CreateTablesClass TableClass = new();
            SQLiteConnection.CreateFile(DBFile_Path);
            //SQLiteCommand _Command = new();
            SQLiteConnection _Connection = new SQLiteConnection("Data Source=" + DBFile_Path); _Connection.Open();
        }


        public static SQLiteConnection AppConnection(string UserName)
        {
            ConnectionClass _ConnectionClass = new(UserName);                           // Establishe Connection Class
            return _ConnectionClass.AppliedConnection;
        }

        public static SQLiteConnection AppTempConnection(string UserName)
        {
            ConnectionClass _ConnectionClass = new(UserName);                           // Establishe Connection Class
            return _ConnectionClass.TempConnection;
        }


    }           // END()




    public class AppliedUsersClass
    {
        public string AppliedUsersFile = AppGlobals.UserDBPath;
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
        internal static string GetClientDBFile(string _User)
        {
            UserProfile userProfile = new(_User);
            if (userProfile.DBFilePath == null)
            {
                userProfile = new("Guest");                                             // If Database File not found switch to Guest Profile.
            }
            return userProfile.DBFilePath;
        }
    }
}



