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
        public string DataFile {get; set;}
        public string Company { get; set; }
        public string Designation { get; set; }

        public UserProfile()
        {
            UserID = "Guest";
            UserName = "My Sweet Guest ";
            Password = "Guest";
            Email = "info@jahangir.com";
            Role = "Guest";
            Company = "Applied Software House";
        }

        public UserProfile(DataRow _Row)
        {
            if(_Row != null)
            {
                UserID = _Row["UserID"].ToString();
                UserName = _Row["DisplayName"].ToString();
                Password = _Row["Password"].ToString();
                Email = _Row["UserEmail"].ToString();
                Role = _Row["Role"].ToString();
                Company = _Row["Company"].ToString();
                
            }
        }

        public string GetIdentityValue (string _ClaimValue, System.Security.Claims.ClaimsIdentity _UserIdentity)
        {
            foreach (Claim _Claim in _UserIdentity.Claims)
            {
                string _Value = _Claim.Value.ToString();
                string _Type    = _Claim.Type.ToString();

                if (_Claim.Type == "Applied")
                {
                    Company = _Claim.Value;
                }
            }

            return Company;
        }

        
    }
}
