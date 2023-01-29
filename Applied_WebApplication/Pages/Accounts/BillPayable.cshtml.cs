using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.ServiceModel.Security;
using static Applied_WebApplication.Pages.Stock.InventoryModel;

namespace Applied_WebApplication.Pages.Accounts
{
    public class BillPayableModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; } = new();
        public DataTable BillRecords { get; set; }

        public List<Message> ErrorMessages = new();
        public int ErrorCount { get => ErrorMessages.Count; }

        #region Get / Post

        public void OnGet(int? id)
        {
            id ??= 1;                                           // Assign Zero if id = null;
            SetVariable((int)id);
        }

        public void OnGetEdit(int id, int Srno)
        {
            SetVariable(id, Srno);
        }
        public void OnGetNew()
        {
            SetVariable(Variables.ID, 0);
        }

        public void OnPostNew()
        {
            SetVariable(Variables.ID, 0);
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

            CommandAction SQLAction; if ((int)Row1["ID"] == 0) { SQLAction = CommandAction.Insert; } else { SQLAction = CommandAction.Update; }
            bool BillValid1 = BillPay1.TableValidation.Validation(Row1, SQLAction);
            bool BillValid2 = BillPay2.TableValidation.Validation(Row2, SQLAction);

            if (BillValid1 && BillValid2)
            {
                BillPay1.Save();
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

            TotInvoice();

        }
        public void SetVariable(int id, int Srno)
        {
            string UserName = User.Identity.Name;
            DataTableClass BillPay1 = new(UserName, Tables.BillPayable);
            DataTableClass BillPay2 = new(UserName, Tables.BillPayable2);
            DataRow Row1, Row2;

            BillPay1.MyDataView.RowFilter = string.Concat("ID=", id.ToString());
            Row1 = BillPay1.MyDataView[0].Row;

            if (Srno != 0)
            {
                BillPay2.MyDataView.RowFilter = string.Concat("ID=", Srno.ToString());
                Row2 = BillPay2.MyDataView[0].Row;
            }
            else
            {
                Row2 = BillPay2.NewRecord();
            }
            
            SetVariablesRows(Row1, Row2);                                                                                   // Assign values to Model Variables.

            BillPay2.MyDataView.RowFilter = string.Concat("TranID=", id.ToString());
            BillRecords = BillPay2.MyDataView.ToTable();                                                            // DataTabale for Table Rows

            TotInvoice();                                                                                                                  // Compute Bill Total Amount
        }
        private void SetVariablesRows(DataRow? Row1, DataRow? Row2)
        {
            string UserName = User.Identity.Name;
            if (Row2["Description"] == DBNull.Value) { Row2["Description"] = string.Empty; }

            if (Row1 == null)
            {
                Variables = new()
                {
                    ID = 0,
                    Vou_No = AppFunctions.GetNewBillPayableVoucher(UserName),
                    Vou_Date = DateTime.Now,
                    Pay_Date = DateTime.Now,
                    Company = 0,
                    Ref_No = string.Empty,
                    Inv_No = string.Empty,
                    Inv_Date = DateTime.Now,
                    Amount = 0.00M,
                    Description = string.Empty,
                    Comments = string.Empty,

                    ID2 = 0,
                    TranID = 0,
                    SR_No = 1,
                    Inventory = 0,
                    Batch = string.Empty,
                    Qty = 0.00M,
                    Rate = 0.00M,
                    Tax = 0,
                    Tax_Rate = 0.00M,
                    Description2 = string.Empty
                };
            }
            else
            {
                Variables = new()
                {
                    ID = (int)Row1["ID"],
                    Vou_No = (string)Row1["Vou_No"],
                    Vou_Date = (DateTime)Row1["Vou_Date"],
                    Pay_Date = (DateTime)Row1["Pay_Date"],
                    Company = (int)Row1["Company"],
                    Ref_No = (string)Row1["Ref_No"],
                    Inv_No = (string)Row1["Inv_No"],
                    Inv_Date = (DateTime)Row1["Inv_Date"],
                    Amount = (decimal)Row1["Amount"],
                    Description = (string)Row1["Description"],
                    Comments = (string)Row1["Comments"],

                    ID2 = (int)Row2["ID"],
                    TranID = (int)Row2["TranID"],
                    SR_No = (int)Row2["Sr_No"],
                    Inventory = (int)Row2["Inventory"],
                    Batch = (string)Row2["Batch"],
                    Qty = (decimal)Row2["Qty"],
                    Rate = (decimal)Row2["Rate"],
                    Tax = (int)Row2["Tax"],
                    Tax_Rate = (decimal)Row2["Tax_Rate"],
                    Description2 = (string)Row2["Description"]
                };

                Variables.TranAmount = Variables.Qty * Variables.Rate;
                Variables.Amount = Variables.TranAmount;
                Variables.TaxAmount = (Variables.TranAmount * AppFunctions.GetTaxRate(UserName, Variables.Tax)) / 100;
                Variables.NetAmount = Variables.TranAmount + Variables.TaxAmount;
                Variables.Tax_Rate = AppFunctions.GetTaxRate(UserName, Variables.Tax);

                if(Variables.ID2==0)
                {
                    Variables.SR_No = MaxSrNo();
                }
            }
        }

        private int MaxSrNo()
        {
            string UserName = User.Identity.Name;
            DataTableClass BillPay2 = new(UserName, Tables.BillPayable2);
            BillPay2.MyDataView.RowFilter = string.Concat("TranID=", Variables.ID);
            BillRecords = BillPay2.MyDataView.ToTable();                                                            // DataTabale for Table Rows
            int _MaxSrNo = (int)BillRecords.Compute("Max(Sr_No)", "");
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