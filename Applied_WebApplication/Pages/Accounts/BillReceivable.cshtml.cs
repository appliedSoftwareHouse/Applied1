using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Accounts
{
    public class BillReceivableModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; } = new();
        public string UserName => User.Identity.Name;


        public void OnGet(int? id)
        {
            if (id == null)
            {
                Variables = new()
                {
                    Vou_No = AppFunctions.GetBillReceivableVoucher(UserName),
                    Vou_Date = DateTime.Now,
                    Pay_Date = DateTime.Now,

                };
            }

        }



        public class MyParameters
        {
            public int ID { get; set; }
            public string Vou_No { get; set; }
            public DateTime Vou_Date { get; set; }
            public DateTime Pay_Date { get; set; }
            public int Company { get; set; }

            public int Employee { get; set; }
            public string Ref_No { get; set; }
            public string Inv_No { get; set; }
            public DateTime Inv_Date { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public string Comments { get; set; }
            public string Status { get; set; }

            public int ID2 { get; set; }
            public int TranID { get; set; }
            public int SR_No { get; set; }
            public int Inventory { get; set; }
            public int Project { get; set; }
            public string Batch { get; set; }
            public decimal Qty { get; set; }
            public decimal Rate { get; set; }
            public int Tax { get; set; }
            public decimal Tax_Rate { get; set; }
            public string Description2 { get; set; }


            public decimal TranAmount { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal NetAmount { get; set; }
            public decimal InvAmount { get; set; }

            public decimal TotQty { set; get; }
            public decimal TotAmt { set; get; }
            public decimal TotTax { set; get; }
            public decimal TotInv { set; get; }

        }
    }
}
