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

        internal static List<Message> PostBillPayable(string UserName, int id)
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
    }
}
