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
        private UserProfile UProfile { get; set; }
        public string DataBaseFile { get; set; }
        public string DataBaseTempFile { get; set; }
        
        public string DBFile_Name = "";

        public bool DBFile_Exist => File.Exists(DataBaseFile); 

        public ConnectionClass(string _UserName)
        {
            UProfile = new(_UserName);
            DataBaseFile = UProfile.DataBaseFile;
            DataBaseTempFile = $"{AppGlobal.AppDBTempPath}{UProfile.UserID}.Temp";

            AppliedConnection = new($"Data Source={DataBaseFile}");                      // Established a Connection with Database File
            AppliedConnection.Open();

            TempConnection = AppTempConnection(UserName);
        }


        public static SQLiteConnection AppConnection(string UserName)
        {
            ConnectionClass _ConnectionClass = new(UserName);                           // Establishe Connection Class
            return _ConnectionClass.AppliedConnection;
        }

        #region Temporary Database Connection

        public static SQLiteConnection AppTempConnection(UserProfile _Profile)
        {
            if (!Directory.Exists(_Profile.DataBaseTempPath)) { Directory.CreateDirectory(_Profile.DataBaseTempPath); }
            if (!File.Exists(_Profile.DataBaseTempFile)) { SQLiteConnection.CreateFile(_Profile.DataBaseTempFile); }

            SQLiteConnection _TempConnection = new($"Data Source={_Profile.DataBaseTempFile}");
            _TempConnection.Open();

            //ConnectionClass _ConnectionClass = new(UserName);                           // Establishe Connection Class
            return _TempConnection;
        }


        public static SQLiteConnection AppTempConnection(string UserName)
        {
            
            UserProfile _Profile = new(UserName);
            if (!Directory.Exists(_Profile.DataBaseTempPath)) { Directory.CreateDirectory(_Profile.DataBaseTempPath); }
            if (!File.Exists(_Profile.DataBaseTempFile)) { SQLiteConnection.CreateFile(_Profile.DataBaseTempFile); }

            SQLiteConnection _TempConnection = new($"Data Source={_Profile.DataBaseTempFile}");
            _TempConnection.Open();

            return _TempConnection;
        }

        #endregion

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
            if (userProfile.DataBaseFile == null)
            {
                userProfile = new("Guest");                                             // If Database File not found switch to Guest Profile.
            }
            return userProfile.DataBaseFile;
        }
    }
}



