using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class CompanyBalancesModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public DataTable MyTable { get; set; }
        public string UserName => User.Identity.Name;



        public void OnGet()
        {
            Variables = new();

        }

        #region Refresh
        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "DueBalance", Variables.ReportDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "DueRptType", Variables.ReportType, KeyType.Text);

            var _ReportDate = Variables.ReportDate;
            var _COA_List = AppRegistry.GetText(UserName, "CompanyGLs");
            var _Filter = $"";
            MyTable = DataTableClass.GetTable(UserName, SQLQuery.CompanyBalances(_Filter, _COA_List));
            return Page();
        }
        #endregion

        #region Print
        public IActionResult OnPostPrint()
        {

            return Page();
        }
        #endregion

        #region Variables
        public class Parameters
        {
            public DateTime ReportDate { get; set; }
            public string ReportType { get; set; }
        }
        #endregion

    }
}