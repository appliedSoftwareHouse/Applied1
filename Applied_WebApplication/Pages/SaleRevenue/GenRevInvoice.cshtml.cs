using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.SaleRevenue
{
    


    public class GenRevInvoiceModel : PageModel
    {
        
        [BindProperty]
        public Parameters Variables { get; set; }
        public string UserName => User.Identity.Name;
        public DataTable MyTable { get; set; } = new DataTable();

        public void OnGet()
        {
            Variables = new();
            Variables.RevSheetID = "";

            MyTable = GetRevenueSheetList();
        }

        private DataTable GetRevenueSheetList()
        {
            DataTable _Table = DataTableClass.GetTable(UserName, SQLQuery.ExpenseSheetList(), "Sheet_No");
            return _Table;
        }

        public IActionResult OnPostGenerate(string _RevSheetID) 
        {
            Variables = new();
            Variables.RevSheetID = _RevSheetID;

            MyTable = GetRevenueSheetList();

            return Page();

        }

    }

    public class Parameters
    {
        public string RevSheetID { get; set; }
        

    }

}
