
using System.Data;
using System.Security.Claims;


namespace Applied_WebApplication.Data
{
    public class UserProfile
    {
        public ClaimsIdentity UserIdentity;
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string DBFilePath { get; set; }
        public string DBFileName { get; set; }
        public string Company { get; set; }
        public string Designation { get; set; }
        
        public UserProfile()
        {
            if (Name == null)
            {
                UserID = "Guest";
                Name = "Test UserName";
                Password = "Guest";
                Email = "info@jahangir.com";
                Role = "Guest";
                Company = "Applied Software House";
                Designation = "Applied Account User";
                DBFilePath = "";
                }
        }

        public UserProfile(string UserName)
        {
            AppliedUsersClass UserClass = new();
            DataRow _Row = UserClass.UserRecord(UserName);
            if (_Row != null)
            {
                UserID = _Row["UserID"].ToString();
                Name = _Row["DisplayName"].ToString();
                Password = _Row["Password"].ToString();
                Email = _Row["UserEmail"].ToString();
                Role = _Row["Role"].ToString();
                Company = _Row["Company"].ToString();
                Designation = _Row["Designation"].ToString();
                DBFilePath = _Row["DataFile"].ToString();
                DBFileName = DBFilePath.Replace(".\\wwwroot\\SQLiteDB\\", string.Empty);          // Replace Path with string.Empty

            }
        }

        public UserProfile(DataRow _Row)
        {
            if (_Row != null)
            {
                UserID = _Row["UserID"].ToString();
                Name = _Row["DisplayName"].ToString();
                Password = _Row["Password"].ToString();
                Email = _Row["UserEmail"].ToString();
                Role = _Row["Role"].ToString();
                Company = _Row["Company"].ToString();
                Designation = _Row["Designation"].ToString();
                DBFilePath = _Row["DataFile"].ToString();

            }
        }

        public static string GetCompanyName(ClaimsPrincipal _ClaimPrincipal)
        {
            // Get a Company name to display at main page of the App.
            string Result = "Applied Software House?";
            foreach (Claim _Claim in _ClaimPrincipal.Claims)
            {
                if (_Claim.Type == "Company")
                {
                    Result = _Claim.Value;
                    break;
                }
            }
            return Result;
        }

        public static string GetUserClaim(ClaimsPrincipal _ClaimPrincipal, string Key)
        {
            // Get user claim value.
            string Result = string.Empty;
            foreach (Claim _Claim in _ClaimPrincipal.Claims)
            {
                if (_Claim.Type == Key)
                {
                    Result = _Claim.Value;
                    break;
                }
            }
            return Result;
        }

    }
}
