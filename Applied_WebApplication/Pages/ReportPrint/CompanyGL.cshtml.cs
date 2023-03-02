using AppReporting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Applied_WebApplication.Pages.ReportPrint
{
    public class CompanyGLModel : PageModel
    {
        [BindProperty]
        public ReportClass.ReportFilters Parameters { get; set; }

        public void OnGet()
        {
        }


    }
}
