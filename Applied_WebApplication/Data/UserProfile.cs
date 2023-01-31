using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;
using System.Data;
using System.Security.Claims;
using System.Security.Principal;


namespace Applied_WebApplication.Data
{
    public class UserProfile
    {
        public ClaimsIdentity UserIdentity;
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string DBFilePath { get; set; }
        public string DBFileName { get; set; }
        public string Company { get; set; }
        public string Designation { get; set; }
        public string DateFormat { get; set; }
        public string CurrencyFormat { get; set; }
        private string default_DateFormat = "dd/MM/yyyy";
        private string default_CurrencyFormat = "N";

        public UserProfile()
        {
            if (UserName == null)
            {
                UserName = "Test UserName";
                Password = "Guest";
                Email = "info@jahangir.com";
                Role = "Guest";
                Company = "Applied Software House";
                Designation = "Applied Account User";
                DBFilePath = "";
                DateFormat = default_DateFormat;
                CurrencyFormat = default_CurrencyFormat;
            }
        }

        public UserProfile(string UserName)
        {
            AppliedUsersClass UserClass = new();
            DataRow _Row = UserClass.UserRecord(UserName);
            if (_Row != null)
            {
                UserID = _Row["UserID"].ToString();
                UserName = _Row["DisplayName"].ToString();
                Password = _Row["Password"].ToString();
                Email = _Row["UserEmail"].ToString();
                Role = _Row["Role"].ToString();
                Company = _Row["Company"].ToString();
                Designation = _Row["Designation"].ToString();
                DBFilePath = _Row["DataFile"].ToString();
                if (string.IsNullOrEmpty(_Row["DateFormat"].ToString())) { DateFormat = default_DateFormat; } else { DateFormat = _Row["DateFormat"].ToString(); }
                if (string.IsNullOrEmpty(_Row["CurrencyFormat"].ToString())) { CurrencyFormat = default_CurrencyFormat; } else { CurrencyFormat = _Row["CurrencyFormat"].ToString(); }
                DBFileName = DBFilePath.Replace(".\\wwwroot\\SQLiteDB\\", string.Empty);          // Replace Path with string.Empty

            }
        }

        public UserProfile(DataRow _Row)
        {
            if (_Row != null)
            {
                UserID = _Row["UserID"].ToString();
                UserName = _Row["DisplayName"].ToString();
                Password = _Row["Password"].ToString();
                Email = _Row["UserEmail"].ToString();
                Role = _Row["Role"].ToString();
                Company = _Row["Company"].ToString();
                Designation = _Row["Designation"].ToString();
                DBFilePath = _Row["DataFile"].ToString();
                if (string.IsNullOrEmpty(_Row["DateFormat"].ToString())) { DateFormat = default_DateFormat; } else { DateFormat = _Row["DateFormat"].ToString(); }
                if (string.IsNullOrEmpty(_Row["CurrencyFormat"].ToString())) { CurrencyFormat = default_CurrencyFormat; } else { CurrencyFormat = _Row["CurrencyFormat"].ToString(); }

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
