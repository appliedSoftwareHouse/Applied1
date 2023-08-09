using System.Data.SQLite;
using System.Security.Claims;
using System.Text;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Data
{
    public class CreateTablesClass
    {
        private string TableName { get; set; }
        private string UserName { get; set; }
        public Array TableList { get; set; }
        public List<Message> MyMessages { get; set; }

        #region Constructor
        public CreateTablesClass(string _UserName, string _TableName)
        {
            TableName = _TableName;
            UserName = _UserName;
            MyMessages = new();

            if (TableName == Tables.SaleReturn.ToString()) { SaleReturn(UserName); }
            if (TableName == Tables.BankBook.ToString()) { BankBook(UserName); }

        }

        public CreateTablesClass(ClaimsPrincipal _User)
        {
            UserName = _User.Identity.Name;
            TableList = Enum.GetValues(typeof(Tables));
        }

        #endregion

        #region Sale Return
        public static void SaleReturn(string UserName)
        {
            try
            {
                var Text = new StringBuilder();
                Text.Append("CREATE TABLE[SaleReturn] (");
                Text.Append("[ID] INT PRIMARY KEY NOT NULL UNIQUE, ");
                Text.Append("[Vou_No] TEXT(12) NOT NULL UNIQUE, ");
                Text.Append("[Vou_Date] DATETIME NOT NULL, ");
                Text.Append("[TranID] INT NOT NULL UNIQUE REFERENCES[BillReceivable2]([ID]), ");
                Text.Append("[QTY] DECIMAL NOT NULL DEFAULT 0, ");
                Text.Append("[Status] TEXT(12) NOT NULL DEFAULT Submitted)");
                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion
        #region Bank Book
        public static void BankBook(string UserName)
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

                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Directories
        public static void Directories(string UserName)
        {
            try
            {
                var Text = new StringBuilder();
                Text.Append("CREATE TABLE[Directories](");
                Text.Append("[ID] INT PRIMARY KEY NOT NULL UNIQUE, ");
                Text.Append("[Directory] NVARCHAR NOT NULL,");
                Text.Append("[Key] INT NOT NULL, ");
                Text.Append("[Value] NVARCHAR NOT NULL); ");
                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void DirectoriesINSERT(string UserName)
        {
            DataTableClass _TableClass = new DataTableClass(UserName, Tables.Directories);
            SQLiteCommand _Command = new(ConnectionClass.AppConnection(UserName));
            string[] Queries = new string[4];

            Queries[0] = "INSERT INTO [Directories] VALUES (1, 'CompanyStatus', 1, 'Customer')";
            Queries[1] = "INSERT INTO [Directories] VALUES (2, 'CompanyStatus', 2, 'Supplier');";
            Queries[2] = "INSERT INTO [Directories] VALUES (3, 'CompanyStatus', 3, 'Vendor');";
            Queries[3] = "INSERT INTO [Directories] VALUES (4, 'CompanyStatus', 4, 'Customer / Vendor');";

            foreach (string Query in Queries)
            {
                _Command.CommandText = Query;
                _Command.ExecuteNonQuery();
            }
        }

        #endregion


        #region Create Tables


        public void CreateTables()
        {
            foreach (object Table in TableList)
            {
                SQLQuery.CreateTable(UserName, (Tables)Table);
            }
        }


        public void CreatTable(Tables _Table)
        {
            var _TableName = _Table.ToString();
        }

        #endregion

    }
}
