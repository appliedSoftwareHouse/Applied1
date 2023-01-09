using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Accounts
{
    public class WriteCheque2Model : PageModel
    {
        [BindProperty]
        public Chequeinfo Cheque { get; set; } = new();
        public List<Chequeinfo> ChequesList = new();

        public void OnGet()
        {
            Cheque = new()
            {
                ID = 0,
                Code = "C-9001",
                TranType = "Cheque",
                TranDate = DateTime.Now,
                Bank = 5,
                ChqNo = "A-789456123",
                ChqDate = DateTime.Now,
                ChqAmount = 125800M,
                Project = 0,
                Employee = 0,
                Tax = 0,
                TaxableAmount = 0M
            };
            ChequesList.Add(Cheque);

            Cheque = new()
            {
                ID = 0,
                Code = "C-9001",
                TranType = "WHT",
                TranDate = DateTime.Now,
                Bank = 5,
                ChqNo = "A-789456123",
                ChqDate = DateTime.Now,
                ChqAmount = 0M,
                Project = 0,
                Employee = 0,
                Tax = 2,
                TaxableAmount = 125800M
            };
            ChequesList.Add(Cheque);

            Cheque = new()
            {
                ID = 0,
                Code = "C-9001",
                TranType = "SRB",
                TranDate = DateTime.Now,
                Bank = 5,
                ChqNo = "A-789456123",
                ChqDate = DateTime.Now,
                ChqAmount = 0M,
                Project = 0,
                Employee = 0,
                Tax = 4,
                TaxableAmount = 15800M
            };
            ChequesList.Add(Cheque);
            Cheque = ChequesList.First();
        }

        public IActionResult OnPost()
        {
            if(ModelState.IsValid == false)
            {
                return Page();
            }
            return RedirectToPage("/Index");
        }

       
        public class Chequeinfo
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string TranType { get; set; }
            public DateTime TranDate { get; set; }
            public int Bank { get; set; }
            public int Project { get; set; }
            public int Employee { get; set; }
            public string ChqNo { get; set; }
            public DateTime ChqDate { get; set; }
            public decimal ChqAmount { get; set; }
            public int Tax { get; set; }
            public decimal TaxableAmount { get; set; }
        }
        

    }
}
