using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using System;
using System.Data.SQLite;
using System.Net;
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

        public static string BankBook(string userName)
        {
            try
            {
                var Text = new StringBuilder();
                Text.Append("CREATE TABLE[BankBook](");
                Text.Append("[ID] INT NOT NULL UNIQUE,");
                Text.Append("[Vou_Date] DATETIME NOT NULL, ");
                Text.Append("[Vou_No] TEXT(10) NOT NULL,");
                Text.Append("[BookID] INT NOT NULL, ");
                Text.Append("[COA] INT NOT NULL, ");
                Text.Append("[Ref_No] NVARCHAR(10), ");
                Text.Append(" [Sheet_No] NVARCHAR(12), ");
                Text.Append("[DR] DECIMAL NOT NULL, ");
                Text.Append("[CR] DECIMAL NOT NULL,");
                Text.Append("[Customer] INT,");
                Text.Append("[Employee] INT, ");
                Text.Append("[Project] INT, ");
                Text.Append("[Description] NVARCHAR(60) NOT NULL,");
                Text.Append("[Comments] NVARCHAR(500), ");
                Text.Append("[Status] NVARCHAR(10) NOT NULL DEFAULT Submitted);");

                return Text.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
