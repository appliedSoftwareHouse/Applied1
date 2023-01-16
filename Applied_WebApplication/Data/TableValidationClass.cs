using System.Data;
using Applied_WebApplication.Data;

namespace Applied_WebApplication.Data
{
    public class TableValidationClass
    {
        public string SQLAction { get; set; }
        public DataTable MyDataTable { get; set; }
        public List<Message> MyMessages = new List<Message>();

        public TableValidationClass()
        {
            MyDataTable = new DataTable();
            MyMessages = new List<Message>();
        }

        public class Message
        {
            public bool Success { get; set; } = false;
            public string Msg { get; set; } = string.Empty;
            public int ErrorID { get; set; } = 0;
        }

        public bool Success()
        {
            if (MyMessages.Count > 0)
            { return true; }
            else
            { return false; }
        }

        public bool Validation(DataRow Row)
        {
            MyMessages = new List<Message>();
            if (Row == null)
            {
                MyMessages.Add(new Message() { Success = false, ErrorID = 10, Msg = "Current row is null" });
                return false;
            }                                      // Return false if row is null

            if (SQLAction == null)
            {
                MyMessages.Add(new Message() { Success = false, ErrorID = 10, Msg = "Database query action is not defined." });
                return false;
            }
            if (Row.Table.TableName == Tables.COA.ToString()) { ValidateTable_COA(Row); }
            if (Row.Table.TableName == Tables.Customers.ToString()) { ValidateTable_Customer(Row); }
            if (Row.Table.TableName == Tables.CashBook.ToString()) { ValidateTable_CashBook(Row); }
            if (Row.Table.TableName == Tables.WriteCheques.ToString()) { ValidateTable_WriteChq(Row); }
            if (MyMessages.Count > 0) { return false; } else { return true; }
        }



        private void ValidateTable_COA(DataRow Row)
        {
            MyMessages = new List<Message>();
            if (SQLAction == CommandAction.Insert.ToString())
            {
                if (Row["Code"].ToString().Trim().Length != 6)                  // Code
                {

                    MyMessages.Add(new Message() { Success = false, ErrorID = 101, Msg = "Code Length must be 6 Character" });
                }

                if (Row["Title"].ToString().Trim().Length == 0)             // Title
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 102, Msg = "Title Length can not be null" });
                }
            }
            if (SQLAction == CommandAction.Update.ToString())
            {
                if (Row["Code"].ToString().Trim().Length != 6)                  // Code
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 101, Msg = "Code Length must be 6 Character" });
                }

                if (Row["Title"].ToString().Trim().Length == 0)             // Title
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 102, Msg = "Title Length can not be null" });
                }
            }
        }
        private void ValidateTable_Customer(DataRow Row)
        {
            MyMessages = new List<Message>();

            #region Insert
            if (SQLAction == CommandAction.Insert.ToString())
            {
                if (Row["Code"] == DBNull.Value)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 103, Msg = "Null value of Code is not allowed." });
                }

                if (Seek("Code", Row["Code"].ToString()))
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 104, Msg = "Code is already assigned. Duplicate value not allowed." });
                }

                if (Row["Title"] == DBNull.Value)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 105, Msg = "Null value of Code is not allowed." });
                }


                if (Seek("Title", Row["Title"].ToString()))
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 106, Msg = "Title is already assigned. duplicate value not allowed." });
                }
            }
            #endregion

            #region Update
            if (SQLAction == CommandAction.Delete.ToString())
            {


            }
            #endregion
        }
        private void ValidateTable_CashBook(DataRow Row)
        {
            MyMessages = new List<Message>();

            if (SQLAction == CommandAction.Insert.ToString())
            {
                if (Seek("ID", Row["ID"].ToString()))
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10501, Msg = "Duplicate of ID found." });
                }

                if (Row["Vou_No"].ToString().Length == 0)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10507, Msg = "Enter Valid Voucher Number." });
                }

                if (Seek("Vou_No", Row["Vou_No"].ToString()))
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10502, Msg = "Duplicate of Voucher No found." });
                }

                if ((int)Row["COA"] == 0)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10505, Msg = "Accounts Head must be selected." });
                }

                if ((decimal)Row["DR"] > 0 && (decimal)Row["CR"] > 0)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10503, Msg = "Only Debit or Credit must be more more than Zero." });
                }

                if ((decimal)Row["DR"] == 0 && (decimal)Row["CR"] == 0)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10504, Msg = "Must be enter Debit or Credit Amount" });
                }


                if (Row["Description"].ToString().Length == 0)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10506, Msg = "Description must be some charactors." });
                }
            }
        }
        private void ValidateTable_WriteChq(DataRow Row)
        {
            MyMessages = new List<Message>();
            //if (SQLAction == CommandAction.Insert.ToString())
            //{
            //    if (Seek("ID", Row["ID"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 10801, Msg = "Duplicate of ID found." }); }
            //}
            //if (SQLAction == CommandAction.Update.ToString())
            //{
            //    if (!Seek("ID", Row["ID"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 10802, Msg = "Cheque Reccord not found." }); }
            //}

            //if (Row["Code"].ToString() == "New Code") { MyMessages.Add(new Message() { Success = false, ErrorID = 10803, Msg = "Cheque Transaction code is not assigned." }); }
            //if ((int)Row["TranType"] == 1)
            //{
            //    if ((decimal)Row["ChqAmount"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 10802, Msg = "Cheque amount can not be null" }); }
            //}
            //if ((int)Row["Bank"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 10803, Msg = "Bank must be selected." }); }
            //if ((int)Row["Company"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 10804, Msg = "Bank must be selected." }); }
            //if (Row["Status"].ToString() == "") { MyMessages.Add(new Message() { Success = false, ErrorID = 1080, Msg = "Cheque Status must be selected." }); }
        }

        private bool Seek(string _Column, string _Value)
        {
            if (MyDataTable != null)
            {
                DataView _DataView = MyDataTable.AsDataView();
                _DataView.RowFilter = _Column + "='" + _Value + "'";
                if (_DataView.Count > 0) { return true; } else { return false; }
            }
            return false;               // Default value    
        }
        //public static List<Message> Validate(DataRow Row, CommandAction Action)
        //{
        //    TableValidationClass MyValidation = new();
        //    _ = MyValidation.MyMessages;
        //    MyValidation.SQLAction = Action.ToString();
        //    MyValidation.Validation(Row);
        //    List<Message> MyMessages = MyValidation.MyMessages;
        //    return MyMessages;
        //}
    }
}