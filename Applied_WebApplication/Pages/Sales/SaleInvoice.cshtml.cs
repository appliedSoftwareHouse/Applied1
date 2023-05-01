using Applied_WebApplication.Data;
using Applied_WebApplication.Pages.ReportPrint;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Util;
using System.Data;
using System.Drawing;
using System.Text;
using System.Transactions;

namespace Applied_WebApplication.Pages.Sales
{
    public class SaleInvoiceModel : PageModel
    {
        #region Setup
        [BindProperty]
        public MyParameters Variables { get; set; }
        public List<Message> ErrorMessages { get; set; } = new();
        public string UserName => User.Identity.Name;
        public DataTable Invoice { get; set; }
        public DataRow Row1 { get; set; }
        public DataRow Row2 { get; set; }
        public bool IsSaved { get; set; } = false;
        #endregion

        #region GET

        public void OnGet(string? Vou_No, int? Sr_No)
        {
            Variables = new();
            TempTableClass TempInvoice1;
            TempTableClass TempInvoice2;
            var TranID = 0;

            #region Null Values

            if (Vou_No == null && Sr_No == null)
            {
                Variables.Vou_No = "New";
                Variables.Sr_No = 1;

                TempInvoice1 = new TempTableClass(UserName, Tables.BillReceivable, Variables.Vou_No);
                if (TempInvoice1.CountTemp > 0) { Variables.TranID = (int)TempInvoice1.TempTable.Rows[0]["ID"]; }
                TempInvoice2 = new TempTableClass(UserName, Tables.BillReceivable2, Variables.TranID);

                if (TempInvoice1.CountTemp == 0)
                {
                    TempInvoice1.CurrentRow = TempInvoice1.NewRecord();
                    TempInvoice1.CurrentRow["ID"] = 0;
                    TempInvoice1.CurrentRow["Vou_No"] = "New";
                    TempInvoice1.CurrentRow["Status"] = VoucherStatus.Submitted.ToString();
                }
                else
                {
                    TempInvoice1.CurrentRow = TempInvoice1.TempTable.Rows[0];
                }

                if (TempInvoice2.CountTemp == 0)
                {
                    TempInvoice2.CurrentRow = TempInvoice2.NewRecord();
                    TempInvoice2.CurrentRow["ID"] = 0;
                    TempInvoice2.CurrentRow["Sr_No"] = 1;
                }
                else
                {
                    TempInvoice2.CurrentRow = TempInvoice2.TempTable.Rows[0];
                }

                Row1 = TempInvoice1.CurrentRow;
                Row2 = TempInvoice2.CurrentRow;
                Row2Variables();
                Invoice = TempInvoice2.TempTable;
                return;
            }

            #endregion

            #region Sales Invoices exist

            if (Vou_No != null && Sr_No != null)
            {
                Variables.Vou_No = Vou_No;
                TempInvoice1 = new TempTableClass(UserName, Tables.BillReceivable, Variables.Vou_No);

                if(TempInvoice1.CountSource>0)
                {
                    TempInvoice1.CurrentRow = TempInvoice1.SourceTable.Rows[0];
                    TranID = (int)TempInvoice1.CurrentRow["ID"];
                }

                TempInvoice2 = new TempTableClass(UserName, Tables.BillReceivable2, TranID);

                if(TempInvoice2.CountTemp > 0)
                {
                    if (Sr_No > 0)
                    {
                        TempInvoice2.TempView = TempInvoice2.TempTable.AsDataView();
                        TempInvoice2.TempView.RowFilter = $"Sr_No={Sr_No}";
                        if (TempInvoice2.TempView.Count > 0)
                        {
                            TempInvoice2.CurrentRow = TempInvoice2.TempView[0].Row;
                            Row1 = TempInvoice1.CurrentRow;
                            Row2 = TempInvoice2.CurrentRow;
                            Invoice = TempInvoice2.TempTable;
                        }
                        
                    }
                    if(Sr_No == -1)
                    {
                        TempInvoice2.CurrentRow = TempInvoice2.NewRecord();
                        var Max_SrNo = TempInvoice2.TempTable.Compute("MAX(Sr_No)", "");
                        TempInvoice2.CurrentRow["Sr_No"] = (int)Max_SrNo + 1;
                    }

                    Row1 = TempInvoice1.CurrentRow;
                    Row2 = TempInvoice2.CurrentRow;
                    Invoice = TempInvoice2.TempTable;

                    Row2Variables();
                    return;

                }
            }
            #endregion
        }
        #endregion

        #region Variables / Rows
        private void Row2Variables()
        {

            Variables = new()
            {
                ID1 = (int)Row1["ID"],
                ID2 = (int)Row2["ID"],

                Vou_No = Row1["Vou_No"].ToString(),
                Vou_Date = (DateTime)Row1["Vou_Date"],
                Company = (int)Row1["Company"],
                Employee = (int)Row1["Employee"],
                Ref_No = Row1["Ref_No"].ToString(),
                Inv_No = Row1["Inv_No"].ToString(),
                Inv_Date = (DateTime)Row1["Inv_Date"],
                Pay_Date = (DateTime)Row1["Pay_Date"],
                Remarks = Row1["Description"].ToString(),
                Comments = Row1["Comments"].ToString(),
                Status = Row1["Status"].ToString(),

                TranID = (int)Row1["ID"],
                Sr_No = (int)Row2["Sr_No"],
                Inventory = (int)Row2["Inventory"],
                Batch = Row2["Batch"].ToString(),
                Qty = (decimal)Row2["Qty"],
                Rate = (decimal)Row2["Rate"],
                Tax = (int)Row2["Tax"],
                Description = Row2["Description"].ToString(),
                Project = (int)Row2["Project"],
            };

            Variables.Amount = (Variables.Qty * Variables.Rate);
            Variables.TaxRate = (int)AppFunctions.GetTaxRate(UserName, Variables.Tax);
            Variables.TaxAmount = (Variables.Amount * Variables.TaxRate) / 100;
            Variables.NetAmount = Variables.Amount + Variables.TaxAmount;


        }
        private void Variables2Row()
        {
            Row1["ID"] = Variables.ID1;
            Row2["ID"] = Variables.ID2;

            Row1["Vou_No"] = Variables.Vou_No;
            Row1["Vou_Date"] = Variables.Vou_Date;
            Row1["Company"] = Variables.Company;
            Row1["Employee"] = Variables.Employee;
            Row1["Ref_No"] = Variables.Ref_No;
            Row1["Inv_No"] = Variables.Inv_No;
            Row1["Inv_Date"] = Variables.Inv_Date;
            Row1["Pay_Date"] = Variables.Pay_Date;
            Row1["Description"] = Variables.Remarks;
            Row1["Comments"] = Variables.Comments;
            Row1["Status"] = Variables.Status;

            Row2["TranID"] = Variables.TranID;
            Row2["Sr_No"] = Variables.Sr_No;
            Row2["Inventory"] = Variables.Inventory;
            Row2["Batch"] = Variables.Batch;
            Row2["Qty"] = Variables.Qty;
            Row2["Rate"] = Variables.Rate;
            Row2["Tax"] = Variables.Tax;
            Row2["Description"] = Variables.Description;
            Row2["Project"] = Variables.Project;

        }

        #endregion

        #region POST

        public IActionResult OnPostAdd()
        {
            TempTableClass TempInvoice1 = new TempTableClass(UserName, Tables.BillReceivable, Variables.Vou_No);
            if (TempInvoice1.CountTemp > 0) { Variables.TranID = (int)TempInvoice1.TempView[0]["ID"]; }
            TempTableClass TempInvoice2 = new TempTableClass(UserName, Tables.BillReceivable2, Variables.TranID);

            Row1 = TempInvoice1.NewRecord();
            Row2 = TempInvoice2.NewRecord();
            Variables2Row();

            var ValidRow1 = TempInvoice1.TableValidate.Validation(Row1);
            var ValidRow2 = TempInvoice2.TableValidate.Validation(Row2);

            if (ValidRow1 && ValidRow2)
            {
                TempInvoice1.Save(false);
                TempInvoice2.CurrentRow["TranID"] = TempInvoice1.CurrentRow["ID"];
                TempInvoice2.Save(false);
            }
            else
            {
                ErrorMessages.AddRange(TempInvoice1.ErrorMessages);
                ErrorMessages.AddRange(TempInvoice2.ErrorMessages);
            }

            // Reset and Update Web Page Data

            if (ErrorMessages.Count == 0)
            {
                //TempInvoice1 = new TempTableClass(UserName, Tables.BillReceivable, Variables.Vou_No);
                //if (TempInvoice1.CountTemp > 0) { Variables.TranID = (int)TempInvoice1.TempView[0]["ID"]; }
                //TempInvoice2 = new TempTableClass(UserName, Tables.BillReceivable2, Variables.TranID);
                Invoice =    TempInvoice2.TempTable;
                var message2 = $"Serial No {Variables.Sr_No} of Invoice No {Variables.Vou_No} has been saved successfully.";
                ErrorMessages.Add(MessageClass.SetMessage(message2, Color.Yellow));
            }
            return Page();
        }
        public IActionResult OnPostEdit(int Sr_No)
        {
            var Vou_No = Variables.Vou_No;
            return RedirectToPage("SaleInvoice", routeValues: new { Vou_No, Sr_No });
        }
        public IActionResult OnPostDelete(int ID)
        {
            var Vou_No = Variables.Vou_No;
            var Sr_No = 1;
            return RedirectToPage("SaleInvoice", routeValues: new { Vou_No, Sr_No });
        }

        public IActionResult OnPostNew()
        {
            var Vou_No = Variables.Vou_No;
            var Sr_No = -1;
            return RedirectToPage("SaleInvoice", routeValues: new { Vou_No, Sr_No });
        }
        public IActionResult OnPostSave()
        {
            // Load Data Table receivable 1 & 2 in  tabe class.
            // Validate Data both Tables
            // Save bill invocie from Temp to Source
            // Delete sales invoice in temp data
            // show messages
            // end


            #region Load Data 
            TempTableClass TempInvoice1 = new TempTableClass(UserName, Tables.BillReceivable, Variables.Vou_No);
            if (TempInvoice1.CountTemp > 0) { Variables.TranID = (int)TempInvoice1.TempView[0]["ID"]; }
            TempTableClass TempInvoice2 = new TempTableClass(UserName, Tables.BillReceivable2, Variables.TranID);
            var TargetVouNo = Variables.Vou_No;

            #endregion

            #region Validation
            // Validate Data
            bool Valid1 = false;
            bool Valid2 = true;
            if (TempInvoice1.CountTemp == 1)
            {
                TempInvoice1.CurrentRow = TempInvoice1.TempTable.Rows[0];
                Valid1 = TempInvoice1.TableValidate.Validation(TempInvoice1.CurrentRow);
            }
            else
            {
                ErrorMessages.Add(MessageClass.SetMessage("No record found..1.." + Variables.Vou_No));
            }



            if (Valid1)                 // If Table 1 valid is true;
            {
                if (TempInvoice2.CountTemp > 0)
                {
                    foreach (DataRow Row in TempInvoice2.TempTable.Rows)
                    {
                        var Valid = TempInvoice2.TableValidate.Validation(Row);
                        if (!Valid2) { Valid2 = false; } else { Valid2 = Valid; }
                        ErrorMessages.AddRange(TempInvoice2.ErrorMessages);
                    }
                }
                else
                {
                    ErrorMessages.Add(MessageClass.SetMessage("No record found..2.." + Variables.Vou_No));
                }
                if (ErrorMessages.Count > 0) { return Page(); }

                #endregion

                if (Valid2)
                {
                    var TargetRow = TempInvoice1.CurrentRow;
                    DataTableClass SourceTable1 = new(UserName, Tables.BillReceivable);
                    SourceTable1.NewRecord();
                    SourceTable1.CurrentRow.ItemArray = TargetRow.ItemArray;

                    if (Variables.Vou_No.ToUpper() == "NEW")
                    {

                        Variables.Vou_No = GetNewVouNo(Variables.Vou_Date, "SL");
                        SourceTable1.CurrentRow["Vou_No"] = Variables.Vou_No;
                        SourceTable1.CurrentRow["ID"] = 0;

                    }

                    #region Save

                    SourceTable1.Save();
                    Variables.TranID = (int)SourceTable1.CurrentRow["ID"];
                    ErrorMessages.AddRange(SourceTable1.TableValidation.MyMessages);
                    if (ErrorMessages.Count > 0)
                    {
                        return Page();
                    }
                    Variables.TranID = (int)SourceTable1.CurrentRow["ID"];


                    DataTableClass SourceTable2 = new(UserName, Tables.BillReceivable2);
                    SourceTable2.MyDataView.RowFilter = $"TranID={Variables.TranID}";
                    var _Table = SourceTable2.MyDataView.ToTable();
                    if (_Table.Rows.Count > 0)                   // Delete if Invoice exist in source table.
                    {
                        foreach (DataRow Row in _Table.Rows)
                        {
                            SourceTable2.CurrentRow = Row;
                            SourceTable2.Delete();
                        }
                    }

                    foreach (DataRow Row in TempInvoice2.TempTable.Rows)
                    {
                        SourceTable2.NewRecord();
                        SourceTable2.CurrentRow.ItemArray = Row.ItemArray;

                        if (TargetVouNo.ToUpper() == "NEW")
                        {
                            SourceTable2.CurrentRow["TranID"] = SourceTable1.CurrentRow["ID"];
                            SourceTable2.CurrentRow["ID"] = 0;
                        }
                        
                        SourceTable2.Save();
                        ErrorMessages.AddRange(SourceTable2.TableValidation.MyMessages);
                        if (ErrorMessages.Count > 0)
                        {
                            return Page();
                        }
                    }

                    #endregion

                    #region Delete
                    // Delete Voucher from Temp Data
                    TempInvoice1.Delete();
                    foreach (DataRow Row in TempInvoice2.TempTable.Rows)
                    {
                        TempInvoice2.CurrentRow = Row;
                        TempInvoice2.Delete();
                    }

                    #endregion

                    //var message = $"Save Serial No {Variables.Sr_No} of Sale invoice {Variables.Vou_No} SAVED successfully., ";
                    var message = $"Sale invoice {Variables.Vou_No} SAVED successfully., ";
                    ErrorMessages.Add(MessageClass.SetMessage(message, Color.Green));
                }
            }
            IsSaved = true;

            var Vou_No = Variables.Vou_No;
            var Sr_No = 1;
            return RedirectToPage("SaleInvoice", routeValues: new { Vou_No, Sr_No});


        }

        #region New Voucher Number
        private string GetNewVouNo(DateTime _Date, string _VouTag)
        {
            string NewNum;
            StringBuilder _Text = new StringBuilder();
            _Text.Append("SELECT [Vou_No], ");
            _Text.Append("substr([Vou_No],1,2) AS Tag, ");
            _Text.Append("SubStr([Vou_No],3,2) AS Year, ");
            _Text.Append("SubStr([Vou_No],5,2) AS Month, ");
            _Text.Append("Cast((SubStr([Vou_No],8,4)) as integer) AS num ");
            _Text.Append("FROM [BillReceivable]");

            DataTable _Table = DataTableClass.GetTable(UserName, _Text.ToString(), "");
            DataView _View = _Table.AsDataView();

            var _Year = _Date.ToString("yy");
            var _Month = _Date.ToString("MM");
            _View.RowFilter = string.Concat("Vou_No like '", _VouTag, _Year, _Month, "%'");
            DataTable _VouList = _View.ToTable();
            var MaxNum = _VouList.Compute("Max(num)", "");
            if (MaxNum == DBNull.Value)
            {
                NewNum = string.Concat(_VouTag, _Year, _Month, "-", "0001");
            }
            else
            {
                var _MaxNum = int.Parse(MaxNum.ToString()) + 1;
                NewNum = string.Concat(_VouTag, _Year, _Month, "-", _MaxNum.ToString("0000"));
            }
            return NewNum;
        }


        #endregion

        public IActionResult OnPostPrint()
        {
            var TranID = Variables.TranID;
            return RedirectToPage("../ReportPrint/PrintReport", pageHandler: "SaleInvoice", routeValues: new { TranID });
        }
        public IActionResult OnPostBack()
        {
            return RedirectToPage("../Accounts/BillReceivableList");
        }
        #endregion

        #region Variables

        public class MyParameters
        {
            public int ID1 { get; set; }
            public int ID2 { get; set; }
            public string Vou_No { get; set; }
            public string Ref_No { get; set; }
            public string Inv_No { get; set; }
            public int TranID { get; set; }
            public int Sr_No { get; set; }
            public int Inventory { get; set; }
            public int Company { get; set; }
            public int Employee { get; set; }
            public int Project { get; set; }
            public int Tax { get; set; }

            public DateTime Vou_Date { get; set; }
            public DateTime Inv_Date { get; set; }
            public DateTime Pay_Date { get; set; }

            public string Remarks { get; set; }
            public string Comments { get; set; }
            public string Description { get; set; }
            public string Batch { get; set; }
            public string Status { get; set; }

            public decimal Qty { get; set; }
            public decimal Rate { get; set; }
            public decimal Amount { get; set; }
            public int TaxRate { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal NetAmount { get; set; }

        }
        #endregion
    }
}
