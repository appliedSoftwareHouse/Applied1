using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Accounts.Directory
{
    [Authorize]
    public class ClassModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
