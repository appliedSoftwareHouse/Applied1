using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Drawing;
using System.Text;

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
        public bool IsLoad { get; set; } = false;
        public string Vou_No { get; set; }
        public int TranID { get; set; }

        #endregion

        #region GET

        public void OnGet(string? Vou_No, int? Sr_No, bool? _IsLoad)
        {
            Vou_No ??= "SL2305-0001";
            _IsLoad ??= true;

            //var TranID = 0;
            //var IsNew = false;

            //TempTableClass TempInvoice1;
            //TempTableClass TempInvoice2;

            TempDBClass TempInvoice11;
            TempDBClass TempInvoice22;

            IsLoad = (bool)_IsLoad;
            //Sr_No ??= 1;

            if (IsLoad)                         // Setup a Voucher in as Temp Data Base; (Reset)
            {
                var _Filter1 = $"Vou_No='{Vou_No}'";
                TempInvoice11  = new(UserName, Tables.BillReceivable, _Filter1, IsLoad);
                var _Filter2 = $"TranID={TempInvoice11.CurrentRow["ID"]}";
                TempInvoice22 = new(UserName, Tables.BillReceivable2, _Filter2, IsLoad);
            }


            //if (Vou_No.Length == 0) { Vou_No = "NEW"; }
            
            //Variables = new();
            
            //if (Vou_No.ToUpper() == "NEW")
            //{
            //    IsNew = true;

            //    TempInvoice1 = new (UserName, Tables.BillReceivable, "NEW", true);
            //    if (TempInvoice1.CountTemp > 0)
            //    {
            //        TranID = (int)TempInvoice1.CurrentRow["ID"];
            //        Row1 = TempInvoice1.CurrentRow;
            //    }
            //    else
            //    {
            //        Row1 = TempInvoice1.NewRecord();
            //        Row1["Vou_No"] = "NEW";
            //        Row1["Status"] = VoucherStatus.Submitted.ToString(); ;
            //        TranID = 1;
            //        Sr_No = -1;
            //    }

            //    TempInvoice2 = new (UserName, Tables.BillReceivable2, TranID, IsNew);
            //    if (Sr_No == -1)
            //    {
            //        Row2 = TempInvoice2.NewRecord();
            //        Row2["Sr_No"] = MaxSrNo(TempInvoice2.TempTable);
            //        Row2["TranID"] = Row1["ID"];
            //    }
            //    else
            //    {
            //        if (TempInvoice2.CountTemp > 0)
            //        {
            //            TempInvoice2.TempView.RowFilter = $"Sr_No={Sr_No}";
            //            if (TempInvoice2.TempView.Count > 0)
            //            {
            //                Row2 = TempInvoice2.TempView[0].Row;
            //            }
            //            else
            //            {
            //                Row2 = TempInvoice2.CurrentRow;                 // Error id not found the record, pass empty row
            //            }

            //        }
            //    }

            //    Invoice = TempInvoice2.TempTable;
            //    Row2Variables();
            //    return;
            //}

            //if (Vou_No.Length == 11)
            //{

            //    TempInvoice1 = new TempTableClass(UserName, Tables.BillReceivable, Vou_No, true);
            //    if (TempInvoice1.CountTemp > 0)
            //    {
            //        TranID = (int)TempInvoice1.CurrentRow["ID"];
            //        Row1 = TempInvoice1.CurrentRow;
            //    }
            //    else
            //    {
            //        ErrorMessages.Add(MessageClass.SetMessage("Error.... Voucher Not Found"));
            //        return;
            //    }

            //    TempInvoice2 = new TempTableClass(UserName, Tables.BillReceivable2, TranID, IsNew);
            //    if (Sr_No == -1)
            //    {
            //        Row2 = TempInvoice2.NewRecord();
            //        Row2["Sr_No"] = MaxSrNo(TempInvoice2.TempTable);
            //        Row2["TranID"] = Row1["ID"];
            //    }
            //    else
            //    {
            //        if (TempInvoice2.CountTemp > 0)
            //        {
            //            TempInvoice2.TempView.RowFilter = $"Sr_No={Sr_No}";
            //            if (TempInvoice2.TempView.Count > 0)
            //            {
            //                Row2 = TempInvoice2.TempView[0].Row;
            //            }
            //            else
            //            {
            //                Row2 = TempInvoice2.CurrentRow;
            //            }

            //        }
            //        else
            //        {
            //            ErrorMessages.Add(MessageClass.SetMessage("Error.... Voucher Not Found"));
            //            return;
            //        }
            //    }
            //    Invoice = TempInvoice2.TempTable;
            //    Row2Variables();
            //    return;

            //}

            //#region Temp


            ////#region Null Values
            ////if (Vou_No == null && Sr_No == null)
            ////{
            ////    Variables.Vou_No = "New";
            ////    Variables.Sr_No = 1;

            ////    TempInvoice1 = new TempTableClass(UserName, Tables.BillReceivable, Variables.Vou_No);
            ////    TempInvoice2 = new TempTableClass(UserName, Tables.BillReceivable2, -1);

            ////    if (TempInvoice1.CountTemp == 0)
            ////    {
            ////        TempInvoice1.CurrentRow = TempInvoice1.NewRecord();
            ////        TempInvoice1.CurrentRow["ID"] = 0;
            ////        TempInvoice1.CurrentRow["Vou_No"] = "New";
            ////        TempInvoice1.CurrentRow["Status"] = VoucherStatus.Submitted.ToString();
            ////    }
            ////    else
            ////    {
            ////        TempInvoice1.CurrentRow = TempInvoice1.TempTable.Rows[0];
            ////    }

            ////    if (TempInvoice2.CountTemp == 0)
            ////    {
            ////        TempInvoice2.CurrentRow = TempInvoice2.NewRecord();
            ////        TempInvoice2.CurrentRow["ID"] = 0;
            ////        TempInvoice2.CurrentRow["Sr_No"] = 1;
            ////    }
            ////    else
            ////    {
            ////        TempInvoice2.CurrentRow = TempInvoice2.TempTable.Rows[0];
            ////    }

            ////    Row1 = TempInvoice1.CurrentRow;
            ////    Row2 = TempInvoice2.CurrentRow;
            ////    Row2Variables();
            ////    Invoice = TempInvoice2.TempTable;
            ////    return;
            ////}
            ////#endregion

            ////#region Sales Invoices exist

            ////if (Vou_No != null && Sr_No != null)
            ////{
            ////    if (Vou_No.ToUpper() == "NEW")
            ////    {
            ////        Sr_No ??= 1;

            ////        TempInvoice1 = new TempTableClass(UserName, Tables.BillReceivable, Vou_No);
            ////        TempInvoice2 = new TempTableClass(UserName, Tables.BillReceivable2, -1);

            ////        if (TempInvoice1.CountTemp == 0)
            ////        {
            ////            TempInvoice1.CurrentRow = TempInvoice1.NewRecord();
            ////            TempInvoice1.CurrentRow["ID"] = 0;
            ////            TempInvoice1.CurrentRow["Vou_No"] = "New";
            ////            TempInvoice1.CurrentRow["Status"] = VoucherStatus.Submitted.ToString();
            ////        }
            ////        else
            ////        {
            ////            TempInvoice1.CurrentRow = TempInvoice1.TempTable.Rows[0];
            ////        }

            ////        if (TempInvoice2.CountTemp == 0)
            ////        {
            ////            TempInvoice2.CurrentRow = TempInvoice2.NewRecord();
            ////            TempInvoice2.CurrentRow["ID"] = 0;
            ////            TempInvoice2.CurrentRow["Sr_No"] = 1;
            ////        }
            ////        else
            ////        {
            ////            if (Sr_No == -1)
            ////            {
            ////                TempInvoice2.NewRecord();
            ////                TempInvoice2.CurrentRow["ID"] = 0;
            ////                TempInvoice2.CurrentRow["Sr_No"] = MaxSrNo(TempInvoice2.TempTable);
            ////            }
            ////            else
            ////            {
            ////                TempInvoice2.CurrentRow = TempInvoice2.TempTable.Rows[0];
            ////            }
            ////        }

            ////        Row1 = TempInvoice1.CurrentRow;
            ////        Row2 = TempInvoice2.CurrentRow;
            ////        Row2Variables();
            ////        Invoice = TempInvoice2.TempTable;

            ////        return;
            ////    }
            ////    else
            ////    {
            ////        Variables.Vou_No = Vou_No;
            ////        TempInvoice1 = new TempTableClass(UserName, Tables.BillReceivable, Variables.Vou_No);

            ////        if (TempInvoice1.CountTemp > 0)
            ////        {
            ////            TempInvoice1.CurrentRow = TempInvoice1.SourceTable.Rows[0];
            ////            TranID = (int)TempInvoice1.CurrentRow["ID"];
            ////        }

            ////        TempInvoice2 = new TempTableClass(UserName, Tables.BillReceivable2, TranID);

            ////        if (TempInvoice2.CountTemp > 0)
            ////        {
            ////            Row1 = TempInvoice1.CurrentRow;
            ////            if (Sr_No > 0)
            ////            {
            ////                TempInvoice2.TempView = TempInvoice2.TempTable.AsDataView();
            ////                if (TempInvoice1.CountTemp == 0)
            ////                {
            ////                    TempInvoice2.TempView.RowFilter = $"Sr_No={Sr_No}";
            ////                    if (TempInvoice2.TempView.Count > 0)
            ////                    {
            ////                        TempInvoice2.CurrentRow = TempInvoice2.TempView[0].Row;
            ////                        Row2 = TempInvoice2.CurrentRow;
            ////                        Invoice = TempInvoice2.TempTable;
            ////                    }
            ////                }
            ////            }
            ////            if (Sr_No == -1)
            ////            {
            ////                TempInvoice2.CurrentRow = TempInvoice2.NewRecord();
            ////                var Max_SrNo = MaxSrNo(TempInvoice2.TempTable);
            ////            }

            ////            Row1 = TempInvoice1.CurrentRow;
            ////            Row2 = TempInvoice2.CurrentRow;
            ////            Invoice = TempInvoice2.TempTable;

            ////            Row2Variables();
            ////            return;
            ////        }
            ////    }
            ////}
            ////#endregion

            //#endregion
        }
        #endregion

        public int MaxSrNo(DataTable _Table)
        {
            var MaxSrNo = _Table.Compute("MAX(Sr_No)", "");
            if (MaxSrNo == DBNull.Value) { return 1; }
            else { return (int)MaxSrNo + 1; }
        }


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
            TempTableClass TempInvoice1 = new TempTableClass(UserName, Tables.BillReceivable, Variables.Vou_No, true);
            if (TempInvoice1.CountTemp > 0) { Variables.TranID = (int)TempInvoice1.TempView[0]["ID"]; } else { Variables.TranID = -1; }
            TempTableClass TempInvoice2 = new TempTableClass(UserName, Tables.BillReceivable2, Variables.TranID, false);

            Row1 = TempInvoice1.NewRecord();
            Row2 = TempInvoice2.NewRecord();
            Variables2Row();

            var ValidRow1 = TempInvoice1.TableValidate.Validation(Row1);
            var ValidRow2 = TempInvoice2.TableValidate.Validation(Row2);

            if (ValidRow1 && ValidRow2)
            {
                TempInvoice1.Save(false);
                TempInvoice2.CurrentRow["TranID"] = TempInvoice1.CurrentRow["ID"];
                TempInvoice2.View_Filter = $"TranID={TempInvoice2.CurrentRow["TranID"]}";
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
                TempInvoice2.TempRefresh();
                Invoice = TempInvoice2.TempTable;
                var message2 = $"Serial No {Variables.Sr_No} of Invoice No {Variables.Vou_No} has been saved successfully.";
                ErrorMessages.Add(MessageClass.SetMessage(message2, Color.Yellow));
            }
            return Page();
        }
        public IActionResult OnPostEdit(int? Sr_No)
        {
            var Vou_No = Variables.Vou_No;
            return RedirectToPage("SaleInvoice", routeValues: new { Vou_No, Sr_No });
        }
        public IActionResult OnPostDelete(int? Sr_No)
        {
            var Vou_No = Variables.Vou_No;
            // Write code here for delete the record.

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
            if (Variables.Vou_No.ToUpper() == "NEW")
            {
                var IsSaved = Save_VoucherNew();
                if (!IsSaved) { return Page(); }
            }
            else
            {
                var IsSaved = Save_Voucher();
                if (!IsSaved) { return Page(); }
            }
            #region Temp
            //// Validate Data
            //bool Valid1 = false;
            //bool Valid2 = true;

            //if (TempTable1.Rows.Count == 1)
            //{
            //    Row1 = TempTable1.Rows[0];
            //    ErrorMessages = TableValidationClass.RowValidation(Row1);
            //    if (ErrorMessages.Count > 0) { return Page(); } else { Valid1 = true; }
            //}
            //else
            //{
            //    ErrorMessages.Add(MessageClass.SetMessage("No record found..1.." + Variables.Vou_No));
            //}

            //if (Valid1)                 // If Table 1 valid is true;
            //{
            //    if (TempTable2.Rows.Count > 0)
            //    {
            //        foreach (DataRow Row in TempTable2.Rows)
            //        {
            //            ErrorMessages = TableValidationClass.RowValidation(Row);
            //            if (ErrorMessages.Count > 0) { return Page(); } else { Valid2 = true; }
            //        }
            //    }
            //    else
            //    {
            //        ErrorMessages.Add(MessageClass.SetMessage("No record found..2.." + Variables.Vou_No));
            //    }

            //    #endregion

            //    if (Valid2)
            //    {
            //        //var TargetRow = Row1;
            //        DataTableClass TargetTable1 = new(UserName, Tables.BillReceivable);
            //        TargetTable1.NewRecord();
            //        TargetTable1.CurrentRow.ItemArray = Row1.ItemArray;

            //        if (Variables.Vou_No.ToUpper() == "NEW")
            //        {
            //            Variables.Vou_No = GetNewVouNo(Variables.Vou_Date, "SL");
            //            TargetTable1.CurrentRow["Vou_No"] = Variables.Vou_No;
            //            TargetTable1.CurrentRow["ID"] = 0;
            //        }

            //        #region Save

            //        TargetTable1.Save();
            //        Variables.TranID = (int)TargetTable1.CurrentRow["ID"];
            //        ErrorMessages.AddRange(TargetTable1.TableValidation.MyMessages);
            //        if (ErrorMessages.Count > 0)
            //        {
            //            return Page();
            //        }
            //        Variables.TranID = (int)TargetTable1.CurrentRow["ID"];


            //        // Table 2


            //        DataTableClass TargetTable2 = new(UserName, Tables.BillReceivable2);
            //        TargetTable2.MyDataView.RowFilter = $"TranID={Variables.TranID}";

            //        DataView TempView2 = TargetTable2.MyDataView;
            //        foreach (DataRow Row in TargetTable2.Rows)
            //        {
            //            TempView2.RowFilter = $"ID={Row["ID"]}";
            //            if (TempView2.Count == 0)
            //            {
            //                TargetTable2.Delete();                      // Delete in Target Table is not exist in Temp Table;
            //            }
            //        }

            //        foreach (DataRow Row in TempTable2.Rows)
            //        {
            //            Row2 = TargetTable2.NewRecord();
            //            Row2.ItemArray = Row.ItemArray;
            //            TargetTable2.CurrentRow = Row2;
            //            TargetTable2.Save();                                    // Save existing all Temp Record in Targe Table.
            //        }
            //        if (TargetTable2.ErrorMessages.Count > 0) { return Page(); }

            //        #endregion


            //        #region Delete
            //        // Delete Voucher from Temp Data
            //        TempTableClass.Delete(UserName, Tables.BillReceivable, (int)Row1["ID"]);

            //        foreach (DataRow Row in TempTable2.Rows)
            //        {
            //            TempTableClass.Delete(UserName, Tables.BillReceivable2, (int)Row1["ID"]);
            //        }

            //        #endregion

            //        var message = $"Sale invoice {Variables.Vou_No} SAVED successfully., ";
            //        ErrorMessages.Add(MessageClass.SetMessage(message, Color.Green));
            //    }
            //}
            //IsSaved = true;


            #endregion

            var Vou_No = Variables.Vou_No;
            var Sr_No = 1;
            return RedirectToPage("SaleInvoice", routeValues: new { Vou_No, Sr_No });

        }

        private bool Save_Voucher()
        {
            var TempTable1 = TempTableClass.GetTable(UserName, Tables.BillReceivable, $"Vou_No = '{Variables.Vou_No}'");
            var TempTable2 = TempTableClass.GetTable(UserName, Tables.BillReceivable2, $"TranID = {TempTable1.Rows[0]["ID"]}");

            #region Validation
            // Validate Data
            bool Valid1 = false;
            bool Valid2 = true;

            if (TempTable1.Rows.Count == 1)
            {
                Row1 = TempTable1.Rows[0];
                ErrorMessages = TableValidationClass.RowValidation(Row1);
                if (ErrorMessages.Count > 0) { return false; } else { Valid1 = true; }
            }
            else
            {
                ErrorMessages.Add(MessageClass.SetMessage("No record found..1.." + Variables.Vou_No));
            }

            if (Valid1)                 // If Table 1 valid is true;
            {
                if (TempTable2.Rows.Count > 0)
                {
                    foreach (DataRow Row in TempTable2.Rows)
                    {
                        ErrorMessages = TableValidationClass.RowValidation(Row);
                        if (ErrorMessages.Count > 0) { return false; } else { Valid2 = true; }
                    }
                }
                else
                {
                    ErrorMessages.Add(MessageClass.SetMessage("No record found..2.." + Variables.Vou_No));
                }

                #endregion

                if (Valid2)
                {
                    DataTableClass TargetTable1 = new(UserName, Tables.BillReceivable);
                    TargetTable1.NewRecord();
                    TargetTable1.CurrentRow.ItemArray = Row1.ItemArray;

                    #region Save

                    TargetTable1.Save();
                    Variables.TranID = (int)TargetTable1.CurrentRow["ID"];
                    ErrorMessages.AddRange(TargetTable1.TableValidation.MyMessages);
                    if (ErrorMessages.Count > 0) { return false; }
                    Variables.TranID = (int)TargetTable1.CurrentRow["ID"];

                    // Table 2
                    DataTableClass TargetTable2 = new(UserName, Tables.BillReceivable2);
                    TargetTable2.MyDataView.RowFilter = $"TranID={Variables.TranID}";

                    DataView TempView2 = TargetTable2.MyDataView;
                    foreach (DataRow Row in TargetTable2.Rows)
                    {
                        TempView2.RowFilter = $"ID={Row["ID"]}";
                        if (TempView2.Count == 0)
                        {
                            TargetTable2.Delete();                      // Delete in Target Table is not exist in Temp Table;
                        }
                    }

                    foreach (DataRow Row in TempTable2.Rows)
                    {
                        Row2 = TargetTable2.NewRecord();
                        Row2.ItemArray = Row.ItemArray;
                        TargetTable2.CurrentRow = Row2;
                        TargetTable2.Save();                                    // Save existing all Temp Record in Targe Table.
                        if (TargetTable2.ErrorMessages.Count > 0) { return false; }
                    }


                    #endregion

                    #region Delete
                    // Delete Voucher from Temp Data
                    TempTableClass.Delete(UserName, Tables.BillReceivable, (int)Row1["ID"]);

                    foreach (DataRow Row in TempTable2.Rows)
                    {
                        TempTableClass.Delete(UserName, Tables.BillReceivable2, (int)Row["ID"]);
                    }

                    #endregion

                }
                //-------------------
            }
            return true;
        }



        private bool Save_VoucherNew()
        {
            var TempTable1 = TempTableClass.GetTable(UserName, Tables.BillReceivable, $"Vou_No = 'NEW'");
            var TempTable2 = TempTableClass.GetTable(UserName, Tables.BillReceivable2, $"TranID = {TempTable1.Rows[0]["ID"]}");

            #region Validation
            // Validate Data
            bool Valid1 = false;
            bool Valid2 = true;

            if (TempTable1.Rows.Count == 1)
            {
                Row1 = TempTable1.Rows[0];
                ErrorMessages = TableValidationClass.RowValidation(Row1);
                if (ErrorMessages.Count > 0) { return false; } else { Valid1 = true; }
            }
            else
            {
                ErrorMessages.Add(MessageClass.SetMessage("No record found..1.." + Variables.Vou_No));
                return false;
            }

            if (Valid1)                 // If Table 1 valid is true;
            {
                if (TempTable2.Rows.Count > 0)
                {
                    foreach (DataRow Row in TempTable2.Rows)
                    {
                        ErrorMessages = TableValidationClass.RowValidation(Row);
                        if (ErrorMessages.Count > 0) { return false; } else { Valid2 = true; }
                    }
                }
                else
                {
                    ErrorMessages.Add(MessageClass.SetMessage("No record found..2.." + Variables.Vou_No));
                    return false;
                }

                #region SAVE
                if (Valid1 && Valid2)
                {
                    DataTableClass TargetTable1 = new(UserName, Tables.BillReceivable);
                    DataTableClass TargetTable2 = new(UserName, Tables.BillReceivable2);

                    Variables.Vou_No = GetNewVouNo(Variables.Vou_Date, "SL");
                    TargetTable1.NewRecord();
                    TargetTable1.CurrentRow["Vou_No"] = Variables.Vou_No;
                    TargetTable1.CurrentRow["ID"] = 0;
                    TargetTable1.Save(false);
                    ErrorMessages = TargetTable1.ErrorMessages;
                    if (ErrorMessages.Count > 0) { return false; }

                    Variables.TranID = (int)TargetTable1.CurrentRow["ID"];
                    foreach (DataRow Row in TempTable2.Rows)
                    {
                        Row2 = TargetTable2.NewRecord();
                        Row2.ItemArray = Row.ItemArray;
                        TargetTable2.CurrentRow = Row2;
                        Row2["TranID"] = Variables.TranID;
                        Row2["ID"] = 0;
                        TargetTable2.Save(false);
                        ErrorMessages = TargetTable2.ErrorMessages;
                        if (ErrorMessages.Count > 0) { return false; }

                    }
                    #endregion

                    #region Delete
                    TempTableClass.Delete(UserName, Tables.BillReceivable, (int)Row1["ID"]);
                    foreach (DataRow Row in TempTable2.Rows)
                    {
                        TempTableClass.Delete(UserName, Tables.BillReceivable2, (int)Row["ID"]);
                    }
                    #endregion
                }
                #endregion
            }
            return true;
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
