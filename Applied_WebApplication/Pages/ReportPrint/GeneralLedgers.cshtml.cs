using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Security.Claims;
using static Applied_WebApplication.Data.ReportClass;


namespace Applied_WebApplication.Pages.ReportPrint
{
    public class GeneralLedgersModel : PageModel
    {

        [BindProperty]
        public ReportFilters Paramaters { get; set; }

        public void OnGet()
        {
            var UserName = User.Identity.Name;
            Paramaters = new ReportFilters();

            Paramaters.N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number);
            Paramaters.Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date);
            Paramaters.Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date);

            if (Paramaters.Dt_From.Year < 2000) { Paramaters.Dt_From = DateTime.Now; }
            if (Paramaters.Dt_To.Year < 2000) { Paramaters.Dt_To = DateTime.Now; }

        }

        public void OnPost()
        {

        }

        public IActionResult OnPostGL()
        {
            var UserName = User.Identity.Name;
            AppRegistry.SetKey(UserName, "GL_COA", Paramaters.N_COA, KeyType.Number);
            AppRegistry.SetKey(UserName, "GL_Dt_From", Paramaters.Dt_From, KeyType.Date);
            AppRegistry.SetKey(UserName, "GL_Dt_To", Paramaters.Dt_To, KeyType.Date);
            Paramaters.TableName = Tables.Ledger;

            return RedirectToPage("PrintReport", "GL", new {Paramaters});
        }
    }

}
