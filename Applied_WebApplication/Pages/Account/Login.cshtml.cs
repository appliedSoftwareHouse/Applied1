using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

namespace Applied_WebApplication.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential MyCredential { get; set; }
        public CredentialSession UserSession = new();

        private AppliedUsersClass UserTableClass = new();                       // Make Connectiona and get Applied Users Table.
        

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            
            UserTableClass.UserView.RowFilter = "UserID='" + MyCredential.Username + "'";                 // Get a Record for the sucessful logged user.

            if (UserTableClass.UserView.Count == 1)
            {
                UserProfile uprofile = new(UserTableClass.UserView[0].Row);                                             // Get a User Profile from User Record in DataTable.

                if(uprofile.Company == null) { uprofile.Company = "Applied Software House"; }
                if(uprofile.Designation== null) { uprofile.Designation = "Guest"; }

                if (MyCredential.Username == uprofile.UserID && MyCredential.Password == uprofile.Password)
                {
                    var Claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Name, uprofile.UserID),
                    new Claim(ClaimTypes.GivenName, uprofile.UserName),
                    new Claim(ClaimTypes.Surname, uprofile.UserName),
                    new Claim(ClaimTypes.Email, uprofile.Email),
                    new Claim(ClaimTypes.Role,uprofile.Role),
                    new Claim("Company", uprofile.Company),
                    new Claim("Designation", uprofile.Designation),
                    new Claim("DBFilePath", uprofile.DBFilePath)
                    };

                    UserSession.UserName = uprofile.UserID;
                    UserSession.UserID = uprofile.UserID;
                    UserSession.CompanyName = uprofile.Company;
                    UserSession.SessionID = Claims.GetType().GUID.ToString();

                    var Identity = new ClaimsIdentity(Claims, "MyCookieAuth");
                    ClaimsPrincipal MyClaimsPrincipal = new ClaimsPrincipal(Identity);
                    await HttpContext.SignInAsync("MyCookieAuth", MyClaimsPrincipal);
                    return RedirectToPage("/Index");
                }
            }
            return Page();
        }

        public class Credential
        {
            [Required]
            public string Username { get; set; } = "User Name";

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public class CredentialSession
        {
            public string UserID { get; set; }
            public string UserName { get; set; }
            public string CompanyName { get; set; }
            public string SessionID { get; set; } 
            public DateTime LoginDatetime { get; set; }
            public DateTime LogoutDateTime { get; set; }
        }
    }
}
