using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages
{
    [Authorize (Roles ="1")]
    
    public class AccountsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
