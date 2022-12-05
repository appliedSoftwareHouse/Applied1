using System.Data;
using System.Security.Principal;

namespace Applied_WebApplication.Data
{
    public class UserProfile
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public string DataFile {get; set;}
        public string Company { get; set; }

        public UserProfile(DataRow _Row)
        {
            if(_Row != null)
            {
                UserID = _Row["UserID"].ToString();
                UserName = _Row["DisplayName"].ToString();
                Password = _Row["Password"].ToString();
                Email = _Row["UserEmail"].ToString();
                Role = int.Parse(_Row["Role"].ToString());
                Company = _Row["Company"].ToString();
            }
        }
        public static string GetUserIdentity(string _ClaimType, System.Security.Principal.IIdentity. _Identity)
        {
            foreach (var claim in _Identity.Claims)
            {
                string _Claim = claim.Value;
                string _Type = claim.Type;
                if(_Type=="Company") { return _Claim; } 
            }
            return "Applied Software House";
        }
    }
}
