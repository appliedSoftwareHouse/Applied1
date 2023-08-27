using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages
{
    [Authorize]
    public class SalesModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
