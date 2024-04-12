using System.Data;
using System.Text;

namespace Applied_WebApplication.Data
{
    public class StockLedgersClass
    {
        public int StockID { get; set; }
        public string StockCOA { get; set; }
        public string Filter { get; set; }
        public string UserName { get; set; }

        #region Constructor
        public StockLedgersClass(string _UserName)
        {
            UserName = _UserName;
            StockCOA = AppRegistry.GetText(UserName, "Stock_COA");
            Filter = AppRegistry.GetText(UserName, "sp-Filter");
        }
        #endregion

        #region Stock Opening Balance
        private string LedgerOB()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM [Ledger] ");
            Text.Append($"WHERE [Vou_Type] = 'OBalStock'");
            return Text.ToString();
        }
        private string StockOB()
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
        public string CombineOB()
        {
            var Text = new StringBuilder();
            Text.Append($"SELECT 1 AS [No], {ReportColumns()}");
            Text.Append($"FROM ({LedgerOB()}) AS [L]");
            Text.Append($"LEFT JOIN ({StockOB()}) [S] ON [S].[ID] = [L].[TranID]");
            return Text.ToString();

        }

        #endregion

        #region Stock from Bill Payable (Purchased)
        private string LedgerPayable()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM (");
            Text.Append(StockLedger());
            Text.Append($") WHERE Vou_Type='Payable'");
            return Text.ToString();

        }
        private string StockPayable()
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
        public string CombinePayable()
        {
            var Text = new StringBuilder();
            Text.Append($"SELECT 2 AS [No], {ReportColumns()}");
            Text.Append($"FROM ({StockPayable()}) AS [S]");
            Text.Append($"LEFT JOIN ({LedgerPayable()}) [L] ON [S].[TranID] = [L].[TranID]");
            return Text.ToString();
        }
        #endregion

        #region Stock from Bill Receivable (Sale Invoice)

        public string LedgerSale()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM (");
            Text.Append(StockLedger());
            Text.Append($") WHERE Vou_Type='Receivable'");
            return Text.ToString();
        }

        private string StockSale()
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
            Text.Append("([S2].[Qty] - ");
            Text.Append("CASE WHEN [R].[QTY] IS NOT NULL THEN [R].[QTY] ELSE 0 END) AS [Qty] ,");
            Text.Append("[S2].[Rate],");
            Text.Append("[T].[Rate] As [TaxRate],");
            Text.Append("(([S2].[Qty] - [R].[Qty]) * [S2].[Rate]) AS [Amount],");
            Text.Append("(([S2].[Qty] - [R].[Qty]) * [S2].[Rate]) * [T].[Rate] As [TaxAmount]");
            Text.Append("FROM [BillReceivable2] [S2]");
            Text.Append("LEFT JOIN [BillReceivable] [S1] ON [S1].[ID] = [S2].[TranID]");
            Text.Append("LEFT JOIN [Taxes]          [T]  ON [T].[ID]  = [S2].[Tax]");
            Text.Append("LEFT JOIN [SaleReturn]     [R]  ON [S2].[ID] = [R].[ID]");
            return Text.ToString();
        }
        public string CombineSale()
        {
            var Text = new StringBuilder();
            Text.Append($"SELECT 3 AS [No], {ReportColumns()}");
            Text.Append($"FROM ({StockSale()}) AS [S]");
            Text.Append($"LEFT JOIN ({LedgerSale()}) [L] ON [S].[TranID] = [L].[TranID]");
            return Text.ToString();

        }

        #endregion

        #region Stock from Production Process

        public string LedgerProduction()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT * FROM (");
            Text.Append(StockLedger());
            Text.Append($") WHERE Vou_Type='Production'");
            return Text.ToString();
        }
        public string StockProduction()
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
        public string CombineProduction()
        {
            var Text = new StringBuilder();
            Text.Append($"SELECT 4 AS [No], {ReportColumns()}");
            Text.Append($"FROM ({StockProduction()}) AS [S]");
            Text.Append($"LEFT JOIN ({LedgerProduction()}) [L] ON [S].[TranID] = [L].[TranID]");
            return Text.ToString();

        }

        #endregion

        #region Ledger

        private string StockLedger()
        {
            var Text = new StringBuilder();
            Text.Append($"SELECT {LedgerColumns()}");
            Text.Append("FROM (");
            Text.Append($"SELECT * FROM [Ledger] WHERE {Filter}) AS [L] ");
            Text.Append($"WHERE [L].[COA] IN ({StockCOA}) ");
            Text.Append("GROUP BY [L].[Vou_No]");
            return Text.ToString();
        }
        private string LedgerColumns()
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
        private string ReportColumns()
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
        public string SQL_StockLedger()
        {
            var _CombineOB = CombineOB();
            var _CombinePayable = CombinePayable();
            var _CombineSale = CombineSale();
            var _CombineProduction = CombineProduction();

            var Text = new StringBuilder();
            Text.Append("SELECT * FROM (");
            Text.Append(_CombineOB);
            Text.Append(" UNION ");
            Text.Append(_CombinePayable);
            Text.Append(" UNION ");
            Text.Append(_CombineSale);
            Text.Append(" UNION ");
            Text.Append(_CombineProduction);
            Text.Append(") WHERE Vou_No not null ");
            Text.Append(" ORDER BY [StockID],[Vou_Date],[No]");
            return Text.ToString();

        }

        public DataTable GetStockLedger()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT [S].*,[I].[Title] AS [Title] FROM (");
            Text.Append(SQL_StockLedger());
            Text.Append(") AS [S]");
            Text.Append($"LEFT JOIN [Inventory] [I] ");
            Text.Append($"ON [I].[ID] = [S].[StockID] ");
            Text.Append($"WHERE [S].[StockID] = {StockID} ");
            Text.Append($"ORDER BY [Vou_Date],[No] ");

            var _StockTable = DataTableClass.GetTable(UserName, Text.ToString());

            return _StockTable;
        }

        public DataTable GetStockInHand()
        {

            var Text = new StringBuilder();

            Text.Append("SELECT ");
            Text.Append("[S].[StockID], ");
            Text.Append("[I].[Title], ");
            Text.Append("SUM([S].[Qty]) AS [Qty], ");
            Text.Append("SUM([S].[Amount]) AS [Amount], ");
            Text.Append("SUM([S].[TaxAmount]) AS [TaxAmount], ");
            Text.Append("SUM([S].[NetAmount]) AS [NetAmount] ");
            Text.Append($"FROM ({SQL_StockLedger()}");
            Text.Append(") AS [S] ");
            Text.Append("LEFT JOIN [Inventory] AS [I] ON [I].[ID] = [S].[StockID] ");
            Text.Append("WHERE [S].[Qty] IS NOT NULL ");
            Text.Append("GROUP BY [StockID]");
            return DataTableClass.GetTable(UserName, Text.ToString());

        }
        #endregion

        #region Generate Stock Ledger
        public DataTable CreateLedgerTable()
        {
            var _Table = new DataTable();
            _Table.Columns.Add("StockID", typeof(int));
            _Table.Columns.Add("Vou_No", typeof(string));
            _Table.Columns.Add("Vou_Date", typeof(DateTime));
            _Table.Columns.Add("Title", typeof(string));
            _Table.Columns.Add("OBQty", typeof(decimal));
            _Table.Columns.Add("OBAmount", typeof(decimal));
            _Table.Columns.Add("PRQty", typeof(decimal));
            _Table.Columns.Add("PRAmount", typeof(decimal));
            _Table.Columns.Add("SLQty", typeof(decimal));
            _Table.Columns.Add("SLAmount", typeof(decimal));
            _Table.Columns.Add("PDQty", typeof(decimal));
            _Table.Columns.Add("PDAmount", typeof(decimal));
            _Table.Columns.Add("NetQty", typeof(decimal));
            _Table.Columns.Add("NetAmount", typeof(decimal));
            _Table.Columns.Add("AvgRate", typeof(decimal));
            _Table.Columns.Add("SoldCost", typeof(decimal));

            return _Table;



        }
        public DataTable GenerateLedgerTable()
        {

            var StockLedgerDetails = GetStockLedger();
            var StockLedger = CreateLedgerTable();
            var Tot_Qty = 0.00M;
            var Tot_Amount = 0.00M;


            foreach (DataRow Row in StockLedgerDetails.Rows)
            {
                var _Row = StockLedger.NewRow();
                _Row["StockID"] = (int)Row["StockID"];
                _Row["Vou_No"] = (string)Row["Vou_No"];
                _Row["Vou_Date"] = (DateTime)Row["Vou_Date"];
                _Row["Title"] = (string)Row["Title"];

                if (Row["Qty"] == DBNull.Value) { Row["Qty"] = 0.00M; }
                if (Row["Amount"] == DBNull.Value) { Row["Amount"] = 0.00M; }
                
                var _Qty = Math.Round(decimal.Parse(Row["Qty"].ToString()), 2);
                var _Amount = Math.Round(decimal.Parse(Row["Amount"].ToString()), 2);

                #region Quantity and Amount
                if ((string)Row["Vou_Type"] == "OBStock")
                {
                    _Row["OBQty"] = _Qty;
                    _Row["OBAmount"] = _Amount;

                    Tot_Qty = Tot_Qty + _Qty;
                    Tot_Amount = Tot_Amount + _Amount;

                }

                if ((string)Row["Vou_Type"] == "Payable")
                {
                    _Row["PRQty"] = _Qty;
                    _Row["PRAmount"] = _Amount;

                    Tot_Qty = Tot_Qty + _Qty;
                    Tot_Amount = Tot_Amount + _Amount;

                }

                if ((string)Row["Vou_Type"] == "Receivable")
                {
                    _Row["SLQty"] = _Qty;
                    _Row["SLAmount"] = _Amount;

                    var _AvgRate = Tot_Amount / Tot_Qty;
                    var _Cost = _AvgRate * _Qty;
                    _Row["SoldCost"] = _Cost;

                    Tot_Qty = Tot_Qty - _Qty;
                    Tot_Amount = Tot_Amount - _Cost;

                }

                if ((string)Row["Vou_Type"] == "Production")
                {
                    _Row["PDQty"] = _Qty;
                    _Row["PDAmount"] = _Amount;

                    Tot_Qty = Tot_Qty + _Qty;
                    Tot_Amount = Tot_Amount + _Amount;

                }
                #endregion

                #region Remove DBNull.Vales
                if (_Row["OBQty"] == DBNull.Value) { _Row["OBQty"] = 0.00M; }
                if (_Row["PRQty"] == DBNull.Value) { _Row["PRQty"] = 0.00M; }
                if (_Row["SLQty"] == DBNull.Value) { _Row["SLQty"] = 0.00M; }
                if (_Row["PDQty"] == DBNull.Value) { _Row["PDQty"] = 0.00M; }
                if (_Row["OBAmount"] == DBNull.Value) { _Row["OBAmount"] = 0.00M; }
                if (_Row["PRAmount"] == DBNull.Value) { _Row["PRAmount"] = 0.00M; }
                if (_Row["SLAmount"] == DBNull.Value) { _Row["SLAmount"] = 0.00M; }
                if (_Row["PDAmount"] == DBNull.Value) { _Row["PDAmount"] = 0.00M; }
                if (_Row["SoldCost"] == DBNull.Value) { _Row["SoldCost"] = 0.00M; }
                #endregion




                _Row["NetQty"] = Math.Round(((decimal)_Row["OBQty"] + (decimal)_Row["PRQty"] - (decimal)_Row["SLQty"] + (decimal)_Row["PDQty"]), 2);
                _Row["NetAmount"] = Math.Round(((decimal)_Row["OBAmount"] + (decimal)_Row["PRAmount"] - (decimal)_Row["SLAmount"] + (decimal)_Row["PDAmount"]), 2);
                 


                if (Tot_Qty > 0 || Tot_Amount > 0)
                {
                    _Row["AvgRate"] = Math.Round(Tot_Amount / Tot_Qty, 2);
                }
                else
                {
                    _Row["AvgRate"] = 0.00M;
                }



                StockLedger.Rows.Add(_Row);

            }
            return StockLedger;

        }
        #endregion
    }
}
