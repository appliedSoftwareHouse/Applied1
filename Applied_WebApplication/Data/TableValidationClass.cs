using Applied_WebApplication.Pages.Sales;
using System.Data;

namespace Applied_WebApplication.Data
{
    public class TableValidationClass
    {
        public bool success { get; set; }
        public string[] message { get; set; }
        public int ErrorID { get; set; }
        public int records { get; set; }
        public string SQLAction { get; set; }
        public List<Message> MyMessages = new List<Message>();
        public DataTable MyDataTable { get; set; }

        public TableValidationClass()
        {
            success = true;
        }

        public TableValidationClass(CommandAction _Action)
        {
            success = true;
        }

        public TableValidationClass(bool TrueFalse)
        {
            success = TrueFalse;

        }

        public bool Validation(DataRow Row)
        {
            if (Row == null) { return false; }                                              // Return false if row is null
            if (MyDataTable == null) { return false; }                              // Return false if Data Table is null
            if(SQLAction == null) { return false; }                                     // Return is SQL Command Action is not define.
            //================================================================================================== 

            MyMessages = new List<Message>();


            #region Table COA
            if (MyDataTable.TableName == Tables.COA.ToString())
            {

                if (SQLAction == CommandAction.Insert.ToString())
                {
                    if (Row["Code"].ToString().Trim().Length != 6)                  // Code
                    {
                        Message MyMessage = new Message() { success = false, ErrorID = 101, message = "Code Length must be 6 Character" };
                    }

                    if (Row["Title"].ToString().Trim().Length == 0)             // Title
                    {
                        Message MyMessage = new Message() { success = false, ErrorID = 102, message = "Title Length can not be null" };
                    }
                }

                if (SQLAction == CommandAction.Update.ToString())
                {
                    if (Row["Code"].ToString().Trim().Length != 6)                  // Code
                    {
                        Message MyMessage = new Message() { success = false, ErrorID = 101, message = "Code Length must be 6 Character" };
                    }

                    if (Row["Title"].ToString().Trim().Length == 0)             // Title
                    {
                        Message MyMessage = new Message() { success = false, ErrorID = 102, message = "Title Length can not be null" };
                    }
                }
            }
            #endregion

            #region Customer
            if (MyDataTable.TableName == Tables.Customers.ToString())
            {
                
                #region Insert
                if (SQLAction == CommandAction.Insert.ToString())
                {
                    if (_Table.Seek("Code", Row["Code"].ToString()))
                    {
                        Message MyMessage = new Message() { success = false, ErrorID = 103, message = "Code is already assigned" };
                    }

                    if (_Table.Seek("Title", Row["Title"].ToString()))
                    {
                        Message MyMessage = new Message() { success = false, ErrorID = 103, message = "Title is already assigned" };
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


            if (MyMessages.Count > 0)
            {
                success = false;
            }
            else
            {
                success = true;
            }
            return success;
        }


        public class Message
        {
            public bool success { get; set; } = false;
            public string message { get; set; } = string.Empty;
            public int ErrorID { get; set; } = 0;
        }
    }
}
