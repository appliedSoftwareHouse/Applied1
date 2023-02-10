using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Applied_WebApplication.Data.ReportClass;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class CompanyGLModel : PageModel
    {
        [BindProperty]
        public ReportFilters Parameters { get; set; }

        public void OnGet()
        {
        }


    }
}
