using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SQLite;

namespace Applied_WebApplication.Pages.Account
{
    public class SettingModel : PageModel
    {
        AppliedUsersClass MyUserClass = new();
        public string CurrencyFormat = string.Empty;
        public string CurrencyFormat1 = "N";
        public string CurrencyFormat2 = "#,0.000";
        public string CurrencyFormat3 = "#,0.00 Rs.";
        public string CurrencyFormat4 = "Rs. #,0";
        public string CurrencyFormat5 = "#,##0";
        public string CurrencyFormat6 = "#,##0.00";

        public string DateFormat = string.Empty;
        public string DateFormat1 = "dd-MM-yy";
        public string DateFormat2 = "dd-MM-yyyy";
        public string DateFormat3 = "MM-dd-yy";
        public string DateFormat4 = "MM-dd-yyyy";
        public string DateFormat5 = "dd-MMM-yy";
        public string DateFormat6 = "dd-MMM-yyyy";
        

        public void OnGet()
        {
        }

        public IActionResult OnGetUpdateCurrencyFormat(int id, string UserName)
        {
            switch (id)
            {
                case 1:
                    CurrencyFormat = CurrencyFormat1;
                    break;
                case 2:
                    CurrencyFormat = CurrencyFormat2;
                    break;
                case 3:
                    CurrencyFormat = CurrencyFormat3;
                    break;
                case 4:
                    CurrencyFormat = CurrencyFormat4;
                    break;
                case 5:
                    CurrencyFormat = CurrencyFormat5;
                    break;
                case 6:
                    CurrencyFormat = CurrencyFormat6;
                    break;

                default:
                    CurrencyFormat = CurrencyFormat1;
                    break;
            }

            MyUserClass.AppliedUserCommand.CommandText = "Update [Users] SET [CurrencyFormat]='" + CurrencyFormat + "' WHERE [UserID] ='" + UserName + "'";
            MyUserClass.AppliedUserCommand.ExecuteNonQuery();
            return Page();
        }

        public IActionResult OnGetUpdateDateFormat(int id, string UserName)
        {
            switch (id)
            {
                case 1:
                    DateFormat = DateFormat1;
                    break;
                case 2:
                    DateFormat = DateFormat2;
                    break;
                case 3:
                    DateFormat = DateFormat3;
                    break;
                case 4:
                    DateFormat = DateFormat4;
                    break;
                case 5:
                    DateFormat = DateFormat5;
                    break;
                case 6:
                    DateFormat = DateFormat6;
                    break;

                default:
                    DateFormat = DateFormat1;
                    break;
            }

            MyUserClass.AppliedUserCommand.CommandText = "Update [Users] SET [DateFormat]='" + DateFormat + "' WHERE [UserID] ='" + UserName + "'";
            MyUserClass.AppliedUserCommand.ExecuteNonQuery();
            return Page();
        }
    }
}
