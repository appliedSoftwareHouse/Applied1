using System.Data;
using System.Data.SQLite;
using System.Security.Principal;
using System.Text;

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
            if (records> 1)
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


        //======================================================== eof
    }
}

