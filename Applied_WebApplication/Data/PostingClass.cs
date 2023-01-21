using System.Data;

namespace Applied_WebApplication.Data
{
    public class PostingClass
    {
        public static bool PostBashBook(string UserName, int id)
        {
            var Result = true;
            DataTableClass Ledger = new(UserName, Tables.Ledger);
            Ledger.MyDataView.RowFilter = string.Concat("TranID=", id.ToString());

            if(Ledger.MyDataView.Count==0)
            {
                List<Message> ErrorMessages = new List<Message>();
                DataRow Row = AppFunctions.GetDataRow(UserName, Tables.CashBook, id);

                Ledger.NewRecord();                                                                                                 // Cash Book DR Entry
                Ledger.CurrentRow["ID"] = 0;
                Ledger.CurrentRow["TranID"] = id;
                Ledger.CurrentRow["Vou_Type"] = "Cash";
                Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                Ledger.CurrentRow["SRNo"] = 1;
                Ledger.CurrentRow["Ref_No"] = Row["Ref_No"];
                Ledger.CurrentRow["BookID"] = Row["BookID"];
                Ledger.CurrentRow["COA"] = Row["COA"];
                Ledger.CurrentRow["DR"] = Row["DR"];
                Ledger.CurrentRow["CR"] = Row["CR"];
                Ledger.CurrentRow["Customer"] = Row["Customer"];
                Ledger.CurrentRow["Project"] = Row["Project"];
                Ledger.CurrentRow["Employee"] = Row["Employee"];
                Ledger.CurrentRow["Description"] = Row["Description"];
                Ledger.CurrentRow["Comments"] = Row["Comments"];
                Ledger.Save();
                ErrorMessages.AddRange(Ledger.TableValidation.MyMessages);                          // Collect Error Messages id occure.


                Ledger.NewRecord();                                                                                                  // Cash Book CR Entry
                Ledger.CurrentRow["ID"] = 0;
                Ledger.CurrentRow["TranID"] = id;
                Ledger.CurrentRow["Vou_Type"] = "Cash";
                Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                Ledger.CurrentRow["SRNo"] = 1;
                Ledger.CurrentRow["Ref_No"] = Row["Ref_No"];
                Ledger.CurrentRow["BookID"] = Row["BookID"];
                Ledger.CurrentRow["COA"] = Row["BookID"];                                                           // COA => Book ID
                Ledger.CurrentRow["DR"] = Row["CR"];                                                                    // DR => CR
                Ledger.CurrentRow["CR"] = Row["DR"];                                                                    // CR => DR
                Ledger.CurrentRow["Customer"] = Row["Customer"];
                Ledger.CurrentRow["Project"] = Row["Project"];
                Ledger.CurrentRow["Employee"] = Row["Employee"];
                Ledger.CurrentRow["Description"] = Row["Description"];
                Ledger.CurrentRow["Comments"] = Row["Comments"];
                Ledger.Save();
                ErrorMessages.AddRange(Ledger.TableValidation.MyMessages);                          // Collect Error Messages id occure.
                if(ErrorMessages.Count>0)
                {
                    Result = false;
                }
            }
            return Result;
        }
    }
}
