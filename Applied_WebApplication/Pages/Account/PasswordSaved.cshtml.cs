using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Account
{
    [Authorize]
    public class PasswordSavedModel : PageModel
    {
        [BindProperty]
        public UserModel Variables { get; set; }


        public void OnGet()
        {
            Variables = new()
            {
                UserName = User.Identity.Name,
                Password = "",
                Repeat = "",
                MyMessage = $"Reset {Variables.UserName} password"
            };
        }

        public IActionResult OnPostSave()
        {

            return Page();
        }

    }

    public class UserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Repeat { get; set; }
        public int attempt { get; set; }
        public string MyMessage { get; set; }

    }

}
