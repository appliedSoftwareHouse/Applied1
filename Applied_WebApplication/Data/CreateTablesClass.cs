using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Data.SQLite;
using System.Text;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Data
{
    public class CreateTablesClass
    {
        private string TableName { get; set; }
        private string UserName { get; set; }
        public List<Message> MyMessages { get; set; }

        public CreateTablesClass(string _UserName, string _TableName)
        {
            TableName = _TableName;
            UserName = _UserName;
            MyMessages = new();

            if (TableName == Tables.SaleReturn.ToString()) { SaleReturn(UserName); }

        }

        public static void SaleReturn(string UserName)
        {
            try
            {
                var Text = new StringBuilder();

                Text.Append("CREATE TABLE[SaleReturn] (");
                Text.Append("[ID] INT PRIMARY KEY NOT NULL UNIQUE,");
                Text.Append("[Vou_No] TEXT(12) NOT NULL UNIQUE,");
                Text.Append("[Vou_Date] DATETIME NOT NULL,");
                Text.Append("[TranID] INT NOT NULL UNIQUE REFERENCES[BillReceivable2]([ID]),");
                Text.Append("[QTY] DECIMAL NOT NULL DEFAULT 0);");
                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }

        }


    }
}
