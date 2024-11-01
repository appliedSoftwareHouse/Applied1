using AppReportClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;



namespace Applied_WebApplication.Pages.ReportPrint
{
    [Authorize]
    public class GeneralLedgersModel : PageModel
    {

        [BindProperty]
        public ReportFilters Parameters { get; set; }

        public void OnGet()
        {
            var UserName = User.Identity.Name;
            Parameters = new ReportFilters
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                N_Customer = (int)AppRegistry.GetKey(UserName, "GL_Company", KeyType.Number),
                N_Employee = (int)AppRegistry.GetKey(UserName, "GL_Employee", KeyType.Number),
                N_Project = (int)AppRegistry.GetKey(UserName, "GL_Project", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date)
            };

            if (Parameters.Dt_From.Year < 2000) { Parameters.Dt_From = DateTime.Now; }
            if (Parameters.Dt_To.Year < 2000) { Parameters.Dt_To = DateTime.Now; }

        }


        public IActionResult OnPostGL(ReportType Option)
        {
            SetKeys();
            return RedirectToPage("PrintReport", "GL", new { _ReportType = Option });
        }

        public IActionResult OnPostGLCompany(ReportType Option)
        {
            SetKeys();
            return RedirectToPage("PrintReport", "GLCompany", new { _ReportType=Option });
        }

        public IActionResult OnPostGLEmployee(ReportType Option)
        {
            SetKeys();
            return RedirectToPage("PrintReport", "GLEmployee", new { _ReportType = Option });
        }

        public IActionResult OnPostGLProjects(ReportType Option)
        {
            SetKeys();
            return RedirectToPage("PrintReport", "GLProject", new { _ReportType = Option });
        }

        public void SetKeys()
        {
            var UserName = User.Identity.Name;

            int CashBookNature = AppRegistry.GetNumber(UserName, "CashBkNature"); 
            int BankBookNature = AppRegistry.GetNumber(UserName, "BankBkNature");

            string GLp_Nature = $"{CashBookNature},{BankBookNature}";
            AppRegistry.SetKey(UserName, "GLp_Nature", GLp_Nature, KeyType.Text);

            AppRegistry.SetKey(UserName, "GL_COA", Parameters.N_COA, KeyType.Number);
            AppRegistry.SetKey(UserName, "GL_Company", Parameters.N_Customer, KeyType.Number);
            AppRegistry.SetKey(UserName, "GL_Employee", Parameters.N_Employee, KeyType.Number);
            AppRegistry.SetKey(UserName, "GL_Project", Parameters.N_Project, KeyType.Number);
            AppRegistry.SetKey(UserName, "GL_Dt_From", Parameters.Dt_From, KeyType.Date);
            AppRegistry.SetKey(UserName, "GL_Dt_To", Parameters.Dt_To, KeyType.Date);


        }

        //================================ END
    }
}
