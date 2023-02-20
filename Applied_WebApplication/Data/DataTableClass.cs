using Applied_WebApplication.Pages;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Data.SQLite;
using System.Text;


namespace Applied_WebApplication.Data
{

    public class DataTableClass
    {
        #region Initial

        public string MyUserName { get; set; }
        public DataTable MyDataTable;
        public DataView MyDataView;
        public SQLiteConnection MyConnection;
        public TableValidationClass TableValidation;
        public int ErrorCount { get => TableValidation.MyMessages.Count; }
        public string MyTableName;
        public bool IsError = false;
        public string MyMessage;
        public string View_Filter { get; set; }
        public DataRow CurrentRow { get; set; }

        private SQLiteCommand Command_Update;
        private SQLiteCommand Command_Delete;
        private SQLiteCommand Command_Insert;

        #endregion

        #region DataTable
        public DataTableClass(string _UserName, Tables _Tables)
        {
            UserProfile UProfile = new(_UserName);
            string ConnectionString = string.Concat("Data Source=", UProfile.DBFilePath);
            MyConnection = new SQLiteConnection(ConnectionString);
            MyUserName = _UserName;
            MyTableName = _Tables.ToString();
            TableValidation = new();
            GetDataTable();                                                                                   // Load DataTable and View
            MyDataView.RowFilter = View_Filter;                                                  // Set a view filter for table view.
            CheckError();

            Command_Update = new SQLiteCommand(MyConnection);
            Command_Delete = new SQLiteCommand(MyConnection);
            Command_Insert = new SQLiteCommand(MyConnection);
        }

        public static DataTable GetDataView(string UserName, Tables TableView)                      // Load Database 
        {
            SQLiteConnection MyConnection = ConnectionClass.AppConnection(UserName);
            if (TableView.ToString() == null) { return new DataTable(); }                 // Exit here if table name is not specified.
            if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }
            SQLiteCommand _Command = new("SELECT * FROM [" + TableView + "]", MyConnection);
            SQLiteDataAdapter _Adapter = new(_Command);
            DataSet _DataSet = new();
            _Adapter.Fill(_DataSet, TableView.ToString());
            DataTable datatable;
            if (_DataSet.Tables.Count == 1)
            {
                datatable = _DataSet.Tables[0];
            }
            else { datatable = new DataTable(); }
            return datatable;
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
            else { MyDataTable = new DataTable(); MyConnection.Close(); }
            return;
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
                    if (_Column.DataType.Name == "DateTime") { CurrentRow[_Column.ColumnName] = DateTime.Now; }
                }
            }
            return CurrentRow;
        }
        private void CheckError()
        {
            if (MyConnection == null) { IsError = true; }
            if (MyTableName == null) { IsError = true; }
            if (MyTableName.Length == 0) { IsError = true; }
            if (MyDataTable == null) { IsError = true; }
            if (MyDataView == null) { IsError = true; }
            if (CurrentRow == null) { IsError = true; }
        }
        #endregion

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

            _Command.CommandText = _CommandString.ToString();

            foreach (DataColumn _Column in _Columns)
            {
                if (_Column == null) { continue; }
                _ParameterName = string.Concat("@", _Column.ColumnName.Replace(" ", ""));
                _Command.Parameters.AddWithValue(_ParameterName, CurrentRow[_Column.ColumnName]);
            }

            Command_Insert = _Command;
            CurrentRow["ID"] = NewID();
            Command_Insert.Parameters["@ID"].Value = CurrentRow["ID"];
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
            if ((int)CurrentRow["ID"] > 0)
            {
                Command_Delete.Parameters.AddWithValue("@ID", CurrentRow["ID"]);
                Command_Delete.CommandText = "DELETE FROM [" + MyTableName + "]  WHERE ID=@ID";
            }
            else
            {
                MyMessage = "Record not deleted.";
            }
        }
        #endregion

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
        public bool Seek(string _Code)
        {
            string Filter = MyDataView.RowFilter;
            MyDataView.RowFilter = String.Concat("Code='", _Code, "'");

            if (MyDataView.Count > 0)
            {
                MyDataView.RowFilter = Filter; return true;
            }
            else
            { MyDataView.RowFilter = Filter; return false; }
        }
        public DataRow SeekRecord(int _ID)
        {
            DataRow row;    // = MyDataTable.NewRow();
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
                    if (_Column.DataType.Name == "DateTime") { CurrentRow[_Column.ColumnName] = DateTime.Now; }
                }
            }
            MyDataView.RowFilter = Filter;
            return row;
        }
        internal bool Seek(string _Column, string _ColumnValue)                     // Search a specific colum by specific valu.
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
        public bool Replace(int _ID, string _Column, object _Value)
        {

            MyDataView.RowFilter = string.Concat("ID=", _ID.ToString());
            if (MyDataView.Count == 1)
            {
                CurrentRow = MyDataView[0].Row;
                CurrentRow[_Column] = _Value;
                Save();
            }
            return true;
        }
        public bool IsRowValid(DataRow Row, CommandAction SQLAction, PostType postType)
        {
            TableValidation = new(MyDataTable);
            TableValidation.SQLAction = SQLAction.ToString();
            TableValidation.MyVoucherType = postType;
            bool Result = TableValidation.Validation(Row);
            return Result;
        }

        #endregion

        #region Save / Delete / NewID
        public void Save()
        {
            IsError = false;
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

                if (TableValidation.MyMessages.Count > 0) { IsError = true; }

            }
        }
        public void Delete()
        {
            IsError = true;
            TableValidation.MyMessages = new List<Message>();
            CommandDelete();
            int records = 0;
            try
            {
                records = Command_Delete.ExecuteNonQuery();
            }
            catch (Exception)
            {
                TableValidation.MyMessages.Add(new Message() { Success = false, ErrorID = 1, Msg = "Transaction FAIL to delete!!!!!. Contact to administrator.." });
                throw;
            }
            if (records == 1)
            {
                IsError = false;
                MyMessage = string.Concat(records.ToString(), " has been deleted.");
            }
            if (records > 1)
            {
                IsError = false;
                MyMessage = string.Concat(records.ToString(), " have been deleted.");
            }
        }
        public int NewID()
        {
            if (MyDataTable.Rows.Count > 0)
            {
                int _result = (int)MyDataTable.Compute("MAX(ID)", "") + 1;
                return _result;
            }
            else
            {
                return 1;
            }
        }

        public static bool Replace(string UserName, Tables table, int _ID, string _Column, object _Value)
        {
            DataTableClass tb_table = new(UserName, table);
            return tb_table.Replace(_ID, _Column, _Value);
        }

        internal DataTable GetTable(string filter)
        {
            MyDataView.RowFilter = filter;
            return MyDataView.ToTable();
        }

        internal DataTable GetTable(string filter, string Sort)
        {
            MyDataView.Sort = Sort;
            MyDataView.RowFilter = filter;
            return MyDataView.ToTable();
        }

        #endregion

        //======================================================== eof
    }
}

