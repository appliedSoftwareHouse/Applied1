
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Applied_WebApplication.Pages.Account
{
    public class SettingModel : PageModel
    {
        #region Setup

        [BindProperty]
        public MyParameters Variables { get; set; }
        public string UserName => User.Identity.Name;

        public string[] CurrencyFormats { get; set; } = new string[] { string.Empty, "#,0", "#.0.00", "#,###0", "#,##0.00" };
        
        public string[] DateFormats { get; set; } = new string[] {string.Empty, "dd-MM-yy", "dd-MM-yyyy", "MM-dd-yy", "MM-dd-yyyy", "dd-MMM-yy", "dd-MMM-yyyy" };
        
        #endregion


        public void OnGet()
        {
            string UserName = User.Identity.Name;
  
            Variables = new()
            {
                OBCompany = AppRegistry.GetNumber(UserName, "OBCompany"),
                OBStock = AppRegistry.GetNumber(UserName, "OBStock"),
                FiscalStart = AppRegistry.GetDate(UserName, "FiscalStart"),
                FiscalEnd = AppRegistry.GetDate(UserName, "FiscalEnd"),
                StockExpiry = AppRegistry.GetNumber(UserName, "StockExpiry"),
                CurrencySign = AppRegistry.GetText(UserName, "CurrencySign"),
                OBDate = AppRegistry.GetDate(UserName, "OBDate"),
                CurrencyFormat = AppRegistry.GetText(UserName, "FMTCurrency"),
                DateFormat = AppRegistry.GetText(UserName, "FMTDate")
                
        };
        }

        public IActionResult OnPostSave()
        {
            
            AppRegistry.SetKey(UserName, "OBCompany", Variables.OBCompany, KeyType.Number);
            AppRegistry.SetKey(UserName, "OBStock", Variables.OBStock, KeyType.Number);
            AppRegistry.SetKey(UserName, "FiscalStart", Variables.FiscalStart, KeyType.Date);
            AppRegistry.SetKey(UserName, "FiscalEnd", Variables.FiscalEnd, KeyType.Date);
            AppRegistry.SetKey(UserName, "StockExpiry", Variables.StockExpiry, KeyType.Number);
            AppRegistry.SetKey(UserName, "CurrencySign", Variables.CurrencySign, KeyType.Text);
            AppRegistry.SetKey(UserName, "OBDate", Variables.OBDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "FMTDate", Variables.DateFormat, KeyType.Text);
            AppRegistry.SetKey(UserName, "FMTCurrency", Variables.CurrencyFormat, KeyType.Text);

            return RedirectToPage();
        }

        public class MyParameters
        {
            public int OBCompany { get; set; }              // COA for opening balance for Company.
            public int OBStock { get; set; }                    // COA for opening balance for Stock.
            public DateTime FiscalStart { get; set; }
            public DateTime FiscalEnd { get; set; }
            public int StockExpiry { set; get; }
            public string CurrencySign { get; set; }
            public string CurrencyFormat { get; set; }
            public string DateFormat { get; set; }
            public DateTime OBDate { get; set; }
          
        }

    }

}


