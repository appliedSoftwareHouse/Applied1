using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
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
        public DataTable tb_BillPay1 { get; set; }
        public DataTable tb_BillPay2 { get; set; }

        public List<Message> ErrorMessages = new();
        public int ErrorCount { get => ErrorMessages.Count; }

        #region Get / Post

        public void OnGet(int? id)
        {
            id ??= 0;                                           // Assign Zero if id = null;
            SetVariable((int)id);
        }

        public IActionResult OnGetEdit(int id, int Srno)
        {
            SetVariable(id, Srno);
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

            Row1["ID"] = Variables.ID;
            Row1["Vou_No"] = Variables.Vou_No;
            Row1["Vou_Date"] = Variables.Vou_Date;
            Row1["Pay_Date"] = Variables.Pay_Date;
            Row1["Company"] = Variables.Company;
            Row1["Inv_No"] = Variables.Inv_No;
            Row1["Inv_Date"] = Variables.Inv_Date;
            Row1["Ref_No"] = Variables.Ref_No;
            Row1["Amount"] = Variables.Amount;
            Row1["Description"] = Variables.Description;
            Row1["Comments"] = Variables.Comments;

            Row2["ID"] = Variables.ID2;
            Row2["TranID"] = Variables.ID;
            Row2["Sr_No"] = Variables.SR_No;
            Row2["Inventory"] = Variables.Inventory;
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
                BillPay1.Save();

                Row2["TranID"] = BillPay1.CurrentRow["ID"];                                                                 // Assign a TranID value from New ID of Bill Payable
                BillPay2.Save();
                return RedirectToPage("BillPayable", routeValues: new { id = (int)Row1["ID"] });

            }
            else
            {
                ErrorMessages.AddRange(BillPay1.TableValidation.MyMessages);
                ErrorMessages.AddRange(BillPay2.TableValidation.MyMessages);
                return Page();
            }
        }
        public IActionResult OnPostBack()
        {
            return RedirectToPage("./BillPayableList");

        }

        #endregion

        #region Methods
        private void SetVariable(int id)
        {
            string UserName = User.Identity.Name;
            DataTableClass BillPay1 = new(UserName, Tables.BillPayable);
            DataTableClass BillPay2 = new(UserName, Tables.BillPayable2);

            BillPay1.MyDataView.RowFilter = string.Concat("ID=", id.ToString());
            BillPay2.MyDataView.RowFilter = string.Concat("TranID=", id.ToString());

            if (BillPay1.MyDataView.Count == 0)
            {
                DataRow Row1 = null;
                DataRow Row2 = null;
                BillRecords = new DataTable();
                SetVariablesRows(Row1, Row2);
            }
            else
            {
                DataRow Row1 = BillPay1.MyDataView[0].Row;
                DataRow Row2 = BillPay2.MyDataView[0].Row;
                BillRecords = BillPay2.MyDataView.ToTable();
                SetVariablesRows(Row1, Row2);
            }
        }
        public void SetVariable(int id, int id2)
        {
            string UserName = User.Identity.Name;
            DataTableClass BillPay1 = new(UserName, Tables.BillPayable);
            DataTableClass BillPay2 = new(UserName, Tables.BillPayable2);

            BillPay2.MyDataView.RowFilter = string.Concat("TranID=", id.ToString());
            BillRecords = BillPay2.MyDataView.ToTable();                                                            // DataTabale for Table Rows

            DataRow Row1, Row2;

            BillPay1.MyDataView.RowFilter = string.Concat("ID=", id.ToString());
            Row1 = BillPay1.MyDataView[0].Row;
            


            if (id2 != 0)
            {
                BillPay2.MyDataView.RowFilter = string.Concat("ID=", id2.ToString());
                Row2 = BillPay2.MyDataView[0].Row;
            }
            else
            {
                Row2 = BillPay2.NewRecord();
                Row2["TranID"] = Variables.ID;
                Row2["SR_No"] = MaxSrNo();
                BillPay2.MyDataTable.Rows.Add(Row2);
            }

            SetVariablesRows(Row1, Row2);                                                                                   // Assign values to Model Variables.
            TotInvoice();                                                                                                                   // Compute Bill Total Amount

            BillPay2.MyDataView.Sort = "SR_No";
            BillPay2.MyDataView.RowFilter = string.Concat("TranID=", id.ToString());
            BillRecords = BillPay2.MyDataView.ToTable();

        }
        private void SetVariablesRows(DataRow? Row1, DataRow? Row2)
        {
            string UserName = User.Identity.Name;
            bool IsNewVoucher = false; if (Row1 == null) { IsNewVoucher = true; }
            Row1 ??= AppFunctions.GetNewRow(UserName, Tables.BillPayable);
            Variables = new();
            if (IsNewVoucher)
            {
                Variables.ID = 0;
                Variables.Vou_No = AppFunctions.GetNewBillPayableVoucher(UserName);
                Variables.Vou_Date = DateTime.Now;
                Variables.Pay_Date = DateTime.Now;
                Variables.Company = 0;
                Variables.Ref_No = string.Empty;
                Variables.Inv_No = string.Empty;
                Variables.Inv_Date = DateTime.Now;
                Variables.Amount = 0.00M;
                Variables.Description = string.Empty;
                Variables.Comments = string.Empty;
            }
            else
            {
                if (Row1["Description"] == DBNull.Value) { Row1["Description"] = string.Empty; }
                if (Row1["Comments"] == DBNull.Value) { Row1["Comments"] = string.Empty; }

                Variables.ID = (int)Row1["ID"];
                Variables.Vou_No = (string)Row1["Vou_No"];
                Variables.Vou_Date = (DateTime)Row1["Vou_Date"];
                Variables.Pay_Date = (DateTime)Row1["Pay_Date"];
                Variables.Company = (int)Row1["Company"];
                Variables.Ref_No = (string)Row1["Ref_No"];
                Variables.Inv_No = (string)Row1["Inv_No"];
                Variables.Inv_Date = (DateTime)Row1["Inv_Date"];
                Variables.Amount = (decimal)Row1["Amount"];
                Variables.Description = (string)Row1["Description"];
                Variables.Comments = (string)Row1["Comments"];
            }

            // ========================================================================================== New Transaction (Record)
            bool IsNewRecord = false; if (Row2 == null || (int)Row2["ID"]==0) { IsNewRecord = true; }
            Row2 ??= AppFunctions.GetNewRow(UserName, Tables.BillPayable2);

            if (Row2["Description"] == DBNull.Value) { Row2["Description"] = string.Empty; }
            if (Row2["Description"] == DBNull.Value) { Row2["Description"] = string.Empty; }

            if (IsNewRecord)
            {
                Variables.ID2 = 0;
                Variables.TranID = Variables.ID;
                Variables.SR_No = 0;
                Variables.Inventory = 0;
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
                Variables.ID2 = (int)Row2["ID"];
                Variables.TranID = (int)Row2["TranID"];
                Variables.SR_No = (int)Row2["Sr_No"];
                Variables.Inventory = (int)Row2["Inventory"];
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
            
            BillRecords = 
            
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
            Variables.TotQty = AppFunctions.GetSum(UserName, Tables.fun_BillPayableAmounts, "Qty", "TranID=" + Variables.ID.ToString());
            Variables.TotAmt = AppFunctions.GetSum(UserName, Tables.fun_BillPayableAmounts, "Amount", "TranID=" + Variables.ID.ToString());
            Variables.TotTax = AppFunctions.GetSum(UserName, Tables.fun_BillPayableAmounts, "TaxAmount", "TranID=" + Variables.ID.ToString());
            Variables.TotInv = Variables.TotAmt + Variables.TotTax;
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
        public string Ref_No { get; set; }
        public string Inv_No { get; set; }
        public DateTime Inv_Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }

        public int ID2 { get; set; }
        public int TranID { get; set; }
        public int SR_No { get; set; }
        public int Inventory { get; set; }
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