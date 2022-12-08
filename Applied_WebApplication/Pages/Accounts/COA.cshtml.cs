using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;


namespace Applied_WebApplication.Pages.Accounts
{
    public class COAModel : PageModel
    {

        
        
        public DataTableClass COA = new DataTableClass(Tables.COA.ToString());
    }
}
