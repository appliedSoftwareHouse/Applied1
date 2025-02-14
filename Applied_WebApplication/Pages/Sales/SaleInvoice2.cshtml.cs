using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Applied_WebApplication.Pages.Sales
{
    public class SaleInvoice2Model : PageModel
    {
        [BindProperty] public Parameters myModel { get; set; }
        public string UserName => User.Identity.Name;
        public List<Message> ErrorMessages { get; set; } = new();
        public Dictionary<int, string> CompanyList { get; set; } = new();
        public Dictionary<int, string> EmployeeList { get; set; } = new();
        public Dictionary<int, string> StockList { get; set; } = new();
        public Dictionary<int, string> ProjectList { get; set; } = new();
        public Dictionary<int, string> TaxesList { get; set; } = new();

        public void OnGet()
        {
            myModel = new();
            CompanyList = AppFunctions.GetList(UserName, Tables.Customers, "");
            EmployeeList = AppFunctions.GetList(UserName, Tables.Employees, "");
            StockList = AppFunctions.GetList(UserName, Tables.Inventory, "");
            ProjectList = AppFunctions.GetList(UserName, Tables.Project, "");
            TaxesList = AppFunctions.GetList(UserName, Tables.Taxes, "");
        }

        public IActionResult OnPostSave()
        {
            

            myModel.Save_Voucher();
            return Page();
            
        }

        
    }


    public class Parameters
    {
        public DataFields fields { get; set; } = new();
        public List<DataFields> fieldsList { get; set; } = new();
        
        public int Count => fieldsList?.Count ?? 0;

        public void Add(DataFields _fields)
        {
            fieldsList.Add(fields);
        }

        public void Remove(DataFields _fields)
        {
            fieldsList.Remove(fields);
        }

        public bool Save_Voucher()
        {
            return true;
        }



    }

    public class DataFields
    {
        [Required] public int ID1 { get; set; }
        [Required] public int ID2 { get; set; }
        [Required] public string Vou_No { get; set; }
        public string Ref_No { get; set; } = string.Empty;
        public string Inv_No { get; set; } = string.Empty;
        public int TranID { get; set; }
        public int Sr_No { get; set; }
        [Required] public int Inventory { get; set; }
        [Required] public int Company { get; set; }
        public int Employee { get; set; }
        public int Project { get; set; }
        public int Tax { get; set; }

        [Required] public DateTime Vou_Date { get; set; }
        [Required] public DateTime Inv_Date { get; set; }
        [Required] public DateTime Pay_Date { get; set; }

        [Required] public string Remarks { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        [Required] public string Status { get; set; } = string.Empty;

        [Required] public decimal Qty { get; set; }
        [Required] public decimal Rate { get; set; }
        [Required] public decimal Amount { get; set; }
        [Required] public int TaxRate { get; set; }
        [Required] public decimal TaxAmount { get; set; }
        [Required] public decimal NetAmount { get; set; }

    }
}


