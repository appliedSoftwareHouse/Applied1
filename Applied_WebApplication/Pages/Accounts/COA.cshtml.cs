using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Accounts
{
    public class COAModel : PageModel 
    {
        public IActionResult OnGET()
        {
            return Page();
        }
    }
}
