using System.Data;
using System.Data.SQLite;
using System.Text;
using static Applied_WebApplication.Pages.Accounts.WriteChequeModel;

namespace Applied_WebApplication.Data
{
    public class AppFunctions
    {
        #region New Voucher
        public static string GetNewCashVoucher(string UserName)
        {
            DataTableClass Table = new(UserName, Tables.CashBook);
            if (Table.MyDataTable.Rows.Count == 0) { return "CB-000001"; }
            int MaxNum = int.Parse(Table.MyDataTable.Compute("Max(ID)", "").ToString()) + 1;
            string NewCode = string.Concat("CB-", MaxNum.ToString("000000"));
            return NewCode;
        }

        public static string GetNewBillPayableVoucher(string UserName)
        {
            DataTableClass Table = new(UserName, Tables.BillPayable);
            if (Table.MyDataTable.Rows.Count == 0) { return "BP-000001"; }
            int MaxNum = int.Parse(Table.MyDataTable.Compute("Max(ID)", "").ToString()) + 1;
            string NewCode = string.Concat("BP-", MaxNum.ToString("000000"));
            return NewCode;
        }
        //public static string GetBillReceivableVoucher(string UserName)
        //{
        //    DataTableClass Table = new(UserName, Tables.BillReceivable);
        //    if (Table.MyDataTable.Rows.Count == 0) { return "BR-000001"; }
        //    int MaxNum = int.Parse(Table.MyDataTable.Compute("Max(ID)", "").ToString()) + 1;
        //    string NewCode = string.Concat("BR-", MaxNum.ToString("000000"));
        //    return NewCode;
        //}

        #endregion

        #region Tax Function

        public static string GetTaxCode(string UserName, int TaxID)
        {
            string TaxCode = string.Empty;
            DataTableClass tb_Tax = new(UserName, Tables.Taxes);
            tb_Tax.MyDataView.RowFilter = $"ID={TaxID}";
            if (tb_Tax.MyDataView.Count > 0)
            {
                TaxCode = tb_Tax.MyDataView[0]["Code"].ToString();
            }
            return TaxCode;
        }

        public static decimal GetTaxRate(string UserName, int TaxID)
        {
            decimal TaxRate = 0.00M;
            DataTableClass tb_Tax = new(UserName, Tables.Taxes);
            tb_Tax.MyDataView.RowFilter = $"ID={TaxID}";
            if (tb_Tax.MyDataView.Count > 0)
            {
                TaxRate = (decimal)tb_Tax.MyDataView[0]["Rate"];
            }
            return TaxRate;
        }

        public static int GetTaxCOA(string UserName, int TaxID)
        {
            int TaxCOA = 0;
            DataTableClass tb_Tax = new(UserName, Tables.Taxes);
            tb_Tax.MyDataView.RowFilter = $"ID={TaxID}";
            if (tb_Tax.MyDataView.Count > 0)
            {
                TaxCOA = (int)tb_Tax.MyDataView[0]["COA"];
            }
            return TaxCOA;
        }

        #endregion

        #region Static Function

        public static Chequeinfo GetChequeInfo(string UserName, string ChqCode)
        {
            Pages.Accounts.WriteChequeModel.Chequeinfo Cheque = new();
            DataTableClass Table = new(UserName, Tables.WriteCheques);
            Table.MyDataView.RowFilter = string.Concat("Code='", ChqCode, "'");
            if (Table.MyDataView.Count > 0)
            {
                DataRow Row = Table.MyDataView[0].Row;
                if ((int)Row["TranType"] == 1)
                {
                    Cheque.ID = (int)Row["ID"];
                    Cheque.Code = Row["Code"].ToString();
                    Cheque.TranType = (int)Row["TranType"];
                    Cheque.TranDate = DateTime.Parse(Row["TranDate"].ToString());
                    Cheque.Bank = (int)Row["Bank"];
                    Cheque.Customer = (int)Row["Company"];
                    Cheque.ChqDate = DateTime.Parse(Row["ChqDate"].ToString());
                    Cheque.ChqNo = Row["ChqNo"].ToString();
                    Cheque.ChqAmount = decimal.Parse(Row["ChqAmount"].ToString());
                    Cheque.Status = (int)Row["Status"];
                    Cheque.Project = (int)Row["Project"];
                    Cheque.Employee = (int)Row["Employee"];
                    Cheque.Description = Row["Description"].ToString();
                }

                if (Table.MyDataView[1] != null)
                {
                    Row = Table.MyDataView[1].Row;
                    Cheque.TaxID1 = (int)Row["TaxID"];
                    Cheque.TaxableAmount1 = decimal.Parse(Row["TaxableAmount"].ToString());
                    Cheque.TaxAmount1 = decimal.Parse(Row["TaxAmount"].ToString());
                }

                if (Table.MyDataView[2] != null)
                {
                    Row = Table.MyDataView[2].Row;
                    Cheque.TaxID2 = (int)Row["TaxID"];
                    Cheque.TaxableAmount2 = decimal.Parse(Row["TaxableAmount"].ToString());
                    Cheque.TaxAmount2 = decimal.Parse(Row["TaxAmount"].ToString());
                }
            }
            return Cheque;
        }
        public readonly static AppliedDependency AppGlobals = new();

        // Get Value of any column from any Data Table.
        public static string GetColumnValue(string UserName, Tables _Table, string _Column, int ID)
        {
            if (UserName.Length == 0) { return ""; }
            if (ID == 0) { return ""; }

            string _Text = $"SELECT [{_Column}] From [{_Table}] where ID={ID}";

            SQLiteConnection _Connection = ConnectionClass.AppConnection(UserName);
            SQLiteDataAdapter _Adapter = new(_Text, _Connection);
            DataSet _DataSet = new DataSet();
            _Adapter.Fill(_DataSet);
            if (_DataSet.Tables.Count > 0)
            {
                return _DataSet.Tables[0].Rows[0][0].ToString();
            }
            return "";
        }

        public static DataRow GetRecord(string UserName, Tables _TableName, int id)
        {
            DataTableClass _Table = new(UserName, _TableName);
            _Table.MyDataView.RowFilter = $"ID={id}";
            if (_Table.MyDataView.Count == 1)
            {
                return _Table.MyDataView[0].Row;
            }
            return _Table.NewRecord();
        }

        public static DataTable GetVoucher(string UserName, int TranID, VoucherType VouType)
        {
            var Filter = $"TranID={TranID} AND Vou_Type = '{VouType}'";
            DataTableClass _Table = new(UserName, Tables.Ledger, Filter);
            if (_Table.MyDataTable.Rows.Count >= 2)
            {
                return _Table.MyDataTable;
            }
            return new DataTable();
        }


        public static string GetDate(object _Date)
        {
            return DateTime.Parse(_Date.ToString()).ToString(AppRegistry.FormatDate);
        }

        public static string GetDate(object _Date, string Format)
        {
            return DateTime.Parse(_Date.ToString()).ToString(Format);
        }


        public static DataRow NewRecord(string UserName, Tables _TableName)
        {
            DataTableClass _Table = new(UserName, _TableName);
            return _Table.NewRecord();
        }

        // Get ID and title from Datatable without Filter
        public static Dictionary<int, string> Titles(string UserName, Tables _TableName)
        {
            Dictionary<int, string> Titles = new Dictionary<int, string>();
            DataTableClass _Table = new(UserName, _TableName);
            _Table.MyDataView.Sort = "Title";

            foreach (DataRow _Row in _Table.MyDataView.ToTable().Rows)
            {
                Titles.Add((int)_Row["ID"], (string)_Row["Title"]);
            }
            return Titles;
        }

        // Get ID and title from Datatable with Filter condition.
        public static Dictionary<int, string> Titles(string UserName, Tables _TableName, string _Filter)
        {
            Dictionary<int, string> Titles = new Dictionary<int, string>();

            var Query = $"SELECT ID,Title FROM {_TableName} WHERE {_Filter} Order by Title ";
            var _Table = DataTableClass.GetTable(UserName, Query);
            foreach(DataRow _Row in _Table.Rows)
            {
                Titles.Add((int)_Row["ID"], (string)_Row["Title"]);
            }


            //DataTableClass _Table = new(UserName, _TableName);
            //_Table.MyDataView.RowFilter = _Filter;
            //foreach (DataRow _Row in _Table.MyDataView.ToTable().Rows)
            //{
            //    Titles.Add((int)_Row["ID"], (string)_Row["Title"]);
            //}
            return Titles;
        }

        public static Dictionary<int, string> Titles(string UserName, string _Command)
        {
            Dictionary<int, string> Titles = new Dictionary<int, string>();
            DataTableClass _Table = new(UserName, _Command);
            foreach (DataRow _Row in _Table.MyDataView.ToTable().Rows)
            {
                Titles.Add((int)_Row["ID"], (string)_Row["Title"]);
            }
            return Titles;
        }

        // Get DatRows from DataTable filter by ID=??
        public static DataRow GetDataRow(string UserName, Tables _TableName, int ID)
        {
            DataTableClass _Table = new(UserName, _TableName);
            _Table.MyDataView.RowFilter = $"ID={ID}";
            if (_Table.MyDataView.Count == 1)
            { return _Table.MyDataView[0].Row; }
            else { return _Table.MyDataTable.NewRow(); }
        }

        // Get Dictionery of ID and title from DataTable for Dropdown List in Razor Pages.
        public static Dictionary<int, string> GetList(string UserName, Tables _TableName, string Filter)
        {
            Dictionary<int, string> _List = new();
            DataTableClass _Table = new(UserName, _TableName);
            _Table.MyDataView.RowFilter = Filter;
            DataTable _TempTable = _Table.MyDataView.ToTable();

            foreach (DataRow Row in _TempTable.Rows)
            {
                _List.Add((int)Row["ID"], Row["Title"].ToString());
            }
            return _List;
        }


        // Get Singal Title from DataTable by ID=??
        public static string GetTitle(string UserName, Tables _TableName, int ID)
        {
            if (ID.Equals(DBNull.Value)) { ID = 0; }
            string _Title;  //= string.Empty;
            DataTableClass _Table = new(UserName, _TableName);
            _Table.MyDataView.RowFilter = $"ID={ID}";
            if (_Table.MyDataView.Count > 0)
            {
                _Title = _Table.MyDataView[0]["Title"].ToString();
            }
            else
            {
                _Title = "Select...";
            }
            return _Title;
        }

        #endregion

        #region Compute

        //public static int GetMax(string UserName, Tables _Table, string _Column, string _Filter)
        //{
        //    DataTableClass Table = new(UserName, _Table);
        //    Table.MyDataView.RowFilter = _Filter;
        //    if (Table.MyDataView.Count == 0) { return 0; }
        //    else { return (int)Table.MyDataTable.Compute("Max(" + _Column + ")", _Filter); }
        //}

        public static object GetSum(string UserName, Tables _Table, string _Column, string _Filter)
        {
            DataTableClass Table = new(UserName, _Table);
            Table.MyDataView.RowFilter = _Filter;
            if (Table.MyDataView.Count == 0) { return 0; }
            else
            {
                var _sum = Table.MyDataTable.Compute("Sum(" + _Column + ")", _Filter);
                return decimal.Parse(_sum.ToString());
            }
        }

        internal static DataRow GetNewRow(string UserName, Tables _Table)
        {
            DataTableClass Table = new(UserName, _Table);
            return Table.NewRecord();
        }

        public static DateTime MinDate(DateTime Date1, DateTime Date2)
        {
            DateTime[] Dates = new DateTime[] { Date1, Date2 };
            return Dates.Min(Dates => Dates.Date);
        }

        public static DateTime MaxDate(DateTime Date1, DateTime Date2)
        {
            DateTime[] Dates = new DateTime[] { Date1, Date2 };
            return Dates.Max(Dates => Dates.Date);
        }

        #endregion

        public static string DateFromTo(DateTime? _Date1, DateTime? _Date2, string _DateFormat)
        {
            StringBuilder Text = new();



            if (_Date1 != null)
            {
                Text.Append($"From {((DateTime)_Date1).ToString(_DateFormat)} ");
            }

            if (_Date2 != null)
            {
                Text.Append($"To {((DateTime)_Date2).ToString(_DateFormat)}");
            }
            return Text.ToString();
        }


    }
}
