using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages
{
    [Authorize(Policy = "MustBelongToHRPolicy")]
    public class HumanResourcesModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
