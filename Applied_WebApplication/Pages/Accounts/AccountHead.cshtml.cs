using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Accounts
{
    public class AccountHeadModel : PageModel
    {
        public AccounHead Record { get; set; }

        public void OnGet()
        {
        }

        public void OnPostAdd()
        {

            Record = new AccounHead();
        }

        public void OnPostEdit()
        {

            Record = new AccounHead();
        }


        public class AccounHead
        {
            public int ID;
            public string Code;
            public string Title;
            public int Nature;
            public int Class;
            public int Notes;
            public decimal OBal;
        }
    }
}
