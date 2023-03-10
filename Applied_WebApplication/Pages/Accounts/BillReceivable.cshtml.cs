using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Configuration;
using System.Data;
//using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Applied_WebApplication.Pages.Accounts
{
    public class BillReceivableModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; } = new();
        public DataTable BillReceivable2 { get; set; }
        public DataTableClass TempBillReceivable { get; set; }
        public DataRowCollection RowList { get; set; }
        public List<Message> ErrorMessages { get; set; } = new();
        public string UserName => User.Identity.Name;

        #region GET & POST

        public void OnGet(int? TranID, int? ID2)
        {
            TranID ??= 0;

            if (TranID == 0)
            {
                Variables = new()
                {
                    Vou_No = AppFunctions.GetBillReceivableVoucher(UserName),
                    Vou_Date = DateTime.Now,
                    Pay_Date = DateTime.Now,
                    Inv_Date = DateTime.Now,
                    SR_No = 1
                };

                // if BP is not in source date but exist in temp data. 
                TempBillReceivable = new(UserName, Tables.view_BillReceivable, false, TranID);

                if (TempBillReceivable.Count > 0)
                {
                    Variables = GetVariables(TempBillReceivable.CurrentRow);
                    RowList = TempBillReceivable.Rows;
                }

            }
            else
            {

                #region Bill Receivable Data Setup

                // Delete All Temp Data table record
                TempBillReceivable = new(UserName, Tables.view_BillReceivable, true, TranID);
                foreach (DataRow Row in TempBillReceivable.Rows)
                {
                    if ((int)Row["TranID"] == TranID)
                    {
                        TempBillReceivable.CurrentRow = Row;
                        TempBillReceivable.Delete();
                    }
                }

                // Fill Temp Data from source data table
                DataTableClass BillReceivable = new(UserName, Tables.view_BillReceivable);
                BillReceivable.MyDataView.RowFilter = string.Concat("TranID=", TranID);
                foreach (DataRow Row in BillReceivable.MyDataView.ToTable().Rows)
                {
                    Variables = GetVariables(Row);                  // Save Row Values into Variables
                    GetRow(Variables);                                     // Transfer Variables into Current Row.
                    TempBillReceivable.Add();                         // Save (Add) row into Temp Table.
                   if (TempBillReceivable.TableValidation.Count > 0) { ErrorMessages.AddRange(TempBillReceivable.TableValidation.MyMessages); }
                }

                //-----------------------------------------------------

                #endregion

                TempBillReceivable.MyDataView.RowFilter = "TranID=" + TranID.ToString();
                if (TempBillReceivable.Count == 0)
                {
                    DataRow _Row = TempBillReceivable.NewRecord();
                }
                else
                {
                    Variables = GetVariables(TempBillReceivable.CurrentRow);
                }
                RowList = TempBillReceivable.Rows;
            }
        }
        public IActionResult OnPostAdd()
        {
            // Generate a record from any saved record.
            TempBillReceivable = new(UserName, Tables.view_BillReceivable, false, 0);

            TempBillReceivable.MyDataView.RowFilter = string.Concat("Vou_No='", Variables.Vou_No, "'");
            if (TempBillReceivable.Count > 0)
            {
                TempBillReceivable.SeekRecord((int)TempBillReceivable.MyDataView[0]["ID"]);
                Variables = GetVariablesAdd(Variables.ID);
                TempBillReceivable.Add();                                           // Add a new record in temp Table.
                //TempBillReceivable = new(UserName, Tables.view_BillReceivable, false, 0);
                TempBillReceivable.SeekRecord((int)TempBillReceivable.CurrentRow["ID"]);
                Variables = GetVariables(TempBillReceivable.CurrentRow);
                RowList = TempBillReceivable.MyDataTable.Rows;

            }
            else
            {
                ErrorMessages.Add(MessageClass.SetMessage("Table has some error. Contact to Administrator."));
                return Page();

            }

            if (TempBillReceivable.TableValidation.Count > 0)
            {
                ErrorMessages.AddRange(TempBillReceivable.TableValidation.MyMessages);
            }
            else
            {
                ErrorMessages.Add(MessageClass.SetMessage(TempBillReceivable.MyMessage, Color.Green));
            }



            return RedirectToPage(pageName: "BillReceivable", pageHandler: "Edit", routeValues: new { ID = TempBillReceivable.CurrentRow["ID"] });

            //return Page();
        }
        public IActionResult OnPostBack()
        {
            return RedirectToPage("./BillReceivableList");
        }

        #endregion

        #region SAVE Entry and Voucher

        public IActionResult OnPostSave()
        {
            int TranID = 0;
            DataTableClass BillReceivable = new(UserName, Tables.view_BillReceivable);
            TempBillReceivable = new(UserName, Tables.view_BillReceivable, true, TranID);

            //TempBillReceivable.NewRecord();
            TempBillReceivable.CurrentRow = GetRow(Variables);
            if (int.Parse(TempBillReceivable.CurrentRow["ID"].ToString()) == 0)
            {
                int MaxID = AppFunctions.GetMax(UserName, Tables.BillReceivable, "ID", "");
                TempBillReceivable.CurrentRow["ID"] = MaxID + 1;
                TempBillReceivable.CurrentRow["TranID"] = MaxID + 1;
            }

            //TempBillReceivable
            TempBillReceivable.Save();                                                                                                      // Insert Record in Table
            if (TempBillReceivable.TableValidation.Count > 0)
            { ErrorMessages.AddRange(TempBillReceivable.TableValidation.MyMessages); }
            else
            { ErrorMessages.Add(MessageClass.SetMessage("Record has been saved in Temp data file. ", Color.Green)); }

            RowList = TempBillReceivable.Rows;

            return Page();

        }
        public IActionResult OnPostSaveFinal()
        {
            TempBillReceivable = new(UserName, Tables.view_BillReceivable, true, Variables.TranID);
            TempBillReceivable.MyDataView.Sort = "SR_NO";
            int _SRNo = 1; bool IsFirstRowSaved = false;
            //DataRow FirstRow;            

            //============================================== Validation of Bill Receivable Records.

            foreach (DataRow Row in TempBillReceivable.Rows)
            {
                bool IsValid = TempBillReceivable.TableValidation.Validation(Row);
                if (!IsValid)
                {
                    ErrorMessages.AddRange(TempBillReceivable.TableValidation.MyMessages);
                    return Page();
                }
                //if (!IsFirstRowSaved) { FirstRow = Row; IsFirstRowSaved = true; }
            }
            //============================================================================


            //==================================================== SAVE RECORDS IN Source DataBase
            DataTableClass _Table1 = new(UserName, Tables.BillReceivable);
            DataTableClass _Table2 = new(UserName, Tables.BillReceivable2);

            //IsFirstRowSaved = false; CommandAction _Action1, _Action2;

            foreach (DataRow Row in TempBillReceivable.Rows)
            {
                #region First Record of Bill Receivable
                if (!IsFirstRowSaved)
                {
                    _Table1.SeekRecord((int)Row["ID1"]);
                    _Table1.CurrentRow["ID"] = Row["ID1"];
                    _Table1.CurrentRow["Vou_No"] = Row["Vou_No"];
                    _Table1.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                    _Table1.CurrentRow["Company"] = Row["Company"];
                    _Table1.CurrentRow["Employee"] = Row["Employee"];
                    _Table1.CurrentRow["Ref_No"] = Row["Ref_No"];
                    _Table1.CurrentRow["Inv_No"] = Row["Inv_No"];
                    _Table1.CurrentRow["Inv_Date"] = Row["Inv_Date"];
                    _Table1.CurrentRow["Inv_No"] = Row["Inv_No"];
                    _Table1.CurrentRow["Inv_Date"] = Row["Inv_Date"];
                    _Table1.CurrentRow["Pay_Date"] = Row["Pay_Date"];
                    _Table1.CurrentRow["Amount"] = Row["Amount"];
                    _Table1.CurrentRow["Description"] = Row["Description"];
                    _Table1.CurrentRow["Comments"] = Row["Comments"];
                    _Table1.CurrentRow["Status"] = VoucherStatus.Submitted.ToString();

                    //if (_Table1.Seek((int)Row["ID"])) { _Action1 = CommandAction.Update; } else { _Action1 = CommandAction.Insert; }
                    _Table1.Save();
                    if (_Table1.TableValidation.Count > 0)
                    {
                        ErrorMessages.AddRange(_Table1.TableValidation.MyMessages);
                        return Page();
                    }
                    IsFirstRowSaved = true;
                }
                #endregion

                _Table2.SeekRecord((int)Row["ID2"]);
                _Table2.CurrentRow["ID"] = Row["ID2"];
                _Table2.CurrentRow["TranID"] = Row["TranID"];
                _Table2.CurrentRow["Sr_No"] = _SRNo; _SRNo++;
                _Table2.CurrentRow["Inventory"] = Row["Inventory"];
                _Table2.CurrentRow["Batch"] = Row["Batch"];
                _Table2.CurrentRow["Qty"] = Row["Qty"];
                _Table2.CurrentRow["Rate"] = Row["Rate"];
                _Table2.CurrentRow["Tax"] = Row["Tax"];
                _Table2.CurrentRow["Tax_Rate"] = Row["Tax_Rate"];
                _Table2.CurrentRow["Description"] = Row["Description2"];
                _Table2.CurrentRow["Project"] = Row["Project"];

                _Table2.Save();
                if (_Table1.TableValidation.Count > 0)
                {
                    ErrorMessages.AddRange(_Table2.TableValidation.MyMessages);
                    return Page();
                }

            }

            foreach (DataRow Row in TempBillReceivable.Rows)
            {
                if ((int)Row["TranID"] == Variables.TranID)
                {
                    TempBillReceivable.Delete();
                }
            }

            ErrorMessages.Add(MessageClass.SetMessage("Bill Receivable is saved sucessfully.....!!!!",Color.Green));
            return RedirectToPage("BillReceivable");
        }

        #endregion

        #region EDIT GET & POST


        public IActionResult OnPostEdit(int ID)
        {

            return RedirectToPage("BillReceivable", pageHandler: "Edit", routeValues: new { ID });
        }

        public IActionResult OnGetEdit(int ID)
        {
            TempBillReceivable = new(UserName, Tables.view_BillReceivable, false, 0);
            TempBillReceivable.SeekRecord(ID);
            Variables = GetVariables(TempBillReceivable.CurrentRow);
            return Page();
        }

        #endregion

        #region SET & GET Variables and DataRow Get
        private MyParameters GetVariablesAdd(int id)
        {
            MyParameters _Variables;
            if (TempBillReceivable.Count > 0)
            {

                TempBillReceivable.CurrentRow["SR_No"] = MaxSrNo(TempBillReceivable.MyDataTable);
                TempBillReceivable.CurrentRow["Qty"] = 0.00M;
                TempBillReceivable.CurrentRow["Rate"] = 0.00M;
                TempBillReceivable.CurrentRow["Inventory"] = 0;
                TempBillReceivable.CurrentRow["Batch"] = "";
                TempBillReceivable.CurrentRow["Tax"] = 0;
                TempBillReceivable.CurrentRow["Tax_Rate"] = 0.00M;
                TempBillReceivable.CurrentRow["Description2"] = "";
                TempBillReceivable.CurrentRow["Project"] = 0;
                TempBillReceivable.CurrentRow["Status"] = VoucherStatus.Add.ToString();


                _Variables = GetVariables(TempBillReceivable.CurrentRow);
            }
            else
            {
                _Variables = GetVariables(TempBillReceivable.NewRecord());
            }

            return _Variables;
        }
        private int MaxSrNo(DataTable _Table)
        {
            if (_Table.Rows.Count == 0) { return 1; }                                                  // Table is empty, return 1
            if (_Table.Rows.Count > 0) { return (int)_Table.Compute("MAX(Sr_No)", "") + 1; }               // Table is not empty and id =0 return max value
            return 0;                                                                                               // Id is already assign, return same value.
        }
        private DataRow GetRow(MyParameters _Variables)
        {
            DataRow Row = TempBillReceivable.NewRecord();
            Row["ID"] = _Variables.ID;
            Row["ID1"] = _Variables.ID1;
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
        private static MyParameters GetVariables(DataRow Row)
        {

            MyParameters _Variables = new();
            if (Row == null) { return _Variables; }                    // return if row is null
            _Variables = new()
            {
                ID = (int)Row["ID"],
                ID1 = (int)Row["ID1"],
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


            return _Variables;
        }

        #endregion

        #region Delete

        public IActionResult OnPostDelete(int ID)
        {
            TempBillReceivable = new(UserName, Tables.view_BillReceivable, false, 0);
            if (TempBillReceivable.Seek(ID))
            {
                TempBillReceivable.SeekRecord(ID);
                TempBillReceivable.Delete();
                ErrorMessages.Add(MessageClass.SetMessage(TempBillReceivable.MyMessage));
                TempBillReceivable = new(UserName, Tables.view_BillReceivable, false, 0);  // Refresh Data Table after delete record
                Variables = GetVariables(TempBillReceivable.CurrentRow);                // Refresh variables for page.
                return RedirectToPage();
            }

            return Page();


        }


        #endregion


        #region Paramters

        public class MyParameters
        {
            public int ID { get; set; }
            public int ID1 { get; set; }
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

