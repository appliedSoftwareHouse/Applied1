using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.JSInterop;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Applied_WebApplication.Pages.Sales
{
    public class SaleInvoice2Model : PageModel
    {

        [BindProperty] public DataFields Variables { get; set; }
        public List<DataFields> RecordList { get; set; }
        public DataFields? FirstRecord => RecordList.Any() ? RecordList.FirstOrDefault() : null;
        public string UserName => User.Identity.Name;
        public int MaxSrNo { get; set; }
        public List<Message> MyMessages { get; set; } = new();
        public Dictionary<int, string> CompanyList { get; set; } = new();
        public Dictionary<int, string> EmployeeList { get; set; } = new();
        public Dictionary<int, string> StockList { get; set; } = new();
        public Dictionary<int, string> ProjectList { get; set; } = new();
        public Dictionary<int, string> TaxesList { get; set; } = new();

        public TempDBClass2 TempTable1 { get; set; }
        public TempDBClass2 TempTable2 { get; set; }
        public decimal Amount { get; set; }
        public string UserRole => UserProfile.GetUserRole(User);



        #region Get
        public void OnGet(int? ID, int? SRNO)
        {
            ID ??= 0; SRNO ??= 1;

            Variables = new();
            RecordList = new();

            if (ID == 0)
            {
                NewInvoice();
            }
            else
            {
                LoadData((int)ID, (int)SRNO);
            }

            if (SRNO > 0) { Variables = RecordList.Where(x => x.Sr_No == SRNO).First(); }
            if (SRNO == -1) { Variables = RecordList.Last(); }

            MaxSrNo = RecordList.Any() ? RecordList.Max(x => x.Sr_No) : 0;

        }
        #endregion

        #region Load Data
        private void LoadData(int ID, int SRNO)
        {
            if (Variables != null)
            {
                CompanyList = AppFunctions.GetList(UserName, Tables.Customers, "");
                EmployeeList = AppFunctions.GetList(UserName, Tables.Employees, "");
                StockList = AppFunctions.GetList(UserName, Tables.Inventory, "");
                ProjectList = AppFunctions.GetList(UserName, Tables.Project, "");
                TaxesList = AppFunctions.GetList(UserName, Tables.Taxes, "");

                var tb1 = DataTableClass.GetTable(UserName, Tables.BillReceivable, $"ID={ID}");
                var tb2 = DataTableClass.GetTable(UserName, Tables.BillReceivable2, $"TranID={ID}");

                TempTable1 = new(User, tb1, "BillRec1");
                TempTable2 = new(User, tb2, "BillRec2");

                RecordList ??= new();

                if (tb1.Rows.Count > 0 && tb2.Rows.Count > 0)
                {
                    var _Row1 = tb1.Rows[0];
                    foreach (DataRow Row2 in tb2.Rows)
                    {
                        Add(_Row1, Row2, UserName);
                    }

                    if (SRNO > 0)
                    {
                        Variables = RecordList.Where(x => x.Sr_No == SRNO).FirstOrDefault();
                    }

                    if (SRNO == -1)
                    {
                        Variables = AddNewRecord();
                    }


                }
                else
                {
                    if (tb1.Rows.Count + tb2.Rows.Count == 0)
                    {
                        MyMessages.Add(MessageClass.SetMessage("Error: Record not found...", ConsoleColor.Red));
                    }
                    else
                    {
                        MyMessages.Add(MessageClass.SetMessage("Error: Record not found in one Data table...", ConsoleColor.Red));
                    }
                }

                Amount = RecordList.Sum(x => x.NetAmount);

                tb1 = null; // Close DataTable
                tb2 = null; // close DataTable

            }
        }
        #endregion

        #region Get Title of Dropdown
        public string GetTitle(Dictionary<int, string> _List, int ID)
        {
            if (_List == null) { return "Select..."; }
            if (ID == 0 || _List.Count == 0) { return "Select...."; }
            return _List.Where(e => e.Key.Equals(ID)).Select(e => e.Value).FirstOrDefault();
        }
        #endregion

        #region OnPost Edit - Delete
        public IActionResult OnPostEdit(int SRNO)
        {
            return RedirectToPage("SaleInvoice2", routeValues: new { ID = Variables.ID1, SRNO });
        }

        public IActionResult OnPostDelete()
        {
            var _ModelState = ModelState.IsValid;
            return Page();
        }
        #endregion

        #region OnPost Add New Record

        private DataFields AddNewRecord()
        {
            Variables = RecordList.First();
            DataFields fields = new();
            if (Variables != null)
            {
                fields.ID1 = Variables.ID1;
                fields.Vou_No = Variables.Vou_No;
                fields.Vou_Date = Variables.Vou_Date;
                fields.Inv_No = Variables.Inv_No;
                fields.Inv_Date = Variables.Inv_Date;
                fields.Pay_Date = Variables.Pay_Date;
                fields.Ref_No = Variables.Ref_No;
                fields.Company = Variables.Company;
                fields.Employee = Variables.Employee;
                fields.Remarks = Variables.Remarks;
                fields.Comments = Variables.Comments;
                fields.Status = "New";

                fields.ID2 = 0;
                fields.Sr_No = RecordList.Max(x => x.Sr_No) + 1;
                fields.Inventory = 0;
                fields.Qty = 0;
                fields.Rate = 0;
                fields.Tax = 0;
                fields.TaxRate = 0;
                fields.Batch = "";
                fields.Project = 0;
                fields.Description = "";
                fields.TitleInventory = string.Empty;
                fields.TitleTax = string.Empty;
            }

            RecordList.Add(fields);
            return fields;
        }

        public IActionResult OnPostAddNew()
        {
            return RedirectToPage(routeValues: new { ID = Variables.ID1, SRNO = -1 });
        }
        #endregion

        #region Save
       

        private bool Validated()
        {
            return true;
        }

        public IActionResult OnPostSave()
        {
            if (UserRole == "Viewer") { return Page(); }
            var tb1 = new DataTableClass(UserName, Tables.BillReceivable);
            var tb2 = new DataTableClass(UserName, Tables.BillReceivable2);

            var Row1 = tb1.MyDataTable.NewRow();
            var Row2 = tb2.MyDataTable.NewRow();

            if (Variables.Vou_No.ToUpper() == "NEW")
            { Variables.Vou_No = NewVoucher.GetNewVoucher(tb1.MyDataTable, "SL", Variables.Vou_Date); }

            Row1["ID"] = Variables.ID1;
            Row1["Vou_No"] = Variables.Vou_No;
            Row1["Vou_Date"] = Variables.Vou_Date;
            Row1["Company"] = Variables.Company;
            Row1["Employee"] = Variables.Employee;
            Row1["Ref_No"] = Variables.Ref_No;
            Row1["Inv_No"] = Variables.Inv_No;
            Row1["Inv_Date"] = Variables.Inv_Date;
            Row1["Pay_Date"] = Variables.Pay_Date;
            Row1["Amount"] = Amount;
            Row1["Description"] = Variables.Remarks;
            Row1["Comments"] = Variables.Comments;
            Row1["Status"] = Variables.Status;

            Row2["ID"] = Variables.ID2;
            Row2["Sr_No"] = Variables.Sr_No;
            Row2["TranID"] = Variables.ID1;                     // TrasID must be equal with ID1
            Row2["Inventory"] = Variables.Inventory;
            Row2["Batch"] = Variables.Batch;
            Row2["Qty"] = Variables.Qty;
            Row2["Rate"] = Variables.Rate;
            Row2["Tax"] = Variables.Tax;
            Row2["Tax_Rate"] = Variables.TaxRate;
            Row2["Description"] = Variables.Description;

            tb1.CurrentRow = Row1;
            tb2.CurrentRow = Row2;
            bool IsSave = false;

            tb1.Save();
            if (!tb1.IsError)
            {
                Variables.ID1 = (int)tb1.CurrentRow["ID"];   // Get an ID if voucher is new.
                tb2.CurrentRow["TranID"] = Variables.ID1;    // Assign TranID as new Voucher ID

                tb2.Save();
                if (!tb2.IsError)
                {
                    IsSave = true;
                }
                else
                {
                    MyMessages = tb2.ErrorMessages;
                }
            }
            else
            {
                MyMessages = tb1.ErrorMessages;
            }

            if (IsSave)
            {
                MyMessages.Add(MessageClass.SetMessage("Invoice has been saved", ConsoleColor.Green));
                return RedirectToPage(routeValues: new { ID = Variables.ID1, SRNO = Variables.Sr_No });
            }

            LoadData(Variables.ID1, Variables.Sr_No);
            return Page();
        }
        #endregion

        #region New Invoice
        public void NewInvoice()
        {
            CompanyList = AppFunctions.GetList(UserName, Tables.Customers, "");
            EmployeeList = AppFunctions.GetList(UserName, Tables.Employees, "");
            StockList = AppFunctions.GetList(UserName, Tables.Inventory, "");
            ProjectList = AppFunctions.GetList(UserName, Tables.Project, "");
            TaxesList = AppFunctions.GetList(UserName, Tables.Taxes, "");

            RecordList = new();

            Variables = new();
            {
                Variables.Vou_No = "New";
                Variables.ID1 = 0;
                Variables.ID2 = 0;
                Variables.Sr_No = 1;

                Variables.Vou_Date = AppRegistry.GetDate(UserName, "invVouDate");
                Variables.Inv_Date = Variables.Vou_Date;
                Variables.Pay_Date = Variables.Vou_Date.AddDays(30);
            }

            RecordList.Add(Variables);

        }
        #endregion

        #region Add Invoices in RecordList
        public void Add(DataRow Row1, DataRow Row2, string UserName)
        {
            DataFields _Variables = new();

            Row1 = DataTableClass.RemoveNull(Row1);
            Row2 = DataTableClass.RemoveNull(Row2);

            _Variables.ID1 = Row1.Field<int>("ID");
            _Variables.Vou_No = Row1.Field<string>("Vou_No");
            _Variables.Vou_Date = Row1.Field<DateTime>("Vou_Date");
            _Variables.Company = Row1.Field<int>("Company");
            _Variables.Employee = Row1.Field<int>("Employee");
            _Variables.Ref_No = Row1.Field<string>("Ref_No");
            _Variables.Inv_No = Row1.Field<string>("Inv_No");
            _Variables.Inv_Date = Row1.Field<DateTime>("Inv_Date");
            _Variables.Pay_Date = Row1.Field<DateTime>("Pay_Date");
            _Variables.InvAmount = Row1.Field<decimal>("amount");
            _Variables.Remarks = Row1.Field<string>("Description");
            _Variables.Comments = Row1.Field<string>("Comments");
            _Variables.Status = Row1.Field<string>("Status");

            _Variables.ID2 = Row2.Field<int>("ID");
            _Variables.Sr_No = Row2.Field<int>("Sr_No");
            _Variables.TranID = Row2.Field<int>("TranID");
            _Variables.Inventory = Row2.Field<int>("Inventory");
            _Variables.Batch = Row2.Field<string>("Batch");
            _Variables.Qty = Row2.Field<decimal>("Qty");
            _Variables.Rate = Row2.Field<decimal>("Rate");
            _Variables.Tax = Row2.Field<int>("Tax");
            _Variables.TaxRate = Row2.Field<decimal>("Tax_Rate");
            _Variables.Description = Row2.Field<string>("Description");
            _Variables.Project = Row2.Field<int>("Project");

            _Variables.TitleInventory = GetTitle(StockList, _Variables.Inventory);
            _Variables.TitleTax = GetTitle(TaxesList, _Variables.Tax);

            RecordList ??= new();
            RecordList.Add(_Variables);

        }
        #endregion

        #region Top - Next - Back - Last
        public IActionResult OnPostTop()
        {
            if (RecordList.Count > 0)
            {
                LoadData(Variables.ID1, Variables.Sr_No);
                Variables = RecordList.First();
                return RedirectToPage(routeValues: new { ID = Variables.ID1, SRNO = Variables.Sr_No });
            }
            return Page();

        }
        public IActionResult OnPostNext()
        {
            if (RecordList.Count > 0)
            {
                LoadData(Variables.ID1, Variables.Sr_No);
                var _NextSrNo = Variables.Sr_No + 1;
                if (_NextSrNo > RecordList.Last().Sr_No) { _NextSrNo = RecordList.Last().Sr_No; }
                Variables = RecordList.Where(x => x.Sr_No == _NextSrNo).First();
                return RedirectToPage(routeValues: new { ID = Variables.ID1, SRNO = Variables.Sr_No });
            }
            return Page();
        }
        public IActionResult OnPostBack()
        {
            if (RecordList.Count > 0)
            {
                LoadData(Variables.ID1, Variables.Sr_No);
                var _BackSrNo = Variables.Sr_No - 1;
                if (_BackSrNo < 0) { _BackSrNo = RecordList.First().Sr_No; }
                Variables = RecordList.Where(x => x.Sr_No == _BackSrNo).First();
                return RedirectToPage(routeValues: new { ID = Variables.ID1, SRNO = Variables.Sr_No });
            }
            return Page();
        }
        public IActionResult OnPostLast()
        {
            if (RecordList.Count > 0)
            {
                LoadData(Variables.ID1, Variables.Sr_No);
                Variables = RecordList.Last();
                return RedirectToPage(routeValues: new { ID = Variables.ID1, SRNO = Variables.Sr_No });
            }
            return Page();
        }
        #endregion

        #region Back
        public IActionResult OnPostBackPage()
        {
            return RedirectToPage("./Accounts/BillReceivableList");
        }
        #endregion

        #region Parameters Variables

        public class DataFields
        {
            [Required] public int ID1 { get; set; }
            [Required] public int ID2 { get; set; }

            [Required]
            public string Vou_No { get; set; }
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
        #endregion
    }
}


