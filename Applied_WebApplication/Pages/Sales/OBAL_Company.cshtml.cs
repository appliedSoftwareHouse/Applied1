using Applied_WebApplication.Pages.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Sales
{
    public class OBAL_CompanyModel : PageModel
    {

        [BindProperty]
        public MyParameters Variables { get; set; }
        public bool IsError = false;
        public List<Message> ErrorMessages;
        

        public void OnGet()
        {
        }

        public class MyParameters
        {
            public int Company { get; set; }
            public int COA { get; set; }
            public decimal Amount { get; set; }




        }

    }
}
