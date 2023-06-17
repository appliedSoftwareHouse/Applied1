using AppReporting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppReportClass;
using System.Data;
using Microsoft.Reporting.NETCore;
using static Applied_WebApplication.Data.AppFunctions;
using Microsoft.ReportingServices.Interfaces;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class ExpenseSheetModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        string UserName => User.Identity.Name;
        public DataTable MyTable { get; set; } = new DataTable();

        public void OnGet()
        {
            Variables = new();
            MyTable = GetExpenseSheetList();
        }

        #region Buttons
        public IActionResult OnPostPrint()
        {
            AppRegistry.SetKey(UserName, "Sheet_No", Variables.ExpenseSheetNo, KeyType.Text);
            return RedirectToPage("PrintReport", "ExpenseSheet", new { ReportType.PDF});
        }

        public IActionResult OnPostExcel()
        {
            AppRegistry.SetKey(UserName, "Sheet_No", Variables.ExpenseSheetNo, KeyType.Text);
            return RedirectToPage("PrintReport", "ExpenseSheet", new { _ReportType=ReportType.Excel});
        }

        public IActionResult OnPostWord()
        {
            AppRegistry.SetKey(UserName, "Sheet_No", Variables.ExpenseSheetNo, KeyType.Text);
            return RedirectToPage("PrintReport", "ExpenseSheet", new { _ReportType = ReportType.Word });
        }

        public IActionResult OnPostHTML()
        {
            AppRegistry.SetKey(UserName, "Sheet_No", Variables.ExpenseSheetNo, KeyType.Text);
            return RedirectToPage("PrintReport", "ExpenseSheet", new { _ReportType = ReportType.HTML });
        }
        #endregion

        public IActionResult OnPostReLoad()
        {
            AppRegistry.SetKey(UserName, "Sheet_No", Variables.ExpenseSheetNo, KeyType.Text);
            return RedirectToPage();
        }

        private DataTable GetExpenseSheetList() 
        {
            DataTable _Table = DataTableClass.GetTable(UserName, SQLQuery.ExpenseSheetList(), "Sheet_No");
            return _Table;
        }


        public class Parameters
        {
            public string ExpenseSheetNo {  get; set; }
            public string ReportType { get; set; }
        }

    }
}
