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

            tb_Ledger.MyDataView.RowFilter = string.Concat("TranID=", id.ToString(), " AND VouType='Cash' ");
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
                if(tb_Ledger.IsRowValid(tb_Ledger.CurrentRow, CommandAction.Insert, PostType.Bankbook))
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

        internal static void PostBillPayable(string UserName, int id)
        {
            bool Result;
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);
            List<Message> ErrorMessages = new List<Message>();
            List<DataRow> VoucherRows = new();
            
            tb_Ledger.MyDataView.RowFilter = string.Concat("TranID=", id.ToString(), " AND VouType='BillPY' ");             // Filter Record for check? Already exist or not.
            if(tb_Ledger.MyDataView.Count==0)
            {
                DataTableClass fun_BillPayable = new(UserName, Tables.fun_BillPayableEntry);                                    // Get SQLite View for Entry
                fun_BillPayable.MyDataView.RowFilter = string.Concat("TranID=", id.ToString());
                foreach(DataRow Row in fun_BillPayable.MyDataView.ToTable().Rows)
                {
                    DataRow LedgerRow = tb_Ledger.NewRecord();


                    //VoucherRows.(LedgerRow);
                }
            }


        }
    }
}
