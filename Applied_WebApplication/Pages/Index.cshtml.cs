using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AspNetCore.Reporting;
using System.Drawing.Text;
using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Configuration;

namespace Applied_WebApplication.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public void OnPOST()
        {

        }

        public async Task<IActionResult> OnPostWChequeAsync(string username)
        {
            await Task.Delay(1000);
            return RedirectToPage("/Accounts/WriteCheque", new { UserName = username});
        }



    }
}