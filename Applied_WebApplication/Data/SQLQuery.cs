using Microsoft.Extensions.Primitives;
using System.Text;

namespace Applied_WebApplication.Data
{
    public class SQLQuery
    {

        #region Sales Invoice
        public static string SalesInvoice()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[B2].[TranID],");
            Text.Append("[B1].[Vou_No], ");
            Text.Append("[B1].[Vou_Date], ");
            Text.Append("[B1].[Company] AS [CompanyID], ");
            Text.Append("[C].[Code] AS [Code], ");
            Text.Append("[C].[NTN] AS [NTN], ");
            Text.Append("[C].[Title] AS [Company], ");
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
            Text.Append("([T].[Rate] / 100) AS [Tax_Rate2], ");
            Text.Append("CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) AS [Amount],");
            Text.Append("(CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) *");
            Text.Append("CAST([T].[Rate] AS FLOAT))/ 100 AS [Tax_Amount],");
            Text.Append("CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) +");
            Text.Append("(CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) *");
            Text.Append("CAST([T].[Rate] AS FLOAT))/ 100 AS [Net_Amount],");
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
        #endregion

        #region Posting Vouchers
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
        #endregion

        #region Posting Cheque Book
        public static string PostWriteCheques(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[WC].[ChqNo] AS[Vou_No], ");
            Text.Append("[WC].[ChqDate] AS[Vou_Date], ");
            Text.Append("[WC].[Bank] AS[COA], ");
            Text.Append("[C].[Title] As[Title], ");
            Text.Append("0.00 AS[DR], ");
            Text.Append("CAST([WC].[TaxableAmount] + [WC].[TaxAmount] AS Float) AS[CR], ");
            Text.Append("[WC].[Status] ");
            Text.Append("FROM WriteCheques [WC] ");
            Text.Append("LEFT JOIN[COA] [C] ON[C].[ID] = [WC].[Bank] ");
            if (_Filter.Length > 0) { Text.Append($" WHERE {_Filter}"); }
            return Text.ToString();
        }
        #endregion

        #region Write Cheque
        public static string WriteCheque(string? _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM WriteCheques");
            if (_Filter != null)
            {
                Text.Append($"WHERE {_Filter} ");
            }
            return Text.ToString();
        }
        #endregion

        #region Expense Sheet
        public static string ExpenseSheetList()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT DISTINCT(Sheet_No) AS [Sheet_No] FROM [CashBook] ");
            Text.Append("UNION ");
            Text.Append("SELECT DISTINCT(Sheet_No) AS [Sheet_No] FROM [BankBook] ");
            return Text.ToString();

        }

        public static string ExpenseSheet(string _Filter)
        {
            var Text = new StringBuilder();

            Text.Append("SELECT ");
            Text.Append("[CB].[Sheet_No], ");
            Text.Append("[CB].[Vou_No], ");
            Text.Append("[CB].[Vou_Date], ");
            Text.Append("[CB].[Ref_No], ");
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

            Text.Append("UNION ");

            Text.Append("SELECT ");
            Text.Append("[CB].[Sheet_No], ");
            Text.Append("[CB].[Vou_No], ");
            Text.Append("[CB].[Vou_Date], ");
            Text.Append("[CB].[Ref_No], ");
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
            Text.Append("FROM[BankBook] AS [CB] ");
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

        public static string ExpenseSheetGroup(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[COA], ");
            Text.Append("[C].[Code][Code], ");
            Text.Append("[C].[title][Title], ");
            Text.Append("[Sheet_No], ");
            Text.Append("SUM([DR]) AS[DR], ");
            Text.Append("SUM([CR]) AS[CR] ");
            Text.Append("FROM[Cashbook][CB] ");
            Text.Append("LEFT JOIN[COA] [C] ON[C].[ID] = [CB].[COA] ");
            if (_Filter.Length > 0) { Text.Append($"WHERE {_Filter}"); }
            Text.Append("GROUP BY[COA], [Sheet_No]");
            return Text.ToString();
        }

        #endregion

        #region Unpost Vouchers
        public static string UnpostBillPayable(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[B].[ID], ");
            Text.Append("[B].[Vou_No], ");
            Text.Append("[B].[Vou_Date], ");
            Text.Append("[C].[TITLE], ");
            Text.Append("0.00 AS[DR], ");
            Text.Append("[B].[AMOUNT] AS[CR], ");
            Text.Append("[B].[Status] ");
            Text.Append("FROM [BillPayable] [B] ");
            Text.Append("LEFT JOIN[Customers] [C] ON [B].[Company] = [C].[ID] ");
            if (_Filter != null)
            {
                if (_Filter.Length > 0)
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
            Text.Append("[B].[ID], ");
            Text.Append("[B].[Vou_No], ");
            Text.Append("[B].[Vou_Date], ");
            Text.Append("[C].[TITLE], ");
            Text.Append("0.00 AS[DR], ");
            Text.Append("[B].[AMOUNT] AS[CR], ");
            Text.Append("[B].[Status] ");
            Text.Append("FROM [BillReceivable] [B] ");
            Text.Append("LEFT JOIN[Customers] [C] ON [B].[Company] = [C].[ID] ");
            if (_Filter != null)
            {
                if (_Filter.Length > 0)
                {
                    Text.Append($"WHERE {_Filter}");
                }
            }
            return Text.ToString();
        }
        #endregion

        #region Ledger
        public static string Ledger(string _Filter)
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
            if (_Filter.Length > 0)
            {
                Text.Append($" WHERE {_Filter}");
            }

            return Text.ToString();
        }
        #endregion

        #region Ledger 2
        public static string Ledger2(string _Filter, string[] Dates, string _OrderBy)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM (");
            Text.Append("SELECT ");
            Text.Append("'OBAL' AS [Vou_No],");
            Text.Append($"DATE('{Dates[0]}') AS [Vou_Date],");
            Text.Append($"'Opening Balance as on ' || DATE('{Dates[0]}') AS [Description],");
            Text.Append("IIF([BAL] > 0,[BAL],0) AS [DR],");
            Text.Append("IIF([BAL] < 0,ABS([BAL]),0) AS [CR],");
            Text.Append("0 AS [AccountTitle],");
            Text.Append("0 AS [CompanyName],");
            Text.Append("0 AS [EmployeeName],");
            Text.Append("0 AS [ProjectTitle],");
            Text.Append("0 AS [StockTitle] ");
            Text.Append("FROM(SELECT SUM([DR] -[CR]) AS [BAL] ");
            Text.Append("FROM(SELECT * ");
            Text.Append($"FROM[Ledger] WHERE DATE([Vou_Date]) < DATE('{Dates[0]}') AND {_Filter})) ");
            Text.Append("UNION ALL ");

            Text.Append("SELECT ");
            Text.Append("[L].[Vou_No], ");
            Text.Append("[L].[Vou_Date],");
            Text.Append("[L].[Description],");
            Text.Append("[L].[DR],");
            Text.Append("[L].[CR],");
            Text.Append("[A].[TITLE] AS [AccountTitle],");
            Text.Append("[C].[TITLE] AS [CompanyName],");
            Text.Append("[E].[TITLE] AS [EmployeeName],");
            Text.Append("[P].[TITLE] AS [ProjectTitle],");
            Text.Append("[I].[TITLE] AS [StockTitle]");
            Text.Append("FROM(");
            Text.Append("SELECT * FROM[Ledger] [L] ");
            Text.Append($"WHERE (Date([Vou_Date]) BETWEEN Date('{Dates[0]}') AND Date('{Dates[1]}')) ");
            Text.Append($"AND {_Filter}");
            Text.Append(" ) AS[L]");
            Text.Append("LEFT JOIN[COA]       [A] ON[A].[ID] = [L].[COA]");
            Text.Append("LEFT JOIN[Customers] [C] ON[C].[ID] = [L].[CUSTOMER]");
            Text.Append("LEFT JOIN[Employees] [E] ON[E].[ID] = [L].[EMPLOYEE]");
            Text.Append("LEFT JOIN[Project]   [P] ON[P].[ID] = [L].[PROJECT]");
            Text.Append("LEFT JOIN[Inventory] [I] ON[I].[ID] = [L].[INVENTORY]");
            Text.Append(") AS [GLedger]");
            Text.Append($"ORDER BY {_OrderBy}");
            return Text.ToString();
        }
        #endregion

        #region Sales and Purchase Register

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
            Text.Append("[T].[Rate] AS [Tax], ");
            Text.Append("CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) AS [Amount],");
            Text.Append("(CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) *");
            Text.Append("CAST([T].[Rate] AS FLOAT))/ 100 AS [TaxAmount],");
            Text.Append("CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) +");
            Text.Append("(CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) *");
            Text.Append("CAST([T].[Rate] AS FLOAT))/ 100 AS [NetAmount],");
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
            if (_Filter != null)
            {
                if (_Filter.Length > 0)
                {
                    Text.Append($" WHERE {_Filter}");

                }
            }

            return Text.ToString();
        }

        public static string PurchaseRegister(string _Filter)
        {
            var Text = new StringBuilder();

            Text.Append("SELECT ");
            Text.Append("[B1].[ID] AS[ID1], ");
            Text.Append("[B2].[ID] AS[ID2], ");
            Text.Append("[B1].[Vou_No],  ");
            Text.Append("[B1].[Vou_Date], ");
            Text.Append("[B1].[Company],");
            Text.Append("[B1].[Employee],");
            Text.Append("[B1].[Ref_No],");
            Text.Append("[B1].[Inv_No],");
            Text.Append("[B1].[Inv_Date],");
            Text.Append("[B1].[Pay_Date],");
            Text.Append("[B1].[Description],");
            Text.Append("[B1].[Status],");
            Text.Append("[B2].[Sr_No],");
            Text.Append("[B2].[TranID],");
            Text.Append("[B2].[Inventory],");
            Text.Append("[B2].[Batch],");
            Text.Append("[B2].[Qty],");
            Text.Append("[B2].[Rate],");
            Text.Append("CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) AS [Amount],");
            Text.Append("(CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) *");
            Text.Append("CAST([T].[Rate] AS FLOAT))/ 100 AS [TaxAmount],");
            Text.Append("CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) +");
            Text.Append("(CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) *");
            Text.Append("CAST([T].[Rate] AS FLOAT))/ 100 AS [NetAmount],");
            Text.Append("[B2].[Description] AS[Description2],");
            Text.Append("[C].[Title] AS[CompanyName],");
            Text.Append("[E].[Title] AS[EmployeeName],");
            Text.Append("[P].[Title] AS[ProjectTitle],");
            Text.Append("[I].[Title] AS[StockTitle],");
            Text.Append("[T].[Code]  AS[TaxTitle],");
            Text.Append("[T].[Rate]  AS[TaxRate]");
            Text.Append("FROM [BillPayable2][B2]");
            Text.Append("LEFT JOIN[BillPayable]  [B1] ON[B1].[ID] = [B2].[TranID]");
            Text.Append("LEFT JOIN[Customers]   [C] ON[C].[ID] = [B1].[Company]");
            Text.Append("LEFT JOIN[Employees]   [E] ON[E].[ID] = [B1].[Employee]");
            Text.Append("LEFT JOIN[Project]         [P] ON[P].[ID] = [B2].[Project]");
            Text.Append("LEFT JOIN[Inventory]     [I] ON[I].[ID] = [B2].[Inventory]");
            Text.Append("LEFT JOIN[Taxes]           [T] ON[T].[ID] = [B2].[Tax] ");
            if (_Filter.Length > 0) { Text.Append($"WHERE {_Filter} "); }
            return Text.ToString();
        }
        public static string PurchaseRegister(string _Filter, string _Order)
        {
            var Text = new StringBuilder();
            Text.Append(PurchaseRegister(_Filter));
            Text.Append($" ORDER BY {_Order}");
            return Text.ToString();
        }

        public static string SaleReturn(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[B2].[ID],");
            Text.Append("[B2].[TranID],");
            Text.Append("[B2].[SR_No],");
            Text.Append("[R].[Vou_No],");
            Text.Append("[B1].[Vou_No] AS [SaleVou_No],");
            Text.Append("[B1].[Vou_Date],");
            Text.Append("[B1].[Company],");
            Text.Append("[B1].[Inv_No],");
            Text.Append("[B1].[Inv_Date],");
            Text.Append("[B1].[Pay_Date],");
            Text.Append("[B2].[Inventory],");
            Text.Append("[B2].[Batch],");
            Text.Append("[B2].[Qty],");
            Text.Append("[R].[Qty] AS [RQty],");
            Text.Append("[B2].[Rate],");
            Text.Append("CAST([T].[Rate] AS FLOAT) AS [Tax], ");
            Text.Append("CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) AS [Amount],");
            Text.Append("(CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) *");
            Text.Append("CAST([T].[Rate] AS FLOAT))/ 100 AS [TaxAmount],");
            Text.Append("CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) +");
            Text.Append("(CAST([B2].[Qty] * [B2].[Rate] AS FLOAT) *");
            Text.Append("CAST([T].[Rate] AS FLOAT))/ 100 AS [NetAmount],");
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
            Text.Append("LEFT JOIN[SaleReturn]     [R]   ON [R].[TranID] = [B2].[ID]");
            if (_Filter != null)
            {
                if (_Filter.Length > 0)
                {
                    Text.Append($" WHERE {_Filter}");

                }
            }

            return Text.ToString();
        }

        #endregion

        #region Trial Balance

        public static string TrialBalance(string _Filter, string _OrderBy)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM (");
            Text.Append("SELECT [Ledger].[COA], [COA].[Code], [COA].[Title], ");
            Text.Append("SUM([Ledger].[DR]) AS [DR], ");
            Text.Append("SUM([Ledger].[CR]) AS [CR], ");
            Text.Append("SUM([Ledger].[DR] - [Ledger].[CR]) AS [BAL] ");
            Text.Append("FROM [Ledger] ");
            Text.Append("LEFT JOIN[COA] ON[COA].[ID] = [Ledger].[COA] ");
            if (_Filter.Length > 0) { Text.Append($" WHERE {_Filter} "); }
            Text.Append("GROUP BY [COA] ");
            Text.Append(") WHERE BAL <> 0 ");
            if (_OrderBy.Length > 0) { Text.Append($" ORDER BY {_OrderBy}"); }

            return Text.ToString();
        }

        #endregion

        #region Voucher
        public static string Voucher(string _Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[L].*, ");
            Text.Append("[A].[Code], ");
            Text.Append("[A].[Title] AS[AccountTitle], ");
            Text.Append("[C].[Title] AS[CompanyName], ");
            Text.Append("[E].[Title] AS[EmployeeName], ");
            Text.Append("[P].[Title] AS[ProjectTitle], ");
            Text.Append("[I].[Title] As[StockTitle] ");
            Text.Append("FROM[Ledger][L] ");
            Text.Append("LEFT JOIN[COA]           [A] ON[A].[ID] = [L].[COA]");
            Text.Append("LEFT JOIN[Customers] [C] ON[C].[ID] = [L].[Customer] ");
            Text.Append("LEFT JOIN[Employees] [E] ON[E].[ID] = [L].[Employee] ");
            Text.Append("LEFT JOIN[Inventory]   [I]   ON[I].[ID] = [L].[Inventory] ");
            Text.Append("LEFT JOIN[Project]       [P] ON[P].[ID] = [L].[Project] ");
            if (_Filter.Length > 0)
            {
                Text.Append($"WHERE {_Filter} ");
            }
            Text.Append("ORDER BY Vou_No, Sr_No");
            return Text.ToString();
        }
        #endregion

        #region Cash & Bank Book
        public static string vwBook(int? BookCode)
        {
            if (BookCode == null) { return "No Book Code defined."; }

            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[Vou_Type], ");
            Text.Append("[Vou_Date], ");
            Text.Append("[Vou_No], ");
            Text.Append("[Description], ");
            Text.Append("[DR], ");
            Text.Append("[CR], ");
            Text.Append("0.00 AS[BAL], ");
            Text.Append(" [Customer], ");
            Text.Append("[Customers].[Title] AS [CustomerTitle], ");
            Text.Append(" '' AS [Status] ");
            Text.Append("FROM[Ledger]");
            Text.Append("LEFT JOIN[Customers] ON[Ledger].[Customer] = [Customers].[ID]; ");
            Text.Append($"WHERE COA={BookCode}");
            return Text.ToString();


        }
        public static string BookTitles(int? NatureID)
        {
            var Text = new StringBuilder();
            Text.Append($"SELECT [ID],[Title] from [COA] Where Nature={NatureID};");

            return Text.ToString();
        }

        #endregion

        #region Cash Book Ledger
        public static string BookLedger(string _Filter, string[] Dates, string Book)
        {
            string _BookName = string.Empty;
            if (Book == "Cash") { _BookName = "CashBook"; }
            if (Book == "Bank") { _BookName = "BankBook"; }

            var Text = new StringBuilder();

            Text.Append("SELECT * FROM ( ");
            Text.Append("SELECT ");
            Text.Append("-1 AS [ID],");
            Text.Append($"'{Dates[0]}' AS [Vou_date], ");
            Text.Append("'OBAL' AS [Vou_No], ");
            Text.Append("'Opening Balance' AS [Description], ");
            Text.Append("SUM([DR]) AS [DR], ");
            Text.Append("SUM([CR]) AS [CR], ");
            Text.Append("'Posted' AS [Status]");
            Text.Append("FROM (");
            Text.Append($"SELECT [DR],[CR] FROM [{_BookName}] WHERE Date([Vou_Date]) < Date('{Dates[0]}') ");

            Text.Append("UNION ALL ");
            Text.Append("SELECT [CR] AS [DR] ,[CR] AS [DR] ");
            Text.Append($"FROM [Ledger] WHERE {_Filter} and [Vou_Type] <> '{Book}' ");
            Text.Append($"AND Date([Vou_Date]) < Date('{Dates[0]}') ");
            Text.Append(") ");

            Text.Append("UNION ALL  ");
            Text.Append("SELECT ");
            Text.Append("[ID],");
            Text.Append("[Vou_Date], ");
            Text.Append("[Vou_No], ");
            Text.Append("[Description], ");
            Text.Append("[DR], ");
            Text.Append("[CR], ");
            Text.Append("[Status]");
            Text.Append($"FROM [{_BookName}] ");
            Text.Append($"WHERE Date([Vou_Date]) BETWEEN Date('{Dates[0]}') AND Date('{Dates[1]}') ");

            Text.Append("UNION ALL ");
            Text.Append("SELECT ");
            Text.Append("0 AS [ID],");
            Text.Append("[Vou_Date], ");
            Text.Append("[Vou_No], ");
            Text.Append("[Description], ");
            Text.Append("[CR] AS [DR] , ");
            Text.Append("[DR] AS [CR], ");
            Text.Append("'Posted' AS [Status]");
            Text.Append("FROM [Ledger] ");
            Text.Append($"WHERE {_Filter} AND Date([Vou_date]) BETWEEN Date('{Dates[0]}') AND Date('{Dates[1]}') ");
            Text.Append($"AND [Vou_type] <> '{Book}' ");
            Text.Append(") AS [CashBookLedger] ");
            Text.Append("ORDER BY [Vou_Date],[Vou_No]");

            return Text.ToString();
        }
        #endregion

        #region Get New Voucher
        public static string NewVouNo(Tables _Table)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT [Vou_No], ");
            Text.Append("substr([Vou_No],1,2) AS Tag, ");
            Text.Append("SubStr([Vou_No],3,2) AS Year, ");
            Text.Append("SubStr([Vou_No],5,2) AS Month, ");
            Text.Append("Cast((SubStr([Vou_No],8,4)) as integer) AS num ");
            Text.Append($"FROM [{_Table}]");
            return Text.ToString();

        }
        #endregion

        #region Posting Cash Book Ledger

        public static string PostBook(Tables _Table, string _Filter, string _Status)
        {
            // Table Name,   it is Cash Book or Bank Book
            // Filter of Records from Cash / Bank Book
            // Transaction Status, it is Submitted and Posted. for Post and Unpost option.

            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[Book].[ID], ");
            Text.Append("[Book].[Vou_No], ");
            Text.Append("[Book].[Vou_Date], ");
            Text.Append("[COA].[TITLE], ");
            Text.Append("[Book].[DR], ");
            Text.Append("[Book].[CR], ");
            Text.Append("[Book].[Status] ");
            Text.Append($"FROM [{_Table}] [Book] ");
            Text.Append($"LEFT JOIN [COA] ON [Book].[COA] = [COA].[ID] ");
            Text.Append($"WHERE [Book].[Status] = '{_Status}' AND {_Filter};");

            return Text.ToString();
        }

        #endregion

        #region Sale Return List

        public static string PostSaleReturnList(string? _Filter)
        {
            _Filter ??= string.Empty;

            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[SR].[ID], ");
            Text.Append("[SR].[TranID], ");
            Text.Append("[B1].[Vou_No], ");
            Text.Append("[B1].[Vou_Date], ");
            Text.Append("[CM].[TITLE], ");
            Text.Append("[B1].[Amount] AS [DR], ");
            Text.Append("00 AS [CR], ");
            Text.Append("[SR].[Status] ");
            Text.Append("FROM [SaleReturn] [SR] ");
            Text.Append("LEFT JOIN [BillReceivable2] [B2] ON [B2].[ID] = [SR].[TranID] ");
            Text.Append("LEFT JOIN [BillReceivable]   [B1] ON [B1].[ID] = [B2].[TranID] ");
            Text.Append("LEFT JOIN [Customers] [CM] ON [B1].[Company] = [CM].[ID] ");
            if (_Filter.Length > 0)
            {
                Text.Append($" WHERE {_Filter}");
            }
            return Text.ToString();
        }
        #endregion

        #region Post & UnPost Sale Return
        public static string PostSaleReturn(string? _Filter)
        {
            _Filter ??= string.Empty;
            var Text = new StringBuilder();
            Text.Append("SELECT *,");
            Text.Append("CAST([SR].[Amount] + [SR].[TaxAmount] AS Float) AS[NetAmount],");
            Text.Append("CAST([SR].[RAmount] + [SR].[RTaxAmount] AS Float) AS [RNetAmount],");
            Text.Append("CAST(");
            Text.Append("(CAST([SR].[Amount] + [SR].[TaxAmount] AS Float)) - ");
            Text.Append("(CAST([SR].[RAmount] + [SR].[RTaxAmount] AS Float)) ");
            Text.Append("AS Float) AS [Total] ");
            Text.Append("FROM(");
            Text.Append("SELECT");
            Text.Append("[SR].[ID]             AS [SR_ID],");
            Text.Append("[SR].[TranID]      AS [SR_TranID],");
            Text.Append("[B1].[ID]             AS [B1_ID],");
            Text.Append("[B2].[ID]             AS [B2_ID],");
            Text.Append("[SR].[Vou_No]    AS [Vou_No],");
            Text.Append("[SR].[Vou_Date] AS [Vou_Date],");
            Text.Append("[B2].[TranID]      AS [B2_TranID],");
            Text.Append("[B2].[Qty]           AS [Qty],");
            Text.Append("[B2].[Rate]         AS [Rate],");
            Text.Append("[TX].[Rate]         AS [TRate],");
            Text.Append("[B1].[Company]  AS [CompanyID],");
            Text.Append("[B1].[Employee] AS [EmployeeID],");
            Text.Append("[B2].[Project]      AS [ProjectID],");
            Text.Append("CAST( [B2].[Qty] * [B2].[Rate] AS Float) AS [Amount],");
            Text.Append("CAST(([B2].[Qty] * [B2].[Rate]) * [TX].[Rate] AS Float) AS [TaxAmount],");
            Text.Append("[SR].[Qty]     AS [QtyR],");
            Text.Append("CAST([SR].[Qty] * [B2].[Rate] As Float) AS [RAmount],");
            Text.Append("CAST(([SR].[Qty] * [B2].[Rate]) * [TX].[Rate] AS Float) AS [RTaxAmount],");
            Text.Append("[B2].[Description],");
            Text.Append("[B1].[Comments] AS [Remarks],");
            Text.Append("[I].[Title] AS [Inventory],");
            Text.Append("[SR].[Status]");
            Text.Append("FROM SaleReturn [SR]");
            Text.Append("LEFT JOIN [BillReceivable2] [B2] ON [B2].[ID] = [SR].[TranID]");
            Text.Append("LEFT JOIN [BillReceivable]   [B1] ON [B1].[ID] = [B2].[TranID]");
            Text.Append("LEFT JOIN [Inventory]           [I] ON [I].[ID] = [B2].[Inventory]");
            Text.Append("LEFT JOIN[Taxes]                 [TX] ON [TX].[ID] = [B2].[Tax]");
            Text.Append(") AS [SR]");
            if (_Filter.Length > 0) { Text.Append($" WHERE {_Filter}"); }

            return Text.ToString();
        }
        #endregion

        #region Company Balances (AR-AP)
        public static string CompanyBalances(string _Filter, string COA_List)
        {
            StringBuilder Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[L].[Customer],");
            Text.Append("[C].[Code],");
            Text.Append("[C].[Title],");
            Text.Append("[N].[Title] [Nature],");
            Text.Append("CAST(ROUND(SUM([L].[DR]), 2) AS FLOAT) AS [DR],");
            Text.Append("CAST(ROUND(SUM([L].[CR]), 2) AS FLOAT) AS [CR],");
            Text.Append("CAST(ROUND(SUM([L].[DR] -[L].[CR]), 2) AS FLOAT) AS [BAL]");
            Text.Append("FROM[Ledger] [L]");
            Text.Append("LEFT JOIN [Customers] [C] on[C].[ID] = [L].[Customer]");
            Text.Append($"LEFT JOIN ({GetDirectory("CompanyStatus")}) [N] ");
            Text.Append("ON [N].[ID] = [C].[Status]");
            Text.Append($"WHERE[COA] IN({COA_List})");
            if (_Filter.Length > 0) { Text.Append($" AND {_Filter} "); }
            Text.Append("GROUP BY[Customer]");

            return Text.ToString();
        }
        #endregion

        #region Get Directory
        public static string GetDirectory(string _DirectoryName)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[Key] AS[ID],");
            Text.Append("[Value] AS[Title]");
            Text.Append($"FROM[Directories] WHERE Directory = '{_DirectoryName}'");
            Text.Append("");

            return Text.ToString();
        }
        #endregion

        #region Stock Position Data (In Hand)
        public static string StockPosition(string Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM ");
            Text.Append($"({StockPositionData(Filter)})");
            return Text.ToString();
        }

        public static string StockPositionData(string Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM (");
            Text.Append("SELECT * FROM ( SELECT ");
            Text.Append("'PURCHASED' AS [TRAN],");
            Text.Append("[B1].[Vou_No],");
            Text.Append("[B1].[Vou_Date],");
            Text.Append("[B2].[Inventory],");
            Text.Append("[B2].[Qty],");
            Text.Append("[B2].[Rate],");
            Text.Append("[B2].[Qty] * [B2].[Rate] AS [Amount],");
            Text.Append("[T].[Rate] AS [TaxRate],");
            Text.Append("([B2].[Qty] * [B2].[Rate]) * [T].[Rate] AS [TaxAmount],");
            Text.Append("([B2].[Qty] * [B2].[Rate]) + (([B2].[Qty] * [B2].[Rate]) * [T].[Rate]) AS [NetAmount] ");
            Text.Append("FROM [BillPayable] [B1] ");
            Text.Append("LEFT JOIN [BillPayable2] [B2] ON [B1].[ID] = [B2].[TranID] ");
            Text.Append("LEFT JOIN Taxes [T] On [T].[ID] = [B2].[Tax] ");
            Text.Append(") AS [Purchased] ");
            Text.Append(" UNION ");
            Text.Append("SELECT * FROM ");
            Text.Append("(SELECT ");
            Text.Append("'SOLD' AS [TRAN], ");
            Text.Append("[B1].[Vou_No], ");
            Text.Append("[B1].[Vou_Date], ");
            Text.Append("[B2].[Inventory], ");
            Text.Append("[B2].[Qty], ");
            Text.Append("[B2].[Rate], ");
            Text.Append("[B2].[Qty] * [B2].[Rate] AS [Amount], ");
            Text.Append("[T].[Rate] AS [TaxRate], ");
            Text.Append("([B2].[Qty] * [B2].[Rate]) * [T].[Rate] AS [TaxAmount], ");
            Text.Append("([B2].[Qty] * [B2].[Rate]) + (([B2].[Qty] * [B2].[Rate]) * [T].[Rate]) AS [NetAmount] ");
            Text.Append("FROM [BillReceivable2] [B2] ");
            Text.Append("LEFT JOIN [BillReceivable] [B1] ON [B1].[ID] = [B2].[TranID] ");
            Text.Append("LEFT JOIN Taxes [T] On [T].[ID] = [B2].[Tax] ");
            Text.Append(") AS [Sold] ");
            Text.Append("UNION ");
            Text.Append("SELECT * FROM ");
            Text.Append("(SELECT ");
            Text.Append("'SRETURN' AS [TRAN], ");
            Text.Append("[SR].[Vou_No], ");
            Text.Append("[SR].[Vou_Date], ");
            Text.Append("[B2].[Inventory], ");
            Text.Append("[SR].[Qty], ");
            Text.Append("[B2].[Rate], ");
            Text.Append("[B2].[Qty] * [B2].[Rate] AS [Amount], ");
            Text.Append("[T].[Rate] AS [TaxRate], ");
            Text.Append("([B2].[Qty] * [B2].[Rate]) * [T].[Rate] AS [TaxAmount], ");
            Text.Append("([B2].[Qty] * [B2].[Rate]) + (([B2].[Qty] * [B2].[Rate]) * [T].[Rate]) AS [NetAmount] ");
            Text.Append("FROM [SaleReturn] [SR] ");
            Text.Append("LEFT JOIN BillReceivable2 [B2] ON [B2].[ID] = [SR].[TranID] ");
            Text.Append("LEFT JOIN BillReceivable   [B1] ON [B1].[ID] = [B2].[TranID] ");
            Text.Append("LEFT JOIN Taxes                 [T]   ON [T].[ID]   = [B2].[Tax] ");
            Text.Append(") AS [SRETURN] ) ");
            if (Filter.Length > 0) { Text.Append($" WHERE {Filter}"); }
            return Text.ToString();


        }
        public static string StockPositionSUM(string Filter)
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM(");
            Text.Append("SELECT ");
            Text.Append("[Inventory],");
            Text.Append("SUM(QTY) AS [PQty],");
            Text.Append("SUM(Amount) AS [PAmount],");
            Text.Append("SUM(TaxAmount) AS [PTaxAmount],");
            Text.Append("SUM(TaxAmount) AS [PNetAmount],");
            Text.Append("0.00 AS [SQty],");
            Text.Append("0.00 AS [SAmount],");
            Text.Append("0.00 AS [STaxAmount],");
            Text.Append("0.00 AS [SNetAmount],");
            Text.Append("0.00 AS [SRQty],");
            Text.Append("0.00 AS [SRAmount],");
            Text.Append("0.00 AS [SRTaxAmount],");
            Text.Append("0.00 AS [SRNetAmount]");
            Text.Append($"FROM ({StockPositionData(Filter)}) WHERE [Tran] = 'PURCHASED'");
            Text.Append("GROUP BY [Inventory]");
            Text.Append(") AS [P] ");
            Text.Append("UNION ");
            Text.Append("SELECT * FROM(");
            Text.Append("SELECT ");
            Text.Append("[Inventory],");
            Text.Append("0.00 AS [PQty],");
            Text.Append("0.00 AS [PAmount],");
            Text.Append("0.00 AS [PTaxAmount],");
            Text.Append("0.00 AS [PNetAmount],");
            Text.Append("SUM(QTY) AS [SQty],");
            Text.Append("SUM(Amount) AS [SAmount],");
            Text.Append("SUM(TaxAmount) AS [STaxAmount],");
            Text.Append("SUM(TaxAmount) AS [SNetAmount],");
            Text.Append("0.00 AS [SRQty],");
            Text.Append("0.00 AS [SRAmount],");
            Text.Append("0.00 AS [SRTaxAmount],");
            Text.Append("0.00 AS [SRNetAmount]");
            Text.Append($"FROM ({StockPositionData(Filter)}) WHERE [Tran] = 'SOLD'");
            Text.Append("GROUP BY [Inventory]");
            Text.Append(") AS [S] ");
            Text.Append("UNION ");
            Text.Append("SELECT * FROM (");
            Text.Append("SELECT ");
            Text.Append("[Inventory],");
            Text.Append("0.00 AS [PQty],");
            Text.Append("0.00 AS [PAmount],");
            Text.Append("0.00 AS [PTaxAmount],");
            Text.Append("0.00 AS [PNetAmount],");
            Text.Append("0.00 AS [SQty],");
            Text.Append("0.00 AS [SAmount],");
            Text.Append("0.00 AS [STaxAmount],");
            Text.Append("0.00 AS [SNetAmount],");
            Text.Append("SUM(QTY) AS [SRRQty],");
            Text.Append("SUM(Amount) AS [SRAmount],");
            Text.Append("SUM(TaxAmount) AS [SRTaxAmount],");
            Text.Append("SUM(NetAmount) AS [SRNetAmount]");
            Text.Append($"FROM ({StockPositionData(Filter)}) WHERE [Tran] = 'SRETURN'");
            Text.Append("GROUP BY [Inventory]");
            Text.Append(") AS [SR]");
            return Text.ToString();
            //ok

        }

        #endregion

        #region Stock in Hand
        public static string StockInHand(string Filter)
        {
            var Text = new StringBuilder();

            Text.Append("SELECT ");
            Text.Append("[S].[Inventory], ");
            Text.Append("[I].[Title], ");
            Text.Append("[I].[Subcategory], ");
            Text.Append("[C].[Title] AS [Category], ");
            Text.Append("[T].[Title] As [Sub - Category], ");
            Text.Append("SUM([S].[PQty]) AS [PQty], ");
            Text.Append("SUM([S].[PAmount]) As [PAmount], ");
            Text.Append("SUM([S].[PTaxAmount]) As [PTaxAmount], ");
            Text.Append("SUM([S].[PNetAmount]) As [PNetAmount], ");
            Text.Append("SUM([S].[SQty]) AS [SQty], ");
            Text.Append("SUM([S].[SAmount]) As [SAmount], ");
            Text.Append("SUM([S].[STaxAmount]) As [STaxAmount], ");
            Text.Append("SUM([S].[SNetAmount]) As [SNetAmount], ");
            Text.Append("SUM([S].[SRQty]) AS [SRQty], ");
            Text.Append("SUM([S].[SRAmount]) As [SRAmount], ");
            Text.Append("SUM([S].[SRTaxAmount]) As [SRTaxAmount], ");
            Text.Append("SUM([S].[SRNetAmount]) As [SRNetAmount] ");
            Text.Append($"FROM ({StockPositionSUM(Filter)}) [S] ");
            Text.Append("LEFT JOIN[Inventory] [I] ON[I].[ID] = [S].[Inventory] ");
            Text.Append("LEFT JOIN[Inv_SubCategory] [T] ON[T].[ID] = [I].[SubCategory] ");
            Text.Append("LEFT JOIN[Inv_Category] [C] ON[C].[ID] = [T].[Category] ");
            Text.Append("GROUP BY[Inventory] ");

            return Text.ToString();

        }
        #endregion

        #region  Check Bill Receivable
        public static string Chk_BillReceivable1()
        {
            // Query show record which has BillReceivable record by not any records in BillReceivable2
            var Text = new StringBuilder();
            Text.Append("SELECT* FROM (");
            Text.Append("SELECT ");
            Text.Append("[B1].[ID] AS[ID1],");
            Text.Append("[B2].[ID] AS[ID2],");
            Text.Append("[B2].[TranID],");
            Text.Append("[B1].[Vou_No],");
            Text.Append("[B1].[Vou_Date] ");
            Text.Append("FROM (SELECT* FROM [BillReceivable] WHERE [Status]= 'Posted') AS [B1]");
            Text.Append(" LEFT JOIN [BillReceivable2] [B2] ON [B2].[TranID] = [B1].[ID]");
            Text.Append(") ");
            Text.Append(" WHERE[ID2] IS NULL");


            return Text.ToString();
        }

        public static string Chk_BillReceivable2()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT");
            Text.Append("[B].[Vou_No][Bill_VNo],");
            Text.Append("[B].[Vou_Date],");
            Text.Append("[B].[Company],");
            Text.Append("[B].[Ref_No],");
            Text.Append("[B].[Amount],");
            Text.Append("[B].[Status],");
            Text.Append("[L].[Vou_No][Led_VNo],");
            Text.Append("[L].[DR],");
            Text.Append("[L].[CR]");

            Text.Append("FROM[BillReceivable][B]");
            Text.Append("LEFT JOIN(SELECT* FROM [Ledger] WHERE Vou_Type = 'Receivable') [L] ON[L].[TranID] = [B].[ID]");
            Text.Append("WHERE[L].[TRanID] IS NULL and[B].[Status] = 'Posted'");

            return Text.ToString();
        }
        #endregion

        #region Bill Payable List
        public static string BillPayableList(string? _Filter)
        {
            _Filter ??= string.Empty;
            var Text = new StringBuilder();
            Text.Append("SELECT[B].*,");
            Text.Append("[C].[Title] AS [CompanyTitle],");
            Text.Append("[E].[Title] AS [EmployeeTitle] ");
            Text.Append("FROM [BillPayable] [B] ");
            Text.Append("LEFT JOIN [Customers] [C] ON [C].[ID] = [B].[Company] ");
            Text.Append("LEFT JOIN [Employees] [E] ON [E].[ID] = [B].[Employee] ");
            if (_Filter.Length > 0)
            {
                Text.Append($"WHERE {_Filter} ");
            }
            return Text.ToString();
        }
        #endregion

        #region Bill Payable Combine (Purchased) View
        public static string view_Purchased()
        {
            var Text = new StringBuilder();

            Text.Append("SELECT [B1].[Vou_No], [B1].[Vou_Date], ");
            Text.Append("[B2].[Inventory], [B2].[Qty], [B2].[Rate], [B2].[Qty] *[B2].[Rate] AS [Amount], ");
            Text.Append("[T].[Rate] AS [TaxRate], ");
            Text.Append("CAST(([B2].[Qty] *[B2].[Rate]) * [T].[Rate] AS Float) AS [TaxAmount] ");
            Text.Append("FROM [BillPayable] [B1] ");
            Text.Append("LEFT JOIN [BillPayable2] [B2] ON [B2].[TranID] = [B1].[ID] ");
            Text.Append("LEFT JOIN [Taxes] [T] ON [T].[ID] = [B2].[Tax]; ");

            return Text.ToString();
        }
        #endregion

        #region Bill Receivable (SOLD) Combine View
        public static string view_Sold()
        {
            var Text = new StringBuilder();

            Text.Append("SELECT [B1].[Vou_No], ");
            Text.Append("[B1].[Vou_Date], ");
            Text.Append("[B2].[Inventory], ");
            Text.Append("[B2].[Qty] * -1 AS [Qty], ");
            Text.Append("[B2].[Rate] * -1 AS [Rate], ");
            Text.Append("[B2].[Qty] *[B2].[Rate] * -1 AS [Amount], ");
            Text.Append("[T].[Rate] *-1 AS [TaxRate], ");
            Text.Append("CAST(([B2].[Qty] *[B2].[Rate]*-1) * ([T].[Rate]*-1) AS Float) AS [TaxAmount] ");
            Text.Append("FROM [BillReceivable] [B1] ");
            Text.Append("LEFT JOIN [BillReceivable2] [B2] ON [B2].[TranID] = [B1].[ID] ");
            Text.Append("LEFT JOIN [Taxes] [T] ON [T].[ID] = [B2].[Tax]; ");

            return Text.ToString();
        }
        #endregion

        #region Opening Balance for Company / Customer / Vendors
        public static string OBALCompany()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT [OB].*, ");
            Text.Append("[C].[Title] AS [CompanyName], ");
            Text.Append("[P].[Title] AS [ProjectTitle], ");
            Text.Append("[E].[Title] AS [EmployeeName] ");
            Text.Append("FROM OBALCompany [OB] ");
            Text.Append("LEFT JOIN [Customers] [C] ON [C].[ID] = [OB].[Company] ");
            Text.Append("LEFT JOIN [Project]       [P] ON [P].[ID] = [OB].[Project] ");
            Text.Append("LEFT JOIN [Employees] [E] ON [E].[ID] = [OB].[Employee] ");
            Text.Append("ORDER BY [OB].[Company]");
            return Text.ToString();
        }

        #endregion

        #region Trial Balance Customer wise
        public static string TBCustomer(string UserName, string _Filter)
        {
            var COAList = AppRegistry.GetText(UserName, "CompanyGLs");

            var Text = new StringBuilder();
            Text.Append("SELECT * FROM ( ");
            Text.Append("SELECT ");
            Text.Append("[Customer],");
            Text.Append("[C].[Title] AS[CustomerName],");
            Text.Append("[COA],");
            Text.Append("[A].[Title] AS[Account],");
            Text.Append("SUM([DR]) AS[DR],");
            Text.Append("SUM([CR]) AS[CR],");
            Text.Append("SUM([DR] -[CR]) AS [BAL] ");
            Text.Append($"FROM (SELECT * FROM [Ledger] WHERE [Customer] > 0 AND {_Filter}) AS [L] ");
            Text.Append("LEFT JOIN [Customers] [C] ON [C].[ID] = [L].[Customer] ");
            Text.Append("LEFT JOIN [COA] [A] ON [A].[ID] = [L].[COA] ");
            Text.Append("GROUP BY [Customer],[COA]");
            Text.Append("ORDER BY [CustomerName],[Account] ) AS [TBCustomer] ");
            Text.Append($"WHERE [COA] IN({COAList})");
            return Text.ToString();
        }
        #endregion


        //------------------------------------------------------------------------------------------ CREATING DATA TABLE AND VIEWS

    }
}

