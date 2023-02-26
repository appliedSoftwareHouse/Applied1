using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Stock
{
    public class FinishedGoodsListModel : PageModel
    {
        public DataTable Goods { get; set; } = new DataTable();
        public string UserName => User.Identity.Name;

        public void OnGet()
        {
            DataTableClass _Table = new(UserName, Tables.FinishedGoods);
            Goods = _Table.GetTable("", "MFDate");
        }

        public IActionResult OnPostEdit(int id)
        {
            return RedirectToPage("./FinishedGoods", routeValues: new { id });
        }

    }
}
