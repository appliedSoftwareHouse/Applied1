using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SQLite;
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
        public string UserRole => UserProfile.GetUserRole(User);
        public DataTable Invoice { get; set; }
        public DataRow Row1 { get; set; }
        public DataRow Row2 { get; set; }
        public bool IsSaved { get; set; } = false;
        public bool IsRefresh { get; set; } = false;
        public string Vou_No { get; set; }
        public int TranID { get; set; }
        public bool IsNew { get; set; }

        public SQLiteCommand MyCommand = new();

        TempDBClass TempInvoice11 { get; set; } = new();
        TempDBClass TempInvoice22 { get; set; } = new();

        public string _Filter1;
        public string _Filter2;
        #endregion

        #region GET

        public void OnGetNew()
        {
            Vou_No = "NEW";

            _Filter1 = $"Vou_No='{Vou_No}'";
            TempInvoice11 = new(UserName, Tables.BillReceivable, _Filter1, true);
            TempInvoice11.TempTableFlash();
            _Filter2 = $"TranID={TempInvoice11.CurrentRow["ID"]}";
            TempInvoice22 = new(UserName, Tables.BillReceivable2, _Filter2, true);
            TempInvoice22.TempTableFlash();

            Row1 = TempInvoice11.CurrentRow;
            Row2 = TempInvoice22.CurrentRow;

            Row1["ID"] = TempInvoice11.MaxTableID();
            Row2["ID"] = 0;
            Row1["Vou_No"] = "NEW";
            Row2["TranID"] = Row1["ID"];
            Row2["Sr_No"] = 1;
            Row1["Status"] = VoucherStatus.Submitted.ToString();

            Invoice = TempInvoice22.TempTable;

            Row2Variables();
            return;

        }

        #endregion

        public void OnGet(string? Vou_No, int? Sr_No, bool? Refresh)
        {
            Vou_No ??= string.Empty;
            Refresh ??= true;
            TranID = 0;

            if (UserName == null) { return; }
            if (UserName.Length == 0) { return; }
            if (Vou_No.Length == 0) { return; }
            Variables = new();

            _Filter1 = $"Vou_No='{Vou_No}'";
            TempInvoice11 = new(UserName, Tables.BillReceivable, _Filter1, (bool)Refresh);
            if (TempInvoice11.TempTable.Rows.Count > 0) { TranID = (int)TempInvoice11.TempTable.Rows[0]["ID"]; }
            Row1 = TempInvoice11.CurrentRow;

            _Filter2 = $"TranID={TranID}";
            TempInvoice22 = new(UserName, Tables.BillReceivable2, _Filter2, (bool)Refresh);
            if (Sr_No == -1)
            {
                Row2 = TempInvoice22.NewRecord();
                Row2["Sr_No"] = MaxSrNo(TempInvoice22.TempTable);
                Row2["TranID"] = TranID;
            }
            else
            {
                if (TempInvoice22.CountTemp > 0)
                {
                    TempInvoice22.TempView.RowFilter = $"Sr_No={Sr_No}";
                    if (TempInvoice22.TempView.Count > 0)
                    {
                        Row2 = TempInvoice22.TempView[0].Row;
                    }
                    else
                    {
                        Row2 = TempInvoice22.CurrentRow;                 // Error id not found the record, pass empty row
                    }
                }
            }

            Invoice = TempInvoice22.TempTable;
            Row2Variables();
            return;
        }

        #region Temp

        //            if (Vou_No.Length == 11)
        //            {
        //                Refresh ??= true;
        //                IsRefresh = (bool) Refresh;
        //        _Filter1 = $"Vou_No='{Vou_No}'";

        //                TempInvoice11 = new TempDBClass(UserName, Tables.BillReceivable, _Filter1, IsRefresh);
        //                if (TempInvoice11.CountTemp > 0)
        //                {
        //                    TranID = (int) TempInvoice11.CurrentRow["ID"];
        //        Row1 = TempInvoice11.CurrentRow;
        //                }
        //                else
        //                {
        //                    ErrorMessages.Add(SetMessage("Error.... Voucher 1 Not Found", Color.Yellow));
        //                    return;
        //                }

        //_Filter2 = $"TranID={TranID}";
        //TempInvoice22 = new TempDBClass(UserName, Tables.BillReceivable2, _Filter2, IsRefresh);
        //}
        //if (Sr_No == -1)
        //{
        //    Row2 = TempInvoice22.NewRecord();
        //    Row2["Sr_No"] = MaxSrNo(TempInvoice22.TempTable);
        //    Row2["TranID"] = Row1["ID"];
        //}
        //else
        //{
        //    if (TempInvoice22.CountTemp > 0)
        //    {
        //        TempInvoice22.TempView.RowFilter = $"Sr_No={Sr_No}";
        //        if (TempInvoice22.TempView.Count > 0)
        //        {
        //            Row2 = TempInvoice22.TempView[0].Row;
        //        }
        //        else
        //        {
        //            Row2 = TempInvoice22.CurrentRow;
        //        }
        //    }
        //    else
        //    {
        //        ErrorMessages.Add(SetMessage("Error.... Voucher 2 Not Found", Color.Green));
        //        return;
        //    }
        //}
        //Invoice = TempInvoice22.TempTable;
        //Row2Variables();
        //return;

        //            }


        //            #endregion
        //        }
        #endregion

        #region Variables / Rows
        private void Row2Variables()
        {

            if (Row1 != null && Row2 != null)
            {
                Variables = new();
                {
                    Variables.ID1 = (int)Row1["ID"];
                    Variables.ID2 = (int)Row2["ID"];

                    Variables.Vou_No = Row1["Vou_No"].ToString();
                    Variables.Vou_Date = (DateTime)Row1["Vou_Date"];
                    Variables.Company = (int)Row1["Company"];
                    Variables.Employee = (int)Row1["Employee"];
                    Variables.Ref_No = Row1["Ref_No"].ToString();
                    Variables.Inv_No = Row1["Inv_No"].ToString();
                    Variables.Inv_Date = (DateTime)Row1["Inv_Date"];
                    Variables.Pay_Date = (DateTime)Row1["Pay_Date"];
                    Variables.Remarks = Row1["Description"].ToString();
                    Variables.Comments = Row1["Comments"].ToString();
                    Variables.Status = Row1["Status"].ToString();

                    Variables.TranID = (int)Row1["ID"];
                    Variables.Sr_No = (int)Row2["Sr_No"];
                    Variables.Inventory = (int)Row2["Inventory"];
                    Variables.Batch = Row2["Batch"].ToString();
                    Variables.Qty = (decimal)Row2["Qty"];
                    Variables.Rate = (decimal)Row2["Rate"];
                    Variables.Tax = (int)Row2["Tax"];
                    Variables.Description = Row2["Description"].ToString();
                    Variables.Project = (int)Row2["Project"];
                };

                Variables.Amount = (Variables.Qty * Variables.Rate);
                Variables.TaxRate = (int)AppFunctions.GetTaxRate(UserName, Variables.Tax);
                Variables.TaxAmount = (Variables.Amount * Variables.TaxRate) / 100;
                Variables.NetAmount = Variables.Amount + Variables.TaxAmount;
            }
            else
            {
                ErrorMessages.Add(MessageClass.SetMessage("Data row is not defined properly. Contact To Administrator"));
            }


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
            

            Vou_No = Variables.Vou_No;
            TranID = Variables.TranID;

            _Filter1 = $"Vou_No='{Vou_No}'";
            TempInvoice11 = new(UserName, Tables.BillReceivable, _Filter1, false);
            _Filter2 = $"TranID={TempInvoice11.CurrentRow["ID"]}";
            TempInvoice22 = new(UserName, Tables.BillReceivable2, _Filter2, false);
            TempInvoice22.TempView.RowFilter = $"Sr_No={Variables.Sr_No}";
            if (TempInvoice22.TempView.Count == 0) { TempInvoice22.NewRecord(); }
            else { TempInvoice22.CurrentRow = TempInvoice22.TempView[0].Row; }

            Row1 = TempInvoice11.CurrentRow;
            Row2 = TempInvoice22.CurrentRow;

            TempInvoice11.TableValidate.SQLAction = CommandAction.Insert.ToString();
            TempInvoice22.TableValidate.SQLAction = CommandAction.Insert.ToString();
            if (TempInvoice11.CountTemp > 0) { TempInvoice11.TableValidate.SQLAction = CommandAction.Update.ToString(); }
            if (TempInvoice22.CountTemp > 0) { TempInvoice22.TableValidate.SQLAction = CommandAction.Update.ToString(); }

            Variables2Row();

            var ValidRow1 = TempInvoice11.TableValidate.Validation(Row1);
            var ValidRow2 = TempInvoice22.TableValidate.Validation(Row2);

            if (ValidRow1 && ValidRow2)
            {
                TempInvoice11.Save(TempInvoice11.MyTempConnection, false);
                TempInvoice22.CurrentRow["TranID"] = TempInvoice11.CurrentRow["ID"];
                TempInvoice22.Save(TempInvoice22.MyTempConnection, false);
            }
            else
            {
                ErrorMessages.AddRange(TempInvoice11.TableValidate.MyMessages);
                ErrorMessages.AddRange(TempInvoice22.TableValidate.MyMessages);
            }



            if (ErrorMessages.Count == 0)
            {
                _Filter1 = $"Vou_No='{Vou_No}'";
                TempInvoice11 = new(UserName, Tables.BillReceivable, _Filter1, false);
                _Filter2 = $"TranID={TempInvoice11.CurrentRow["ID"]}";
                TempInvoice22 = new(UserName, Tables.BillReceivable2, _Filter2, false);
                var message2 = $"Serial No {Variables.Sr_No} of Invoice No {Variables.Vou_No} has been saved successfully.";
                ErrorMessages.Add(MessageClass.SetMessage(message2, Color.Yellow));
            }

            // Reset and Update Web Page Data

            Invoice = TempInvoice22.TempTable;
            return Page();
        }
        public IActionResult OnPostEdit(int? Sr_No)
        {
            Sr_No ??= 1;
            var Vou_No = Variables.Vou_No;
            var Refresh = false;
            return RedirectToPage("SaleInvoice", routeValues: new { Vou_No, Sr_No, Refresh });
        }
        public IActionResult OnPostDelete(int? Sr_No)
        {
            var Vou_No = Variables.Vou_No;
            var Refresh = false;

            if (Sr_No == 0) { return Page(); }                    // return page 

            var _Receivable1 = new DataTableClass(UserName, Tables.BillReceivable);
            var _Receivable2 = new DataTableClass(UserName, Tables.BillReceivable2);
            var _ValidforDelete1 = false;
            var _ValidforDelete2 = false;

            _Receivable1.MyDataView.RowFilter = $"ID={Variables.ID1}";
            _Receivable2.MyDataView.RowFilter = $"ID={Variables.ID2}";


            if (_Receivable1.CountView == 1) { _ValidforDelete1 = true; }
            if (_Receivable2.CountView > 0) { _ValidforDelete2 = true; }

            if (_ValidforDelete1 && _ValidforDelete2)
            {
                var _ID = (int)_Receivable2.MyDataView[0]["ID"];
                var _BillReceivable2 = new DataTableClass(UserName, Tables.BillReceivable2);
                _BillReceivable2.CurrentRow = _BillReceivable2.SeekRecord(_ID);

                if ((int)_BillReceivable2.CurrentRow["ID"] == _ID)
                {
                    _BillReceivable2.Delete();
                    Sr_No = 1;
                    TempInvoice11 = null;
                    TempInvoice22 = null;
                }
                _BillReceivable2.Dispose();
            }


            // Write code here for delete the record.

            return RedirectToPage("SaleInvoice", routeValues: new { Vou_No, Sr_No, Refresh });
        }
        public IActionResult OnPostNew()
        {
            var Vou_No = Variables.Vou_No;
            var Sr_No = -1;
            var Refresh = false;
            return RedirectToPage("SaleInvoice", routeValues: new { Vou_No, Sr_No, Refresh });
        }

        #endregion

        #region Save

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
            var Vou_No = Variables.Vou_No;
            var Sr_No = 1;
            var Refresh = true;
            return RedirectToPage("SaleInvoice", routeValues: new { Vou_No, Sr_No, Refresh });
        }

        private bool Save_Voucher()
        {
            if(UserRole=="Viewer") { return false; }

            _Filter1 = $"Vou_No = '{Variables.Vou_No}'";
            TempInvoice11 = new(UserName, Tables.BillReceivable, _Filter1, IsRefresh);
            _Filter2 = $"TranID = {TempInvoice11.TempTable.Rows[0]["ID"]}";
            TempInvoice22 = new(UserName, Tables.BillReceivable2, _Filter2, IsRefresh);

            #region Validation
            // Validate Data
            bool Valid1 = false;
            bool Valid2 = true;

            if (TempInvoice11.CountTemp == 1)
            {
                Row1 = TempInvoice11.TempTable.Rows[0];
                ErrorMessages = TableValidationClass.RowValidation(Row1);
                if (ErrorMessages.Count > 0) { return false; } else { Valid1 = true; }
            }
            else
            {
                ErrorMessages.Add(MessageClass.SetMessage("No record found..1.." + Variables.Vou_No));
            }

            if (Valid1)                 // If Table 1 valid is true;
            {
                if (TempInvoice22.CountTemp > 0)
                {
                    foreach (DataRow Row in TempInvoice22.TempTable.Rows)
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
                    DataRow dataRow = TargetTable1.NewRecord();
                    TargetTable1.CurrentRow.ItemArray = Row1.ItemArray;

                    #region Save 1

                    // Calcualte the total Amount of Invoice from Invocioe Table 2
                    decimal TotalAmount = 0.00M;
                    foreach (DataRow Row in TempInvoice22.TempTable.Rows)
                    {
                        var _Amount = (decimal)Row["Qty"] * (decimal)Row["Rate"];
                        var _TaxRate = AppFunctions.GetTaxRate(UserName, (int)Row["Tax"]);
                        var _TaxAmount = (decimal)_Amount * (decimal)_TaxRate;
                        TotalAmount += (decimal)_Amount + (decimal)_TaxAmount;
                    }

                    // END

                    TargetTable1.CurrentRow["Amount"] = TotalAmount;
                    TargetTable1.Save();                                                                                                    // Insert / Update Record in DataBase Table.
                    Variables.TranID = (int)TargetTable1.CurrentRow["ID"];
                    ErrorMessages.AddRange(TargetTable1.TableValidation.MyMessages);
                    if (ErrorMessages.Count > 0) { return false; }

                    #endregion

                    #region Save 2

                    // Table 2
                    DataTableClass TargetTable2 = new(UserName, Tables.BillReceivable2);
                    TargetTable2.MyDataView.RowFilter = $"TranID={Variables.TranID}";

                    DataView TempView2 = TargetTable2.MyDataView;
                    foreach (DataRow Row in TempView2.ToTable().Rows)
                    {
                        TempView2.RowFilter = $"ID={Row["ID"]}";
                        if (TempView2.Count == 0)
                        {
                            TargetTable2.Delete();                      // Delete in Target Table is not exist in Temp Table;
                        }
                    }

                    foreach (DataRow Row in TempInvoice22.TempTable.Rows)
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

                    TempInvoice11.DeleteAll();
                    TempInvoice22.DeleteAll();

                    #endregion

                }
                //-------------------
            }
            return true;
        }
        private bool Save_VoucherNew()
        {
            if (UserRole == "Viewer") { return false; }
            var TempTable1 = xTempTableClass.GetTable(UserName, Tables.BillReceivable, $"Vou_No = 'NEW'");
            var TempTable2 = xTempTableClass.GetTable(UserName, Tables.BillReceivable2, $"TranID = {TempTable1.Rows[0]["ID"]}");

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
                    TargetTable1.CurrentRow["ID"] = 0;
                    TargetTable1.CurrentRow["Vou_No"] = Variables.Vou_No;
                    TargetTable1.CurrentRow["Vou_Date"] = Variables.Vou_Date;
                    TargetTable1.CurrentRow["Company"] = Variables.Company;
                    TargetTable1.CurrentRow["Employee"] = Variables.Employee;
                    TargetTable1.CurrentRow["Ref_No"] = Variables.Ref_No;
                    TargetTable1.CurrentRow["Inv_No"] = Variables.Inv_No;
                    TargetTable1.CurrentRow["Inv_Date"] = Variables.Inv_Date;
                    TargetTable1.CurrentRow["Pay_Date"] = Variables.Pay_Date;
                    TargetTable1.CurrentRow["Amount"] = Variables.Amount;
                    TargetTable1.CurrentRow["Description"] = Variables.Description;
                    TargetTable1.CurrentRow["Comments"] = Variables.Comments;
                    TargetTable1.CurrentRow["Status"] = Variables.Status;
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

                    xTempTableClass.Delete(UserName, Tables.BillReceivable, (int)Row1["ID"]);
                    foreach (DataRow Row in TempTable2.Rows)
                    {
                        xTempTableClass.Delete(UserName, Tables.BillReceivable2, (int)Row["ID"]);
                    }
                    #endregion
                }
                #endregion
            }
            return true;
        }

        #endregion

        #region Max
        public int MaxSrNo(DataTable _Table)
        {
            var MaxSrNo = _Table.Compute("MAX(Sr_No)", "");
            if (MaxSrNo == DBNull.Value) { return 1; }
            else { return (int)MaxSrNo + 1; }
        }

        #endregion

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

        #region Print & Back

        public IActionResult OnPostPrint()
        {
            var TranID = Variables.ID1;
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
