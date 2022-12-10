using System.Data;

namespace Applied_WebApplication.Data
{
    public class TableValidationClass
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int ErrorID { get; set; }
        public int records { get; set; }

        public TableValidationClass()
        {
            success = true;
            message = string.Empty;
            records = 0;
        }

        public bool Validation(DataRow Row)
        {
            success = true; ;
            ErrorID = 100;

            #region Table COA
            if (Row.Table.TableName == Tables.COA.ToString())
            {
                if (Row["Code"].ToString().Trim().Length != 6)                  // Code
                {
                    ErrorID = 101;
                    success = false;
                    message = "Code Length must be 6 Character";
                }

                if (Row["Title"].ToString().Trim().Length == 0)             // Title
                {
                    ErrorID = 102;
                    success = false;
                    message = "Title Length can not be null";
                }
            }
            #endregion

            return success;
        }
    }
}
