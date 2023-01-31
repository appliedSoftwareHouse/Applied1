using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Accounts
{
    public class BillPayableListModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTableClass BillPayable { get; set; }
        public void OnGet()
        {
            string UserName = User.Identity.Name;
            BillPayable = new(UserName, Tables.BillPayable);


        }

        public class MyParameters
        {
            public int ID { get; set; }
            public string Vou_No { get; set; }
            public DateTime Vou_Date { get; set; }
            public DateTime Pay_Date { get; set; }
            public int Company { get; set; }
            public string Ref_No { get; set; }
            public string Inv_No { get; set; }
            public DateTime Inv_Date { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public string Comments { get; set; }

        }

    }
}
