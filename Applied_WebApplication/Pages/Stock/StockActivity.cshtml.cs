using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Stock
{
    [Authorize]
    public class StockActivityModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { set; get; }
        public string UserName => User.Identity.Name;
        public DataTable MyTable { get; set; }
        public void OnGet()
        {
            Variables = new()
            {
                MinDate = AppRegistry.GetDate(UserName, "sa_Date1"),
                MaxDate = AppRegistry.GetDate(UserName, "sa_Date2"),
                Inventory = AppRegistry.GetNumber(UserName, "sa_Inventory")
            };

            if (Variables.Inventory > 0)
            {
                var _Date1 = AppFunctions.MinDate(Variables.MinDate, Variables.MaxDate).ToString(AppRegistry.DateYMD);
                var _Date2 = AppFunctions.MaxDate(Variables.MinDate, Variables.MaxDate).ToString(AppRegistry.DateYMD);
                var _Filter = $"Date(Vou_Date) BETWEEN Date('{_Date1}') AND Date('{_Date2}') AND [Inventory] = {Variables.Inventory}";
                MyTable = DataTableClass.GetTable(UserName, SQLQuery.StockPositionData(_Filter), "[Vou_Date],[Vou_No]");
            }
            else
            {
                MyTable = new DataTable();
            }
        }

        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "sa_Date1", Variables.MinDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "sa_Date2", Variables.MaxDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "sa_Inventory", Variables.Inventory, KeyType.Number);

            return RedirectToPage();
        }
        public class Parameters
        {
            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }
            public int Inventory { get; set; }
        }
    }
}
