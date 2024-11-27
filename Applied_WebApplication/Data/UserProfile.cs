
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
        public string DataBaseFile { get; set; }
        public string DataBaseTempPath { get; set; }
        public string DataBaseTempFile { get; set; }
        public string Company { get; set; }
        public string Designation { get; set; }
        public string RoleString = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";



        public readonly AppliedDependency AppGlobal;
        
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
                DataBaseFile = $"{AppGlobal.DataBasePath}Applied.db";
                //DBFilePath = "";
                //DBFileTempPath = "";
                }
        }

        public UserProfile(string UserName)
        {
            AppliedDependency AppGlobal = new();
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
                DataBaseFile = _Row["DataFile"].ToString();
                DataBaseTempPath = $"{AppGlobal.AppDBTempPath}{UserID}\\";
                DataBaseTempFile = $"{DataBaseTempPath}{UserID}Temp.db";
                RoleString = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
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
                DataBaseFile = _Row["DataFile"].ToString();
                //DataBaseTempFile = $"{AppGlobal.AppDBTempPath}{UserID}\\{UserID}.db";
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

        public static string GetUserRole(ClaimsPrincipal _ClaimPrincipal)
        {
            // Get a user role from user identity.
            string Result = "Client";
            foreach (Claim _Claim in _ClaimPrincipal.Claims)
            {
                if (_Claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
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

                if(_Claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                {
                    if (Key == "Role") { return _Claim.Value; }
                }

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
