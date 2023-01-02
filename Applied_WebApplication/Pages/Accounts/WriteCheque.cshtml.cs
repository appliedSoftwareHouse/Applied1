using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Applied_WebApplication.Data.CreateTablesClass; 

namespace Applied_WebApplication.Pages.Accounts
{
    public class WriteChequeModel : PageModel
    {
        public Variables variables { get; set; }
        public List<Message> ErrorMessages;



        public void OnGet()
        {
            variables = new Variables();
        }

        public void OnPost()
        {
            variables = new Variables();
            ErrorMessages = new List<Message>();
        }


        public class Variables
        {
            public bool ChqArea { get; set; } = true;
            public bool ListArea { get; set; } = false;
            public bool ErrorArea { get; set; } = true;

            
            public int ID { get; set; }
            public string TranType { get; set; }
            public DateOnly TranDate { get; set; }
            public int Bank { get; set; }
            public int Customer { get; set; }
            public DateOnly ChqDate { get; set; }
            public string ChqNo { get; set; }
            public string ChqAmount { get; set; }
            public int TaxType { get; set; }
            public decimal TaxableAmount { get; set; }
            public decimal TaxRate { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal Status { get; set; }
            public string Description { get; set; }
        }
    }
}
