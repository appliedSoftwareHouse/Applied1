using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Accounts
{
    public class WriteChequeModel : PageModel
    {
        [BindProperty]
        public Chequeinfo Cheque { get; set;} = new();
        public List<Chequeinfo> ChequeList = new();
        public List<Message> ErrorMessages = new List<Message>();
        public bool IsLoad { get; set; } = false;


        public void OnGet(string Username)
        {
            if (Cheque == null) { Cheque = new Chequeinfo(); }
            var _TaxRate1 = 0.00M;
            var _TaxRate2 = 0.00M;

            Cheque.Code = "New Code";
            Cheque.Customer = 25;
            Cheque.Bank = 4;

            if (Cheque.TaxRate1 != 0)
            {
                _TaxRate1 = (int.Parse(DataTableClass.GetColumnValue(Username, Tables.Taxes, "Rate", Cheque.TaxRate1)));
                Cheque.TaxAmount1 = Cheque.TaxableAmount1 * (_TaxRate1 / 100.00M);
            }
            if (Cheque.TaxRate2 != 0)
            {
                _TaxRate2 = (int.Parse(DataTableClass.GetColumnValue(Username, Tables.Taxes, "Rate", Cheque.TaxRate2)));
                Cheque.TaxAmount2 = Cheque.TaxableAmount2 * (_TaxRate2 / 100.00M);
            }
        }

        public IActionResult OnPost()
        {
            // Validation of Record code type here.
            ChequeList.Add(Cheque);
            return Page();
        }

        public class Chequeinfo
        {
            public bool ChqArea { get; set; }
            public bool ListArea { get; set; } 
            public bool ErrorArea { get; set; } 

            public int ID { get; set; }
            public string Code { get; set; }
            public int TranType { get; set; }
            public DateTime TranDate { get; set; }
            public int Bank { get; set; }
            public int Customer { get; set; }
            public int Project { get; set; }
            public int Employee { get; set; }
            public DateTime ChqDate { get; set; }
            public string ChqNo { get; set; }
            public decimal ChqAmount { get; set; }
            public int TaxType { get; set; }
            public int Status { get; set; }
            public string Description { get; set; }

            // WHT Income Tax

            public decimal TaxableAmount1 { get; set; }
            public int TaxRate1 { get; set; }
            public decimal TaxAmount1 { get; set; }

            // Sales Tax
            public decimal TaxableAmount2 { get; set; }
            public int TaxRate2 { get; set; }
            public decimal TaxAmount2 { get; set; }

        }
    }
}
