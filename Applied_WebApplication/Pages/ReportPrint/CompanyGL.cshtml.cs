using AppReporting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Applied_WebApplication.Pages.ReportPrint
{
    public class CompanyGLModel : PageModel
    {
        [BindProperty]
        public AppReportClass.ReportFilters Parameters { get; set; }

        public void OnGet()
        {
        }


    }
}
