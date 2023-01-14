using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ServiceModel.Security;

namespace Applied_WebApplication.Pages.Accounts
{
    public class ChequeListModel : PageModel
    {
        [BindProperty]
        public string ChqCode { get; set; }
        public DataTableClass Table;
        public string UserName;
        

        public void OnGet()
        {
            UserName = User.Identity.Name;
            Table = new DataTableClass(UserName, Tables.WriteCheques);
            Table.MyDataView.RowFilter = "TranType=1";

        }

        public IActionResult OnGetAdd()
        {
            return RedirectToPage("/Accounts/WriteCheque", routeValues: new { ChqCode = "" });
            
        }

        public IActionResult OnGetEdit(string ChqCode)
        {
            return RedirectToPage("/Accounts/WriteCheque", routeValues: new { ChqCode});

        }


    }
}
