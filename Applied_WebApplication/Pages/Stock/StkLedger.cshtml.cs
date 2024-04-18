using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Runtime.CompilerServices;

namespace Applied_WebApplication.Pages.Stock
{
    public class StkLedgerModel : PageModel
    {
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
    }
}

