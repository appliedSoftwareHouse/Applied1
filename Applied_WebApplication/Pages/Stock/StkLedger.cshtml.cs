using AppReportClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Runtime.CompilerServices;

namespace Applied_WebApplication.Pages.Stock
{
    public class StkLedgerModel : PageModel
    {
        [BindProperty]

        public string UserName => User.Identity.Name;
        public string Filter => AppRegistry.GetText(UserName, "sp-Filter");   // sp=Stock Position
        public int StockID => AppRegistry.GetNumber(UserName, "sp-StockID");   // sp=Stock Position
        public DataTable StockLedger { get; set; }


        public void OnGet()
        {
            var StockClass = new StockLedgersClass(UserName);
            StockClass.Filter = Filter;
            StockClass.StockID = StockID;
            StockLedger = StockClass.GenerateLedgerTable();
        }


        public IActionResult OnPostPrint(ReportType Option)
        {
            SetKeys();
            return RedirectToPage("/ReportPrint/PrintReport", "StockLedger", new { RptType = Option });

            //stkLedHead1
        }

        private void SetKeys()
        {
            var _Title = AppFunctions.GetTitle(UserName, Tables.Inventory, StockID);
            var _UptoDate = AppRegistry.GetDate(UserName, "stkLedDate");
            var _Heading1 = $"Stock Ledger : {_Title}";
            var _Heading2 = $"Position as on {_UptoDate.ToString(AppRegistry.FormatDate)}";
            AppRegistry.SetKey(UserName, "stkLedger", "StockLedger", KeyType.Text);
            AppRegistry.SetKey(UserName, "stkLedHead1", _Heading1, KeyType.Text);
            AppRegistry.SetKey(UserName, "stkLedHead2", _Heading2, KeyType.Text);
            AppRegistry.SetKey(UserName, "stkLedID", StockID, KeyType.Number);
            AppRegistry.SetKey(UserName, "stkLedTitle", _Title, KeyType.Text);

        }


    }
}

