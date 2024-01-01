
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


        #region Get
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
                DateFormat = AppRegistry.GetText(UserName, "FMTDate"),
                SalesReportRDL = AppRegistry.GetText(UserName, "SalesReportRDL"),

                BPay_Stock = AppRegistry.GetNumber(UserName, "BPay_Stock"),
                BPay_Tax = AppRegistry.GetNumber(UserName, "BPay_Tax"),
                BPay_Payable = AppRegistry.GetNumber(UserName, "BPay_Payable"),

                BRec_Stock = AppRegistry.GetNumber(UserName, "BRec_Stock"),
                BRec_Tax = AppRegistry.GetNumber(UserName, "BRec_Tax"),
                BRec_Receivable = AppRegistry.GetNumber(UserName, "BRec_Receivable"),
                
                CompanyGLs = AppRegistry.GetText(UserName, "CompanyGLs"),

                CashBookNature = AppRegistry.GetNumber(UserName, "CashBkNature"),
                BankBookNature = AppRegistry.GetNumber(UserName, "BankBkNature")

            };
        }
        #endregion
        #region Save
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

            AppRegistry.SetKey(UserName, "BPay_Stock", Variables.BPay_Stock, KeyType.Number);
            AppRegistry.SetKey(UserName, "BPay_Tax", Variables.BPay_Tax, KeyType.Number);
            AppRegistry.SetKey(UserName, "BPay_Payable", Variables.BPay_Payable, KeyType.Number);

            AppRegistry.SetKey(UserName, "BRec_Stock", Variables.BRec_Stock, KeyType.Number);
            AppRegistry.SetKey(UserName, "BRec_Tax", Variables.BRec_Tax, KeyType.Number);
            AppRegistry.SetKey(UserName, "BRec_Receivable", Variables.BRec_Receivable, KeyType.Number);
            AppRegistry.SetKey(UserName, "SalesReportRDL", Variables.SalesReportRDL, KeyType.Text);
            AppRegistry.SetKey(UserName, "CompanyGLs", Variables.CompanyGLs, KeyType.Text);

            AppRegistry.SetKey(UserName, "CashBkNature", Variables.CashBookNature, KeyType.Number);
            AppRegistry.SetKey(UserName, "BankBkNature", Variables.BankBookNature, KeyType.Number);

            return RedirectToPage();
        }
        #endregion
        #region Variables
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

            public int BPay_Stock { get; set; }
            public int BPay_Tax { get; set; }
            public int BPay_Payable { get; set; }

            public int BRec_Stock { get; set; }
            public int BRec_Tax { get; set; }
            public int BRec_Receivable { get; set; }
            public string SalesReportRDL { get; set; }
            public string CompanyGLs { get; set; }
            public int CashBookNature { get; set; }
            public int BankBookNature { get; set; }

        }
        #endregion
        #region SQL Query
        public IActionResult OnPostSQLQuery()
        {
            return RedirectToPage("/Applied/SQLExecute");
        }

        #endregion

    }

}


