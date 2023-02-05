using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
using System.Drawing;
using System.ServiceModel.Security;
using System.Xml.Linq;
using static Applied_WebApplication.Pages.Stock.InventoryModel;

namespace Applied_WebApplication.Pages.Accounts
{
    public class BillPayableModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; } = new();
        public DataTable BillRecords { get; set; }
        public int ErrorCount { get => ErrorMessages.Count; }
        public bool IsPosted = false;
        public List<Message> ErrorMessages = new();

        #region Get / Post

        public void OnGet(int? id, int? id2)
        {
            id ??= 0;                                           // Assign Zero if id = null;
            id2 ??= 0;
            SetVariable((int)id, (int)id2);
        }

        public IActionResult OnGetDelete(int? id, int? id2)
        {
            id ??= 0;
            id2 ??= 0;

            if (id2 > 0)
            {
                string UserName = User.Identity.Name;
                DataTableClass BillPay2 = new(UserName, Tables.BillPayable2);
                if (BillPay2.Seek((int)id2))
                {
                    BillPay2.SeekRecord((int)id2);
                    BillPay2.Delete();
                    BillPay2 = new(UserName, Tables.BillPayable2);              // Reload Datatable after delete a record.

                    // Adjust Serial Number of Bill Payable Table 2
                    BillPay2.MyDataView.RowFilter = String.Concat("TranID=", id.ToString());
                    if(BillPay2.MyDataView.Count>0)
                    {
                        int Sr_No = 1;
                        foreach(DataRow Row in BillPay2.MyDataView.ToTable().Rows)
                        {
                            BillPay2.CurrentRow = Row;
                            BillPay2.CurrentRow["Sr_No"] = Sr_No; Sr_No++;
                            BillPay2.Save();
                        }
                    }


                    // Check all the records of the Bill Payable has been deleted, if yes then delete the record of Bill Payable Master Record.
                    BillPay2.MyDataView.RowFilter = string.Concat("TranID=", id.ToString());
                    if(BillPay2.MyDataView.Count==0)
                    {
                        DataTableClass BillPay1 = new(UserName, Tables.BillPayable);
                        if(BillPay1.Seek((int)id))
                        {
                            BillPay1.SeekRecord((int)id);
                            BillPay1.Delete();                                              // Delete the record from Mater Bill Payable Table.
                            return RedirectToPage("BillPayableList");
                        }
                        else
                        {
                           
                        }
                    }
                    return RedirectToPage("BillPayable", routeValues: new { id, id2 = -1 });
                }
            }
            return Page();
        }

        public IActionResult OnPostNew()
        {
            SetVariable(Variables.ID, 0);
            return Page();
        }

        public IActionResult OnPostSave()
        {
            string UserName = User.Identity.Name;
            DataTableClass BillPay1 = new DataTableClass(UserName, Tables.BillPayable);
            DataTableClass BillPay2 = new DataTableClass(UserName, Tables.BillPayable2);

            DataRow Row1 = BillPay1.NewRecord();
            DataRow Row2 = BillPay2.NewRecord();

            Variables.Status = VoucherStatus.Submitted.ToString();

            Row1["ID"] = Variables.ID;
            Row1["Vou_No"] = Variables.Vou_No;
            Row1["Vou_Date"] = Variables.Vou_Date;
            Row1["Pay_Date"] = Variables.Pay_Date;
            Row1["Company"] = Variables.Company;
            Row1["Employee"] = Variables.Employee;
            Row1["Inv_No"] = Variables.Inv_No;
            Row1["Inv_Date"] = Variables.Inv_Date;
            Row1["Ref_No"] = Variables.Ref_No;
            Row1["Amount"] = Variables.Amount;
            Row1["Description"] = Variables.Description;
            Row1["Comments"] = Variables.Comments;
            Row1["Status"] = Variables.Status;

            Row2["ID"] = Variables.ID2;
            Row2["TranID"] = Variables.ID;
            Row2["Sr_No"] = Variables.SR_No;
            Row2["Inventory"] = Variables.Inventory;
            Row2["Project"] = Variables.Project;
            Row2["Batch"] = Variables.Batch;
            Row2["Qty"] = Variables.Qty;
            Row2["Rate"] = Variables.Rate;
            Row2["Tax"] = Variables.Tax;
            Row2["Tax_Rate"] = Variables.Tax_Rate;
            Row2["Description"] = Variables.Description2;

            CommandAction SQLAction;
            if ((int)Row1["ID"] == 0) { SQLAction = CommandAction.Insert; } else { SQLAction = CommandAction.Update; }
            bool BillValid1 = BillPay1.TableValidation.Validation(Row1, SQLAction);

            if ((int)Row2["ID"] == 0) { SQLAction = CommandAction.Insert; } else { SQLAction = CommandAction.Update; }
            bool BillValid2 = BillPay2.TableValidation.Validation(Row2, SQLAction);

            if (BillValid1 && BillValid2)
            {
                BillPay1.CurrentRow["Status"] = VoucherStatus.Submitted.ToString();
                BillPay1.Save();

                Row2["TranID"] = BillPay1.CurrentRow["ID"];                                                                 // Assign a TranID value from New ID of Bill Payable
                BillPay2.Save();
            }
            else
            {
                ErrorMessages.AddRange(BillPay1.TableValidation.MyMessages);
                ErrorMessages.AddRange(BillPay2.TableValidation.MyMessages);
                return Page();
            }

            BillPay2.MyDataView.RowFilter = string.Concat("TranID=", Variables.ID.ToString());      // Refresh Bill Records (Table)
            BillRecords = BillPay2.MyDataView.ToTable();                                                            // DataTabale for Table Rows
            return RedirectToPage("BillPayable", routeValues: new { id = (int)Row1["ID"], id2 = (int)Row2["ID"] });

        }
        public IActionResult OnPostBack()
        {
            return RedirectToPage("./BillPayableList");

        }
        public IActionResult OnPostDelete(int id, int id2)
        {
            string UserName = User.Identity.Name;
            DataTableClass BillPay2 = new(UserName, Tables.BillPayable2);



            return Page();
        }

        #endregion

        #region Methods

        public void SetVariable(int id, int id2)
        {
            string UserName = User.Identity.Name;
            DataTableClass BillPay1 = new(UserName, Tables.BillPayable);
            DataTableClass BillPay2 = new(UserName, Tables.BillPayable2);

            BillPay2.MyDataView.RowFilter = string.Concat("TranID=", id.ToString());
            BillRecords = BillPay2.MyDataView.ToTable();                                                            // DataTabale for Table Rows
            DataRow Row1, Row2;

            Row1 = BillPay1.NewRecord();
            Row2 = BillPay2.NewRecord();

            BillPay1.MyDataView.RowFilter = string.Concat("ID=", id.ToString());                        // Get Record of Bill Payable 1
            if (BillPay1.MyDataView.Count > 0) { Row1 = BillPay1.MyDataView[0].Row; }

            if (id2 == -1)                                          // Get First record of Bill Payable 2 Table
            {
                if (BillRecords.Rows.Count > 0) { Row2 = BillRecords.Rows[0]; }
            }

            if (id2 == 0)                                          // Create a new record.
            {
                Row2 = BillPay2.NewRecord();
                Row2["TranID"] = id;
                Row2["SR_No"] = MaxSrNo();
                BillPay2.MyDataTable.Rows.Add(Row2);
                BillRecords = BillPay2.MyDataView.ToTable();                                                            // DataTabale for Table Rows
            }

            if (id2 > 0)
            {
                BillPay2.MyDataView.RowFilter = String.Concat("TranID=", id.ToString(), " AND ID=", id2.ToString());
                if (BillPay2.MyDataView.Count > 0) { Row2 = BillPay2.MyDataView[0].Row; }
            }

            SetVariablesRows(Row1, Row2);                                                                                   // Assign values to Model Variables.
            BillPay2.MyDataView.Sort = "SR_No";
            BillPay2.MyDataView.RowFilter = string.Concat("TranID=", id.ToString());
            BillRecords = BillPay2.MyDataView.ToTable();

            TotInvoice();                                                                                                                   // Compute Bill Total Amount


        }
        private void SetVariablesRows(DataRow? Row1, DataRow? Row2)
        {
            string UserName = User.Identity.Name;
            bool IsNewVoucher = false; if ((int)Row1["ID"] == 0) { IsNewVoucher = true; }
            Row1 ??= AppFunctions.GetNewRow(UserName, Tables.BillPayable);                      // Get a new row if row is null;
            Variables = new();
            if (IsNewVoucher)
            {
                Variables.ID = 0;
                Variables.Vou_No = AppFunctions.GetNewBillPayableVoucher(UserName);
                Variables.Vou_Date = DateTime.Now;
                Variables.Pay_Date = DateTime.Now;
                Variables.Company = 0;
                Variables.Employee = 0;
                Variables.Ref_No = string.Empty;
                Variables.Inv_No = string.Empty;
                Variables.Inv_Date = DateTime.Now;
                Variables.Amount = 0.00M;
                Variables.Description = string.Empty;
                Variables.Comments = string.Empty;
                Variables.Status = VoucherStatus.Submitted.ToString(); ;
            }
            else
            {

                if (Row1["ID"] == DBNull.Value) { Row1["ID"] = 0; }
                if (Row1["Vou_No"] == DBNull.Value) { Row1["Vou_No"] = string.Empty; }
                if (Row1["Vou_Date"] == DBNull.Value) { Row1["Vou_Date"] = DateTime.Now; }
                if (Row1["Pay_Date"] == DBNull.Value) { Row1["Pay_Date"] = DateTime.Now; }
                if (Row1["Company"] == DBNull.Value) { Row1["Company"] = 0; }
                if (Row1["Employee"] == DBNull.Value) { Row1["Employee"] = 0; }
                if (Row1["Ref_No"] == DBNull.Value) { Row1["Ref_No"] = string.Empty; }
                if (Row1["Inv_No"] == DBNull.Value) { Row1["Inv_No"] = string.Empty; }
                if (Row1["Inv_Date"] == DBNull.Value) { Row1["Inv_Date"] = DateTime.Now; }
                if (Row1["Amount"] == DBNull.Value) { Row1["Amount"] = 0.00M; }
                if (Row1["Description"] == DBNull.Value) { Row1["Description"] = string.Empty; }
                if (Row1["Comments"] == DBNull.Value) { Row1["Comments"] = string.Empty; }
                if (Row1["Status"] == DBNull.Value) { Row1["Status"] = VoucherStatus.Submitted.ToString(); }

                Variables.ID = (int)Row1["ID"];
                Variables.Vou_No = (string)Row1["Vou_No"];
                Variables.Vou_Date = (DateTime)Row1["Vou_Date"];
                Variables.Pay_Date = (DateTime)Row1["Pay_Date"];
                Variables.Company = (int)Row1["Company"];
                Variables.Employee = (int)Row1["Employee"];
                Variables.Ref_No = (string)Row1["Ref_No"];
                Variables.Inv_No = (string)Row1["Inv_No"];
                Variables.Inv_Date = (DateTime)Row1["Inv_Date"];
                Variables.Amount = (decimal)Row1["Amount"];
                Variables.Description = (string)Row1["Description"];
                Variables.Comments = (string)Row1["Comments"];
                Variables.Status = (string)Row1["Status"];
            }

            // ========================================================================================== New Transaction (Record)
            bool IsNewRecord = false; if (Row2 == null || (int)Row2["ID"] == 0) { IsNewRecord = true; }
            Row2 ??= AppFunctions.GetNewRow(UserName, Tables.BillPayable2);

            if (IsNewRecord)
            {
                Variables.ID2 = 0;
                Variables.TranID = Variables.ID;
                Variables.SR_No = 0;
                Variables.Inventory = 0;
                Variables.Project = 0;
                Variables.Batch = string.Empty;
                Variables.Qty = 0.00M;
                Variables.Rate = 0.00M;
                Variables.Tax = 0;
                Variables.Tax_Rate = 0.00M;
                Variables.Description2 = string.Empty;
                if (Variables.ID2 == 0) { Variables.SR_No = MaxSrNo(); }                    // Get A Max SR No.
            }
            else
            {

                if (Row2["ID"] == DBNull.Value) { Row2["ID"] = 0; }
                if (Row2["TranID"] == DBNull.Value) { Row2["TranID"] = 0; }
                if (Row2["Sr_No"] == DBNull.Value) { Row2["Sr_No"] = 0; }
                if (Row2["Inventory"] == DBNull.Value) { Row2["Inventory"] = 0; }
                if (Row2["Project"] == DBNull.Value) { Row2["Project"] = 0; }
                if (Row2["Batch"] == DBNull.Value) { Row2["Batch"] = string.Empty; }
                if (Row2["Qty"] == DBNull.Value) { Row2["Qty"] = 0.00M; }
                if (Row2["Rate"] == DBNull.Value) { Row2["Rate"] = 0.00M; }
                if (Row2["Tax"] == DBNull.Value) { Row2["Tax"] = 0; }
                if (Row2["Tax_Rate"] == DBNull.Value) { Row2["Tax_Rate"] = 0.00M; }
                if (Row2["Description"] == DBNull.Value) { Row2["Description"] = string.Empty; }

                Variables.ID2 = (int)Row2["ID"];
                Variables.TranID = (int)Row2["TranID"];
                Variables.SR_No = (int)Row2["Sr_No"];
                Variables.Inventory = (int)Row2["Inventory"];
                Variables.Project = (int)Row2["Project"];
                Variables.Batch = (string)Row2["Batch"];
                Variables.Qty = (decimal)Row2["Qty"];
                Variables.Rate = (decimal)Row2["Rate"];
                Variables.Tax = (int)Row2["Tax"];
                Variables.Tax_Rate = (decimal)Row2["Tax_Rate"];
                Variables.Description2 = (string)Row2["Description"];

            }

            //-----------------------------------------------------------------------------------  CALCUALT Transaction total
            Variables.TranAmount = Variables.Qty * Variables.Rate;
            Variables.Amount = Variables.TranAmount;
            Variables.TaxAmount = (Variables.TranAmount * AppFunctions.GetTaxRate(UserName, Variables.Tax)) / 100;
            Variables.NetAmount = Variables.TranAmount + Variables.TaxAmount;
            Variables.Tax_Rate = AppFunctions.GetTaxRate(UserName, Variables.Tax);
            // ------------------------------------------------------------------------------------------------------- Get Bill Records.

            BillRecords = AppFunctions.GetRecords(UserName, Tables.BillPayable2, "TranID=" + Variables.ID);
        }
        private int MaxSrNo()
        {
            string UserName = User.Identity.Name;
            DataTableClass Temp1 = new(UserName, Tables.BillPayable2);
            Temp1.MyDataView.RowFilter = string.Concat("TranID=", Variables.ID);
            var Temp2 = Temp1.MyDataView.ToTable();
            int _MaxSrNo = 0;
            try
            {
                _MaxSrNo = (int)Temp2.Compute("Max(Sr_No)", "TranID=" + Variables.ID);
            }
            catch (Exception)
            {
                _MaxSrNo = 0;
            }

            return _MaxSrNo + 1;
        }
        private void TotInvoice()
        {
            string UserName = User.Identity.Name;
            try
            {
                Variables.TotQty = (decimal)AppFunctions.GetSum(UserName, Tables.fun_BillPayableAmounts, "Qty", "TranID=" + Variables.ID.ToString());
                Variables.TotAmt = (decimal)AppFunctions.GetSum(UserName, Tables.fun_BillPayableAmounts, "Amount", "TranID=" + Variables.ID.ToString());
                Variables.TotTax = (decimal)AppFunctions.GetSum(UserName, Tables.fun_BillPayableAmounts, "TaxAmount", "TranID=" + Variables.ID.ToString());
                Variables.TotInv = Variables.TotAmt + Variables.TotTax;
            }
            catch (Exception)
            {
                Variables.TotQty = -1;
                Variables.TotAmt = -1;
                Variables.TotTax = -1;
                Variables.TotInv = -1;
            }
            

        }
    }

    #endregion

    #region Paramaters
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