using Applied_WebApplication.Data;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.Design.Serialization;
using System.Data;
using static Applied_WebApplication.Pages.Stock.InventoryModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace Applied_WebApplication.Pages.Accounts
{
    public class BillReceivableModel : PageModel
    {


        [BindProperty]
        public MyParameters Variables { get; set; } = new();
        public DataTable BillReceivable2 { get; set; }
        public DataTableClass TempBillReceivable { get; set; }
        public List<Message> ErrorMessages { get; set; } = new();
        public string UserName => User.Identity.Name;

        public void OnGet(int? ID, int? TranID)
        {
            if (ID == null || ID == 0)
            {
                Variables = new()
                {
                    Vou_No = AppFunctions.GetBillReceivableVoucher(UserName),
                    Vou_Date = DateTime.Now,
                    Pay_Date = DateTime.Now,
                    Inv_Date = DateTime.Now,
                    SR_No = 1
                };
            }
            else
            {
                TempBillReceivable = new(UserName, Tables.view_BillReceivable, true, ID, TranID);
                Variables = GetVariables();
            }
        }

        public IActionResult OnPostAdd()
        {
            GetVariablesAdd(Variables.ID);
            return Page();
        }

        private MyParameters GetVariablesAdd(int id)
        {
            DataTableClass BillReceivable = new(UserName, Tables.view_BillReceivable);
            DataRow FirstRow, NewRow;
            BillReceivable.MyDataView.RowFilter = "TranID=" + Variables.TranID;
            FirstRow = BillReceivable.SeekRecord(id);
            NewRow = BillReceivable.NewRecord();

            //DataTable BillReceivable2 = AppFunctions.GetRecords(UserName, Tables.view_BillReceivable, "TranID=" + Variables.TranID);


            //DataTable Table1 = TempReceivable(Tables.view_BillReceivable, BillReceivable2);
            MyParameters _Variables = new();
            //DataTableClass BillReceivable = new(UserName, Tables.view_BillReceivable);
            //BillReceivable.SeekRecord((int)id);
            //DataRow FirstRow = BillReceivable.CurrentRow;


            //================================================================================
            //   Assign values for new record (ADD)
            //BillReceivable.NewRecord();
            _Variables = new()
            {
                ID = 0,
                Vou_No = FirstRow["Vou_No"].ToString(),
                Vou_Date = (DateTime)FirstRow["Vou_Date"],
                Company = (int)FirstRow["Company"],
                Employee = (int)FirstRow["Employee"],
                Ref_No = FirstRow["Ref_No"].ToString(),
                Inv_No = FirstRow["Inv_No"].ToString(),
                Inv_Date = (DateTime)FirstRow["Inv_Date"],
                Pay_Date = (DateTime)FirstRow["Pay_Date"],
                Amount = (decimal)FirstRow["Amount"],
                Description = FirstRow["Description"].ToString(),
                Comments = FirstRow["Comments"].ToString(),
                Status = FirstRow["Status"].ToString(),

                ID2 = 0,
                SR_No = MaxSrNo(id) + 1,
                TranID = (int)FirstRow["TranID"],
                Inventory = (int)NewRow["Inventory"],
                Batch = NewRow["Batch"].ToString(),
                Qty = (decimal)NewRow["Qty"],
                Rate = (decimal)NewRow["Rate"],
                Tax = (int)NewRow["Tax"],
                Tax_Rate = (decimal)NewRow["Tax_Rate"],
                Description2 = NewRow["Description2"].ToString(),
                Project = (int)NewRow["Project"]
            };

            BillReceivable.MyDataView.RowFilter = string.Concat("TranID=", _Variables.TranID.ToString());
            if (BillReceivable.Count > 0) { BillReceivable2 = BillReceivable.MyDataView.ToTable(); }

            return _Variables;
        }


        public IActionResult OnPostSave()
        {
            DataTableClass TempBillReceivable = new(UserName, Tables.view_BillReceivable, true);
           
            TempBillReceivable.NewRecord();
            TempBillReceivable.CurrentRow = GetRow(Variables);
            TempBillReceivable.Save();

            return Page();
            
        }


        public IActionResult OnPostSaveFinal()
        {
            DataTableClass _Table1 = new(UserName, Tables.BillReceivable);
            DataTableClass _Table2 = new(UserName, Tables.BillReceivable2);

            _Table1.SeekRecord(Variables.ID);
            _Table1.CurrentRow["ID"] = Variables.ID;
            _Table1.CurrentRow["Vou_no"] = Variables.Vou_No;
            _Table1.CurrentRow["Vou_Date"] = Variables.Vou_Date;
            _Table1.CurrentRow["Company"] = Variables.Company;
            _Table1.CurrentRow["Employee"] = Variables.Employee;
            _Table1.CurrentRow["Ref_No"] = Variables.Ref_No;
            _Table1.CurrentRow["Inv_No"] = Variables.Inv_No;
            _Table1.CurrentRow["Inv_Date"] = Variables.Inv_Date;
            _Table1.CurrentRow["Inv_No"] = Variables.Inv_No;
            _Table1.CurrentRow["Inv_Date"] = Variables.Inv_Date;
            _Table1.CurrentRow["Pay_Date"] = Variables.Pay_Date;
            _Table1.CurrentRow["Amount"] = Variables.InvAmount;
            _Table1.CurrentRow["Description"] = Variables.Description;
            _Table1.CurrentRow["Comments"] = Variables.Comments;
            _Table1.CurrentRow["Status"] = VoucherStatus.Submitted.ToString();

            _Table2.SeekRecord(Variables.ID2);
            _Table2.CurrentRow["ID"] = Variables.ID2;
            _Table2.CurrentRow["Sr_No"] = MaxSrNo(Variables.ID);
            _Table2.CurrentRow["Inventory"] = Variables.Inventory;
            _Table2.CurrentRow["Batch"] = Variables.Batch;
            _Table2.CurrentRow["Qty"] = Variables.Qty;
            _Table2.CurrentRow["Rate"] = Variables.Rate;
            _Table2.CurrentRow["Tax"] = Variables.Tax;
            _Table2.CurrentRow["Tax_Rate"] = Variables.Tax_Rate;
            _Table2.CurrentRow["Description"] = Variables.Description;
            _Table2.CurrentRow["Project"] = Variables.Project;

            CommandAction _Action1;
            CommandAction _Action2;
            if (_Table1.Seek(Variables.ID)) { _Action1 = CommandAction.Update; } else { _Action1 = CommandAction.Insert; }
            if (_Table1.Seek(Variables.ID2)) { _Action2 = CommandAction.Update; } else { _Action2 = CommandAction.Insert; }

            bool TableValid1 = _Table1.TableValidation.Validation(_Table1.CurrentRow, _Action1);
            bool TableValid2 = _Table2.TableValidation.Validation(_Table2.CurrentRow, _Action2);

            if (TableValid1 && TableValid2)
            {
                // Save Record id not error found.
                _Table1.Save();

                _Table2.CurrentRow["TranID"] = _Table1.CurrentRow["ID"];
                _Table2.Save();
                return RedirectToPage("./BillReceivable", routeValues: new { id = (int)_Table1.CurrentRow["ID"] });
            }
            else
            {
                // Show errors if errors found.
                ErrorMessages.AddRange(_Table1.TableValidation.MyMessages);
                ErrorMessages.AddRange(_Table2.TableValidation.MyMessages);
                return Page();
            }


        }

        private int MaxSrNo(int id)
        {
            DataTable _Table = AppFunctions.GetRecords(UserName, Tables.BillPayable2, "TranID=" + id.ToString());
            if (_Table.Rows.Count == 0) { return 1; }                                                  // Table is empty, return 1
            if (_Table.Rows.Count > 0) { return (int)_Table.Compute("MAX(Sr_No)", ""); }               // Table is not empty and id =0 return max value
            return id;                                                                                               // Id is already assign, return same value.
        }

        public IActionResult OnPostBack()
        {
            return RedirectToPage("./BillReceivableList");
        }


        #region SET & GET Variables and DataRow Get

        private DataRow GetRow(MyParameters _Variables)
        {
            DataRow Row = TempBillReceivable.NewRecord();
            Row["ID"] = _Variables.ID;
            Row["Vou_No"] = _Variables.Vou_No;
            Row["Vou_Date"] = _Variables.Vou_Date;
            Row["Company"] = _Variables.Company;
            Row["Employee"] = _Variables.Employee;
            Row["Ref_No"] = _Variables.Ref_No;
            Row["Inv_No"] = _Variables.Inv_No;
            Row["Inv_Date"] = _Variables.Inv_Date;
            Row["Pay_Date"] = _Variables.Pay_Date;
            Row["Amount"] = _Variables.Amount;
            Row["Description"] = _Variables.Description;
            Row["Comments"] = _Variables.Comments;
            Row["Status"] = _Variables.Status;

            Row["ID2"] = _Variables.ID2;
            Row["Sr_No"] = _Variables.SR_No;
            Row["TranID"] = _Variables.TranID;
            Row["Inventory"] = _Variables.Inventory;
            Row["Batch"] = _Variables.Batch;
            Row["Qty"] = _Variables.Qty;
            Row["Rate"] = _Variables.Rate;
            Row["Tax"] = _Variables.Tax;
            Row["Tax_Rate"] = _Variables.Tax_Rate;
            Row["Description2"] = _Variables.Description2;
            Row["Project"] = _Variables.Project;

            return Row;
        }
        private MyParameters GetVariables(DataRow Row)
        {
            MyParameters _Variables = new();
            _Variables = new()
            {
                ID = (int)Row["ID"],
                Vou_No = Row["Vou_No"].ToString(),
                Vou_Date = (DateTime)Row["Vou_Date"],
                Company = (int)Row["Company"],
                Employee = (int)Row["Employee"],
                Ref_No = Row["Ref_No"].ToString(),
                Inv_No = Row["Inv_No"].ToString(),
                Inv_Date = (DateTime)Row["Inv_Date"],
                Pay_Date = (DateTime)Row["Pay_Date"],
                Amount = (decimal)Row["Amount"],
                Description = Row["Description"].ToString(),
                Comments = Row["Comments"].ToString(),
                Status = Row["Status"].ToString(),

                ID2 = (int)Row["ID2"],
                SR_No = (int)Row["Sr_No"],
                TranID = (int)Row["TranID"],
                Inventory = (int)Row["Inventory"],
                Batch = Row["Batch"].ToString(),
                Qty = (decimal)Row["Qty"],
                Rate = (decimal)Row["Rate"],
                Tax = (int)Row["Tax"],
                Tax_Rate = (decimal)Row["Tax_Rate"],
                Description2 = Row["Description2"].ToString(),
                Project = (int)Row["Project"]
            };
            //}
            //else
            //{

            //}

            return _Variables;
        }

        #endregion

        #region Paramters

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
        #endregion
    }
}
