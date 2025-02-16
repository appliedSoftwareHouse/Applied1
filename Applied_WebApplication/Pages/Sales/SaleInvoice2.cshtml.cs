using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Applied_WebApplication.Pages.Sales
{
    public class SaleInvoice2Model : PageModel
    {
        [BindProperty] public Parameters myModel { get; set; }


        public string UserName => User.Identity.Name;
        public int MaxSrNo { get; set; }
        public List<Message> ErrorMessages { get; set; } = new();
        public Dictionary<int, string> CompanyList { get; set; } = new();
        public Dictionary<int, string> EmployeeList { get; set; } = new();
        public Dictionary<int, string> StockList { get; set; } = new();
        public Dictionary<int, string> ProjectList { get; set; } = new();
        public Dictionary<int, string> TaxesList { get; set; } = new();


        public void OnGet(int? ID, int? SRNO)
        {
            ID ??= 0; SRNO ??= 0;

            if (ID > 0)
            {
                myModel = new(UserName, (int)ID, (int)SRNO);

            }
            else
            {
                myModel = new(UserName);
                myModel.NewInvoice();
            }

            MaxSrNo = myModel.fieldsList.Any() ? myModel.fieldsList.Max(x => x.Sr_No) : 0;

            CompanyList = AppFunctions.GetList(UserName, Tables.Customers, "");
            EmployeeList = AppFunctions.GetList(UserName, Tables.Employees, "");
            StockList = AppFunctions.GetList(UserName, Tables.Inventory, "");
            ProjectList = AppFunctions.GetList(UserName, Tables.Project, "");
            TaxesList = AppFunctions.GetList(UserName, Tables.Taxes, "");
        }



        private DataFields AddNew()
        {
            DataFields _fields = new();

            if (myModel.FirstRecord != null || myModel.fieldsList.Count > 0)
            {
                _fields = myModel.FirstRecord;
                _fields.Sr_No = myModel.fieldsList.Max(x => x.Sr_No) + 1;
                _fields.Inventory = 0;
                _fields.Qty = 0;
                _fields.Rate = 0;
                //_fields.Amount = 0;
                _fields.Tax = 0;
                _fields.TaxRate = 0;
                //_fields.TaxAmount = 0;
                //_fields.NetAmount = 0;
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
        public string UserName { get; set; }

        public Parameters() { }
        public Parameters(string _UserName) { UserName = _UserName; }

        public Parameters(string _UserName, int ID, int SRNO) { UserName = _UserName; LoadData(ID, SRNO); }
        public Parameters(string _UserName, int ID) { UserName = _UserName; LoadData(ID, 1); }

        private void LoadData(int ID, int SRNO)
        {
            if (ID > 0)
            {
                var tb1 = DataTableClass.GetTable(UserName, Tables.BillReceivable, $"ID={ID}");
                var tb2 = DataTableClass.GetTable(UserName, Tables.BillReceivable2, $"TranID={ID}");

                if (tb1.Rows.Count > 0 && tb2.Rows.Count > 0)
                {
                    var _Row1 = tb1.Rows[0];
                    foreach (DataRow Row2 in tb2.Rows)
                    {
                        Add(_Row1, Row2, UserName);

                    }
                    fields = fieldsList.Where(x => x.Sr_No == SRNO).FirstOrDefault();
                    if (fields == null) { fields = fieldsList.First(); } // assign as new if found null value;
                }
                else
                {
                    if (tb1.Rows.Count + tb2.Rows.Count == 0)
                    {
                        // Record not found error message show
                    }
                    else
                    {

                        // Records not found in one Table
                    }
                }

                tb1.Dispose();  // Close DataTable
                tb2.Dispose(); // close DataTable
            }
        }


        public DataFields fields { get; set; } = new();
        public List<DataFields> fieldsList { get; set; } = new();
        public DataFields? FirstRecord => fieldsList.Any() ? fieldsList.FirstOrDefault() : null;


        public int Count => fieldsList?.Count ?? 0;
        public void Add(DataFields _fields) { fieldsList.Add(_fields); }
        public void Remove(DataFields _fields) { fieldsList.Remove(_fields); }

        public void Add(DataRow Row1, DataRow Row2, string UserName)
        {
            fields = new();
            fields.ID1 = Row1.Field<int>("ID");
            fields.Vou_No = Row1.Field<string>("Vou_No");
            fields.Vou_Date = Row1.Field<DateTime>("Vou_Date");
            fields.Company = Row1.Field<int>("Company");
            fields.Employee = Row1.Field<int>("Employee");
            fields.Ref_No = Row1.Field<string>("Ref_No");
            fields.Inv_No = Row1.Field<string>("Inv_No");
            fields.Inv_Date = Row1.Field<DateTime>("Inv_Date");
            fields.Pay_Date = Row1.Field<DateTime>("Pay_Date");
            fields.InvAmount = Row1.Field<decimal>("amount");
            fields.Remarks = Row1.Field<string>("Description");
            fields.Comments = Row1.Field<string>("Comments");
            fields.Status = Row1.Field<string>("Status");

            fields.ID2 = Row2.Field<int>("ID");
            fields.Sr_No = Row2.Field<int>("Sr_No");
            fields.TranID = Row2.Field<int>("TranID");
            fields.Inventory = Row2.Field<int>("Inventory");
            fields.Qty = Row2.Field<decimal>("Qty");
            fields.Rate = Row2.Field<decimal>("Rate");

            fields.Tax = Row2.Field<int>("Tax");
            fields.TaxRate = Row2.Field<decimal>("Tax_Rate");
            fields.Description = Row2.Field<string>("Description");
            fields.Project = Row2.Field<int>("Project");

            fields.TitleInventory = AppFunctions.GetTitle(UserName, Tables.Inventory, fields.Inventory);
            fields.TitleTax = AppFunctions.GetTitle(UserName, Tables.Taxes, fields.Tax);


            fieldsList.Add(fields);

        }

        public void NewInvoice()
        {
            fieldsList = new();

            fields = new();
            {
                fields.Vou_No = "New";
                fields.ID1 = 0;
                fields.ID2 = 0;
                fields.Sr_No = 1;

                fields.Vou_Date = AppRegistry.GetDate(UserName, "invVouDate");
                fields.Inv_Date = fields.Vou_Date;
                fields.Pay_Date = fields.Vou_Date.AddDays(30);
            }

            fieldsList.Add(fields);

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
        [Required] public string Status { get; set; } = AppFunctions.Status.Submitted.ToString();

        [Required] public decimal Qty { get; set; }
        [Required] public decimal Rate { get; set; }
        [Required] public decimal TaxRate { get; set; }
        public decimal InvAmount { get; set; }
        public decimal Amount => Qty * Rate;
        public decimal TaxAmount => Amount * TaxRate;
        public decimal NetAmount => Amount + TaxAmount;

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


