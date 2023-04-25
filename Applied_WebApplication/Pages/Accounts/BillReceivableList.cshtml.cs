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
            DataTableClass _Table = new(UserName, Tables.BillReceivable);
            //_Table.MyDataView.RowFilter = "";
            BillReceivable = _Table.MyDataView.ToTable();
        }

        public void OnGetEdit()
        {
        }
        public IActionResult OnPostAdd()
        {
            return RedirectToPage("../Sales/SaleInvoice");
        }
        public IActionResult OnPost(int? id)
        {
            id ??= 0;
            return RedirectToPage("./BillReceivable", routeValues: new { id });

        }
    }
}
