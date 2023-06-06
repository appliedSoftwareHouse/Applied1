using Applied_WebApplication.Pages.Sales;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.SS.Formula.Functions;
using System.Data;
using System.Data.SQLite;
using System.Text;
using static Applied_WebApplication.Pages.Stock.InventoryModel;
using static NPOI.HSSF.Util.HSSFColor;

namespace Applied_WebApplication.Data
{
    public class SQLQuery
    {
        public static string SalesInvoice()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[B2].[TranID],");
            Text.Append("[B1].[Vou_No], ");
            Text.Append("[B1].[Vou_Date], ");
            Text.Append("[B1].[Company] AS [CompanyID], ");
            Text.Append("[C].[Title] AS [Company], ");
            Text.Append("[B1].[Employee] AS [EmployeeID], ");
            Text.Append("[E].[Title] AS [Employee], ");
            Text.Append("[B2].[Project] AS [ProjectID], ");
            Text.Append("[P].[Title] AS [Project], ");
            Text.Append("[B1].[Ref_No], ");
            Text.Append("[B1].[Inv_No], ");
            Text.Append("[B1].[Inv_Date], ");
            Text.Append("[B1].[Pay_Date], ");
            Text.Append("[B1].[Description], ");
            Text.Append("[B2].[Sr_No], ");
            Text.Append("[B2].[Inventory] AS [InventoryID], ");
            Text.Append("[I].[Title] AS [Inventory], ");
            Text.Append("[B2].[Batch], ");
            Text.Append("[B2].[Qty], ");
            Text.Append("[B2].[Rate], ");
            Text.Append("[B2].[Qty] * [B2].[Rate] AS [Amount], ");
            Text.Append("[T].[Title] AS [Tax], ");
            Text.Append("[T].[Rate] AS [Tax_Rate], ");
            Text.Append("([B2].[Qty] * [B2].[Rate]) * [T].[Rate] AS [Tax_Amount], ");
            Text.Append("([B2].[Qty] * [B2].[Rate]) + (([B2].[Qty] * [B2].[Rate]) * [T].[Rate]) AS [Net_Amount], ");
            Text.Append("[B2].[Description] AS [Remarks], ");
            Text.Append("[C].[Address1], ");
            Text.Append("[C].[Address2], ");
            Text.Append("[C].[City] || ' ' || [C].[State] || ' ' || [C].[Country] AS [Address3], ");
            Text.Append("[C].[Phone], ");
            Text.Append("[B1].[Status] ");
            Text.Append("FROM [BillReceivable] [B1] ");
            Text.Append("LEFT JOIN[BillReceivable2] [B2] ON[B2].[TranID] = [B1].[ID] ");
            Text.Append("LEFT JOIN[Customers] [C] ON[C].[ID] = [B1].[Company] ");
            Text.Append("LEFT JOIN[Employees] [E] ON[E].[ID] = [B1].[Employee] ");
            Text.Append("LEFT JOIN[Project] [P] ON[P].[ID] = [B2].[Project] ");
            Text.Append("LEFT JOIN[Inventory] [I] ON[I].[ID] = [B2].[Inventory] ");
            Text.Append("LEFT JOIN[Taxes] [T] ON[T].[ID] = [B2].[Tax] ");
            Text.Append("WHERE TranID=@ID");
            return Text.ToString();

        }

        public static string PostBillReceivable(string? _Filter)
        {
            StringBuilder Text = new StringBuilder();

            Text.Append("SELECT ");
            Text.Append("[B1].[ID], ");
            Text.Append("[B1].[Vou_No], ");
            Text.Append("[B1].[Vou_Date], ");
            Text.Append("[C].[Title], ");
            Text.Append("[B1].[Amount] AS [DR], ");
            Text.Append("0.00 AS [CR], ");
            Text.Append("[B1].[Status] ");
            Text.Append("FROM [BillReceivable] [B1] ");
            Text.Append("LEFT JOIN [Customers] [C] ON [B1].[Company] = [C].[ID] ");
            if (_Filter != null)
            {
                Text.Append($"WHERE {_Filter} ");
            }
            return Text.ToString();
        }

        public static string ExpenseSheetList()
        {
            var Text = "SELECT DISTINCT(Sheet_No) AS [Sheet_No] FROM [CashBook] ";
            return Text;

        }

        public static string ExpenseSheet(string _Filter)
        {
            var Text = new StringBuilder();

            Text.Append("SELECT ");
            Text.Append("[CB].[Sheet_No], ");
            Text.Append("[CB].[Vou_No], ");
            Text.Append("[CB].[Vou_Date], ");
            Text.Append("[CB].[COA], ");
            Text.Append("[A].[Title] AS [AccountTitle], ");
            Text.Append("[CB].[Customer], ");
            Text.Append("[C].[Title] AS [CompanyName], ");
            Text.Append("[CB].[Employee],");
            Text.Append("[E].[Title] AS [EmployeeName], ");
            Text.Append("[CB].[Project], ");
            Text.Append("[P].[Title] AS [ProjectName], ");
            Text.Append("[CB].[Description], ");
            Text.Append("[CB].[DR], ");
            Text.Append("[CB].[CR] ");
            Text.Append("FROM[CashBook] AS [CB] ");
            Text.Append("LEFT JOIN [COA]          [A] ON [A].[ID] = [CB].[COA] ");
            Text.Append("LEFT JOIN[Customers] [C] ON [C].[ID] = [CB].[Customer] ");
            Text.Append("LEFT JOIN[Employees] [E] ON [E].[ID] = [CB].[Employee] ");
            Text.Append("LEFT JOIN[Project]       [P] ON [P].[ID] = [CB].[Project] ");
            if (_Filter != null)
            {
                Text.Append(" WHERE ");
                Text.Append(_Filter);
            }
            return Text.ToString();
        }

        public static string UnpostBillPayable(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[BillPayable].[ID], ");
            Text.Append("[BillPayable].[Vou_No], ");
            Text.Append("[BillPayable].[Vou_Date], ");
            Text.Append("[Customers].[TITLE], ");
            Text.Append("0.00 AS[DR], ");
            Text.Append("[BillPayable].[AMOUNT] AS[CR], ");
            Text.Append("[BillPayable].[Status] ");
            Text.Append("FROM [BillPayable] ");
            Text.Append("LEFT JOIN[Customers] ON [BillPayable].[Company] = [Customers].[ID] ");
            if(_Filter != null )
            {
                if(_Filter.Length > 0)
                {
                    Text.Append($"WHERE {_Filter}");
                }
            }
            return Text.ToString();
        }

        public static string UnpostBillReceivable(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[BillReceivable].[ID], ");
            Text.Append("[BillReceivable].[Vou_No], ");
            Text.Append("[BillReceivable].[Vou_Date], ");
            Text.Append("[Customers].[TITLE], ");
            Text.Append("0.00 AS[DR], ");
            Text.Append("[BillReceivable].[AMOUNT] AS[CR], ");
            Text.Append("[BillReceivable].[Status] ");
            Text.Append("FROM [BillReceivable] ");
            Text.Append("LEFT JOIN[Customers] ON [BillReceivable].[Company] = [Customers].[ID] ");
            if (_Filter != null)
            {
                if (_Filter.Length > 0)
                {
                    Text.Append($"WHERE {_Filter}");
                }
            }
            return Text.ToString();
        }

        public static string Ledger()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT [L].*, ");
            Text.Append("[A].[TITLE] AS [AccountTitle], ");
            Text.Append("[C].[TITLE] AS [CompanyName], ");
            Text.Append("[E].[TITLE] AS [EmployeeName], ");
            Text.Append("[P].[TITLE] AS [ProjectTitle], ");
            Text.Append("[I].[TITLE]  AS [StockTitle] ");
            Text.Append("FROM [Ledger] [L] ");
            Text.Append("LEFT JOIN [COA]           [A] ON [A].[ID] = [L].[COA] ");
            Text.Append("LEFT JOIN [Customers] [C] ON [C].[ID] = [L].[CUSTOMER] ");
            Text.Append("LEFT JOIN [Employees] [E] ON [E].[ID] = [L].[EMPLOYEE] ");
            Text.Append("LEFT JOIN [Project]       [P] ON [P].[ID] = [L].[PROJECT] ");
            Text.Append("LEFT JOIN [Inventory]   [I]  ON [I].[ID]  = [L].[INVENTORY]");
            Text.Append("");

            return Text.ToString();
        }

        public static string SaleRegister(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[B2].[ID],");
            Text.Append("[B2].[TranID],");
            Text.Append("[B2].[SR_No],");
            Text.Append("[B1].[Vou_No],");
            Text.Append("[B1].[Vou_Date],");
            Text.Append("[B1].[Company],");
            Text.Append("[B1].[Inv_No],");
            Text.Append("[B1].[Inv_Date],");
            Text.Append("[B1].[Pay_Date],");
            Text.Append("[B2].[Inventory],");
            Text.Append("[B2].[Batch],");
            Text.Append("[B2].[Qty],");
            Text.Append("[B2].[Rate],");
            Text.Append("[B2].[Qty] * [B2].[Rate] AS [Amount],");
            Text.Append("([B2].[Qty] * [B2].[Rate]) * [T].[Rate] As [TaxAmount],");
            Text.Append("[B2].[Qty] * [B2].[Rate] + (([B2].[Qty] * [B2].[Rate]) * [T].[Rate]) AS [NetAmount],");
            Text.Append("[B2].[ID] AS [ID2], ");
            Text.Append("[I].[Title] AS [StockTitle],");
            Text.Append("[C].[Title] AS [CompanyName],");
            Text.Append("[E].[Title] AS [EmployeeName],");
            Text.Append("[P].[title] AS [ProjectTitle],");
            Text.Append("[T].[Code]  As [TaxTitle] ");
            Text.Append("FROM[BillReceivable2] [B2] ");
            Text.Append("LEFT JOIN[BillReceivable] [B1] ON [B1].[ID] = [B2].[TranID] ");
            Text.Append("LEFT JOIN[Inventory]         [I]    ON [I].[ID] = [B2].[Inventory] ");
            Text.Append("LEFT JOIN[Customers]      [C]   ON [C].[ID] = [B1].[Company] ");
            Text.Append("LEFT JOIN[Employees]      [E]   ON [E].[ID] = [B1].[Employee] ");
            Text.Append("LEFT JOIN[Project]            [P]   ON [P].[ID] = [B2].[Project]");
            Text.Append("LEFT JOIN[Taxes]              [T]   ON [T].[ID] = [B2].[Tax]");
            if(_Filter != null)
            {
                if(_Filter.Length >0)
                {
                    Text.Append($" WHERE {_Filter}");

                }
            }

            return Text.ToString();
        }

        #region Create DataTable into Source Data

        public static void CreateTable(string UserName, Tables _Table)
        {
            #region return if table exist
            var _TableName = _Table.ToString();
            var _CommandText = $"SELECT count(name) FROM sqlite_master WHERE type = 'table' AND name ='{_TableName}'";
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
                case Tables.TB:
                    break;
                case Tables.BillReceivable:
                    break;
                case Tables.BillReceivable2:
                    break;
                case Tables.view_BillReceivable:
                    break;
                case Tables.OBALCompany:
                    break;
                case Tables.JVList:
                    break;
                case Tables.ExpenseSheet:
                    var Text = new StringBuilder();

                    Text.Append("CREATE [ExpenceSheet] (");
                    Text.Append("[ID] INT NOT NULL UNIQUE,");
                    Text.Append("[Sheet_No] NVARCHAR(12), ");
                    Text.Append("[Vou_No] NVARCHAR(12), ");
                    Text.Append("[Status] NVARCHAR(10) NOT NULL DEFAULT Submitted);");

                    var Command = new SQLiteCommand(Text.ToString(), ConnectionClass.AppConnection(UserName));
                    Command.ExecuteNonQuery();
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
                case Tables.SamiFinished:
                    break;
                case Tables.OBALStock:
                    break;
                case Tables.BOMProfile:
                    break;
                case Tables.BOMProfile2:
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
                default:
                    break;
            }
        }
        #endregion




    }
}

