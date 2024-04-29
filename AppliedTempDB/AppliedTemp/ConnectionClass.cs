using System.Data.SQLite;

namespace AppliedTemp
{
    public class ConnectionClass
    {
        public UserProfileModel UserProfile { get; set; }
        public string TempDBPath => GetTempDBPath();  // Physical File Name to create 
        public bool TempDBFileExist => IsDBFileExist();
        public SQLiteConnection TempDBConnection { get; set; }
        public bool ConnectionIsValid { get; set; }
        public string Message { get; set; }

        public ConnectionClass() { }
        public ConnectionClass(UserProfileModel _TempUserProfile)
        {
            UserProfile = _TempUserProfile;
            ConnectionIsValid = GetTempFileConnection();
        }

        private bool GetTempFileConnection()
        {
            try
            {
                if (!TempDBFileExist)
                {
                    CreateTempDB();
                }

                if (TempDBFileExist)
                {
                    TempDBConnection = new($"Data Source={TempDBPath}");
                    if (TempDBConnection.State != System.Data.ConnectionState.Open)
                    {
                        TempDBConnection.Open();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }

            return false;
        }

        private string GetTempDBPath()
        {
            if (UserProfile != null)
            {
                return $"{UserProfile.TempFolder}{UserProfile.TempFile}";
            }
            return "";

        }

        private bool IsDBFileExist()
        {
            if (File.Exists(TempDBPath))
            { return true; }
            return false;
        }

        private bool CreateTempDB()
        {
            SQLiteConnection.CreateFile(TempDBPath);
            return IsDBFileExist();
        }

    }
}
