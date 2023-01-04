using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using static Applied_WebApplication.Data.CreateTablesClass; 


namespace Applied_WebApplication.Pages.Accounts
{
    public class WriteChequeModel : PageModel
    {
        public Variables variables = new();
        public List<Message> ErrorMessages;

        public void OnGet(string UserName)
        {
            ErrorMessages = new List<Message>();
            variables = new Variables();
            variables.TranDate = DateTime.Now;
            variables.ChqDate = DateTime.Now;
            variables.TranType = 1;
        }

        public void OnGetLoad(string UserName)
        {
            ErrorMessages = new List<Message>();
            variables = new Variables();
            variables.TranDate = DateTime.Now;
            variables.ChqDate = DateTime.Now;
            variables.TranType = 1;
        }

        public void OnPost()
        {
            ErrorMessages = new List<Message>();
            variables = new Variables();
            variables.TranDate = DateTime.Now;
            variables.ChqDate = DateTime.Now;
            variables.TranType = 1;
        }

        public void OnPostSave(string UserName, Variables variables)
        {
            List<DataSet> Records = new List<DataSet>();

            DataTableClass Cheques = new(UserName, Tables.WriteCheques);
            Cheques.NewRecord();
            Cheques.CurrentRow["ID"] = variables.ID;
            Cheques.CurrentRow["TranType"] = variables.ID;
            Cheques.CurrentRow["TranDate"] = variables.ID;
            Cheques.CurrentRow["ChqNo"] = variables.ID;
            Cheques.CurrentRow["ChqDate"] = variables.ID;
            Cheques.CurrentRow["ChqAmount"] = variables.ID;
            Cheques.CurrentRow["COA"] = variables.ID;
            Cheques.CurrentRow["Project"] = variables.ID;
            Cheques.CurrentRow["Employee"] = variables.ID;
            Cheques.CurrentRow["Project"] = variables.ID;




            variables = new Variables();
            ErrorMessages = new List<Message>();
        }
                
        public class Variables
        {
            public bool ChqArea { get; set; } = true;
            public bool ListArea { get; set; } = false;
            public bool ErrorArea { get; set; } = true;

            
            public int ID { get; set; }
            [Range (1,3) ]
            public int TranType { get; set; }
            public DateTime TranDate { get; set; }
            public int Bank { get; set; }
            public int Customer { get; set; }
            public int Project { get; set; }
            public int Employees { get; set; }
            public DateTime ChqDate { get; set; }
            public string ChqNo { get; set; }
            public string ChqAmount { get; set; }
            public int TaxType { get; set; }

            // WHT Income Tax
            
            public decimal TaxableAmount1 { get; set; }
            public decimal TaxRate1 { get; set; }
            public decimal TaxAmount1 { get; set; }

            // Sales Tax
            public decimal TaxableAmount2 { get; set; }
            public decimal TaxRate2 { get; set; }
            public decimal TaxAmount2 { get; set; }
            public decimal Status { get; set; }
            public string Description { get; set; }
        }
    }
}
