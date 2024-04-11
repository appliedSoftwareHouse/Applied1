using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text;

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
                Rpt_Type = AppRegistry.GetText(UserName, "sp-RptType"),
                ShowDetail = AppRegistry.GetBool(UserName, "sp-Detailed")
            };

            var Date1 = Variables.Rpt_Date1.AddDays(-1).ToString(AppRegistry.DateYMD);
            var Date2 = Variables.Rpt_Date2.AddDays(1).ToString(AppRegistry.DateYMD);
            var _Filter = $"Date(Vou_Date) > Date('{Date1}') AND Date(Vou_Date) < Date('{Date2}')";
            //MyTable = DataTableClass.GetTable(UserName, SQLQuery.StockInHand(_Filter));

            MyTable = GenerateStockInHand(_Filter);


        }
        #endregion

        #region Generate Stock in Hand Data
        public DataTable GenerateStockInHand(string _Filter)
        {
            return StockLedgers.GetStockInHand(UserName, _Filter);
        }
        #endregion

        #region Refresh
        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "sp-Date1", Variables.Rpt_Date1, KeyType.Date);
            AppRegistry.SetKey(UserName, "sp-Date2", Variables.Rpt_Date2, KeyType.Date);
            AppRegistry.SetKey(UserName, "sp-RptType", Variables.Rpt_Type, KeyType.Text);
            AppRegistry.SetKey(UserName, "sp-Detailed", Variables.ShowDetail, KeyType.Boolean);
            return RedirectToPage();
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
            public DateTime Rpt_Date1 { get; set; }
            public DateTime Rpt_Date2 { get; set; }
            public string Rpt_Type { get; set; }
            public bool ShowDetail { get; set; }
        }
        #endregion
    }
}

