using System.Data;

namespace Applied_WebApplication.Data
{
    public class PostingClass
    {
        public static bool PostCashBook(string UserName, int id)
        {
            bool Result;
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);
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
                if (tb_Ledger.IsRowValid(tb_Ledger.CurrentRow, CommandAction.Insert, PostType.Bankbook))
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
                else { _CR = ((decimal)Row["Opening_Balance"]) * -1; 
                    _DR = 0.00M; }
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
                        string _Code = AppFunctions.GetColumnValue(UserName, Tables.COA,"Code", (int)Row["COA"]);
                        decimal Amount = (decimal)Row["DR"] - (decimal)Row["CR"];

                        string msg = string.Concat("Delete Account ID ", _Code, " : ", _Title, ", Amount ", (Amount).ToString(AppRegistry.FormatCurrency1));
                        MyMessages.Add(new Message { ErrorID = 3, Msg = msg, Success = true });
                    }
                }
            }
            return MyMessages;
        }
    }
}
