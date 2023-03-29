using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Accounts
{
    public class JVListModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable JVList { get; set; }


        public void OnGet()
        {
        }

        public class MyParameters
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }

        }

    }
}
