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
        public string Filter { get; set; } = string.Empty;
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
            //Filter = $"Date(Vou_Date) > Date('{Date1}') AND Date(Vou_Date) < Date('{Date2}')";
            Filter = $"Date(Vou_Date) < Date('{Date2}') ";

            AppRegistry.SetKey(UserName, "sp-Filter", Filter, KeyType.Text);
            AppRegistry.SetKey(UserName, "Stock_COA", "6,7,8,43,44,45", KeyType.Text);

            MyTable = GenerateStockInHand(Filter);
        }
        #endregion

        #region Generate Stock in Hand Data
        public DataTable GenerateStockInHand(string _Filter)
        {
            var StockClass = new StockLedgersClass(UserName);
            return StockClass.GetStockInHand();
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

        public IActionResult OnPostStockLed(int StockID)
        {
            AppRegistry.SetKey(UserName, "sp-StockID", StockID, KeyType.Number);
            Filter = AppRegistry.GetText(UserName, "sp-Filter");   // sp=Stock Position
            return RedirectToPage("./StkLedger");
            
        }


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

