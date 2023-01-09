using System.Data;

namespace Applied_WebApplication.Data
{
    public class TableValidationClass
    {
        public string SQLAction { get; set; }
        public DataTable MyDataTable { get; set; }
        public List<Message> MyMessages = new List<Message>();

        public TableValidationClass()
        {
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
                Message MyMessage = new Message() { success = false, ErrorID = 10, message = "Current row is null" };
                return false;
            }                                              // Return false if row is null

            if (SQLAction == null)
            {
                Message MyMessage = new Message() { success = false, ErrorID = 10, message = "Current row is null" };
                return false;
            }                                     // Return is SQL Command Action is not define.
            //================================================================================================== 

            #region Table COA
            if (Row.Table.TableName == Tables.COA.ToString())
            {
                MyMessages = new List<Message>();

                if (SQLAction == CommandAction.Insert.ToString())
                {
                    if (Row["Code"].ToString().Trim().Length != 6)                  // Code
                    {

                        MyMessages.Add(new Message() { success = false, ErrorID = 101, message = "Code Length must be 6 Character" });
                    }

                    if (Row["Title"].ToString().Trim().Length == 0)             // Title
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 102, message = "Title Length can not be null" });
                    }
                }

                if (SQLAction == CommandAction.Update.ToString())
                {
                    if (Row["Code"].ToString().Trim().Length != 6)                  // Code
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 101, message = "Code Length must be 6 Character" });
                    }

                    if (Row["Title"].ToString().Trim().Length == 0)             // Title
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 102, message = "Title Length can not be null" });
                    }
                }
            }
            #endregion

            #region Customer
            if (Row.Table.TableName == Tables.Customers.ToString())
            {
                MyMessages = new List<Message>();

                #region Insert
                if (SQLAction == CommandAction.Insert.ToString())
                {
                    if (Row["Code"] == DBNull.Value)
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 103, message = "Null value of Code is not allowed." });
                    }

                    if (Seek("Code", Row["Code"].ToString()))
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 104, message = "Code is already assigned. Duplicate value not allowed." });
                    }

                    if (Row["Title"] == DBNull.Value)
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 105, message = "Null value of Code is not allowed." });
                    }


                    if (Seek("Title", Row["Title"].ToString()))
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 106, message = "Title is already assigned. duplicate value not allowed." });
                    }
                }
                #endregion

                #region Update
                if (SQLAction == CommandAction.Delete.ToString())
                {


                }
                #endregion
            }
            #endregion

            #region CashBook
            if (Row.Table.TableName == Tables.CashBook.ToString())
            {
                MyMessages = new List<Message>();
                #region Insert
                if (SQLAction == CommandAction.Insert.ToString())
                {
                    if (Seek("ID", Row["ID"].ToString()))
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 10501, message = "Dublicate of ID found." });
                    }

                    if (Row["Vou_No"].ToString().Length == 0)
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 10507, message = "Enter Valid Voucher Number." });
                    }

                    if (Seek("Vou_No", Row["Vou_No"].ToString()))
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 10502, message = "Dublicate of Voucher No found." });
                    }

                    if ((int)Row["COA"] == 0)
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 10505, message = "Accounts Head must be selected." });
                    }

                    if ((decimal)Row["DR"] > 0 && (decimal)Row["CR"] > 0)
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 10503, message = "Only Debit or Credit must be more more than Zero." });
                    }

                    if ((decimal)Row["DR"] == 0 && (decimal)Row["CR"] == 0)
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 10504, message = "Must be enter Debit or Credit Amount" });
                    }


                    if (Row["Description"].ToString().Length == 0)
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 10506, message = "Description must be some charactors." });
                    }
                }
                #endregion
            }

            #endregion

            #region Write Cheque
            if (Row.Table.TableName == Tables.WriteCheques.ToString())
            {
                MyMessages = new List<Message>();

                if (SQLAction == CommandAction.Insert.ToString())
                {
                    if (Seek("ID", Row["ID"].ToString())) { MyMessages.Add(new Message() { success = false, ErrorID = 10801, message = "Dublicate of ID found." }); }
                    if ((decimal)Row["ChqAmount"] == 0) { MyMessages.Add(new Message() { success = false, ErrorID = 10802, message = "Cheque amount can not be null" }); }
                    if ((int)Row["Bank"] == 0) { MyMessages.Add(new Message() { success = false, ErrorID = 10803, message = "Bank must be selected." }); }
                    if (Row["Status"].ToString() == "") { MyMessages.Add(new Message() { success = false, ErrorID = 10804, message = "Cheque Status must be selected." }); }
                }

                if (SQLAction == CommandAction.Update.ToString())
                {
                    if ((decimal)Row["ChqAmount"] == 0) { MyMessages.Add(new Message() { success = false, ErrorID = 10802, message = "Cheque amount can not be null" }); }
                    if ((int)Row["Bank"] == 0) { MyMessages.Add(new Message() { success = false, ErrorID = 10803, message = "Bank must be selected." }); }
                    if ((int)Row["Status"] == 0) { MyMessages.Add(new Message() { success = false, ErrorID = 10804, message = "Cheque Status must be selected." }); }
                }
            }
            #endregion


            //=========================================================  result.....
            if (MyMessages.Count > 0) { return false; } else { return true; }
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

        public class Message
        {
            public bool success { get; set; } = false;
            public string message { get; set; } = string.Empty;
            public int ErrorID { get; set; } = 0;
        }

        public static List<Message> Validate(DataRow Row, CommandAction Action)
        {
            List<Message> MyMessages = new List<Message>();
            TableValidationClass MyValidation = new();
            MyValidation.SQLAction = Action.ToString();
            MyValidation.Validation(Row);
            MyMessages = MyValidation.MyMessages;
            return MyMessages;
        }
    }
}
