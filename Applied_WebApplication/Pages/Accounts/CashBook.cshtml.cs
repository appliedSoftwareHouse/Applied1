using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Globalization;

namespace Applied_WebApplication.Pages.Accounts
{
    public class CashBookModel : PageModel
    {
        public bool IsSelected { get; set; } = false;
        public DataTableClass Ledger;
        public DataTable CashBook;
        public string BookTitle;
        

        public void OnGet(string UserName, int id)
        {
            IsSelected = true;
            Ledger = new(UserName, Tables.Ledger);
            Ledger.MyDataView.Sort = "Vou_Date";
            Ledger.MyDataView.RowFilter = "ID=" + id;
            CashBook = Ledger.MyDataView.ToTable();
            BookTitle = DataTableClass.GetColumnValue(UserName,Tables.COA,"Title",id);

        }

      

    }
}
