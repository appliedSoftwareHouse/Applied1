using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Accounts.Directory
{
    [Authorize]
    public class COAModel : PageModel
    {
        public IActionResult OnGET()
        {
            return Page();
        }
    }
}
