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
                // Opening Balance
                OBCompany = AppRegistry.GetNumber(UserName, "OBCompany"),
                OBStock = AppRegistry.GetNumber(UserName, "OBStock"),

                // General Setup
                FiscalStart = AppRegistry.GetDate(UserName, "FiscalStart"),
                FiscalEnd = AppRegistry.GetDate(UserName, "FiscalEnd"),
                StockExpiry = AppRegistry.GetNumber(UserName, "StockExpiry"),
                
                OBDate = AppRegistry.GetDate(UserName, "OBDate"),
                
                DateFormat = AppRegistry.GetText(UserName, "FMTDate"),
                SalesReportRDL = AppRegistry.GetText(UserName, "SalesReportRDL"),

                // Currency Setup
                CurrencyTitle = AppRegistry.GetText(UserName, "CurrencyTitle"),
                CurrencyUnit = AppRegistry.GetText(UserName, "CurrencyUnit"),
                CurrencySign = AppRegistry.GetText(UserName, "CurrencySign"),
                CurrencyFormat = AppRegistry.GetText(UserName, "FMTCurrency"),

                // Bill Payable Setup
                BPay_Stock = AppRegistry.GetNumber(UserName, "BPay_Stock"),
                BPay_Tax = AppRegistry.GetNumber(UserName, "BPay_Tax"),
                BPay_Payable = AppRegistry.GetNumber(UserName, "BPay_Payable"),

                // Bill Receivable
                BRec_Stock = AppRegistry.GetNumber(UserName, "BRec_Stock"),
                BRec_Tax = AppRegistry.GetNumber(UserName, "BRec_Tax"),
                BRec_Receivable = AppRegistry.GetNumber(UserName, "BRec_Receivable"),
                
                // Accounts setrup for ledger
                CompanyGLs = AppRegistry.GetText(UserName, "CompanyGLs"),
                COAStocks = AppRegistry.GetText(UserName, "COAStocks"),

                // Accounts Nature
                CashBookNature = AppRegistry.GetNumber(UserName, "CashBkNature"),
                BankBookNature = AppRegistry.GetNumber(UserName, "BankBkNature"),
                RevenueNote = AppRegistry.GetNumber(UserName, "RevenueNote"),

                // Production
                ProductIN = AppRegistry.GetNumber(UserName, "ProductIN"),
                ProductOUT = AppRegistry.GetNumber(UserName, "ProductOUT"),

                ImagePath = AppRegistry.GetText(UserName, "ImagePath"),
                ReportPath = AppRegistry.GetText(UserName, "ReportPath"),
            };
        }
        #endregion
        #region Save
        public IActionResult OnPostSave()
        {
            
            // General Setup
            AppRegistry.SetKey(UserName, "OBCompany", Variables.OBCompany, KeyType.Number);
            AppRegistry.SetKey(UserName, "OBStock", Variables.OBStock, KeyType.Number);
            AppRegistry.SetKey(UserName, "FiscalStart", Variables.FiscalStart, KeyType.Date);
            AppRegistry.SetKey(UserName, "FiscalEnd", Variables.FiscalEnd, KeyType.Date);
            AppRegistry.SetKey(UserName, "StockExpiry", Variables.StockExpiry, KeyType.Number);
            AppRegistry.SetKey(UserName, "OBDate", Variables.OBDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "FMTDate", Variables.DateFormat, KeyType.Text);
            
            // Currency Setup
            AppRegistry.SetKey(UserName, "CurrencyTitle", Variables.CurrencyTitle, KeyType.Text);
            AppRegistry.SetKey(UserName, "CurrencyUnit", Variables.CurrencyUnit, KeyType.Text);
            AppRegistry.SetKey(UserName, "CurrencySign", Variables.CurrencySign, KeyType.Text);
            AppRegistry.SetKey(UserName, "FMTCurrency", Variables.CurrencyFormat, KeyType.Text);

            // Bill payable Setup
            AppRegistry.SetKey(UserName, "BPay_Stock", Variables.BPay_Stock, KeyType.Number);
            AppRegistry.SetKey(UserName, "BPay_Tax", Variables.BPay_Tax, KeyType.Number);
            AppRegistry.SetKey(UserName, "BPay_Payable", Variables.BPay_Payable, KeyType.Number);


            // Bill Receivable Setup
            AppRegistry.SetKey(UserName, "BRec_Stock", Variables.BRec_Stock, KeyType.Number);
            AppRegistry.SetKey(UserName, "BRec_Tax", Variables.BRec_Tax, KeyType.Number);
            AppRegistry.SetKey(UserName, "BRec_Receivable", Variables.BRec_Receivable, KeyType.Number);
            AppRegistry.SetKey(UserName, "SalesReportRDL", Variables.SalesReportRDL, KeyType.Text);
            AppRegistry.SetKey(UserName, "CompanyGLs", Variables.CompanyGLs, KeyType.Text);
            AppRegistry.SetKey(UserName, "COAStocks", Variables.COAStocks, KeyType.Text);

            // Accounts Nature Setup
            AppRegistry.SetKey(UserName, "CashBkNature", Variables.CashBookNature, KeyType.Number);
            AppRegistry.SetKey(UserName, "BankBkNature", Variables.BankBookNature, KeyType.Number);
            AppRegistry.SetKey(UserName, "RevenueNote", Variables.RevenueNote, KeyType.Number);

            // Production Setup
            AppRegistry.SetKey(UserName, "ProductIN", Variables.ProductIN, KeyType.Number);
            AppRegistry.SetKey(UserName, "ProductOUT", Variables.ProductOUT, KeyType.Number);


            // Image and report Path Setup
            AppRegistry.SetKey(UserName, "ImagePath", Variables.ImagePath, KeyType.Text); 
            AppRegistry.SetKey(UserName, "ReportPath", Variables.ReportPath, KeyType.Text);
            
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
            public string COAStocks { get; set; }
            public int CashBookNature { get; set; }
            public int BankBookNature { get; set; }
            public int ProductIN { get; set; }
            public int ProductOUT { get; set; }
            public string ImagePath {  get; set; }
            public string ReportPath {  get; set; }
            public int RevenueNote {  get; set; }
            public string CurrencyTitle {  get; set; }
            public string CurrencyUnit {  get; set; }

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


