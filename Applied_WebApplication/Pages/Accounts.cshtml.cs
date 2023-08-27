using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages
{
    [Authorize]
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
            ThisMessages.Add(SetMessage("Posting of Opening Balance of Comapny (Suppliers / Cusotmers) done!!!!"));
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

