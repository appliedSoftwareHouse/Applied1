using AppReporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SQLite;

namespace Applied_WebApplication.Pages.Stock
{
    [Authorize]
    public class StockPositionModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public DataTable MyTable { get; set; }
        public string UserName => User.Identity.Name;

        #region GET
        public void OnGet()
        {
            Variables = new()
            {
                Rpt_Date1 = AppRegistry.GetDate(UserName, "sp-Date1"),
                Rpt_Date2 = AppRegistry.GetDate(UserName, "sp-Date2"),
                Rpt_Type = AppRegistry.GetText(UserName, "sp-RptType")
            };

            var Date1 = Variables.Rpt_Date1.AddDays(-1).ToString(AppRegistry.DateYMD);
            var Date2 = Variables.Rpt_Date2.AddDays(1).ToString(AppRegistry.DateYMD);
            var _Filter = $"Date(Vou_Date) > Date('{Date1}') AND Date(Vou_Date) < Date('{Date2}')";
            MyTable = DataTableClass.GetTable(UserName, SQLQuery.StockInHand(_Filter));
        
        
        }
        #endregion

        #region Refresh
        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "sp-Date1", Variables.Rpt_Date1, KeyType.Date);
            AppRegistry.SetKey(UserName, "sp-Date2", Variables.Rpt_Date1, KeyType.Date);
            AppRegistry.SetKey(UserName, "sp-RptType", Variables.Rpt_Type, KeyType.Text);
            return RedirectToPage();
        }
        #endregion

        #region Refresh
        public IActionResult OnPostPrint()
        {

            return Page();
        }
        #endregion


        #region Variables
        public class Parameters
        {
            public DateTime Rpt_Date1 { get; set; }
            public DateTime Rpt_Date2 { get; set; }
            public string Rpt_Type { get; set; }
        }
        #endregion
    }
}

