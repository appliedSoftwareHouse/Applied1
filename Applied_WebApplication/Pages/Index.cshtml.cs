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

        public IActionResult OnPostTest()
        {
            return RedirectToPage("/Accounts/WriteCheque", routeValues: new { ChqCode = "" });
        }

        public IActionResult OnPostCashBook()
        {
            return RedirectToPage("/Accounts/Cashbook", routeValues: new { ChqCode = "" });
        }

        public IActionResult OnPostBankBook()
        {
            return RedirectToPage("/Accounts/Bankbook", routeValues: new { ChqCode = "" });
        }


        public IActionResult OnPostWCheque()
        {
            return RedirectToPage("/Accounts/WriteCheque", routeValues: new { ChqCode = ""});
        }



    }
}