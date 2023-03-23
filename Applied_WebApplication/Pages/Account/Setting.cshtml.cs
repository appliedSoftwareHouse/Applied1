
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Applied_WebApplication.Pages.Account
{
    public class SettingModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public string UserName => User.Identity.Name;

        readonly AppliedUsersClass MyUserClass = new();

        #region Currency and Date Format Variables

        public string CurrencyFormat = string.Empty;
        public string CurrencyFormat1 = "#,0";
        public string CurrencyFormat2 = "#,0.00";
        public string CurrencyFormat3 = "#,0.00 Rs.";
        public string CurrencyFormat4 = "#,0 Rs.";
        public string CurrencyFormat5 = "Rs. #,0";
        public string CurrencyFormat6 = "Rs. #,0.00";
        public string CurrencyFormat7 = "#,##0";
        public string CurrencyFormat8 = "#,##0.00";

        public string DateFormat = string.Empty;
        public string DateFormat1 = "dd-MM-yy";
        public string DateFormat2 = "dd-MM-yyyy";
        public string DateFormat3 = "MM-dd-yy";
        public string DateFormat4 = "MM-dd-yyyy";
        public string DateFormat5 = "dd-MMM-yy";
        public string DateFormat6 = "dd-MMM-yyyy";

        #endregion




        public void OnGet()
        {
            string UserName = User.Identity.Name;
            DateFormat = AppRegistry.GetKey(UserName, "MFTDate", KeyType.Text).ToString();
            CurrencyFormat = AppRegistry.GetKey(UserName, "MFTCurrency", KeyType.Text).ToString();

            Variables = new()
            {
                OBCompany = (int)AppRegistry.GetKey(UserName, "OBCompany", KeyType.Number),
                OBStock = (int)AppRegistry.GetKey(UserName, "OBStock", KeyType.Number),
                FiscalStart = (DateTime)AppRegistry.GetKey(UserName, "FiscalStart", KeyType.Date),
                FiscalEnd = (DateTime)AppRegistry.GetKey(UserName, "FiscalEnd", KeyType.Date),
                StockExpiry = (int)AppRegistry.GetKey(UserName, "StockExpiry", KeyType.Number),
                CurrencySign = (string)AppRegistry.GetKey(UserName, "CurrencySign", KeyType.Text),
                OBDate = (DateTime)AppRegistry.GetKey(UserName, "OBDate", KeyType.Date)

            };
        }

        public void OnPostSave()
        {
            AppRegistry.SetKey(UserName, "OBCompany", Variables.OBCompany, KeyType.Number);
            AppRegistry.SetKey(UserName, "OBStock", Variables.OBStock, KeyType.Number);
            AppRegistry.SetKey(UserName, "FiscalStart", Variables.FiscalStart, KeyType.Date);
            AppRegistry.SetKey(UserName, "FiscalEnd", Variables.FiscalEnd, KeyType.Date);
            AppRegistry.SetKey(UserName, "StockExpiry", Variables.StockExpiry, KeyType.Number);
            AppRegistry.SetKey(UserName, "CurrencySign", Variables.CurrencySign, KeyType.Text);
            AppRegistry.SetKey(UserName, "OBDate", Variables.OBDate, KeyType.Date);
        }




        #region Date and Currency Format
        public IActionResult OnGetUpdateCurrencyFormat(int id)
        {
            string UserName = User.Identity.Name;
            CurrencyFormat = id switch
            {
                1 => CurrencyFormat1,
                2 => CurrencyFormat2,
                3 => CurrencyFormat3,
                4 => CurrencyFormat4,
                5 => CurrencyFormat5,
                6 => CurrencyFormat6,
                7 => CurrencyFormat7,
                8 => CurrencyFormat8,
                _ => CurrencyFormat1,
            };

            AppRegistry.SetKey(UserName, "FMTCurrency", CurrencyFormat, KeyType.Text);
            // Delete this codes in future if registry peerfect works. //
            MyUserClass.AppliedUserCommand.CommandText = "Update [Users] SET [CurrencyFormat]='" + CurrencyFormat + "' WHERE [UserID] ='" + UserName + "'";
            MyUserClass.AppliedUserCommand.ExecuteNonQuery();
            return Page();
        }
        public IActionResult OnGetUpdateDateFormat(int id)
        {
            string UserName = User.Identity.Name;
            DateFormat = id switch
            {
                1 => DateFormat1,
                2 => DateFormat2,
                3 => DateFormat3,
                4 => DateFormat4,
                5 => DateFormat5,
                6 => DateFormat6,
                _ => DateFormat1,
            };

            AppRegistry.SetKey(UserName, "FMTDate", DateFormat, KeyType.Text);
            // Delete this codes in future if registry peerfect works. //
            MyUserClass.AppliedUserCommand.CommandText = "Update [Users] SET [DateFormat]='" + DateFormat + "' WHERE [UserID] ='" + UserName + "'";
            MyUserClass.AppliedUserCommand.ExecuteNonQuery();
            return Page();
        }
        #endregion

        public class MyParameters
        {
            public int OBCompany { get; set; }              // COA for opening balance for Company.
            public int OBStock { get; set; }                    // COA for opening balance for Stock.
            public DateTime FiscalStart { get; set; }
            public DateTime FiscalEnd { get; set; }
            public int StockExpiry { set; get; }
            public string CurrencySign { get; set; }
            public DateTime OBDate { get; set; }
        }

    }

}


