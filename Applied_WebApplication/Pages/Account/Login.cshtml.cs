using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;



namespace Applied_WebApplication.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential MyCredential { get; set; } = new();
        
        private readonly IConfiguration Config;
        private readonly IWebHostEnvironment Env;
        public string Username { get; set; }
        private AppliedUsersClass UserTableClass { get; set; } = new();                       // Make Connection and get Applied Users Table.
        public string MyMessage { get; set; }

        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }

        public LoginModel(IConfiguration _config, IWebHostEnvironment _Env)
        {
            Config = _config;
            Env = _Env;
        }

        public void OnGet()
        {

            var _Author = Config.GetValue<string>("Author");
            var _imgPath = Config.GetValue<string>("AppPaths:ImagePath");
            

            //var AppPath = AppRegistry.GetText(Username, "AppPaths:ImagePath");

            Image1 = $"{_imgPath}/Accounts.jpg";
            Image2 = $"{_imgPath}/Inventory.jpg";
            Image3 = $"{_imgPath}/Payroll.jpg";
            Image4 = $"{_imgPath}/Taxation.jpg";

            //MyMessage = $"Add Path = {AppPath}";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid) { return Page(); }          // Return the login page if user login unsuccessful.

                UserTableClass.UserView.RowFilter = "UserID='" + MyCredential.Username + "'";                 // Get a Record for the sucessful logged user.

                if (UserTableClass.UserView.Count == 1)
                {
                    UserProfile uprofile = new(UserTableClass.UserView[0].Row);                                             // Get a User Profile from User Record in DataTable.

                    if (uprofile.Company == null) { uprofile.Company = "Applied Software House"; }
                    if (uprofile.Designation == null) { uprofile.Designation = "Guest"; }

                    if (MyCredential.Username == uprofile.UserID && MyCredential.Password == uprofile.Password)
                    {
                        var Claims = new List<Claim>
                        {
                        new Claim(ClaimTypes.Name, uprofile.UserID),
                        new Claim(ClaimTypes.GivenName, uprofile.Name),
                        new Claim(ClaimTypes.Surname, uprofile.Name),
                        new Claim(ClaimTypes.Email, uprofile.Email),
                        new Claim(ClaimTypes.Role,uprofile.Role),
                        new Claim("Company", uprofile.Company),
                        new Claim("Designation", uprofile.Designation),
                        new Claim("DataBaseFile", uprofile.DataBaseFile),
                        new Claim("AppSession", Guid.NewGuid().ToString())
                        };
                            var Identity = new ClaimsIdentity(Claims, "MyCookieAuth");
                            ClaimsPrincipal MyClaimsPrincipal = new ClaimsPrincipal(Identity);
                            await HttpContext.SignInAsync("MyCookieAuth", MyClaimsPrincipal);

                            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                            Encoding.GetEncoding("windows-1252");

                            return RedirectToPage("/Index", uprofile.Name);
                    }
                    else
                    {
                        return RedirectToPage("/Account/NotLogin");
                    }
                }
                else
                {
                    MyMessage = "No Record Found....";
                }


            }
            catch (Exception)
            {

                return RedirectToPage("/Account/NotLogin");
            }

            return Page();
        }

        public class Credential
        {
            [Required(ErrorMessage = "User Name is required")]
            public string Username { get; set; } = "User Name";

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
            public string MyMessage { get; set; } = DateTime.Now.ToString("dd-MMM-yyyyy");
        }
    }
}
