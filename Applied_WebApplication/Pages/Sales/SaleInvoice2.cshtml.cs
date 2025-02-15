using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Applied_WebApplication.Pages.Sales
{
    public interface ISaleInvoice2
    {

    }


    public class SaleInvoice2Model : PageModel, ISaleInvoice2
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
            myModel.fields = NewRecord();

            CompanyList = AppFunctions.GetList(UserName, Tables.Customers, "");
            EmployeeList = AppFunctions.GetList(UserName, Tables.Employees, "");
            StockList = AppFunctions.GetList(UserName, Tables.Inventory, "");
            ProjectList = AppFunctions.GetList(UserName, Tables.Project, "");
            TaxesList = AppFunctions.GetList(UserName, Tables.Taxes, "");
        }

        private DataFields NewRecord()
        {
            myModel = new();
            DataFields _fields = new();
            {
                _fields.Vou_No = "New";
                _fields.ID1 = 0;
                _fields.ID2 = 0;
                _fields.Sr_No = 1;

                _fields.Vou_Date = AppRegistry.GetDate(UserName, "invVouDate");
                _fields.Inv_Date = _fields.Vou_Date;
                _fields.Pay_Date = _fields.Vou_Date.AddDays(30);
            }

            myModel.Add(_fields);
            myModel.fields = myModel.FirstRecord;

            return _fields;
        }

        private DataFields AddRecord()
        {
            DataFields _fields = new();

            if (myModel.FirstRecord != null || myModel.fieldsList.Count > 0)
            {
                _fields = myModel.FirstRecord;
                _fields.Sr_No = myModel.fieldsList.Max(x => x.Sr_No) + 1;
                _fields.Inventory = 0;
                _fields.Qty = 0;
                _fields.Rate = 0;
                _fields.Amount = 0;
                _fields.Tax = 0;
                _fields.TaxRate = 0;
                _fields.TaxAmount = 0;
                _fields.NetAmount = 0;
                _fields.Batch = "";
                _fields.Project = 0;
                _fields.Description = "";
            }

            myModel.fieldsList.Add(_fields);
            return _fields;
        }

        public IActionResult OnPostSave()
        {
            myModel.Save_Voucher();
            return Page();
        }
    }

    


    public class Parameters 
    {
        public Parameters()
        {
        }

        public DataFields fields { get; set; } = new();
        public List<DataFields> fieldsList { get; set; } = new();
        public DataFields? FirstRecord => fieldsList.Any() ? fieldsList.FirstOrDefault() : null;
        public int MaxSrNo => fieldsList.Any() ? fieldsList.Max(x => x.Sr_No) : 0;


        public int Count => fieldsList?.Count ?? 0;

        public void Add(DataFields _fields)
        {
            fieldsList.Add(_fields);
        }

        public void Remove(DataFields _fields)
        {
            fieldsList.Remove(_fields);
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

        public string TitleInventory { get; set; } = string.Empty;
        public string TitleTax { get; set; } = string.Empty;


        public string NumFormat2 = AppRegistry.Currency2d;
        public string TQty => Qty.ToString(NumFormat2);
        public string TRate => Rate.ToString(NumFormat2);
        public string TAmount => Amount.ToString(NumFormat2);
        public string TNetAmount => NetAmount.ToString(NumFormat2);
        public string TTaxAmount => TaxAmount.ToString(NumFormat2);
        public string TTaxRate => TaxRate.ToString(NumFormat2);

    }
}


