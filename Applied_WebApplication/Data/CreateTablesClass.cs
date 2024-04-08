using Applied_WebApplication.Pages.Stock;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Data;
using System.Data.SQLite;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;
using static Applied_WebApplication.Data.MessageClass;
using static Applied_WebApplication.Pages.Stock.InventoryModel;

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
            var _SQLQuery = $"SELECT name FROM sqlite_master WHERE type in('table', 'view') ORDER BY 1";
            var _TablesName = DataTableClass.GetTable(UserName, _SQLQuery);
            var _TableView = _TablesName.AsDataView();
            MyMessages = new();

            foreach (Tables EnumTable in TableList)
            {
                if ((int)EnumTable > 8999) { continue; }                      // Skip on Temporary Table Names. 

                bool IsTableFound;
                var _TargetTable = EnumTable;
                var _Table = EnumTable.ToString();

                _TableView.RowFilter = $"Name='{_Table}'";
                if (_TableView.Count == 0) { IsTableFound = false; } else { IsTableFound = true; }

                if (!IsTableFound)
                {
                    MyMessages.Add(SetMessage($"Crerated Table {_TargetTable}", ConsoleColor.Yellow));
                    CreateTable(UserName, _TargetTable);

                }
            }
        }


        #endregion

        #region Stock Position Data
        public static void StockPositionData(string UserName)
        {
            try
            {
                AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
                var Text = new StringBuilder();
                Text.Append("CREATE VIEW [StockPositionData] AS ");
                Text.Append(SQLQuery.StockPositionData(""));
                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                AppRegistry.SetKey(UserName, "QueryMessage", ex.Message, KeyType.Text);
            }
        }

        public static void StockPosition(string UserName)
        {
            try
            {
                AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
                var Text = new StringBuilder();
                Text.Append("CREATE VIEW [StockPosition] AS ");
                Text.Append(SQLQuery.StockPosition(UserName));
                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                AppRegistry.SetKey(UserName, "QueryMessage", ex.Message, KeyType.Text);
            }
        }

        public static void StockPositionSUM(string UserName)
        {
            try
            {
                AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
                var Text = new StringBuilder();
                Text.Append("CREATE VIEW [StockPositionSUM] AS ");
                Text.Append(SQLQuery.StockPositionSUM(UserName));
                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                AppRegistry.SetKey(UserName, "QueryMessage", ex.Message, KeyType.Text);
            }
        }

        #endregion

        #region Cheak Bill Receivable

        public static void Chk_BillReceivable1(string UserName)
        {
            try
            {
                AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
                var Text = new StringBuilder();
                Text.Append("CREATE VIEW [Chk_BillReceivable1] AS ");
                Text.Append(SQLQuery.Chk_BillReceivable1());
                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();



            }
            catch (Exception ex)
            {
                AppRegistry.SetKey(UserName, "QueryMessage", ex.Message, KeyType.Text);
            }
        }


        public static void Chk_BillReceivable2(string UserName)
        {
            try
            {
                AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
                var Text = new StringBuilder();
                Text.Append("CREATE VIEW [Chk_BillReceivable2] AS ");
                Text.Append(SQLQuery.Chk_BillReceivable2());
                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                AppRegistry.SetKey(UserName, "QueryMessage", ex.Message, KeyType.Text);
            }
        }
        #endregion

        #region Purchased

        private static void Purchased(string UserName)
        {
            try
            {
                AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
                var Text = new StringBuilder();
                Text.Append("CREATE VIEW [view_Purchased] AS ");
                Text.Append(SQLQuery.view_Purchased());
                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                AppRegistry.SetKey(UserName, "QueryMessage", ex.Message, KeyType.Text);

            }

        }
        #endregion

        #region Sold
        private static void Sold(string UserName)
        {
            try
            {
                AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
                var Text = new StringBuilder();
                Text.Append("CREATE VIEW [view_Sold] AS ");
                Text.Append(SQLQuery.view_Sold());
                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                AppRegistry.SetKey(UserName, "QueryMessage", ex.Message, KeyType.Text);
            }
        }
        #endregion

        #region Production

        private static void Production1(string UserName)
        {
            try
            {
                AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
                var Text = new StringBuilder();
                Text.Append("CREATE TABLE[Production]( ");
                Text.Append("[ID] INT PRIMARY KEY NOT NULL UNIQUE,");
                Text.Append("[Vou_No] TEXT(10) NOT NULL UNIQUE, ");
                Text.Append("[Vou_Date] DATETIME NOT NULL, ");
                Text.Append("[Batch] NVARCHAR(25) NOT NULL, ");
                Text.Append("[Remarks] NVARCHAR, ");
                Text.Append("[Comments] NVARCHAR);");
                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                AppRegistry.SetKey(UserName, "QueryMessage", ex.Message, KeyType.Text);

            }
        }

        private static void Production2(string UserName)
        {
            try
            {
                AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
                var Text = new StringBuilder();
                Text.Append("CREATE TABLE[Production2](");
                Text.Append("[ID] INT PRIMARY KEY NOT NULL UNIQUE,");
                Text.Append("[TranID] INT NOT NULL REFERENCES[Production]([ID]), ");
                Text.Append("[Stock] INT NOT NULL REFERENCES[Inventory]([ID]), ");
                Text.Append("[Flow] TEXT(3) NOT NULL, ");
                Text.Append("[Qty] DECIMAL NOT NULL, ");
                Text.Append("[UOM] DECIMAL NOT NULL, ");
                Text.Append("[Rate] DECIMAL NOT NULL, ");
                Text.Append("[Remarks] NVARCHAR(100));");
                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                AppRegistry.SetKey(UserName, "QueryMessage", ex.Message, KeyType.Text);
            }
        }

        private static void ProductionView(string UserName)
        {
            try
            {
                AppRegistry.SetKey(UserName, "QueryMessage", string.Empty, KeyType.Text);
                var Text = new StringBuilder();
                Text.Append("CREATE VIEW [view_Production] AS ");
                Text.Append(SQLQuery.View_Production(UserName));
                var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                AppRegistry.SetKey(UserName, "QueryMessage", ex.Message, KeyType.Text);
            }
        }

        #endregion

        #region Accounts (COA)
        private static void COA_Map(string UserName)
        {
            var Text = new StringBuilder();
            Text.Append("CREATE TABLE[COA_Map](");
            Text.Append("[ID] INT PRIMARY KEY NOT NULL UNIQUE,");
            Text.Append("[COA] INT NOT NULL UNIQUE REFERENCES[COA]([ID]), ");
            Text.Append("[Stock] INT NOT NULL REFERENCES[Inventory]([ID]));");
            var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
            Command.ExecuteNonQuery();
        }
        #endregion


        //========================================================================= CREATE
        #region Create DataTable into Source Data

        public static void CreateTable(string UserName, Tables _Table)
        {
            #region return if table exist
            var _TableName = _Table.ToString();
            var _CommandText = $"SELECT count(name) FROM sqlite_master WHERE type in('table', 'view') AND name ='{_TableName}'";
            var _Command = new SQLiteCommand(_CommandText, ConnectionClass.AppConnection(UserName));
            long TableExist = (long)_Command.ExecuteScalar();
            if (TableExist > 0) { return; }
            #endregion

            switch (_Table)
            {
                case Tables.Registry:
                    break;
                case Tables.COA:
                    break;
                case Tables.COA_Nature:
                    break;
                case Tables.COA_Class:
                    break;
                case Tables.COA_Notes:
                    break;
                case Tables.CashBook:
                    break;
                case Tables.COA_Map:
                    COA_Map(UserName);
                    break;
                case Tables.BankBook:
                    BankBook(UserName);
                    break;
                case Tables.WriteCheques:
                    break;
                case Tables.Taxes:
                    break;
                case Tables.ChequeTranType:
                    break;
                case Tables.ChequeStatus:
                    break;
                case Tables.TaxTypeTitle:
                    break;
                case Tables.BillPayable:
                    break;
                case Tables.BillPayable2:
                    break;
                case Tables.view_Purchased:
                    Purchased(UserName);
                    break;
                case Tables.view_Sold:
                    Sold(UserName);
                    break;
                case Tables.Production:
                    Production1(UserName);
                    Production2(UserName);
                    break;
                case Tables.view_Production:
                    ProductionView(UserName);
                    break;
                case Tables.TB:
                    break;
                case Tables.BillReceivable:
                    break;
                case Tables.BillReceivable2:
                    break;
                case Tables.SaleReturn:
                    SaleReturn(UserName);
                    break;
                case Tables.view_BillReceivable:
                    break;
                case Tables.OBALCompany:
                    break;
                case Tables.JVList:
                    break;
                case Tables.Customers:
                    break;
                case Tables.City:
                    break;
                case Tables.Country:
                    break;
                case Tables.Project:
                    break;
                case Tables.Employees:
                    break;
                case Tables.Directories:
                    Directories(UserName);
                    DirectoriesINSERT(UserName);
                    break;
                case Tables.Inventory:
                    break;
                case Tables.Inv_Category:
                    break;
                case Tables.Inv_SubCategory:
                    break;
                case Tables.Inv_Packing:
                    break;
                case Tables.Inv_UOM:
                    break;
                case Tables.FinishedGoods:
                    break;
                case Tables.OBALStock:
                    break;
                case Tables.BOMProfile:
                    break;
                case Tables.BOMProfile2:
                    break;
                case Tables.StockPositionData:
                    StockPositionData(UserName);
                    break;
                case Tables.StockPosition:
                    StockPosition(UserName);
                    break;
                case Tables.StockPositionSUM:
                    StockPositionSUM(UserName);
                    break;
                case Tables.Ledger:
                    break;
                case Tables.view_Ledger:
                    break;
                case Tables.CashBookTitles:
                    break;
                case Tables.VouMax_JV:
                    break;
                case Tables.VouMax:
                    break;
                case Tables.PostCashBook:
                    break;
                case Tables.PostBankBook:
                    break;
                case Tables.PostWriteCheque:
                    break;
                case Tables.PostBillReceivable:
                    break;
                case Tables.PostBillPayable:
                    break;
                case Tables.PostPayments:
                    break;
                case Tables.PostReceipts:
                    break;
                case Tables.UnpostCashBook:
                    break;
                case Tables.UnpostBillPayable:
                    break;
                case Tables.fun_BillPayableAmounts:
                    break;
                case Tables.fun_BillPayableEntry:
                    break;
                case Tables.TempLedger:
                    break;
                case Tables.Chk_BillReceivable1:
                    Chk_BillReceivable1(UserName);
                    break;
                case Tables.Chk_BillReceivable2:
                    Chk_BillReceivable2(UserName);
                    break;


                default:
                    break;
            }
        }

       


        #endregion
    }
}
