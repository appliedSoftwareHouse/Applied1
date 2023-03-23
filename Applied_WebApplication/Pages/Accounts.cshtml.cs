using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages
{

    public class AccountsModel : PageModel
    {
        public List<Message> ThisMessages { get; set; } = new();
        public string UserName => User.Identity.Name;

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

        public IActionResult OnPostOBalPostCompany()
        {
            ThisMessages = PostingClass.PostOpeningBalanceCompany(UserName);
            return Page();
        }

        public IActionResult OnPostOBalPostStock()
        {
            //ThisMessages = PostingClass.PostOpeningBalanceStock(UserName);
            return RedirectToPage("./Stock/OpeningStock");
        }

    }
}

