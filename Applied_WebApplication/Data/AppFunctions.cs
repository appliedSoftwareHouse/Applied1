using System.Data;
using System.Data.SQLite;
using System.Text;
using AppReporting;
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

        internal static string GetBillReceivableVoucher(string UserName)
        {
            DataTableClass Table = new(UserName, Tables.BillReceivable);
            if (Table.MyDataTable.Rows.Count == 0) { return "BR-000001"; }
            int MaxNum = int.Parse(Table.MyDataTable.Compute("Max(ID)", "").ToString()) + 1;
            string NewCode = string.Concat("BR-", MaxNum.ToString("000000"));
            return NewCode;
        }

        #endregion

        #region Tax Function

        public static string GetTaxCode(string UserName, int TaxID)
        {
            string TaxCode = string.Empty;
            DataTableClass tb_Tax = new(UserName, Tables.Taxes);
            tb_Tax.MyDataView.RowFilter = "ID=" + TaxID.ToString();
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
            tb_Tax.MyDataView.RowFilter = "ID=" + TaxID.ToString();
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
            tb_Tax.MyDataView.RowFilter = "ID=" + TaxID.ToString();
            if (tb_Tax.MyDataView.Count > 0)
            {
                TaxCOA = (int)tb_Tax.MyDataView[0]["COA"];
            }
            return TaxCOA;
        }

        #endregion

        #region Static Function
        public static string GetUserValue(string UserName, string _Column)
        {
            AppliedUsersClass UserClass = new();
            DataRow _Row = UserClass.UserRecord(UserName);
            return _Row[_Column].ToString();
        }
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

        public static DataTable GetAppliedTable(string UserName, ReportClass.ReportFilters ReportFilter)
        {
            string CommandText = GetQueryText(ReportFilter);
            ConnectionClass _Connection = new(UserName);
            SQLiteDataAdapter _Adapter = new(CommandText, _Connection.AppliedConnection);
            DataSet _DataSet = new DataSet();
            _Adapter.Fill(_DataSet);
            if (_DataSet.Tables.Count > 0)
            {
                return _DataSet.Tables[0];
            }
            return new DataTable();

        }
        public static string GetQueryText(ReportClass.ReportFilters ReportFilter)
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

            if (ReportFilter.Dt_From < new DateTime(2000, 1, 1)) { _Where.Append("Vou_Date>=" + ReportFilter.Dt_From.ToString(DateFormat)); }
            if (ReportFilter.Dt_To < new DateTime(2000, 1, 1)) { _Where.Append(" Vou_Date<=" + ReportFilter.Dt_To.ToString(DateFormat)); }
            if (ReportFilter.N_ID != 0) { _Where.Append(" [ID]=" + ReportFilter.N_ID.ToString() + " AND"); }
            if (ReportFilter.N_COA != 0) { _Where.Append(" [COA]=" + ReportFilter.N_COA.ToString() + " AND"); }
            if (ReportFilter.N_Customer != 0) { _Where.Append(" [Customer]=" + ReportFilter.N_Customer.ToString() + " AND"); }
            if (ReportFilter.N_Project != 0) { _Where.Append(" [Project]=" + ReportFilter.N_Project.ToString() + " AND"); }
            if (ReportFilter.N_Employee != 0) { _Where.Append(" [Employee]=" + ReportFilter.N_Employee.ToString() + " AND"); }
            if (ReportFilter.N_InvCategory != 0) { _Where.Append(" [Category]=" + ReportFilter.N_InvCategory.ToString() + " AND"); }
            if (ReportFilter.N_InvSubCategory != 0) { _Where.Append(" [SubCategory]=" + ReportFilter.N_InvSubCategory.ToString() + " AND"); }
            if (ReportFilter.N_Inventory != 0) { _Where.Append(" [Inventory]=" + ReportFilter.N_Inventory.ToString() + " AND"); }
            if (_Where.Length > 0) { _Where.Insert(0, " WHERE "); }
            string Where = _Where.ToString();
            //if (Where.EndsWith("AND")) { Where = Where.Substring(0, Where.Length - 3); }
            if (Where.EndsWith("AND")) { Where = Where[..^3]; }

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
        // Get Data Rows from DataTable by filter conditions.
        public static DataTable GetRecords(string UserName, Tables _TableName, string _Filter)
        {
            DataTableClass _Table = new(UserName, _TableName);
            _Table.MyDataView.RowFilter = _Filter;
            return _Table.MyDataView.ToTable();
        }

        public static DataRow GetRecord(string UserName, Tables _TableName, int id)
        {
            DataTableClass _Table = new(UserName, _TableName);
            _Table.MyDataView.RowFilter = String.Concat("ID=", id.ToString());
            if (_Table.MyDataView.Count == 1)
            {
                return _Table.MyDataView[0].Row;
            }
            return _Table.NewRecord();
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

        // Get ID and title from Datatable with Filter condition
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

        // Get DatRows from DataTable filter by ID=??
        public static DataRow GetDataRow(string UserName, Tables _TableName, int ID)
        {
            DataTableClass _Table = new(UserName, _TableName);
            _Table.MyDataView.RowFilter = string.Concat("ID=", ID.ToString());
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


        // Get New Voucher No for White Cheque List
        public static string GetNewChqCode()
        {
            // Temporary codes. in future it has to be developed.
            string _ChqCode = "<<New>>";
            return _ChqCode;
        }




        #endregion

        #region Compute

        public static int GetMax(string UserName, Tables _Table, string _Column, string _Filter)
        {
            DataTableClass Table = new(UserName, _Table);
            Table.MyDataView.RowFilter = _Filter;
            if (Table.MyDataView.Count == 0) { return 0; }
            else { return (int)Table.MyDataTable.Compute("Max(" + _Column + ")", _Filter); }
        }

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


        #endregion


        #region Temporary Local Database
        internal static SQLiteConnection GetTempConnection(string UserName)
        {
            StringBuilder TempConnectionString = new();


            //TempConnectionString.Append("Data Source=");
            TempConnectionString.Append(AppGlobals.LocalDB);
            TempConnectionString.Append(UserName);
            string _Directory = TempConnectionString.ToString();                            // Get Temp Full Path;
            TempConnectionString.Append("\\" + UserName + "DB.temp");                     // Get Full Path and File Name;
            string _FileName = TempConnectionString.ToString();

            if (!Directory.Exists(_Directory)) { Directory.CreateDirectory(_Directory); }
            if (!File.Exists(_FileName)) { SQLiteConnection.CreateFile(_FileName); }

            SQLiteConnection _TempConnection = new("Data Source=" + _FileName);
            _TempConnection.Open();

            return _TempConnection;
        }



        #endregion




    }
}
