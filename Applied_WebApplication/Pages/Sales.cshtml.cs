using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages
{
    [Authorize]
    public class SalesModel : PageModel
    {
        public string UserName => User.Identity.Name;
        public bool ShowSaleRevenueMenu { get; set; }

        public void OnGet()
        {
            ShowSaleRevenueMenu = AppRegistry.GetBool(UserName, "SaleRevMenu");
        }
    }
}
