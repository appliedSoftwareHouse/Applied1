using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Dynamic;

namespace Applied_WebApplication.Pages.Account
{
    public class LogoutModel : PageModel
    {

        public async Task<IActionResult> OnGet()
        {
            await HttpContext.SignOutAsync();
            return RedirectToPage("Login");
        }
        

    }
}
