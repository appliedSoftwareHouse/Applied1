﻿using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Data.SQLite;
using System.Security.Principal;
using System.Text;
using Applied_WebApplication.Data;
using System.ServiceModel.Security;
using static Applied_WebApplication.Data.ReportClass;
using System.Reflection.Metadata;

namespace Applied_WebApplication.Data
{

    public class DataTableClass
    {
        public string MyUserName { get; set; }
        private ConnectionClass MyConnectionClass = new();
        public DataTable MyDataTable;
        public DataView MyDataView;
        public SQLiteConnection MyConnection;
        public TableValidationClass TableValidation;
        public string MyTableName;
        public bool IsError = false;
        public string MyMessage = "";
        public string View_Filter { get; set; } = "";
        public DataRow CurrentRow { get; set; }

        private SQLiteCommand Command_Update;
        private SQLiteCommand Command_Delete;
        private SQLiteCommand Command_Insert;


        public DataTableClass(string _UserName, Tables _Tables)
        {
            MyUserName = _UserName;
            MyConnectionClass = new(MyUserName);
            MyConnection = MyConnectionClass.AppliedConnection;

            MyTableName = _Tables.ToString();
            GetDataTable();                                                                                   // Load DataTable and View
            MyDataView.RowFilter = View_Filter;                                                  // Set a view filter for table view.
            CheckError();

            Command_Update = new SQLiteCommand(MyConnection);
            Command_Delete = new SQLiteCommand(MyConnection);
            Command_Insert = new SQLiteCommand(MyConnection);
        }

        public DataTableClass(string _UserName, string _TableName)
        {
            MyUserName = _UserName;
            MyConnectionClass = new(MyUserName);
            MyConnection = MyConnectionClass.AppliedConnection;

            MyTableName = _TableName;
            GetDataTable();                                                                                   // Load DataTable and View
            MyDataView.RowFilter = View_Filter;                                                  // Set a view filter for table view.
            CheckError();

            Command_Update = new SQLiteCommand(MyConnection);
            Command_Delete = new SQLiteCommand(MyConnection);
            Command_Insert = new SQLiteCommand(MyConnection);

        }

        public DataTableClass(string _TableName)
        {
            MyConnection = MyConnectionClass.AppliedConnection;
            MyTableName = _TableName;
            GetDataTable();                                                                                   // Load DataTable and View
            MyDataView.RowFilter = View_Filter;                                                  // Set a view filter for table view.
            CheckError();

            Command_Update = new SQLiteCommand(MyConnection);
            Command_Delete = new SQLiteCommand(MyConnection);
            Command_Insert = new SQLiteCommand(MyConnection);
        }

        public DataTableClass(string _TableName, int _ID)                               // Loan DataTablle and load current row specific by Table Row ID.
        {
            MyConnection = MyConnectionClass.AppliedConnection;
            MyTableName = _TableName;
            GetDataTable();                                                                                   // Load DataTable and View
            MyDataView.RowFilter = View_Filter;                                                  // Set a view filter for table view.
            CheckError();
            Command_Update = new SQLiteCommand(MyConnection);
            Command_Delete = new SQLiteCommand(MyConnection);
            Command_Insert = new SQLiteCommand(MyConnection);
            SeekRecord(_ID);
        }

        public DataRow NewRecord()
        {
            CurrentRow = MyDataTable.NewRow();

            foreach (DataColumn _Column in CurrentRow.Table.Columns)                                                                // DBNull remove and assign a Data Type Empty Value.
            {
                if (CurrentRow[_Column.ColumnName] == DBNull.Value)
                {
                    if (_Column.DataType.Name == "String") { CurrentRow[_Column.ColumnName] = ""; }
                    if (_Column.DataType.Name == "Int32") { CurrentRow[_Column.ColumnName] = 0; }
                    if (_Column.DataType.Name == "Decimal") { CurrentRow[_Column.ColumnName] = 0.00; }
                }
            }
            return CurrentRow;
        }

        #region Commands INSERT / UPDATE / DELETE

        private void CommandInsert()
        {
            DataColumnCollection _Columns = MyDataTable.Columns;
            SQLiteCommand _Command = new SQLiteCommand(MyConnection);

            StringBuilder _CommandString = new StringBuilder();
            string _LastColumn = _Columns[_Columns.Count - 1].ColumnName.ToString();
            string _TableName = MyTableName;
            string _ParameterName;

            _CommandString.Append("INSERT INTO [");
            _CommandString.Append(_TableName);
            _CommandString.Append("] VALUES (");

            foreach (DataColumn _Column in _Columns)
            {
                string _ColumnName = _Column.ColumnName.ToString();
                _CommandString.Append(string.Concat("@", _Column.ColumnName));
                if (_ColumnName != _LastColumn)
                { _CommandString.Append(","); }
                else
                { _CommandString.Append(") "); }
            }

            //_CommandString.Remove(_CommandString.ToString().Trim().Length - 1, 1);
            _Command.CommandText = _CommandString.ToString();

            foreach (DataColumn _Column in _Columns)
            {
                if (_Column == null) { continue; }
                _ParameterName = string.Concat("@", _Column.ColumnName.Replace(" ", ""));
                _Command.Parameters.AddWithValue(_ParameterName, CurrentRow[_Column.ColumnName]);
            }

            Command_Insert = _Command;
            Command_Insert.Parameters["@ID"].Value = NewID(CurrentRow.Table);

        }

        public void Delete()
        {
            CommandDelete();
            int records = Command_Delete.ExecuteNonQuery();
            if (records == 1)
            {
                MyMessage = string.Concat(records.ToString(), " has been deleted.");
            }
            if (records > 1)
            {
                MyMessage = string.Concat(records.ToString(), " have been deleted.");
            }
        }

        private static int NewID(DataTable table)
        {
            int _result = 0;
            string MaxID = string.Empty;
            _result = (int)table.Compute("MAX(ID)", "") + 1;
            return _result;
        }

        private void CommandUpdate()
        {
            DataColumnCollection _Columns = CurrentRow.Table.Columns;
            SQLiteCommand _Command = new SQLiteCommand(MyConnection);

            StringBuilder _CommandString = new StringBuilder();
            string _LastColumn = _Columns[_Columns.Count - 1].ColumnName.ToString();
            string _TableName = MyTableName;
            string _ParameterName;

            _CommandString.Append(string.Concat("UPDATE [", _TableName, "] SET "));

            foreach (DataColumn _Column in _Columns)
            {
                if (_Column.ColumnName == "ID") { continue; }
                _CommandString.Append(String.Concat("[", _Column.ColumnName, "]"));
                _CommandString.Append("=");
                _CommandString.Append(String.Concat("@", _Column.ColumnName.Replace(" ", "")));

                if (_Column.ColumnName == _LastColumn)
                {
                    _CommandString.Append(String.Concat(" WHERE ID = @ID"));
                }
                else
                {
                    _CommandString.Append(",");
                }
            }

            _Command.CommandText = _CommandString.ToString();
            Command_Update = _Command;




            foreach (DataColumn _Column in _Columns)
            {
                if (_Column == null) { continue; }
                _ParameterName = string.Concat("@", _Column.ColumnName.Replace(" ", ""));
                _Command.Parameters.AddWithValue(_ParameterName, CurrentRow[_Column.ColumnName]);
            }

        }
        private void CommandDelete()
        {
            Command_Delete.Parameters.AddWithValue("@ID", CurrentRow["ID"]);
            Command_Delete.CommandText = "DELETE FROM [" + MyTableName + "]  WHERE ID=@ID";
        }


        #endregion


        private void CheckError()
        {
            if (MyConnectionClass.AppliedConnection == null) { IsError = true; }
            if (MyTableName == null) { IsError = true; }
            if (MyTableName.Length == 0) { IsError = true; }
            if (MyDataTable == null) { IsError = true; }
            if (MyDataView == null) { IsError = true; }
            if (CurrentRow == null) { IsError = true; }
        }

        private void GetDataTable()
        {
            if (MyTableName == null) { return; }                 // Exit here if table name is not specified.

            if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }

            SQLiteCommand _Command = new("SELECT * FROM [" + MyTableName + "]", MyConnection);
            SQLiteDataAdapter _Adapter = new(_Command);
            DataSet _DataSet = new();
            _Adapter.Fill(_DataSet, MyTableName);

            if (_DataSet.Tables.Count == 1)
            {
                MyDataTable = _DataSet.Tables[0];
                MyDataView = MyDataTable.AsDataView();
            }
            else { MyDataTable = new DataTable(); }
            return;
        }


        #region Table's Command
        public bool Seek(int _ID)
        {
            string Filter = MyDataView.RowFilter;
            MyDataView.RowFilter = "ID=" + _ID.ToString();

            if (MyDataView.Count > 0)
            {
                MyDataView.RowFilter = Filter; return true;
            }
            else
            { MyDataView.RowFilter = Filter; return false; }
        }

        public DataRow SeekRecord(int _ID)
        {
            DataRow row = MyDataTable.NewRow();
            string Filter = MyDataView.RowFilter;
            MyDataView.RowFilter = "ID=" + _ID.ToString();

            if (MyDataView.Count > 0)
            { row = MyDataView[0].Row; }
            else
            { row = MyDataTable.NewRow(); }
            CurrentRow = row;

            foreach (DataColumn _Column in CurrentRow.Table.Columns)                                                                // DBNull remove and assign a Data Type Empty Value.
            {
                if (CurrentRow[_Column.ColumnName] == DBNull.Value)
                {
                    if (_Column.DataType.Name == "String") { CurrentRow[_Column.ColumnName] = ""; }
                    if (_Column.DataType.Name == "Int32") { CurrentRow[_Column.ColumnName] = 0; }
                    if (_Column.DataType.Name == "Decimal") { CurrentRow[_Column.ColumnName] = 0.00; }
                }
            }



            MyDataView.RowFilter = Filter;
            return row;
            //return row;
        }

        internal bool Seek(string _Column, string _ColumnValue)
        {
            bool _result = true;
            string _Filter = MyDataView.RowFilter;
            MyDataView.RowFilter = _Column + "='" + _ColumnValue + "'";
            if (MyDataView.Count > 0)
            {
                _result = false;
            }
            MyDataView.RowFilter = _Filter;
            return _result;
        }

        public string Title(int _ID)
        {
            string Title = "";
            string Filter = MyDataView.RowFilter;
            MyDataView.RowFilter = "ID=" + _ID.ToString();
            if (MyDataView.Count > 0)
            { Title = (string)MyDataView[0]["Title"]; }
            MyDataView.RowFilter = Filter;
            return Title;
        }

        public string Title(string _Code)
        {
            string Title = "";
            string Filter = MyDataView.RowFilter;
            MyDataView.RowFilter = "ID='" + _Code.ToString() + "'";
            if (MyDataView.Count > 0)
            { Title = (string)MyDataView[0]["Title"]; }
            MyDataView.RowFilter = Filter;
            return Title;
        }


        public int ViewRecordCount() { return MyDataView.Count; }

        public void Save()
        {
            TableValidation = new TableValidationClass();
            if (CurrentRow != null)
            {
                TableValidation.MyDataTable = CurrentRow.Table;
                MyDataView.RowFilter = "ID=" + CurrentRow["ID"].ToString();

                if (MyDataView.Count > 0)
                {
                    TableValidation.SQLAction = CommandAction.Update.ToString();
                    if (TableValidation.Validation(CurrentRow))
                    {
                        CommandUpdate();
                        Command_Update.ExecuteNonQuery();
                        GetDataTable();
                    }
                }

                if (MyDataView.Count == 0)
                {
                    TableValidation.SQLAction = CommandAction.Insert.ToString();
                    if (TableValidation.Validation(CurrentRow))
                    {
                        CommandInsert();
                        Command_Insert.ExecuteNonQuery();
                        GetDataTable();
                    }
                }

            }
        }

        #endregion

        #region Static Function


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
            if (UserName == null) { return ""; } if(ID==0) { return ""; }
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
            Ledger = null;

            if(_Table.TableName == "CashBook")
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
                    Balance = Balance + (Debit - Credit);

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

        public static Dictionary<int,string> Titles(string UserName, Tables _TableName)
        {
            Dictionary<int, string> Titles = new Dictionary<int, string>();
            DataTableClass _Table = new(UserName, _TableName);
            foreach(DataRow _Row in _Table.MyDataTable.Rows)
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
            if(_Table.MyDataTable.Rows.Count > 0)
            { return _Table.MyDataTable.Rows[0]; }
            else { return _Table.MyDataTable.NewRow();}
        }

        #endregion
        

        //======================================================== eof
    }
}

