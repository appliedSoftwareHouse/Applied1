using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages
{

    public class AccountsModel : PageModel
    {
        public List<Message> AppMessages { get; set; } = new();

        public IActionResult OnGetSubmit()
        {
            return RedirectToPage("AccountHead");
        }

        public IActionResult OnPostOBalPost()
        {
            var UserName = User.Identity.Name;
            List<Message> MyMessages = PostingClass.PostOpeningBalance(UserName);
            return RedirectToPage("./Shared/AppMessages", routeValues: new { MyMessages });
        }
    }
}

