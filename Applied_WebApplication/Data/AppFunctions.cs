using System.Data.SQLite;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace Applied_WebApplication.Data
{
    public class AppFunctions
    {

        #region Static Function
        
        public static AppliedDependency AppGlobals = new();

        public static DataTable GetAppliedTable(string UserName, ReportClass.Filters ReportFilter)
        {
            string _Text = GetQueryText(ReportFilter);
            ConnectionClass _Connection = new(UserName);
            SQLiteDataAdapter _Adapter = new(_Text, _Connection.AppliedConnection);
            DataSet _DataSet = new DataSet();
            _Adapter.Fill(_DataSet);
            if (_DataSet.Tables.Count > 0)
            {
                return _DataSet.Tables[0];
            }
            return new DataTable();

        }

        public static string GetQueryText(ReportClass.Filters ReportFilter)
        {
            // Create a query to fatch data from Data Table for Display Reports.
            string DateFormat = "yyyy-MM-dd";
            if (ReportFilter == null) { return string.Empty; }                           // Return empty value if object is null
            string _TableName = ReportFilter.TableName.ToString();
            string _TableColumns = ReportFilter.Columns;
            StringBuilder _SQL = new();
            StringBuilder _Where = new();

            _SQL.Append("SELECT ");
            _SQL.Append(_TableColumns);
            _SQL.Append(" FROM [" + _TableName + "]");

            if (ReportFilter.dt_From < new DateTime(2000, 1, 1)) { _Where.Append("Vou_Date>=" + ReportFilter.dt_From.ToString(DateFormat)); }
            if (ReportFilter.dt_To < new DateTime(2000, 1, 1)) { _Where.Append(" Vou_Date<=" + ReportFilter.dt_To.ToString(DateFormat)); }
            if (ReportFilter.n_ID != 0) { _Where.Append(" [ID]=" + ReportFilter.n_ID.ToString() + " AND"); }
            if (ReportFilter.n_COA != 0) { _Where.Append(" [COA]=" + ReportFilter.n_COA.ToString() + " AND"); }
            if (ReportFilter.n_Customer != 0) { _Where.Append(" [Customer]=" + ReportFilter.n_Customer.ToString() + " AND"); }
            if (ReportFilter.n_Project != 0) { _Where.Append(" [Project]=" + ReportFilter.n_Project.ToString() + " AND"); }
            if (ReportFilter.n_Employee != 0) { _Where.Append(" [Employee]=" + ReportFilter.n_Employee.ToString() + " AND"); }
            if (ReportFilter.n_InvCategory != 0) { _Where.Append(" [Category]=" + ReportFilter.n_InvCategory.ToString() + " AND"); }
            if (ReportFilter.n_InvSubCategory != 0) { _Where.Append(" [SubCategory]=" + ReportFilter.n_InvSubCategory.ToString() + " AND"); }
            if (ReportFilter.n_Inventory != 0) { _Where.Append(" [Inventory]=" + ReportFilter.n_Inventory.ToString() + " AND"); }
            if (_Where.Length > 0) { _Where.Insert(0, " WHERE "); }
            string Where = _Where.ToString();
            if (Where.EndsWith("AND")) { Where = Where.Substring(0, Where.Length - 3); }

            return string.Concat(_SQL.ToString(), Where);
        }

        // Get Value of any column from any Data Table.
        public static string GetColumnValue(string UserName, Tables _Table, string _Column, int ID)
        {
            if (UserName == null) { return ""; }
            if (ID == 0) { return ""; }
            string _Text = string.Concat("SELECT [", _Column, "] From [", _Table, "] where ID=", ID.ToString());
            ConnectionClass _Connection = new(UserName);
            SQLiteDataAdapter _Adapter = new(_Text, _Connection.AppliedConnection);
            DataSet _DataSet = new DataSet();
            _Adapter.Fill(_DataSet);
            if (_DataSet.Tables.Count > 0)
            {
                return _DataSet.Tables[0].Rows[0][0].ToString();
            }
            return "";
        }

        public static DataTable ConvertLedger(string UserName, DataTable _Table)
        {
            DataTableClass Ledger = new(UserName, Tables.view_Ledger);
            DataTable _Ledger = Ledger.MyDataTable.Clone();

            if (_Table.TableName == "CashBook")
            {
                DataView _DataView = _Table.AsDataView();
                _DataView.Sort = "Vou_Date";

                decimal Debit = 0M;
                decimal Credit = 0M;
                decimal Balance = 0M;

                foreach (DataRow _Row in _DataView.ToTable().Rows)
                {
                    Debit = decimal.Parse(_Row["DR"].ToString());
                    Credit = decimal.Parse(_Row["CR"].ToString());
                    Balance += (Debit - Credit);

                    DataRow _NewRow = _Ledger.NewRow();
                    _NewRow["ID"] = _Row["ID"];
                    _NewRow["Vou_Type"] = "Cash";
                    _NewRow["Vou_Date"] = _Row["Vou_Date"];
                    _NewRow["Vou_No"] = _Row["Vou_No"];
                    _NewRow["Description"] = _Row["Description"];
                    _NewRow["DR"] = _Row["DR"];
                    _NewRow["CR"] = _Row["CR"];
                    _NewRow["BAL"] = Balance;
                    _Ledger.Rows.Add(_NewRow);
                }
            }
            return _Ledger;
        }

        public static DataTable GetRecords(string UserName, Tables _TableName, string _Filter)
        {
            DataTableClass _Table = new(UserName, _TableName);
            _Table.MyDataView.RowFilter = _Filter;
            return _Table.MyDataView.ToTable();
        }

        public static DataRow NewRecord(string UserName, Tables _TableName)
        {
            DataTableClass _Table = new(UserName, _TableName);
            return _Table.NewRecord();
        }

        public static Dictionary<int, string> Titles(string UserName, Tables _TableName)
        {
            Dictionary<int, string> Titles = new Dictionary<int, string>();
            DataTableClass _Table = new(UserName, _TableName);
            foreach (DataRow _Row in _Table.MyDataTable.Rows)
            {
                Titles.Add((int)_Row["ID"], (string)_Row["Title"]);
            }
            return Titles;
        }

        public static Dictionary<int, string> Titles(string UserName, Tables _TableName, string _Filter)
        {
            Dictionary<int, string> Titles = new Dictionary<int, string>();
            DataTableClass _Table = new(UserName, _TableName);
            _Table.MyDataView.RowFilter = _Filter;
            foreach (DataRow _Row in _Table.MyDataView.ToTable().Rows)
            {
                Titles.Add((int)_Row["ID"], (string)_Row["Title"]);
            }
            return Titles;
        }

        public static DataRow GetDataRow(string UserName, Tables _TableName, int ID)
        {
            DataTableClass _Table = new(UserName, _TableName);
            _Table.MyDataView.RowFilter = string.Concat("ID=", ID.ToString());
            if (_Table.MyDataView.Count > 0)
            { return _Table.MyDataTable.Rows[0]; }
            else { return _Table.MyDataTable.NewRow(); }
        }

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

        public static string GetTitle(string UserName, Tables _TableName, int ID)
        {
            string _Title;  //= string.Empty;
            DataTableClass _Table = new(UserName, _TableName);
            _Table.MyDataView.RowFilter = string.Concat("ID=", ID.ToString());
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

        public static string GetNewChqCode()
        {
            string _ChqCode = "<<New>>";
            return _ChqCode;
        }

        private static int NewID(DataTable table)
        {
            if (table.Rows.Count > 0)
            {
                int _result = (int)table.Compute("MAX(ID)", "") + 1;
                return _result;
            }
            else { return 1; }
        }


        #endregion
    }
}
