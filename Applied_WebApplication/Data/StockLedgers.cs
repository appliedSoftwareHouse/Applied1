using Applied_WebApplication.Pages.Sales;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using static Applied_WebApplication.Pages.Stock.InventoryModel;

namespace Applied_WebApplication.Data
{
    public class StockLedgers
    {
        public StockLedgers() { }


        #region Stock Opening Balance
        public static string LedgerOB(string _Filter)
        {
            var _StockAccounts = "6,7,8,43,44,45";
            var Text = new StringBuilder();
           
            Text.Append("SELECT * FROM [Ledger] ");
            Text.Append($"WHERE [Vou_Type] = 'OBalStock' AND [COA] IN ({_StockAccounts})");
            return Text.ToString();
        }

        public static string StockOB()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[ID],");
            Text.Append("[Inventory] AS[StockID],");
            Text.Append("[Batch],");
            Text.Append("[Project],");
            Text.Append("[Qty],");
            Text.Append("[Rate],");
            Text.Append("[Amount],");
            Text.Append("0.00 AS [TaxRate],");
            Text.Append("0.00 AS [TaxAmount],");
            Text.Append("[Amount] AS [NetAmount]");
            Text.Append("FROM [OBalStock]");
            return Text.ToString();
        }

        public static string CombineOB(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append($"SELECT 1 AS [No], {ReportColumns()}");
            Text.Append($"FROM ({LedgerOB(_Filter)}) AS [L]");
            Text.Append($"LEFT JOIN ({StockOB()}) [S] ON [S].[ID] = [L].[TranID]");
            return Text.ToString();

        }

        #endregion

        #region Stock from Bill Payable (Purchased)

        public static string LedgerPayable(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM (");
            Text.Append(StockLedger(_Filter));
            Text.Append(") WHERE Vou_Type='Payable'");
            return Text.ToString();

        }
        public static string LedgerPayableReturn(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM (");
            Text.Append(StockLedger(_Filter));
            Text.Append(") WHERE Vou_Type='PurReturn'");
            return Text.ToString();
        }
        public static string StockPayable()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[B1].[ID],");
            Text.Append("[B2].[TranID],");
            Text.Append("[B1].[Vou_No],");
            Text.Append("[B1].[Vou_Date],");
            Text.Append("[B1].[Company],");
            Text.Append("[B1].[Ref_No],");
            Text.Append("[B1].[Inv_No],");
            Text.Append("[B1].[Inv_Date],");
            Text.Append("[B1].[Pay_Date],");
            Text.Append("[B2].[Batch],");
            Text.Append("[B2].[Inventory] AS [StockID],");
            Text.Append("[B2].[Qty],");
            Text.Append("[B2].[Rate],");
            Text.Append("[T].[Rate] As [TaxRate],");
            Text.Append("([B2].[Qty] * [B2].[Rate]) AS [Amount],");
            Text.Append("([B2].[Qty] * [B2].[Rate]) * [T].[Rate] As [TaxAmount]");
            Text.Append("FROM [BillPayable2] [B2]");
            Text.Append("LEFT JOIN [BillPayable] [B1] ON [B1].[ID] = [B2].[TranID]");
            Text.Append("LEFT JOIN [Taxes]       [T]  ON [T].[ID]  = [B2].[Tax]");
            return Text.ToString();
        }
        public static string CombinePayable(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append($"SELECT 2 AS [No], {ReportColumns()}");
            Text.Append($"FROM ({StockPayable()}) AS [S]");
            Text.Append($"LEFT JOIN ({LedgerPayable(_Filter)}) [L] ON [S].[TranID] = [L].[TranID]");
            return Text.ToString();
        }
        #endregion

        #region Stock from Bill Receivable (Sale Invoice)

        public static string LedgerSale(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM (");
            Text.Append(StockLedger(_Filter));
            Text.Append(") WHERE Vou_Type='Receivable'");
            return Text.ToString();
        }
        public static string LedgerSaleReturn(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM (");
            Text.Append(StockLedger(_Filter));
            Text.Append(") WHERE Vou_Type='SaleReturn'");
            return Text.ToString();
        }
        public static string StockSale()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[S1].[ID],");
            Text.Append("[S2].[TranID],");
            Text.Append("[S1].[Vou_No],");
            Text.Append("[S1].[Vou_Date],");
            Text.Append("[S1].[Company],");
            Text.Append("[S1].[Ref_No],");
            Text.Append("[S1].[Inv_No],");
            Text.Append("[S1].[Inv_Date],");
            Text.Append("[S1].[Pay_Date],");
            Text.Append("[S2].[Batch],");
            Text.Append("[S2].[Inventory] AS [StockID],");
            Text.Append("[S2].[Qty],");
            Text.Append("[S2].[Rate],");
            Text.Append("[T].[Rate] As [TaxRate],");
            Text.Append("([S2].[Qty] * [S2].[Rate]) AS [Amount],");
            Text.Append("([S2].[Qty] * [S2].[Rate]) * [T].[Rate] As [TaxAmount]");
            Text.Append("FROM [BillReceivable2] [S2]");
            Text.Append("LEFT JOIN [BillReceivable] [S1] ON [S1].[ID] = [S2].[TranID]");
            Text.Append("LEFT JOIN [Taxes]          [T]  ON [T].[ID]  = [S2].[Tax]");
            return Text.ToString();
        }
        public static string CombineSale(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append($"SELECT 3 AS [No], {ReportColumns()}");
            Text.Append($"FROM ({StockSale()}) AS [S]");
            Text.Append($"LEFT JOIN ({LedgerSale(_Filter)}) [L] ON [S].[TranID] = [L].[TranID]");
            return Text.ToString();

        }

        #endregion

        #region Stock from Production Process

        public static string LedgerProduction(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM (");
            Text.Append(StockLedger(_Filter));
            Text.Append(") WHERE Vou_Type='Production'");
            return Text.ToString();
        }
        public static string StockProduction()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[P1].[ID] AS[ID],");
            Text.Append("[P2].[TranID],");
            Text.Append("[P1].[Vou_No],");
            Text.Append("[P1].[Vou_Date],");
            Text.Append("0 AS [Company],");
            Text.Append("'' AS [Ref_No],");
            Text.Append("'' AS [Inv_No],");
            Text.Append("null AS [Inv_Date],");
            Text.Append("null AS [Pay_Date],");
            Text.Append("[P1].[Batch],");
            Text.Append("[P2].[Stock] AS [StockID],");
            Text.Append("[P1].[Comments],");
            Text.Append("[P2].[ID] AS[ID2],");
            Text.Append("[P2].[Flow],");
            Text.Append("[P2].[Qty],");
            Text.Append("[P2].[Rate],");
            Text.Append("0 AS [TaxRate],");
            Text.Append("([P2].[Qty] * [P2].[Rate]) AS [Amount],");
            Text.Append("0 AS [TaxAmount], ");
            Text.Append("[P2].[Remarks] AS [Remarks2],");
            Text.Append("[I].[Title] AS [StockTitle],");
            Text.Append("[U].[Code] AS [UnitTag],");
            Text.Append("[U].[Title] AS [UnitTitle]");
            Text.Append("FROM [Production2] [P2]");
            Text.Append("LEFT JOIN [Production] [P1] ON [P2].[TranID] = [P1].[ID]");
            Text.Append("LEFT JOIN [Inventory] [I] ON[I].[ID] = [P2].[Stock]");
            Text.Append("LEFT JOIN [Inv_UOM] [U] ON[U].[ID] = [I].[UOM]");
            return Text.ToString();
        }
        public static string CombineProduction(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append($"SELECT 4 AS [No], {ReportColumns()}");
            Text.Append($"FROM ({StockProduction()}) AS [S]");
            Text.Append($"LEFT JOIN ({LedgerProduction(_Filter)}) [L] ON [S].[TranID] = [L].[TranID]");
            return Text.ToString();

        }

        #endregion


        #region Ledger

        public static string StockLedger(string _Filter)
        {
            var _StockAccounts = "6,7,8,43,44,45";
            return $"SELECT {LedgerColumns()} FROM [Ledger] [L] WHERE [L].[COA] IN ({_StockAccounts}) GROUP BY [L].[Vou_No]";
        }

        public static string LedgerColumns()
        {
            var Text = new StringBuilder();

            Text.Append("[L].[TranID],");
            Text.Append("[L].[Vou_Type],");
            Text.Append("[L].[Vou_Date],");
            Text.Append("[L].[Vou_No],");
            Text.Append("[L].[Ref_No],");
            Text.Append("[L].[COA],");
            Text.Append("SUM([L].[DR]) AS [DR],");
            Text.Append("SUM([L].[CR]) AS [CR],");
            Text.Append("[L].[Customer],");
            Text.Append("[L].[Employee],");
            Text.Append("[L].[Inventory],");
            Text.Append("[L].[Project],");
            Text.Append("[L].[Description],");
            Text.Append("[L].[Status]");

            return Text.ToString();
        }

        public static string ReportColumns()
        {
            var Text = new StringBuilder();
            Text.Append("[S].[ID],");
            Text.Append("[L].[TranID],");
            Text.Append("[L].[Vou_Type],");
            Text.Append("[L].[Vou_Date],");
            Text.Append("[L].[Vou_No],");
            Text.Append("[L].[Customer],");
            Text.Append("[L].[Employee],");
            Text.Append("[L].[Project],");
            Text.Append("[L].[Status],");

            Text.Append("[S].[StockID],");
            Text.Append("[S].[Qty],");
            Text.Append("[S].[Rate],");
            Text.Append("[S].[TaxRate],");
            Text.Append("[S].[Amount],");
            Text.Append("[S].[TaxAmount],");
            Text.Append("[S].[Amount] + [S].[TaxAmount] AS [NetAmount]");
            return Text.ToString();
        }

        public static DataTable tb_StockLedger(string UserName, string _Filter)
        {
            return DataTableClass.GetTable(UserName, StockLedger(_Filter), "[COA],[Vou_Date],[Vou_Type]");
        }



        #endregion
    }
}
