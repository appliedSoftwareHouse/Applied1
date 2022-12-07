using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Dynamic;

namespace Applied_WebApplication.Pages.Account
{
    public class LogoutModel : PageModel
    {

        public void OnGet()
        {
            HttpContext.SignOutAsync();
            RedirectToPage("/Account/Login");
            
            
        }
        

    }
}
