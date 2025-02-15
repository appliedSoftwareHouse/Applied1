using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Account
{
    [Authorize]
    public class RestoreModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
