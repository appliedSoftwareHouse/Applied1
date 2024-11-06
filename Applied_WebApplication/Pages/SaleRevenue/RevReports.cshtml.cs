using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.SaleRevenue
{
    [Authorize]
    public class RevReportsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
