using Applied_WebApplication.Pages.Sales;
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
            if(MyMessages.Count > 0)
            { return true; }
            else
            { return false; }
        }

        public bool Validation(DataRow Row)
        {
            MyMessages = new List<Message>();
            if (Row == null)  {
                Message MyMessage = new Message() { success = false, ErrorID = 10, message = "Current row is null" };
                return false;  }                                              // Return false if row is null

            if(SQLAction == null) {
                Message MyMessage = new Message() { success = false, ErrorID = 10, message = "Current row is null" };
                return false; }                                     // Return is SQL Command Action is not define.
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
            if (MyDataTable.TableName == Tables.Customers.ToString())
            {
                MyMessages = new List<Message>();

                #region Insert
                if (SQLAction == CommandAction.Insert.ToString())
                {
                    if(Row["Code"]==DBNull.Value)
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 103, message = "Null value of Code is not allowed." });
                    }

                    if (Seek("Code", Row["Code"].ToString()))
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 104, message = "Code is already assigned. Dublicate not allowed." });
                    }

                    if (Row["Title"] == DBNull.Value)
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 105, message = "Null value of Code is not allowed." });
                    }


                    if (Seek("Title", Row["Title"].ToString()))
                    {
                        MyMessages.Add(new Message() { success = false, ErrorID = 106, message = "Title is already assigned. dublicate not allowed." });
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

            if(MyMessages.Count > 0) { return false; } else { return true; }
        }

        private bool Seek(string _Column, string _Value)
        {
            if(MyDataTable!=null)
            {
                DataView _DataView = MyDataTable.AsDataView();
                _DataView.RowFilter = _Column + "='" + _Value + "'";
                if(_DataView.Count > 0) { return true; } else { return false; }
            }
            return false;               // Default value    
        }

        public class Message
        {
            public bool success { get; set; } = false;
            public string message { get; set; } = string.Empty;
            public int ErrorID { get; set; } = 0;
        }
    }
}
