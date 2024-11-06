using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppReportClass;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace Applied_WebApplication.Pages.ReportPrint
{
    [Authorize]
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

        public IActionResult OnPostReLoad()
        {
            AppRegistry.SetKey(UserName, "Sheet_No", Variables.ExpenseSheetNo, KeyType.Text);
            return RedirectToPage();
        }

        public IActionResult OnPostPrint(ReportType Option)
        {
            AppRegistry.SetKey(UserName, "Sheet_No", Variables.ExpenseSheetNo, KeyType.Text);
            return RedirectToPage("PrintReport", "ExpenseSheet", new { _ReportType = Option });
        }

        public IActionResult OnPostExpenseGroup(ReportType Option)
        {
            return RedirectToPage("PrintReport", "ExpenseGroup", new { _ReportType=Option });
        }

        private DataTable GetExpenseSheetList() 
        {
            DataTable _Table = DataTableClass.GetTable(UserName, SQLQuery.ExpenseSheetList(), "Sheet_No");
            return _Table;
        }
        #endregion

        public class Parameters
        {
            public string ExpenseSheetNo {  get; set; }
            public string ReportType { get; set; }
        }

    }
}
