using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Applied_WebApplication.Pages.Accounts
{
    public class COAModel : PageModel 
    {
        private IHttpContextAccessor Accessor;

        public IActionResult OnGET()
        {
            return Page();
        }

        public COAModel(IHttpContextAccessor _accessor)
        {
            Accessor = _accessor;
        }

        public IActionResult OnGetHTTP(IHttpContextAccessor? _accessor)
        {


            return Page();
        }

        

        public DataTableClass Test = new();
        
        public DataTableClass COA = new DataTableClass(Tables.COA.ToString());
    }
}
