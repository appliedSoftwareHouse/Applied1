using Applied_WebApplication.Pages.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Stock
{
    public class FinishedGoodsListModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable Goods { get; set; } = new DataTable();
        public string UserName => User.Identity.Name;

        public void OnGet()
        {
            Variables = new()
            {
                DateFrom = AppRegistry.GetDate(UserName, "FGoods_From"),
                DateTo = AppRegistry.GetDate(UserName, "FGoods_To")
            };

            var _Filter = string.Format("MFDate >='{0}' AND MFDate <='{1}'", Variables.DateFrom, Variables.DateTo); 
            DataTableClass _Table = new(UserName, Tables.FinishedGoods);
            Goods = _Table.GetTable(_Filter, "MFDate");
        }

        public IActionResult OnPostEdit(int id)
        {
            return RedirectToPage("./FinishedGoods", routeValues: new { id });
        }

        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "FGoods_From", Variables.DateFrom, KeyType.Date);
            AppRegistry.SetKey(UserName, "FGoods_To", Variables.DateTo, KeyType.Date);
            return RedirectToPage();
        }

        public IActionResult OnPostProcess(int ID)
        {
            return RedirectToPage("BOMProcess", routeValues: new {ID});
        }
        public class MyParameters
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            
        }

    }
}
