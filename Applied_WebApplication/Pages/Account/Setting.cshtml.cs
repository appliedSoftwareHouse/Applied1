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
        

        public void OnGet()
        {
        }

        public IActionResult OnGetUpdateCurrencyFormat(int id, string UserName)
        {
            
            string CurrencyFormat1 = "N";
            string CurrencyFormat2 = "#,0.000";
            string CurrencyFormat3 = "#,0.00 Rs.";
            string CurrencyFormat4 = "Rs. #,0";
            string CurrencyFormat = CurrencyFormat1;

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
                default:
                    CurrencyFormat = CurrencyFormat1;
                    break;
            }

            MyUserClass.AppliedUserCommand.CommandText = "Update [Users] SET [CurrencyFormat]='" + CurrencyFormat + "' WHERE [UserID] ='" + UserName + "'";
            MyUserClass.AppliedUserCommand.ExecuteNonQuery();
            return Page();
        }

        public IActionResult OnPostUpdateCurrencyFormat(int id)
        {
            return Page();
        }


        public IActionResult OnGetUpdateDateFormat(int id)
        {
            AppliedUsersClass MyUserClass = new();
            return Page();
        }


        public IActionResult OnPostUpdateDateFormat(int id)
        {
            AppliedUsersClass MyUserClass = new();
            return Page();
        }
    }
}
