using Applied_WebApplication.Pages;
using System.Data;
using System.Data.SQLite;
using System.Drawing;

namespace Applied_WebApplication.Data
{
    public class PostingClass
    {
        #region Cash Book

        public static bool PostCashBook(string UserName, int id)
        {
            bool Result;
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger, "");
            List<Message> ErrorMessages = new List<Message>();
            List<DataRow> VoucherRows = new();

            tb_Ledger.MyDataView.RowFilter = string.Concat("TranID=", id.ToString(), " AND Vou_Type='", VoucherType.Cash.ToString(), "'");
            if (tb_Ledger.MyDataView.Count == 0)
            {
                DataRow Row = AppFunctions.GetDataRow(UserName, Tables.CashBook, id);
                tb_Ledger.NewRecord();                                                                                                 // Cash Book DR Entry
                tb_Ledger.CurrentRow["ID"] = 0;
                tb_Ledger.CurrentRow["TranID"] = id;
                tb_Ledger.CurrentRow["Vou_Type"] = "Cash";
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
                tb_Ledger.CurrentRow["Vou_Type"] = "Cash";
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
                Result = true;
                tb_Ledger.CurrentRow = VoucherRows[0];                  // DR Transaction
                tb_Ledger.Save();
                tb_Ledger.CurrentRow = VoucherRows[1];                  // Credit Transaction.
                tb_Ledger.Save();
                DataTableClass.Replace(UserName, Tables.CashBook, id, "Status", VoucherStatus.Posted.ToString());
            }
            else
            {
                Result = false;
            }
            return Result;
        }
        #endregion

        #region Bill Payable
        public static List<Message> PostBillPayable(string UserName, int id)
        {
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);
            List<Message> ErrorMessages = new List<Message>();
            List<DataRow> VoucherRows = new();
            DataRow RowBill1 = AppFunctions.GetRecord(UserName, Tables.BillPayable, id);

            int COA_Purchase = 7;                   // COA: Purchsase on Credit 
            int COA_Payable = 32;                   // COA: Trade Payable.

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
                    tb_Ledger.CurrentRow["DR"] = Row["Amount"];                                                                    // DR => CR
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
                        tb_Ledger.CurrentRow["COA"] = AppFunctions.GetTaxCOA(UserName, (int)Row["TaxID"]);
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
                    tb_Ledger.CurrentRow["CR"] = Row["Total"];
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

            if (SaleInvoice == null)
            {
                ErrorMessages.Add(MessageClass.SetMessage("Error: Sale invocie object is null here. Contact to Administrator"));
                return ErrorMessages;
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

            int COA_DR = 4;
            int COA_CR = 43;
            int COA_Tax = 33;
            bool IsValidated = true;
            int SRNO = 1;
            string Vou_No = SaleInvoice.Rows[0]["Vou_No"].ToString();

            #region Check the voher is already exist in the ledger ? or not exist.
            tb_Ledger.MyDataView.RowFilter = $"Vou_No='{Vou_No}'";
            if (tb_Ledger.Count > 0)
            {
                ErrorMessages.Add(MessageClass.SetMessage("Voucher Numbre is already exist in the ledger. Contact to Administrator."));
                return ErrorMessages;
            }
            #endregion

            foreach (DataRow Row in SaleInvoice.Rows)
            {
                Vou_No = Row["Vou_No"].ToString();
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
                tb_Ledger.CurrentRow["DR"] = 0;
                tb_Ledger.CurrentRow["CR"] = Row["Amount"];
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
                    var NoValidteAgain = false;
                    tb_Ledger.CurrentRow = Row;
                    tb_Ledger.Save(NoValidteAgain);
                    ErrorMessages.AddRange(tb_Ledger.ErrorMessages);
                }
                DataTableClass.Replace(UserName, Tables.BillReceivable, id, "Status", VoucherStatus.Posted);
                ErrorMessages.Add(MessageClass.SetMessage($"Voucher No {Vou_No}  has been posted sucessfully.", Color.Green));
                
            }
            else
            {
                ErrorMessages.Add(MessageClass.SetMessage($"Voucher No {Vou_No}  has not been posted sucessfully.", Color.Red));
            }

            return ErrorMessages;
        }
        #endregion

        #region Opening Balances Posting
        public static List<Message> PostOpeningBalance(string UserName)
        {
            List<Message> MyMessages = new List<Message>();
            int SrNo_Msg = 1;

            DataTableClass tb_COA = new(UserName, Tables.COA);
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);

            int SRNO = 0;
            tb_Ledger.MyDataView.RowFilter = string.Concat("Vou_Type='" + VoucherType.OBalance.ToString(), "'");
            if (tb_Ledger.MyDataView.Count > 0)
            {
                SRNO = (int)(tb_Ledger.MyDataView.ToTable()).Compute("MAX(SR_No)", "");
            }

            DateTime Vou_Date = (DateTime)AppRegistry.GetKey(UserName, "OBal_Date", KeyType.Date);
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
                tb_Ledger.MyDataView.RowFilter = string.Concat("Vou_Type='", VoucherType.OBalance.ToString(), "' AND COA=", Row["ID"].ToString());
                if (tb_Ledger.MyDataView.Count == 1)
                {
                    tb_Ledger.CurrentRow = tb_Ledger.MyDataView[0].Row;
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

            DataTableClass OBALCompany = new(UserName, Tables.OBALCompany);
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);

            if (OBALCompany.Count == 0)
            {
                MyMessages.Add(MessageClass.SetMessage("No Record Found."));
                return MyMessages;
            }

            DateTime Vou_Date = (DateTime)AppRegistry.GetKey(UserName, "OBDate", KeyType.Date);
            int OBCom = (int)AppRegistry.GetKey(UserName, "OBCompany", KeyType.Number);
            decimal _DR, _CR; List<DataRow> Voucher;

            foreach (DataRow Row in OBALCompany.Rows)
            {
                var TranID = Row["ID"];
                Voucher = new();
                if ((decimal)Row["Amount"] >= 0) { _DR = (decimal)Row["Amount"]; _CR = 0.00M; }
                else
                {
                    _CR = ((decimal)Row["Amount"]) * -1;
                    _DR = 0.00M;
                }

                string _Filter = string.Format("Vou_Type='{0}' AND TranID={1}", VoucherType.OBalCom.ToString(), Row["ID"].ToString());
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

                //Voucher[0]["ID"] = Row["ID"];
                //Voucher[0]["TranID"] = Row["TranID"];
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
                Voucher[0]["Description"] = string.Concat("Stock Opening Balance as on ", Vou_Date.ToString("dd-MMM-yyyy"));
                Voucher[0]["Comments"] = DBNull.Value;

                //Voucher[1]["ID"] = Row["ID"];
                //Voucher[1]["TranID"] = Row["TranID"];
                Voucher[1]["Vou_Type"] = VoucherType.OBalCom.ToString();
                Voucher[1]["Vou_Date"] = Vou_Date;
                Voucher[1]["Vou_No"] = VoucherType.OBalCom.ToString();
                Voucher[1]["SR_No"] = 2;
                Voucher[1]["Ref_No"] = "OBalance";
                Voucher[1]["BookID"] = 0;
                Voucher[1]["COA"] = OBCom;
                Voucher[1]["DR"] = _CR;                                           // DR equal of CR of first entry
                Voucher[1]["CR"] = _DR;                                           // CR equal of DR of first entry
                Voucher[1]["Customer"] = Row["Company"];
                Voucher[1]["Project"] = Row["Project"];
                Voucher[1]["Employee"] = Row["Employee"];
                Voucher[1]["Description"] = string.Concat("Stock Opening Balance as on ", Vou_Date.ToString("dd-MMM-yyyy"));
                Voucher[1]["Comments"] = DBNull.Value;


                // Validation of Voucher;
                bool IsValidated1 = tb_Ledger.TableValidation.Validation(Voucher[0]);
                bool IsValidated2 = tb_Ledger.TableValidation.Validation(Voucher[1]);

                if (IsValidated1 && IsValidated2)
                {
                    tb_Ledger.CurrentRow = Voucher[0]; tb_Ledger.Save(); MyMessages.AddRange(tb_Ledger.TableValidation.MyMessages);
                    tb_Ledger.CurrentRow = Voucher[1]; tb_Ledger.Save(); MyMessages.AddRange(tb_Ledger.TableValidation.MyMessages);
                }
            }
            return MyMessages;
        }

        public static List<Message> PostOpeningBalanceStock(string UserName)
        {
            List<Message> MyMessages = new();

            DataTableClass OBALStock = new(UserName, Tables.OBALStock);
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);

            if (OBALStock.Count == 0)
            {
                MyMessages.Add(MessageClass.SetMessage("No Record Found."));
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
