using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Accounts
{
    [Authorize]
    public class ReceiptListModel : PageModel
    {
        [BindProperty]
        public ReceiptsFilter Variables { get; set; }
        public string UserName => User.Identity.Name;
        
        
        public void OnGet()
        {
            GetKeys();

        }

        public IActionResult OnPostRefresh()
        {
            SetKeys();
            return RedirectToPage();
        }


        public void SetKeys()
        {
            AppRegistry.SetKey(UserName, "rcptFrom", Variables.From, KeyType.Date);
            AppRegistry.SetKey(UserName, "rcptTo", Variables.To, KeyType.Date);
            AppRegistry.SetKey(UserName, "rcptAccount", Variables.Account, KeyType.Number);
            AppRegistry.SetKey(UserName, "rcptPayer", Variables.Payer, KeyType.Number);
        }

        public void GetKeys()
        {
            Variables = new()
            {
                From = AppRegistry.GetDate(UserName, "rcptFrom"),
                To = AppRegistry.GetDate(UserName, "rcptTo"),
                Account = AppRegistry.GetNumber(UserName, "rcptAccount"),
                Payer = AppRegistry.GetNumber(UserName, "rcptPayer")
            };

        }

        
    }

    public class ReceiptsFilter
    {
        
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Payer { get; set; }
        public int Account { get; set; }
        public int Project { get; set; }
        public int Employee { get; set; }
        
    }
}
