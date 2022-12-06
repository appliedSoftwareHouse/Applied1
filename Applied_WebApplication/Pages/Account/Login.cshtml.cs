using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Linq;

namespace Applied_WebApplication.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential MyCredential { get; set; }

        private DataTableClass tb_User = new DataTableClass("Users");

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            tb_User.View_Filter = "UserID='" + MyCredential.Username + "'";                 // Get a Record for the sucessful logged user.

            tb_User.MyDataView.RowFilter = "UserID='" + MyCredential.Username.Trim() +"'";

            if (tb_User.MyDataView.Count == 1)
            {
                UserProfile uprofile = new(tb_User.UserRow(MyCredential.Username));                                             // Get a User Profile from User Record in DataTable.

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
                    new Claim(ClaimTypes.Role,uprofile.Role.ToString()),
                    new Claim("Company", uprofile.Company),
                    new Claim("Designation", uprofile.Designation)
                    };

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
    }
}
