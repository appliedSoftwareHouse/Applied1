using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages
{

    [Authorize(Policy="AdminOnly")]
    public class SettingModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
