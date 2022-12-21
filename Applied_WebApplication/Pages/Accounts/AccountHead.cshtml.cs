using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Accounts
{
    public class AccountHeadModel : PageModel
    {
        public string PageAction { get; set;  } = string.Empty;
        public AccounHead Record { get; set; }
        public int RecordID { get; set; } = 0;

        public void OnGet()
        {
        }

        public void OnPostAdd()
        {
            PageAction = "Add";

            Record = new AccounHead();
        }

        public void OnGetEdit(string UserName, int id)
        {
            PageAction = "Edit";
            RecordID = id;
            Record = new AccounHead();
        }

        public void OnGetDelete(string UserName, int id)
        {
            PageAction = "Delete";
            RecordID = id;
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
