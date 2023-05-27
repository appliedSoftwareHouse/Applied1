using AppReporting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.Win32;
using System.Data;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class ExpenseSheetModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        string UserName => User.Identity.Name;
        public DataTable MyTable { get; set; } = new DataTable();
        public ReportClass.ReportFilters Parameters1 { get; set; }

        public void OnGet()
        {
            Variables = new();
            MyTable = GetMyTable();


        }

        public IActionResult OnPostPrint()
        {
            Parameters1 = new();
            AppRegistry.SetKey(UserName, "Sheet_No", Variables.ExpenseSheetNo, KeyType.Text);
            return RedirectToPage("PrintReport", "ExpenseSheet");
        }



        private DataTable GetMyTable() 
        {
            DataTable _Table = DataTableClass.GetTable(UserName, SQLQuery.ExpenseSheetList(), "Sheet_No");
            return _Table;
        }


        public class Parameters
        {
            public string ExpenseSheetNo {  get; set; }
        }

    }
}
