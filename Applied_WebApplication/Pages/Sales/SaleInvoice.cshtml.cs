using Applied_WebApplication.Data;
using Applied_WebApplication.Pages.ReportPrint;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using NPOI.SS.Util;
using System.Data;
using System.Drawing;

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
        #endregion

        #region GET

        public void OnGet(string? Vou_No, int? Sr_No)
        {
            Vou_No ??= "New";
            Sr_No ??= 1;


            Variables = new()
            {
                Vou_No = Vou_No,
                Sr_No = (int)Sr_No,
                TranID = 0
            };
            TempTableClass TempInvoice1 = new TempTableClass(UserName, Tables.BillReceivable, Variables.Vou_No);
            if (TempInvoice1.CountTemp > 0) { Variables.TranID = (int)TempInvoice1.TempVoucher.Rows[0]["ID"]; }
            TempTableClass TempInvoice2 = new TempTableClass(UserName, Tables.BillReceivable2, Variables.TranID);
            Invoice = TempInvoice2.TempVoucher;

            if (TempInvoice1.TempVoucher.Rows.Count == 0)
            {
                TempInvoice1.CurrentRow = TempInvoice1.NewRecord();
                TempInvoice1.CurrentRow["ID"] = 0;
                TempInvoice1.CurrentRow["Vou_No"] = "New";
                TempInvoice1.CurrentRow["Status"] = VoucherStatus.Submitted.ToString();
            }
            else
            {
                TempInvoice1.CurrentRow = TempInvoice1.TempVoucher.Rows[0];
            }

            // Table No 2..............
            if (Sr_No == -1)
            {
                Sr_No = (int)TempInvoice2.TempVoucher.Compute("Max(Sr_No)", $"TranID={Variables.TranID}");
                TempInvoice2.CurrentRow = TempInvoice2.NewRecord();
                TempInvoice2.CurrentRow["Sr_No"] = Sr_No + 1;
            }
            else
            {
                if (TempInvoice2.TempVoucher.Rows.Count == 0)
                {
                    TempInvoice2.CurrentRow = TempInvoice2.NewRecord();
                    TempInvoice2.CurrentRow["ID"] = 0;
                    TempInvoice2.CurrentRow["TranID"] = TempInvoice1.CurrentRow["ID"];
                    TempInvoice2.CurrentRow["Sr_No"] = 1;
                }
                else
                {
                    TempInvoice2.MyDataView.RowFilter = $"Sr_No={Sr_No}";
                    if (TempInvoice2.MyDataView.Count == 1)
                    { TempInvoice2.CurrentRow = TempInvoice2.MyDataView[0].Row; }
                    else
                    {
                        TempInvoice2.CurrentRow = TempInvoice2.TempVoucher.Rows[0];
                    }
                }

            }

            Row1 = TempInvoice1.CurrentRow;
            Row2 = TempInvoice2.CurrentRow;
            Invoice = TempInvoice2.TempVoucher;

            Row2Variables();
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


                Amount = (Variables.Qty * Variables.Rate),
                TaxRate = (int)AppFunctions.GetTaxRate(UserName, Variables.Tax),
                TaxAmount = Variables.Amount * Variables.TaxRate,
                NetAmount = Variables.Amount + Variables.TaxAmount
            };
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
            if (TempInvoice1.CountTemp > 0) { Variables.TranID = (int)TempInvoice1.MyDataView[0]["ID"]; }
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
                ErrorMessages.Add(MessageClass.SetMessage("Sale Invoice saved sucessfully !!!", Color.Green));
               
            }
            else
            {
                ErrorMessages.AddRange(TempInvoice1.ErrorMessages);
                ErrorMessages.AddRange(TempInvoice2.ErrorMessages);
              
            }


            TempInvoice2 = new TempTableClass(UserName, Tables.BillReceivable2, Variables.TranID);
            Invoice = TempInvoice2.TempVoucher;
            return Page();

           
        }

        public IActionResult OnPostNew()
        {
            if (Variables == null) { return Page(); }                    // Not Execute if variable not define properly.

            var VouNumber = Variables.Vou_No;
            TempTableClass TempInvoice1 = new TempTableClass(UserName, Tables.BillReceivable, Variables.Vou_No);
            if (TempInvoice1.CountTemp > 0) { Variables.TranID = (int)TempInvoice1.MyDataView[0]["ID"]; }
            TempTableClass TempInvoice2 = new TempTableClass(UserName, Tables.BillReceivable2, Variables.TranID);
            Invoice = TempInvoice2.TempVoucher;

            if (TempInvoice1.CountTemp == 1)
            {
                var Max_SrNo = TempInvoice2.TempVoucher.Compute("Max(Sr_No)", $"TranID={Variables.TranID}");

                TempInvoice1.CurrentRow = TempInvoice1.TempVoucher.Rows[0];
                TempInvoice2.NewRecord();
                TempInvoice2.CurrentRow["TranID"] = Variables.TranID;
                TempInvoice2.CurrentRow["Sr_No"] = Conversion.ToInteger(Max_SrNo) + 1;
                Row1 = TempInvoice1.CurrentRow;
                Row2 = TempInvoice2.CurrentRow;
                Row2Variables();

                var Vou_No = Variables.Vou_No;
                var Sr_No = -1;

                return RedirectToPage("SaleInvoice", routeValues: new {Vou_No, Sr_No});
            }

            return RedirectToPage();
        }
        public IActionResult OnPostSave()
        {
            return RedirectToPage();
        }
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
