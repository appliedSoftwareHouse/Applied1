using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Accounts
{
    public class BillReceivableListModel : PageModel
    {
        public string UserName => User.Identity.Name;
        public DataTable BillReceivable { get; set; } = new();

        public void OnGet()
        {
        }

        public void OnGetEdit()
        {
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("./BillReceivable");

        }
    }
}
