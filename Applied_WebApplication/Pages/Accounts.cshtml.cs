using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages
{

    public class AccountsModel : PageModel
    {
        public List<Message> ThisMessages { get; set; } = new();

        public IActionResult OnGetSubmit()
        {
            return RedirectToPage("AccountHead");
        }

        public IActionResult OnPostOBalPost()
        {
            var UserName = User.Identity.Name;
            ThisMessages = PostingClass.PostOpeningBalance(UserName);
            return Page();
        }
    }
}

