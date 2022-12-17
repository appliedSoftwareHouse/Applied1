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


        public TableValidationClass()
        {
            success = true;
            message[0] = string.Empty;
            records = 0;
            SQLAction = "";
        }

        public TableValidationClass(bool TrueFalse)
        {
            success = TrueFalse;
            message[0] = string.Empty;
            records = 0;
        }

        public bool Validation(DataRow Row)
        {
            success = true; ;
            ErrorID = 100;

            #region Table COA
            if (Row.Table.TableName == Tables.COA.ToString())
            {
                DataTableClass _Table = new(Tables.COA.ToString());

                if (SQLAction == CommandAction.Insert.ToString())
                {
                    if (Row["Code"].ToString().Trim().Length != 6)                  // Code
                    {
                        ErrorID = 101;
                        success = false;
                        message[0] = "Code Length must be 6 Character";
                    }

                    if (Row["Title"].ToString().Trim().Length == 0)             // Title
                    {
                        ErrorID = 102;
                        success = false;
                        message[0] = "Title Length can not be null";
                    }
                }

                if (SQLAction == CommandAction.Update.ToString())
                {
                    if (Row["Code"].ToString().Trim().Length != 6)                  // Code
                    {
                        ErrorID = 101;
                        success = false;
                        message[0] = "Code Length must be 6 Character";
                    }

                    if (Row["Title"].ToString().Trim().Length == 0)             // Title
                    {
                        ErrorID = 102;
                        success = false;
                        message[0] = "Title Length can not be null";
                    }


                }




            }
            #endregion

            #region Customer
            if (Row.Table.TableName == Tables.Customers.ToString())
            {
                DataTableClass _Table = new(Tables.Customers.ToString());
                if (SQLAction == "Insert")
                {
                    if (_Table.Seek("Code", Row["Code"].ToString()))


                        success = false;
                    message[0] = "Code is already assigned";

                }
                if (SQLAction == "Update")
                {


                }




            }




            #endregion


            return success;
        }
    }
}
