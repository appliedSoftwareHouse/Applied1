using AppReporting;
using AppReportClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;



namespace Applied_WebApplication.Pages.ReportPrint
{
    public class GeneralLedgersModel : PageModel
    {

        [BindProperty]
        public AppReportClass.ReportFilters Parameters { get; set; }

        public void OnGet()
        {
            var UserName = User.Identity.Name;
            Parameters = new AppReportClass.ReportFilters
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                N_Customer = (int)AppRegistry.GetKey(UserName, "GL_Company", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date)
            };

            if (Parameters.Dt_From.Year < 2000) { Parameters.Dt_From = DateTime.Now; }
            if (Parameters.Dt_To.Year < 2000) { Parameters.Dt_To = DateTime.Now; }

        }

        public void OnPost()
        {

        }

        public IActionResult OnPostGL()
        {
            SetKeys();
            return RedirectToPage("PrintReport", "GL", new { Parameters });
        }

        #region POST Company

        public IActionResult OnPostCompany()
        {
            SetKeys();
            return RedirectToPage("PrintReport", "GLCompany", new { _ReportType =  ReportType.PDF });

        }

        public IActionResult OnPostCompanyExcel()
        {
            SetKeys();
            return RedirectToPage("PrintReport", "GLCompany", new { _ReportType =  ReportType.Excel });

        }

        public IActionResult OnPostCompanyWord()
        {
            SetKeys();
            return RedirectToPage("PrintReport", "GLCompany", new { _ReportType = ReportType.Word });

        }

        public IActionResult OnPostCompanyHTML()
        {
            SetKeys();
            return RedirectToPage("PrintReport", "GLCompany", new { _ReportType = ReportType.HTML });

        }

        #endregion

        public void SetKeys()
        {
            var UserName = User.Identity.Name;
            AppRegistry.SetKey(UserName, "GL_COA", Parameters.N_COA, KeyType.Number);
            AppRegistry.SetKey(UserName, "GL_Company", Parameters.N_Customer, KeyType.Number);
            AppRegistry.SetKey(UserName, "GL_Dt_From", Parameters.Dt_From, KeyType.Date);
            AppRegistry.SetKey(UserName, "GL_Dt_To", Parameters.Dt_To, KeyType.Date);


        }

        //================================ END
    }
}
