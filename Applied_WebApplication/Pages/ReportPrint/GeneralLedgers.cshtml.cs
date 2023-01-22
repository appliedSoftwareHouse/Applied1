using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class GeneralLedgersModel : PageModel
    {

        [BindProperty]
        public ReportClass.ReportFilters  Paramaters{ get; set; }

        public void OnGet()
        {
            Paramaters = new ReportClass.ReportFilters();
        }

        public void OnPost()
        {
            DateTime d1 = Paramaters.Dt_From;
            DateTime d2 = Paramaters.Dt_To;
        }

    }

}
