using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Sales
{
    [Authorize]
    public class CustomersModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
