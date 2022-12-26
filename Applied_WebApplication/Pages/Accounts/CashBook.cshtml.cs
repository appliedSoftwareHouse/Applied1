using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Accounts
{
    public class CashBookModel : PageModel
    {
        public bool IsSelected { get; set; } = false;
        public DataTableClass Ledger;

        public void OnGet(string UserName, int id)
        {
            Ledger = new(UserName, Tables.Ledger);
            Ledger.MyDataView.Sort = "Vou_Date";
            Ledger.MyDataView.RowFilter = "ID=" + id;


        }

      

    }
}
