using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Net.Http.Headers;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Data
{
    public class PostingClass
    {
        #region Cash Book
        public static List<Message> PostCashBook(string UserName, int id)
        {

            DataTableClass tb_Ledger = new(UserName, Tables.Ledger, "");
            List<Message> ErrorMessages = new List<Message>();
            List<DataRow> VoucherRows = new();

            tb_Ledger.MyDataView.RowFilter = $"TranID='{id}' AND Vou_Type='{VoucherType.Cash}'";
            if (tb_Ledger.MyDataView.Count == 0)
            {
                tb_Ledger.TableValidation.SQLAction = CommandAction.Insert.ToString();
                DataRow Row = AppFunctions.GetDataRow(UserName, Tables.CashBook, id);
                tb_Ledger.NewRecord();                                                                                                 // Cash Book DR Entry
                tb_Ledger.CurrentRow["ID"] = 0;
                tb_Ledger.CurrentRow["TranID"] = id;
                tb_Ledger.CurrentRow["Vou_Type"] = VoucherType.Cash.ToString();
                tb_Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                tb_Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                tb_Ledger.CurrentRow["SR_No"] = 1;
                tb_Ledger.CurrentRow["Ref_No"] = Row["Ref_No"];
                tb_Ledger.CurrentRow["BookID"] = Row["BookID"];
                tb_Ledger.CurrentRow["COA"] = Row["COA"];
                tb_Ledger.CurrentRow["DR"] = Row["DR"];
                tb_Ledger.CurrentRow["CR"] = Row["CR"];
                tb_Ledger.CurrentRow["Customer"] = Row["Customer"];
                tb_Ledger.CurrentRow["Project"] = Row["Project"];
                tb_Ledger.CurrentRow["Employee"] = Row["Employee"];
                tb_Ledger.CurrentRow["Description"] = Row["Description"];
                tb_Ledger.CurrentRow["Comments"] = Row["Comments"];
                if (tb_Ledger.IsRowValid(tb_Ledger.CurrentRow, CommandAction.Insert, PostType.CashBook))
                {
                    VoucherRows.Add(tb_Ledger.CurrentRow);
                }
                else
                {
                    ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages);                          // Collect Error Messages id occure.
                }

                tb_Ledger.NewRecord();                                                                                                  // Cash Book CR Entry
                tb_Ledger.CurrentRow["ID"] = 0;
                tb_Ledger.CurrentRow["TranID"] = id;
                tb_Ledger.CurrentRow["Vou_Type"] = VoucherType.Cash.ToString();
                tb_Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                tb_Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                tb_Ledger.CurrentRow["SR_No"] = 2;
                tb_Ledger.CurrentRow["Ref_No"] = Row["Ref_No"];
                tb_Ledger.CurrentRow["BookID"] = Row["BookID"];
                tb_Ledger.CurrentRow["COA"] = Row["BookID"];                                                           // COA => Book ID
                tb_Ledger.CurrentRow["DR"] = Row["CR"];                                                                    // DR => CR
                tb_Ledger.CurrentRow["CR"] = Row["DR"];                                                                    // CR => DR
                tb_Ledger.CurrentRow["Customer"] = Row["Customer"];
                tb_Ledger.CurrentRow["Project"] = Row["Project"];
                tb_Ledger.CurrentRow["Employee"] = Row["Employee"];
                tb_Ledger.CurrentRow["Description"] = Row["Description"];
                tb_Ledger.CurrentRow["Comments"] = Row["Comments"];
                if (tb_Ledger.IsRowValid(tb_Ledger.CurrentRow, CommandAction.Insert, PostType.CashBook))
                {
                    VoucherRows.Add(tb_Ledger.CurrentRow);
                }
                else
                {
                    ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages);                          // Collect Error Messages id occure.
                }

            }
            else
            {
                ErrorMessages.Add(new Message() { Success = false, ErrorID = 105, Msg = string.Concat(tb_Ledger.MyDataView[0]["Vou_No"], " is already posted") });
            }

            if (ErrorMessages.Count == 0)
            {
                tb_Ledger.TableValidation.SQLAction = CommandAction.Insert.ToString();
                tb_Ledger.CurrentRow = VoucherRows[0];                  // Debit Transaction
                tb_Ledger.Save();
                tb_Ledger.CurrentRow = VoucherRows[1];                  // Credit Transaction.
                tb_Ledger.Save();
                DataTableClass.Replace(UserName, Tables.CashBook, id, "Status", VoucherStatus.Posted.ToString());
            }

            return ErrorMessages;
        }
        #endregion

        #region Bank Book
        public static List<Message> PostBankBook(string UserName, int id)
        {

            DataTableClass tb_Ledger = new(UserName, Tables.Ledger, "");
            List<Message> ErrorMessages = new List<Message>();
            List<DataRow> VoucherRows = new();

            tb_Ledger.MyDataView.RowFilter = $"TranID='{id}' AND Vou_Type='{VoucherType.Bank}'";
            if (tb_Ledger.MyDataView.Count == 0)
            {
                tb_Ledger.TableValidation.SQLAction = CommandAction.Insert.ToString();
                DataRow Row = AppFunctions.GetDataRow(UserName, Tables.BankBook, id);
                tb_Ledger.NewRecord();                                                                                                 // Bank Book DR Entry
                tb_Ledger.CurrentRow["ID"] = 0;
                tb_Ledger.CurrentRow["TranID"] = id;
                tb_Ledger.CurrentRow["Vou_Type"] = VoucherType.Bank.ToString();
                tb_Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                tb_Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                tb_Ledger.CurrentRow["SR_No"] = 1;
                tb_Ledger.CurrentRow["Ref_No"] = Row["Ref_No"];
                tb_Ledger.CurrentRow["BookID"] = Row["BookID"];
                tb_Ledger.CurrentRow["COA"] = Row["COA"];
                tb_Ledger.CurrentRow["DR"] = Row["DR"];
                tb_Ledger.CurrentRow["CR"] = Row["CR"];
                tb_Ledger.CurrentRow["Customer"] = Row["Customer"];
                tb_Ledger.CurrentRow["Project"] = Row["Project"];
                tb_Ledger.CurrentRow["Employee"] = Row["Employee"];
                tb_Ledger.CurrentRow["Description"] = Row["Description"];
                tb_Ledger.CurrentRow["Comments"] = Row["Comments"];
                if (tb_Ledger.IsRowValid(tb_Ledger.CurrentRow, CommandAction.Insert, PostType.BankBook))
                {
                    VoucherRows.Add(tb_Ledger.CurrentRow);
                }
                else
                {
                    ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages);                          // Collect Error Messages id occure.
                }

                tb_Ledger.NewRecord();                                                                                                  // Bank Book CR Entry
                tb_Ledger.CurrentRow["ID"] = 0;
                tb_Ledger.CurrentRow["TranID"] = id;
                tb_Ledger.CurrentRow["Vou_Type"] = VoucherType.Bank.ToString();
                tb_Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                tb_Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                tb_Ledger.CurrentRow["SR_No"] = 2;
                tb_Ledger.CurrentRow["Ref_No"] = Row["Ref_No"];
                tb_Ledger.CurrentRow["BookID"] = Row["BookID"];
                tb_Ledger.CurrentRow["COA"] = Row["BookID"];                                                           // COA => Book ID
                tb_Ledger.CurrentRow["DR"] = Row["CR"];                                                                    // DR => CR
                tb_Ledger.CurrentRow["CR"] = Row["DR"];                                                                    // CR => DR
                tb_Ledger.CurrentRow["Customer"] = Row["Customer"];
                tb_Ledger.CurrentRow["Project"] = Row["Project"];
                tb_Ledger.CurrentRow["Employee"] = Row["Employee"];
                tb_Ledger.CurrentRow["Description"] = Row["Description"];
                tb_Ledger.CurrentRow["Comments"] = Row["Comments"];
                if (tb_Ledger.IsRowValid(tb_Ledger.CurrentRow, CommandAction.Insert, PostType.BankBook))
                {
                    VoucherRows.Add(tb_Ledger.CurrentRow);
                }
                else
                {
                    ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages);                          // Collect Error Messages id occure.
                }

            }
            else
            {
                ErrorMessages.Add(new Message() { Success = false, ErrorID = 105, Msg = string.Concat(tb_Ledger.MyDataView[0]["Vou_No"], " is already posted") });
            }

            if (ErrorMessages.Count == 0)
            {
                tb_Ledger.TableValidation.SQLAction = CommandAction.Insert.ToString();
                tb_Ledger.CurrentRow = VoucherRows[0];                  // Debit Transaction
                tb_Ledger.Save(false);
                tb_Ledger.CurrentRow = VoucherRows[1];                  // Credit Transaction.
                tb_Ledger.Save(false);
                DataTableClass.Replace(UserName, Tables.BankBook, id, "Status", VoucherStatus.Posted.ToString());
            }

            return ErrorMessages;
        }
        #endregion

        #region Bill Payable
        public static List<Message> PostBillPayable(string UserName, int id)
        {
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);
            List<Message> ErrorMessages = new List<Message>();
            List<DataRow> VoucherRows = new();
            DataRow RowBill1 = AppFunctions.GetRecord(UserName, Tables.BillPayable, id);

            int COA_Purchase = AppRegistry.GetNumber(UserName, "BPay_Stock");                   // COA: Purchsase on Credit 
            int COA_Tax = AppRegistry.GetNumber(UserName, "BPay_Tax");
            int COA_Payable = AppRegistry.GetNumber(UserName, "BPay_Payable"); ;                   // COA: Trade Payable.

            if (COA_Purchase == 0 || COA_Tax == 0 || COA_Payable == 0)
            {
                ErrorMessages.Add(SetMessage("Posting Accounts are not define properly. Select (Assign) them in Setting."));
                return ErrorMessages;
            }

            tb_Ledger.MyDataView.RowFilter = string.Concat("TranID=", id.ToString(), " AND Vou_Type='", VoucherType.Payable.ToString(), "'");             // Filter Record for check? Already exist or not.
            if (tb_Ledger.MyDataView.Count == 0)
            {
                DataTableClass fun_BillPayable = new(UserName, Tables.fun_BillPayableEntry);                                    // Get SQLite View for Entry
                fun_BillPayable.MyDataView.RowFilter = string.Concat("TranID=", id.ToString());

                int SRNO = 1;
                bool IsValidated = true;

                foreach (DataRow Row in fun_BillPayable.MyDataView.ToTable().Rows)
                {
                    // Purchase Entry
                    tb_Ledger.NewRecord();
                    tb_Ledger.CurrentRow["ID"] = 0;
                    tb_Ledger.CurrentRow["TranID"] = RowBill1["ID"];
                    tb_Ledger.CurrentRow["Vou_Type"] = VoucherType.Payable.ToString();
                    tb_Ledger.CurrentRow["Vou_Date"] = RowBill1["Vou_Date"];
                    tb_Ledger.CurrentRow["Vou_No"] = RowBill1["Vou_No"];
                    tb_Ledger.CurrentRow["SR_No"] = SRNO; SRNO += 1;
                    tb_Ledger.CurrentRow["Ref_No"] = RowBill1["Ref_No"];
                    tb_Ledger.CurrentRow["BookID"] = 0;
                    tb_Ledger.CurrentRow["COA"] = COA_Purchase;                                                           // COA => Book ID
                    tb_Ledger.CurrentRow["DR"] = Row["Total"];                                                                 // DR => CR
                    tb_Ledger.CurrentRow["CR"] = 0;                                                                    // CR => DR
                    tb_Ledger.CurrentRow["Customer"] = RowBill1["Company"];
                    tb_Ledger.CurrentRow["Project"] = 0;
                    tb_Ledger.CurrentRow["Employee"] = RowBill1["Employee"];
                    tb_Ledger.CurrentRow["Description"] = RowBill1["Description"];
                    tb_Ledger.CurrentRow["Comments"] = RowBill1["Comments"];
                    tb_Ledger.TableValidation.Validation(tb_Ledger.CurrentRow, CommandAction.Insert);
                    if (tb_Ledger.ErrorCount > 0) { IsValidated = false; ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages); }
                    else { VoucherRows.Add(tb_Ledger.CurrentRow); }

                    if (decimal.Parse(Row["TaxAmount"].ToString()) > 0)
                    {
                        // Tax Entry
                        tb_Ledger.NewRecord();
                        tb_Ledger.CurrentRow["ID"] = 0;
                        tb_Ledger.CurrentRow["TranID"] = RowBill1["ID"];
                        tb_Ledger.CurrentRow["Vou_Type"] = VoucherType.Payable.ToString();
                        tb_Ledger.CurrentRow["Vou_Date"] = RowBill1["Vou_Date"];
                        tb_Ledger.CurrentRow["Vou_No"] = RowBill1["Vou_No"];
                        tb_Ledger.CurrentRow["SR_No"] = SRNO; SRNO += 1;
                        tb_Ledger.CurrentRow["Ref_No"] = RowBill1["Ref_No"];
                        tb_Ledger.CurrentRow["BookID"] = 0;
                        tb_Ledger.CurrentRow["COA"] = COA_Tax;
                        tb_Ledger.CurrentRow["DR"] = 0;
                        tb_Ledger.CurrentRow["CR"] = Row["TaxAmount"];
                        tb_Ledger.CurrentRow["Customer"] = RowBill1["Company"];
                        tb_Ledger.CurrentRow["Project"] = Row["Project"];
                        tb_Ledger.CurrentRow["Employee"] = RowBill1["Employee"];
                        tb_Ledger.CurrentRow["Description"] = RowBill1["Description"];
                        tb_Ledger.CurrentRow["Comments"] = RowBill1["Comments"];
                        tb_Ledger.TableValidation.Validation(tb_Ledger.CurrentRow, CommandAction.Insert);
                        if (tb_Ledger.ErrorCount > 0) { IsValidated = false; ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages); }
                        else { VoucherRows.Add(tb_Ledger.CurrentRow); }
                    }
                    // Credit [CR] Entry
                    tb_Ledger.NewRecord();
                    tb_Ledger.CurrentRow["ID"] = 0;
                    tb_Ledger.CurrentRow["TranID"] = RowBill1["ID"];
                    tb_Ledger.CurrentRow["Vou_Type"] = VoucherType.Payable.ToString();
                    tb_Ledger.CurrentRow["Vou_Date"] = RowBill1["Vou_Date"];
                    tb_Ledger.CurrentRow["Vou_No"] = RowBill1["Vou_No"];
                    tb_Ledger.CurrentRow["SR_No"] = SRNO; SRNO += 1;
                    tb_Ledger.CurrentRow["Ref_No"] = RowBill1["Ref_No"];
                    tb_Ledger.CurrentRow["BookID"] = 0;
                    tb_Ledger.CurrentRow["COA"] = COA_Payable;
                    tb_Ledger.CurrentRow["DR"] = 0;
                    tb_Ledger.CurrentRow["CR"] = Row["Amount"];
                    tb_Ledger.CurrentRow["Customer"] = RowBill1["Company"];
                    tb_Ledger.CurrentRow["Project"] = Row["Project"];
                    tb_Ledger.CurrentRow["Employee"] = RowBill1["Employee"];
                    tb_Ledger.CurrentRow["Description"] = RowBill1["Description"];
                    tb_Ledger.CurrentRow["Comments"] = RowBill1["Comments"];
                    tb_Ledger.TableValidation.Validation(tb_Ledger.CurrentRow, CommandAction.Insert);
                    if (tb_Ledger.ErrorCount > 0) { IsValidated = false; ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages); }
                    else { VoucherRows.Add(tb_Ledger.CurrentRow); }
                }

                if (IsValidated)
                {
                    foreach (DataRow Row in VoucherRows)
                    {
                        tb_Ledger.CurrentRow = Row;
                        tb_Ledger.Save();
                        DataTableClass.Replace(UserName, Tables.BillPayable, id, "Status", VoucherStatus.Posted);
                        ErrorMessages = new();
                        ErrorMessages.Add(new Message { ErrorID = 103, Msg = string.Concat("Voucher No ", RowBill1["Vou_No"].ToString(), " has been posted sucessfully.") });
                    }
                }
            }
            else
            {
                ErrorMessages = new();
                ErrorMessages.Add(new Message { ErrorID = 103, Msg = string.Concat("Voucher No ", RowBill1["Vou_No"].ToString(), " is already posted. Contact to Administrator") });
            }

            return ErrorMessages;
        }
        #endregion

        #region Bill Receivable / Sales Invoices
        public static List<Message> PostBillReceivable(string UserName, int id)
        {
            List<Message> ErrorMessages = new List<Message>();
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);
            List<DataRow> VoucherRows = new();
            SQLiteParameter pID = new SQLiteParameter("@ID", id);
            DataTable SaleInvoice = DataTableClass.GetTable(UserName, SQLQuery.SalesInvoice(), pID);

            #region Validations
            if (SaleInvoice == null)
            {
                ErrorMessages.Add(MessageClass.SetMessage("Error: Sale invocie object is null here. Contact to Administrator"));
            }

            if (SaleInvoice.Rows.Count == 0)
            {
                ErrorMessages.Add(MessageClass.SetMessage("Error: Sale invocie does't have any record to post. Contact to Administrator"));
                return ErrorMessages;
            }

            if (SaleInvoice.Rows[0]["Status"].ToString() == VoucherStatus.Posted.ToString())
            {
                ErrorMessages.Add(MessageClass.SetMessage("Error: Sale Invocie is already posted. Contact to Administrator"));
                return ErrorMessages;
            }
            #endregion

            int COA_DR = AppRegistry.GetNumber(UserName, "BRec_Receivable");
            int COA_CR = AppRegistry.GetNumber(UserName, "BRec_Stock");
            int COA_Tax = AppRegistry.GetNumber(UserName, "BRec_Tax");
            bool IsValidated = true;
            int SRNO = 1;
            string Vou_No = SaleInvoice.Rows[0]["Vou_No"].ToString();

            #region Check the vocher is already exist in the ledger ? or not exist.
            tb_Ledger.MyDataView.RowFilter = $"Vou_No='{Vou_No}'";
            if (tb_Ledger.CountView > 0)
            {
                ErrorMessages.Add(SetMessage("Voucher Numbre is already exist in the ledger. Contact to Administrator."));
                return ErrorMessages;
            }
            #endregion

            foreach (DataRow Row in SaleInvoice.Rows)
            {
                if (Vou_No != Row["Vou_No"].ToString())
                {
                    ErrorMessages.Add(SetMessage("Voucher Number not matched. Posting process suspended.", ConsoleColor.Red));
                    break;
                }
                var _Description = (string)Row["Inventory"] + ": " + (string)Row["Description"];
                #region Debit Entry
                tb_Ledger.NewRecord();
                tb_Ledger.CurrentRow["ID"] = 0;
                tb_Ledger.CurrentRow["TranID"] = Row["TranID"];
                tb_Ledger.CurrentRow["Vou_Type"] = VoucherType.Receivable.ToString();
                tb_Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                tb_Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                tb_Ledger.CurrentRow["SR_No"] = SRNO; SRNO += 1;
                tb_Ledger.CurrentRow["Ref_No"] = Row["Ref_No"];
                tb_Ledger.CurrentRow["BookID"] = 0;
                tb_Ledger.CurrentRow["COA"] = COA_DR;
                tb_Ledger.CurrentRow["DR"] = Row["Amount"];
                tb_Ledger.CurrentRow["CR"] = 0;
                tb_Ledger.CurrentRow["Customer"] = Row["CompanyID"];
                tb_Ledger.CurrentRow["Project"] = Row["ProjectID"];
                tb_Ledger.CurrentRow["Employee"] = Row["EmployeeID"];
                tb_Ledger.CurrentRow["Description"] = _Description;
                tb_Ledger.CurrentRow["Comments"] = Row["Remarks"];
                tb_Ledger.TableValidation.Validation(tb_Ledger.CurrentRow, CommandAction.Insert);
                if (tb_Ledger.ErrorCount > 0) { IsValidated = false; ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages); }
                else { VoucherRows.Add(tb_Ledger.CurrentRow); }
                #endregion

                #region Tax Entry
                if (Conversion.ToDecimal(Row["Tax_Amount"]) > 0)
                {
                    tb_Ledger.NewRecord();
                    tb_Ledger.CurrentRow["ID"] = 0;
                    tb_Ledger.CurrentRow["TranID"] = Row["TranID"];
                    tb_Ledger.CurrentRow["Vou_Type"] = VoucherType.Receivable.ToString();
                    tb_Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                    tb_Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                    tb_Ledger.CurrentRow["SR_No"] = SRNO; SRNO += 1;
                    tb_Ledger.CurrentRow["Ref_No"] = Row["Ref_No"];
                    tb_Ledger.CurrentRow["BookID"] = 0;
                    tb_Ledger.CurrentRow["COA"] = COA_Tax;
                    tb_Ledger.CurrentRow["DR"] = Row["Tax_Amount"]; ;
                    tb_Ledger.CurrentRow["CR"] = 0;
                    tb_Ledger.CurrentRow["Customer"] = Row["CompanyID"];
                    tb_Ledger.CurrentRow["Project"] = Row["ProjectID"];
                    tb_Ledger.CurrentRow["Employee"] = Row["EmployeeID"];
                    tb_Ledger.CurrentRow["Description"] = string.Concat(Row["Tax"], ": ", Row["Description"]);
                    tb_Ledger.CurrentRow["Comments"] = Row["Remarks"];
                    tb_Ledger.TableValidation.Validation(tb_Ledger.CurrentRow, CommandAction.Insert);
                    if (tb_Ledger.ErrorCount > 0) { IsValidated = false; ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages); }
                    else { VoucherRows.Add(tb_Ledger.CurrentRow); }
                }
                #endregion

                #region Credit Entry
                tb_Ledger.NewRecord();
                tb_Ledger.CurrentRow["ID"] = 0;
                tb_Ledger.CurrentRow["TranID"] = Row["TranID"];
                tb_Ledger.CurrentRow["Vou_Type"] = VoucherType.Receivable.ToString();
                tb_Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                tb_Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                tb_Ledger.CurrentRow["SR_No"] = SRNO; SRNO += 1;
                tb_Ledger.CurrentRow["Ref_No"] = Row["Ref_No"];
                tb_Ledger.CurrentRow["BookID"] = 0;
                tb_Ledger.CurrentRow["COA"] = COA_CR;
                tb_Ledger.CurrentRow["DR"] = 0;
                tb_Ledger.CurrentRow["CR"] = Row["Net_Amount"];
                tb_Ledger.CurrentRow["Customer"] = Row["CompanyID"];
                tb_Ledger.CurrentRow["Project"] = Row["ProjectID"];
                tb_Ledger.CurrentRow["Employee"] = Row["EmployeeID"];
                tb_Ledger.CurrentRow["Description"] = Row["Description"];
                tb_Ledger.CurrentRow["Comments"] = Row["Remarks"];
                tb_Ledger.TableValidation.Validation(tb_Ledger.CurrentRow, CommandAction.Insert);
                if (tb_Ledger.ErrorCount > 0) { IsValidated = false; ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages); }
                else { VoucherRows.Add(tb_Ledger.CurrentRow); }
                #endregion
            }

            // Save Voucher.
            if (IsValidated)
            {
                foreach (DataRow Row in VoucherRows)
                {
                    var NoValidateAgain = false;
                    tb_Ledger.CurrentRow = Row;
                    tb_Ledger.Save(NoValidateAgain);
                    ErrorMessages.AddRange(tb_Ledger.ErrorMessages);
                }
                DataTableClass.Replace(UserName, Tables.BillReceivable, id, "Status", VoucherStatus.Posted);
                ErrorMessages.Add(MessageClass.SetMessage($"Voucher No {Vou_No}  has been posted sucessfully.", Color.Green));

            }
            else
            {
                ErrorMessages.Add(SetMessage($"Voucher No {Vou_No}  has not been posted sucessfully.", Color.Red));
            }

            return ErrorMessages;
        }
        #endregion

        #region Sales Return
        public static List<Message> PostSaleReturn(string UserName, int id)
        {
            // Get a Records from Sale Return Submitted only.
            // Make Two Entry voucher
            // Save in Ledger
            // End

            var _Filter = $"SR_ID={id}";
            List<Message> ErrorMessages = new List<Message>();
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);
            List<DataRow> VoucherRows = new();
            DataTable SaleReturn = DataTableClass.GetTable(UserName, SQLQuery.PostSaleReturn(_Filter));

            #region Validation
            if (SaleReturn == null)
            {
                ErrorMessages.Add(SetMessage("Error: Sale invocie object is null here. Contact to Administrator"));
                return ErrorMessages;
            }

            if (SaleReturn.Rows.Count == 0)
            {
                ErrorMessages.Add(SetMessage("Error: Sale invocie does't have any record to post. Contact to Administrator"));
                return ErrorMessages;
            }

            if (SaleReturn.Rows[0]["Status"].ToString() == VoucherStatus.Posted.ToString())
            {
                ErrorMessages.Add(MessageClass.SetMessage("Error: Sale Invocie is already posted. Contact to Administrator"));
                return ErrorMessages;
            }
            #endregion

            var COA_DR = AppRegistry.GetNumber(UserName, "BRec_Stock");
            var COA_CR = AppRegistry.GetNumber(UserName, "BRec_Receivable");
            var COA_Tax = AppRegistry.GetNumber(UserName, "BRec_Tax");
            var IsValidated = true;
            var SRNO = 1;
            var Vou_No = SaleReturn.Rows[0]["Vou_No"].ToString();
            var Vou_Type = VoucherType.SaleReturn;

            #region Check the voher is already exist in the ledger ? or not exist.
            tb_Ledger.MyDataView.RowFilter = $"Vou_No='{Vou_No}'";
            if (tb_Ledger.CountView > 0)
            {
                ErrorMessages.Add(SetMessage("Voucher Numbre is already exist in the ledger. Contact to Administrator."));
                return ErrorMessages;
            }
            #endregion

            #region Create Voucher

            foreach (DataRow Row in SaleReturn.Rows)
            {
                IsValidated = true;         //  Default value.
                if (Vou_No != Row["Vou_No"].ToString())
                {

                }
                var _Description = (string)Row["Inventory"] + ": " + (string)Row["Description"];
                #region Debit Entry
                tb_Ledger.NewRecord();
                tb_Ledger.CurrentRow["ID"] = 0;
                tb_Ledger.CurrentRow["TranID"] = Row["SR_TranID"];
                tb_Ledger.CurrentRow["Vou_Type"] = Vou_Type;
                tb_Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                tb_Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                tb_Ledger.CurrentRow["SR_No"] = SRNO; SRNO += 1;
                tb_Ledger.CurrentRow["Ref_No"] = DBNull.Value;
                tb_Ledger.CurrentRow["BookID"] = DBNull.Value;
                tb_Ledger.CurrentRow["COA"] = COA_DR;
                tb_Ledger.CurrentRow["DR"] = Row["RAmount"];
                tb_Ledger.CurrentRow["CR"] = 0;
                tb_Ledger.CurrentRow["Customer"] = Row["CompanyID"];
                tb_Ledger.CurrentRow["Project"] = Row["ProjectID"];
                tb_Ledger.CurrentRow["Employee"] = Row["EmployeeID"];
                tb_Ledger.CurrentRow["Description"] = _Description;
                tb_Ledger.CurrentRow["Comments"] = Row["Remarks"];
                tb_Ledger.TableValidation.Validation(tb_Ledger.CurrentRow, CommandAction.Insert);
                if (tb_Ledger.ErrorCount > 0) { IsValidated = false; ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages); }
                else { VoucherRows.Add(tb_Ledger.CurrentRow); }
                #endregion

                #region Tax Entry
                if (Conversion.ToDecimal(Row["TaxAmount"]) > 0)
                {
                    tb_Ledger.NewRecord();
                    tb_Ledger.CurrentRow["ID"] = 0;
                    tb_Ledger.CurrentRow["TranID"] = Row["SR_TranID"];
                    tb_Ledger.CurrentRow["Vou_Type"] = Vou_Type;
                    tb_Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                    tb_Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                    tb_Ledger.CurrentRow["SR_No"] = SRNO; SRNO += 1;
                    tb_Ledger.CurrentRow["Ref_No"] = DBNull.Value;
                    tb_Ledger.CurrentRow["BookID"] = DBNull.Value;
                    tb_Ledger.CurrentRow["COA"] = COA_Tax;
                    tb_Ledger.CurrentRow["DR"] = 0;
                    tb_Ledger.CurrentRow["CR"] = Row["RTaxAmount"];
                    tb_Ledger.CurrentRow["Customer"] = Row["CompanyID"];
                    tb_Ledger.CurrentRow["Project"] = Row["ProjectID"];
                    tb_Ledger.CurrentRow["Employee"] = Row["EmployeeID"];
                    tb_Ledger.CurrentRow["Description"] = string.Concat(Row["Tax"], ": ", _Description);
                    tb_Ledger.CurrentRow["Comments"] = Row["Remarks"];
                    tb_Ledger.TableValidation.Validation(tb_Ledger.CurrentRow, CommandAction.Insert);
                    if (tb_Ledger.ErrorCount > 0) { IsValidated = false; ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages); }
                    else { VoucherRows.Add(tb_Ledger.CurrentRow); }
                }
                #endregion

                #region Credit Entry
                tb_Ledger.NewRecord();
                tb_Ledger.CurrentRow["ID"] = 0;
                tb_Ledger.CurrentRow["TranID"] = Row["SR_TranID"];
                tb_Ledger.CurrentRow["Vou_Type"] = Vou_Type;
                tb_Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                tb_Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                tb_Ledger.CurrentRow["SR_No"] = SRNO; SRNO += 1;
                tb_Ledger.CurrentRow["Ref_No"] = DBNull.Value;
                tb_Ledger.CurrentRow["BookID"] = DBNull.Value;
                tb_Ledger.CurrentRow["COA"] = COA_CR;
                tb_Ledger.CurrentRow["DR"] = 0;
                tb_Ledger.CurrentRow["CR"] = Row["RNetAmount"];
                tb_Ledger.CurrentRow["Customer"] = Row["CompanyID"];
                tb_Ledger.CurrentRow["Project"] = Row["ProjectID"];
                tb_Ledger.CurrentRow["Employee"] = Row["EmployeeID"];
                tb_Ledger.CurrentRow["Description"] = _Description;
                tb_Ledger.CurrentRow["Comments"] = Row["Remarks"];
                tb_Ledger.TableValidation.Validation(tb_Ledger.CurrentRow, CommandAction.Insert);
                if (tb_Ledger.ErrorCount > 0) { IsValidated = false; ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages); }
                else { VoucherRows.Add(tb_Ledger.CurrentRow); }
                #endregion
            }

            #endregion
            // Save Voucher.
            if (IsValidated)
            {
                foreach (DataRow Row in VoucherRows)
                {
                    var NoValidateAgain = false;
                    tb_Ledger.CurrentRow = Row;
                    tb_Ledger.Save(NoValidateAgain);
                    ErrorMessages.AddRange(tb_Ledger.ErrorMessages);
                }
                DataTableClass.Replace(UserName, Tables.SaleReturn, id, "Status", VoucherStatus.Posted);
                ErrorMessages.Add(SetMessage($"Voucher No {Vou_No}  has been posted sucessfully.", Color.Green));

            }
            else
            {
                ErrorMessages.Add(SetMessage($"Voucher No {Vou_No}  has not been posted sucessfully.", Color.Red));
            }


            return ErrorMessages;
        }
        #endregion

        #region Production
        public static List<Message> PostProduction(string UserName, int id)
        {
            var ErrorMessages = new List<Message>();
            var COA_DR = AppRegistry.GetNumber(UserName, "ProductOUT");
            var COA_CR = AppRegistry.GetNumber(UserName, "ProductIN");

            if (COA_DR > 0 && COA_CR > 0)                   // if Chart of account is valid.
            {
                var _Filter = $"[TranID]={id}";

                DataTableClass tb_Ledger = new(UserName, Tables.Ledger);
                List<DataRow> VoucherRows = new();
                DataTable Production = DataTableClass.GetTable(UserName, SQLQuery.View_Production(_Filter));

                decimal Tot_DR = Conversion.ToDecimal(Production.Compute("SUM(Amount)", "Flow='In'"));
                decimal Tot_CR = Conversion.ToDecimal(Production.Compute("SUM(Amount)", "Flow='Out'"));

                Tot_DR = Math.Round(Tot_DR, 2);
                Tot_CR = Math.Round(Tot_CR, 2);


                #region Validation
                if (Production == null)
                {
                    ErrorMessages.Add(SetMessage("Error: Production voucher is null here. Contact to Administrator"));
                    return ErrorMessages;
                }

                if (Production.Rows.Count == 0)
                {
                    ErrorMessages.Add(SetMessage("Error: Production voucher does't have any record to post. Contact to Administrator"));
                    return ErrorMessages;
                }

                if (Production.Rows[0]["Status"].ToString() == VoucherStatus.Posted.ToString())
                {
                    ErrorMessages.Add(SetMessage("Error: Production voucher is already posted. Contact to Administrator"));
                    return ErrorMessages;
                }

                if (!Tot_DR.Equals(Tot_CR))
                {
                    ErrorMessages.Add(SetMessage("Error: Production voucher amount is not equal. Contact to Administrator"));
                    return ErrorMessages;
                }

                #endregion

                var IsValidated = true;
                var SRNO = 1;
                var Vou_No = Production.Rows[0]["Vou_No"].ToString();
                var Vou_Type = VoucherType.Production;

                #region Check the voher is already exist in the ledger ? or not exist.
                tb_Ledger.MyDataView.RowFilter = $"Vou_No='{Vou_No}'";
                if (tb_Ledger.CountView > 0)
                {
                    ErrorMessages.Add(SetMessage("Voucher Numbre is already exist in the ledger. Contact to Administrator."));
                    return ErrorMessages;
                }
                #endregion


                #region Create Voucher

                foreach (DataRow Row in Production.Rows)
                {
                    IsValidated = true;         //  Default value.
                    if (Vou_No == Row["Vou_No"].ToString())
                    {
                        //var _Description = $"{Row["StockTitle"]}:{Row["Qty"]}/{Row["UOM"]} {Row["Flow"]}| {(string)Row["Remarks2"]} ";
                        var _Description = $"{Row["StockTitle"]}:{Row["Qty"]} | {(string)Row["Remarks2"]} ";
                        var _Amount = Math.Round(Conversion.ToDecimal(Row["Amount"]), 2);
                        #region Debit Entry
                        if ((string)Row["Flow"] == "Out")               // Stock Out from Productuon Process (Finish / Sami Finish Goods)
                        {
                            tb_Ledger.NewRecord();
                            tb_Ledger.CurrentRow["ID"] = 0;
                            tb_Ledger.CurrentRow["TranID"] = Row["TranID"];
                            tb_Ledger.CurrentRow["Vou_Type"] = Vou_Type;
                            tb_Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                            tb_Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                            tb_Ledger.CurrentRow["SR_No"] = SRNO; SRNO += 1;
                            tb_Ledger.CurrentRow["Ref_No"] = DBNull.Value;
                            tb_Ledger.CurrentRow["BookID"] = DBNull.Value;
                            tb_Ledger.CurrentRow["COA"] = COA_DR;
                            tb_Ledger.CurrentRow["DR"] = _Amount;
                            tb_Ledger.CurrentRow["CR"] = 0;
                            tb_Ledger.CurrentRow["Inventory"] = Row["Stock"];
                            tb_Ledger.CurrentRow["Customer"] = 0;
                            tb_Ledger.CurrentRow["Project"] = 0;
                            tb_Ledger.CurrentRow["Employee"] = 0;
                            tb_Ledger.CurrentRow["Description"] = _Description;
                            tb_Ledger.CurrentRow["Comments"] = Row["Comments"];
                            tb_Ledger.TableValidation.Validation(tb_Ledger.CurrentRow, CommandAction.Insert);
                            if (tb_Ledger.ErrorCount > 0) { IsValidated = false; ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages); }
                            else { VoucherRows.Add(tb_Ledger.CurrentRow); }
                        }
                        #endregion

                        #region Credit Entry
                        if ((string)Row["Flow"] == "In")               // Stock In to produce Finish / Sami Finish Goods
                        {
                            tb_Ledger.NewRecord();
                            tb_Ledger.CurrentRow["ID"] = 0;
                            tb_Ledger.CurrentRow["TranID"] = Row["TranID"];
                            tb_Ledger.CurrentRow["Vou_Type"] = Vou_Type;
                            tb_Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                            tb_Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                            tb_Ledger.CurrentRow["SR_No"] = SRNO; SRNO += 1;
                            tb_Ledger.CurrentRow["Ref_No"] = DBNull.Value;
                            tb_Ledger.CurrentRow["BookID"] = DBNull.Value;
                            tb_Ledger.CurrentRow["COA"] = COA_CR;
                            tb_Ledger.CurrentRow["DR"] = 0;
                            tb_Ledger.CurrentRow["CR"] = _Amount;
                            tb_Ledger.CurrentRow["Inventory"] = Row["Stock"];
                            tb_Ledger.CurrentRow["Customer"] = 0;
                            tb_Ledger.CurrentRow["Project"] = 0;
                            tb_Ledger.CurrentRow["Employee"] = 0;
                            tb_Ledger.CurrentRow["Description"] = _Description; 
                            tb_Ledger.CurrentRow["Comments"] = Row["Comments"];
                            tb_Ledger.TableValidation.Validation(tb_Ledger.CurrentRow, CommandAction.Insert);
                            if (tb_Ledger.ErrorCount > 0) { IsValidated = false; ErrorMessages.AddRange(tb_Ledger.TableValidation.MyMessages); }
                            else { VoucherRows.Add(tb_Ledger.CurrentRow); }
                        }
                        #endregion
                    }
                }

                #endregion

                // Save Voucher.
                #region Save Voucher into DB
                if (IsValidated)
                {
                    foreach (DataRow Row in VoucherRows)
                    {
                        var NoValidateAgain = false;
                        tb_Ledger.CurrentRow = Row;
                        tb_Ledger.Save(NoValidateAgain);
                        ErrorMessages.AddRange(tb_Ledger.ErrorMessages);
                    }
                    //DataTableClass.Replace(UserName, Tables.Production, id, "Status", VoucherStatus.Posted);
                    ErrorMessages.Add(SetMessage($"Voucher No {Vou_No}  has been posted sucessfully.", Color.Green));

                }
                else
                {
                    ErrorMessages.Add(SetMessage($"Voucher No {Vou_No}  has not been posted sucessfully.", Color.Red));
                }
                #endregion
            }
            else
            {
                ErrorMessages.Add(SetMessage("Chart of Accounrs are not assign. Go to Setting for assign them."));
            }

            return ErrorMessages;
        }
        #endregion


        #region Opening Balances Posting
        public static List<Message> PostOpeningBalance(string UserName)
        {
            List<Message> MyMessages = new List<Message>();
            int SrNo_Msg = 1;

            var _Filter = $"Vou_Type='{VoucherType.OBalance}'";

            DataTableClass tb_COA = new(UserName, Tables.COA);
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);

            int SRNO = 0;
            tb_Ledger.MyDataView.RowFilter = _Filter;
            //tb_Ledger.MyDataView.RowFilter = string.Concat("Vou_Type='" + VoucherType.OBalance.ToString(), "'");
            if (tb_Ledger.MyDataView.Count > 0)
            {
                SRNO = (int)(tb_Ledger.MyDataView.ToTable()).Compute("MAX(SR_No)", "");
            }

            DateTime Vou_Date = AppRegistry.GetDate(UserName, "OBDate");
            decimal _DR = 0.00M, _CR = 0.00M;

            tb_COA.MyDataView.RowFilter = "Opening_Balance <> 0";
            DataTable _Table = tb_COA.MyDataView.ToTable();
            foreach (DataRow Row in _Table.Rows)
            {
                if ((decimal)Row["Opening_Balance"] >= 0) { _DR = (decimal)Row["Opening_Balance"]; _CR = 0.00M; }
                else
                {
                    _CR = ((decimal)Row["Opening_Balance"]) * -1;
                    _DR = 0.00M;
                }
                tb_Ledger.MyDataView.RowFilter = $"Vou_Type='{VoucherType.OBalance}' AND COA={Row["ID"]}";
                if (tb_Ledger.MyDataView.Count == 1)
                {

                    tb_Ledger.CurrentRow = tb_Ledger.MyDataView[0].Row;
                    tb_Ledger.CurrentRow["Vou_Date"] = Vou_Date;
                    tb_Ledger.CurrentRow["DR"] = _DR;
                    tb_Ledger.CurrentRow["CR"] = _CR;
                    tb_Ledger.Save();
                    if (tb_Ledger.TableValidation.MyMessages.Count > 0)
                    {
                        MyMessages.AddRange(tb_Ledger.TableValidation.MyMessages);
                    }
                    else
                    {
                        string _Title = AppFunctions.GetTitle(UserName, Tables.COA, (int)Row["ID"]);
                        string msg = string.Concat("Update Account ID ", Row["Code"].ToString(), " : ", _Title, ", Amount ", ((decimal)Row["Opening_Balance"]).ToString(AppRegistry.FormatCurrency1));
                        MyMessages.Add(new Message { ErrorID = 2, Msg = msg, Success = true });
                        SrNo_Msg++;
                    }
                }
                else
                {
                    tb_Ledger.NewRecord();
                    tb_Ledger.CurrentRow["ID"] = 0;
                    tb_Ledger.CurrentRow["TranID"] = 1;
                    tb_Ledger.CurrentRow["Vou_Type"] = VoucherType.OBalance.ToString();
                    tb_Ledger.CurrentRow["Vou_Date"] = Vou_Date;
                    tb_Ledger.CurrentRow["Vou_No"] = "OBAL";
                    tb_Ledger.CurrentRow["SR_No"] = SRNO; SRNO += 1;
                    tb_Ledger.CurrentRow["Ref_No"] = "OBalance";
                    tb_Ledger.CurrentRow["BookID"] = 0;
                    tb_Ledger.CurrentRow["COA"] = (int)Row["ID"];
                    tb_Ledger.CurrentRow["DR"] = _DR;
                    tb_Ledger.CurrentRow["CR"] = _CR;
                    tb_Ledger.CurrentRow["Customer"] = DBNull.Value;
                    tb_Ledger.CurrentRow["Project"] = DBNull.Value;
                    tb_Ledger.CurrentRow["Employee"] = DBNull.Value;
                    tb_Ledger.CurrentRow["Description"] = string.Concat("Opening Balance as on ", Vou_Date.ToString("dd-MMM-yyyy"));
                    tb_Ledger.CurrentRow["Comments"] = DBNull.Value;
                    tb_Ledger.Save();
                    if (tb_Ledger.TableValidation.MyMessages.Count > 0)
                    {
                        MyMessages.AddRange(tb_Ledger.TableValidation.MyMessages);
                    }
                    else
                    {
                        string _Title = AppFunctions.GetTitle(UserName, Tables.COA, (int)Row["ID"]);
                        string msg = string.Concat("Insert Account ID ", Row["Code"].ToString(), " : ", _Title, ", Amount ", ((decimal)Row["Opening_Balance"]).ToString(AppRegistry.FormatCurrency1));
                        MyMessages.Add(new Message { ErrorID = 1, Msg = msg, Success = true });
                        SrNo_Msg++;
                    }
                }
            }

            // Delete if Account has zero and entry exist in General Ledger

            tb_Ledger.MyDataView.RowFilter = string.Concat("Vou_Type='" + VoucherType.OBalance.ToString(), "'");
            foreach (DataRow Row in tb_Ledger.MyDataView.ToTable().Rows)
            {
                if (tb_COA.Seek((int)Row["COA"]))
                {
                    tb_COA.SeekRecord((int)Row["COA"]);
                    if ((decimal)tb_COA.CurrentRow["Opening_Balance"] == 0)
                    {
                        tb_Ledger.SeekRecord((int)Row["ID"]);
                        tb_Ledger.Delete();
                        string _Title = AppFunctions.GetTitle(UserName, Tables.COA, (int)Row["COA"]);
                        string _Code = AppFunctions.GetColumnValue(UserName, Tables.COA, "Code", (int)Row["COA"]);
                        decimal Amount = (decimal)Row["DR"] - (decimal)Row["CR"];

                        string msg = string.Concat("Delete Account ID ", _Code, " : ", _Title, ", Amount ", (Amount).ToString(AppRegistry.FormatCurrency1));
                        MyMessages.Add(new Message { ErrorID = 3, Msg = msg, Success = true });
                    }
                }
            }
            return MyMessages;
        }

        public static List<Message> PostOpeningBalanceCompany(string UserName)
        {
            List<Message> MyMessages = new List<Message>();

            var Currency = AppRegistry.GetText(UserName, "CurrencySign");
            var CurrencyFormat = AppRegistry.GetText(UserName, "FMTCurrency");
            var COA_DR = AppRegistry.GetNumber(UserName, "OBCompanyDR");
            var COA_CR = AppRegistry.GetNumber(UserName, "OBCompanyCR");
            var OBCOA = 0;                                                                                                  // COA for DR or CR subject to O Bal Amount.

            if (COA_DR == 0 || COA_CR == 0)
            {
                MyMessages.Add(SetMessage("Chart of Accounts is not valid to post company balances."));
                return MyMessages;
            }

            DataTableClass OBALCompany = new(UserName, SQLQuery.OBALCompany());
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger, "Vou_Type='OBalCom'");

            #region Delete Ledger Records, if not exist in Opening Balance Data Table.
            var Records = 0;
            foreach (DataRow Row in tb_Ledger.Rows)
            {
                if ((string)Row["Vou_Type"] != "OBalCom") { continue; }

                OBALCompany.MyDataView.RowFilter = $"Company={Row["Customer"]}";
                if (OBALCompany.MyDataView.Count > 0) { continue; }
                else
                {
                    tb_Ledger.SeekRecord((int)Row["ID"]);
                    tb_Ledger.Delete();
                    Records++;
                    var CustomerName = AppFunctions.GetTitle(UserName, Tables.Customers, (int)Row["Customer"]);
                    var CustomerAmount = ((decimal)Row["DR"] - (decimal)Row["CR"]);
                    MyMessages.Add(SetMessage($" {Records}: {CustomerName} of {Currency} {CustomerAmount} Transaction DELETED.", ConsoleColor.Red));
                }
            }
            MyMessages.Add(SetMessage($"Total {Records} record(s) deleted."));
            #endregion

            if (OBALCompany.Count == 0)
            {
                MyMessages.Add(SetMessage("No Record Found."));
                return MyMessages;
            }

            DateTime Vou_Date = (DateTime)AppRegistry.GetKey(UserName, "OBDate", KeyType.Date);
            decimal _DR, _CR; List<DataRow> Voucher;

            Records = 0;
            foreach (DataRow Row in OBALCompany.Rows)
            {
                #region Voucher Setup
                var TranID = Row["ID"];
                Voucher = new();
                if ((decimal)Row["Amount"] >= 0) { _DR = (decimal)Row["Amount"]; _CR = 0.00M; }
                else
                {
                    _CR = ((decimal)Row["Amount"]) * -1;
                    _DR = 0.00M;
                }

                string _Filter = string.Format($"Vou_Type='{VoucherType.OBalCom}' AND TranID={Row["ID"]}");
                tb_Ledger.MyDataView.RowFilter = _Filter;
                if (tb_Ledger.Count == 2)
                {
                    Voucher.Add(tb_Ledger.MyDataView[0].Row);
                    Voucher.Add(tb_Ledger.MyDataView[1].Row);
                }
                else
                {
                    Voucher.Add(tb_Ledger.NewRecord());
                    Voucher.Add(tb_Ledger.NewRecord());

                    Voucher[0]["ID"] = 0;
                    Voucher[1]["ID"] = 0;
                    Voucher[0]["TranID"] = (int)TranID;
                    Voucher[1]["TranID"] = (int)TranID;
                }
                #endregion

                #region Voucher Transactions
                Voucher[0]["Vou_Type"] = VoucherType.OBalCom.ToString();
                Voucher[0]["Vou_Date"] = Vou_Date;
                Voucher[0]["Vou_No"] = VoucherType.OBalCom.ToString();
                Voucher[0]["SR_No"] = 1;
                Voucher[0]["Ref_No"] = "OBalance";
                Voucher[0]["BookID"] = 0;
                Voucher[0]["COA"] = (int)Row["COA"];
                Voucher[0]["DR"] = _DR;
                Voucher[0]["CR"] = _CR;
                Voucher[0]["Customer"] = Row["Company"];
                Voucher[0]["Project"] = Row["Project"];
                Voucher[0]["Employee"] = Row["Employee"];
                Voucher[0]["Description"] = string.Concat("Company Opening Balance as on ", Vou_Date.ToString("dd-MMM-yyyy"));
                Voucher[0]["Comments"] = DBNull.Value;

                if (_DR == 0) { OBCOA = COA_DR; }           // Assign DR COA if Debit amount is zero
                if (_CR == 0) { OBCOA = COA_CR; }            // Assign CR COA if Credit amount is zero

                Voucher[1]["Vou_Type"] = VoucherType.OBalCom.ToString();
                Voucher[1]["Vou_Date"] = Vou_Date;
                Voucher[1]["Vou_No"] = VoucherType.OBalCom.ToString();
                Voucher[1]["SR_No"] = 2;
                Voucher[1]["Ref_No"] = "OBalance";
                Voucher[1]["BookID"] = 0;
                Voucher[1]["COA"] = OBCOA;
                Voucher[1]["DR"] = _CR;                                           // DR equal of CR of first entry
                Voucher[1]["CR"] = _DR;                                           // CR equal of DR of first entry
                Voucher[1]["Customer"] = Row["Company"];
                Voucher[1]["Project"] = Row["Project"];
                Voucher[1]["Employee"] = Row["Employee"];
                Voucher[1]["Description"] = string.Concat("Company Opening Balance as on ", Vou_Date.ToString("dd-MMM-yyyy"));
                Voucher[1]["Comments"] = DBNull.Value;
                #endregion

                #region Save Voucher

                // Validation of Voucher;
                bool IsValidated1 = tb_Ledger.TableValidation.Validation(Voucher[0]);
                bool IsValidated2 = tb_Ledger.TableValidation.Validation(Voucher[1]);

                if (IsValidated1 && IsValidated2)
                {
                    tb_Ledger.CurrentRow = Voucher[0]; tb_Ledger.Save(); MyMessages.AddRange(tb_Ledger.TableValidation.MyMessages);
                    tb_Ledger.CurrentRow = Voucher[1]; tb_Ledger.Save(); MyMessages.AddRange(tb_Ledger.TableValidation.MyMessages);
                    Records++;
                    MyMessages.Add(SetMessage($" {Records}: {Row["CompanyName"]} of {Currency} {_DR.ToString(CurrencyFormat)} voucher posted.", ConsoleColor.Green));
                }
                #endregion
            }
            MyMessages.Add(SetMessage($"Total {Records} records posted."));
            return MyMessages;
        }

        public static List<Message> PostOpeningBalanceStock(string UserName)
        {
            List<Message> MyMessages = new();

            DataTableClass OBALStock = new(UserName, Tables.OBALStock);
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);

            if (OBALStock.Count == 0)
            {
                MyMessages.Add(SetMessage("No Record Found."));
                return MyMessages;
            }

            DateTime Vou_Date = (DateTime)AppRegistry.GetKey(UserName, "OBDate", KeyType.Date);
            int OBStock_DR = (int)AppRegistry.GetKey(UserName, "OBStockDR", KeyType.Number);
            int OBStock_CR = (int)AppRegistry.GetKey(UserName, "OBStockCR", KeyType.Number);
            decimal _DR, _CR; List<DataRow> Voucher;

            foreach (DataRow Row in OBALStock.Rows)
            {
                Voucher = new();
                if ((decimal)Row["Amount"] >= 0) { _DR = (decimal)Row["Amount"]; _CR = 0.00M; }
                else
                {
                    _CR = ((decimal)Row["Amount"]) * -1;
                    _DR = 0.00M;
                }

                string _Filter = string.Format("Vou_Type='{0}' AND TranID={1}", VoucherType.OBalStock.ToString(), Row["ID"].ToString());
                tb_Ledger.MyDataView.RowFilter = _Filter;
                if (tb_Ledger.Count == 2)
                {
                    Voucher.Add(tb_Ledger.MyDataView[0].Row);
                    Voucher.Add(tb_Ledger.MyDataView[1].Row);

                    Voucher[0]["COA"] = OBStock_DR;
                    Voucher[1]["COA"] = OBStock_CR;


                }
                else
                {
                    tb_Ledger.NewRecord();
                    tb_Ledger.CurrentRow["ID"] = 0;
                    tb_Ledger.CurrentRow["TranID"] = Row["ID"];
                    tb_Ledger.CurrentRow["Vou_Type"] = VoucherType.OBalStock.ToString();
                    tb_Ledger.CurrentRow["Vou_Date"] = Vou_Date;
                    tb_Ledger.CurrentRow["Vou_No"] = VoucherType.OBalStock.ToString();
                    tb_Ledger.CurrentRow["SR_No"] = 1;
                    tb_Ledger.CurrentRow["Ref_No"] = Row["Batch"];
                    tb_Ledger.CurrentRow["BookID"] = 0;
                    tb_Ledger.CurrentRow["COA"] = OBStock_CR;
                    tb_Ledger.CurrentRow["DR"] = _DR;
                    tb_Ledger.CurrentRow["CR"] = _CR;
                    tb_Ledger.CurrentRow["Customer"] = 0;
                    tb_Ledger.CurrentRow["Project"] = Row["Project"];
                    tb_Ledger.CurrentRow["Employee"] = 0;
                    tb_Ledger.CurrentRow["Inventory"] = Row["Inventory"];
                    tb_Ledger.CurrentRow["Description"] = string.Concat("Opening Balance as on ", Vou_Date.ToString("dd-MMM-yyyy"));
                    tb_Ledger.CurrentRow["Comments"] = DBNull.Value;
                    Voucher.Add(tb_Ledger.CurrentRow);

                    tb_Ledger.NewRecord();
                    tb_Ledger.CurrentRow["ID"] = 0;
                    tb_Ledger.CurrentRow["TranID"] = Row["ID"];
                    tb_Ledger.CurrentRow["Vou_Type"] = VoucherType.OBalStock.ToString();
                    tb_Ledger.CurrentRow["Vou_Date"] = Vou_Date;
                    tb_Ledger.CurrentRow["Vou_No"] = VoucherType.OBalStock.ToString();
                    tb_Ledger.CurrentRow["SR_No"] = 2;
                    tb_Ledger.CurrentRow["Ref_No"] = Row["Batch"];
                    tb_Ledger.CurrentRow["BookID"] = 0;
                    tb_Ledger.CurrentRow["COA"] = OBStock_CR;
                    tb_Ledger.CurrentRow["DR"] = _CR;                                           // DR equal of CR of first entry
                    tb_Ledger.CurrentRow["CR"] = _DR;                                           // CR equal of DR of first entry
                    tb_Ledger.CurrentRow["Customer"] = 0;
                    tb_Ledger.CurrentRow["Project"] = Row["Project"];
                    tb_Ledger.CurrentRow["Employee"] = 0;
                    tb_Ledger.CurrentRow["Inventory"] = Row["Inventory"];
                    tb_Ledger.CurrentRow["Description"] = string.Concat("Opening Balance as on ", Vou_Date.ToString("dd-MMM-yyyy"));
                    tb_Ledger.CurrentRow["Comments"] = DBNull.Value;
                    Voucher.Add(tb_Ledger.CurrentRow);
                }
                // Validation of Voucher;
                bool IsValidated1 = tb_Ledger.TableValidation.Validation(Voucher[0]);
                bool IsValidated2 = tb_Ledger.TableValidation.Validation(Voucher[1]);

                if (IsValidated1 && IsValidated2)
                {
                    List<Message> _messages = new List<Message>();
                    tb_Ledger.CurrentRow = Voucher[0]; tb_Ledger.Save(); _messages.AddRange(tb_Ledger.ErrorMessages);
                    tb_Ledger.CurrentRow = Voucher[1]; tb_Ledger.Save(); _messages.AddRange(tb_Ledger.ErrorMessages);
                    var StockTitle = AppFunctions.GetTitle(UserName, Tables.Inventory, (int)Voucher[0]["Inventory"]);
                    if (_messages.Count == 0)
                    {

                        MyMessages.Add(MessageClass.SetMessage($"{StockTitle}: Opening Balance saved sucessfully.", Color.Green));
                    }
                    else
                    {
                        MyMessages.Add(MessageClass.SetMessage($"{StockTitle}: Opening Balance NOT saved.", Color.Red));
                    }
                }
                else
                {
                    MyMessages.Add(MessageClass.SetMessage($"Stock Opening Transaction {Voucher[0]["ID"]} not saved."));
                }
            }
            return MyMessages;
        }
        #endregion



    }
}
