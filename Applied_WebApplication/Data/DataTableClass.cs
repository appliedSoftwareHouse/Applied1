using System.Data;
using System.Data.SQLite;
using System.Text;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Data
{

    public class DataTableClass
    {
        #region Initial

        public string UserName { get; set; }
        public DataTable MyDataTable { get; set; }
        public DataView MyDataView { get; set; }
        public SQLiteConnection MyConnection { get; set; }
        public TableValidationClass TableValidation { get; set; }
        public int ErrorCount => TableValidation.MyMessages.Count;
        public List<Message> ErrorMessages => TableValidation.MyMessages;
        public int Count => MyDataView.Count;
        public int CountTable => MyDataTable.Rows.Count;
        public int CountView => MyDataView.Count;
        public string MyTableName { get; set; }
        public bool IsError { get; set; } = false;
        public string MyMessage { get; set; }
        public string View_Filter { get; set; }
        public DataRow CurrentRow { get; set; }
        public DataRowCollection Rows => MyDataTable.Rows;
        public DataColumnCollection Columns => MyDataTable.Columns;


        private SQLiteCommand Command_Update;
        private SQLiteCommand Command_Delete;
        private SQLiteCommand Command_Insert;

        #endregion

        #region Constructor

        public DataTableClass(string _UserName, Tables _Tables)
        {
            SetTableClass(_UserName, _Tables, "");
        }
        public DataTableClass(string _UserName, Tables _Tables, string _Filter)
        {
            SetTableClass(_UserName, _Tables, _Filter);
        }
        public DataTableClass(string _UserName, string _Text)
        {
            if (_Text.Length > 0 || _Text != null)
            {
                SetTableClass(_UserName, _Text, "");
            }
        }

        public DataTableClass(string _UserName, string _Text, Tables _TableName)
        {
            MyTableName = _TableName.ToString();
            if (_Text.Length > 0 || _Text != null)
            {
                SetTableClass(_UserName, _Text, "");
            }
        }
        public DataTableClass(string _UserName, string _Text, string _Filter)
        {
            if (_Text.Length > 0 || _Text != null)
            {
                SetTableClass(_UserName, _Text, _Filter);

            }
        }
        #endregion

        #region Connection

        private SQLiteConnection GetConnection()
        {
            if (UserName == null) { return null; }
            MyConnection ??= MyConnection = ConnectionClass.AppConnection(UserName);
            if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }
            return MyConnection;
        }

        #endregion

        #region DataTable

        public void SetTableClass(string _UserName, Tables _Tables, string? _Filter)
        {
            try
            {
                UserName = _UserName;
                MyConnection = GetConnection();
                _Filter ??= string.Empty;
                View_Filter = _Filter;
                MyTableName = _Tables.ToString();
                GetDataTable();
                MyDataView ??= new DataView();
                TableValidation = new(MyDataTable);
                if (_Filter != null) { View_Filter = _Filter; }
                CheckError();

                Command_Update = new SQLiteCommand(MyConnection);
                Command_Delete = new SQLiteCommand(MyConnection);
                Command_Insert = new SQLiteCommand(MyConnection);
            }
            catch (Exception e)
            {
                MyMessage = e.Message;
            }
        }
        public void SetTableClass(string _UserName, string? _Text, string? _Filter)
        {
            _Filter ??= string.Empty;
            _Text ??= string.Empty;

            try
            {
                UserName = _UserName;
                MyConnection = GetConnection();
                View_Filter = _Filter;
                GetDataTable(_Text);
                MyDataView ??= new DataView();
                TableValidation = new(MyDataTable);
                if (_Filter.Length > 0) { View_Filter = _Filter; }
                CheckError();
                Command_Update = new SQLiteCommand(MyConnection);
                Command_Delete = new SQLiteCommand(MyConnection);
                Command_Insert = new SQLiteCommand(MyConnection);
            }
            catch (Exception e)
            {
                MyMessage = e.Message;
            }
        }
        public static DataTable GetTable(string UserName, Tables _Table)                      // Load Database 
        {
            SQLiteConnection MyConnection = ConnectionClass.AppConnection(UserName);
            if (_Table.ToString() == null) { return new DataTable(); }                 // Exit here if table name is not specified.
            if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }
            SQLiteCommand _Command = new("SELECT * FROM [" + _Table + "]", MyConnection);
            SQLiteDataAdapter _Adapter = new(_Command);
            DataSet _DataSet = new();
            _Adapter.Fill(_DataSet, _Table.ToString());
            DataTable datatable;
            if (_DataSet.Tables.Count == 1)
            {
                datatable = _DataSet.Tables[0];
            }
            else { datatable = new DataTable(); }
            return datatable;
        }
        public static DataTable GetTable(string UserName, string SQLQuery)
        {
            return GetTable(UserName, SQLQuery, "");
        }
        public static DataTable GetTable(string UserName, string _Text, string? _Sort)                      // Load Database 
        {
            DataTable _Table = new DataTable();
            if (_Text.Length > 0)
            {
                try
                {
                    _Sort ??= string.Empty;
                    if (_Sort.Length > 0) { _Text = string.Concat(_Text, "ORDER BY ", _Sort); }
                    SQLiteConnection MyConnection = ConnectionClass.AppConnection(UserName);

                    if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }
                    SQLiteCommand _Command = new(_Text, MyConnection);
                    SQLiteDataAdapter _Adapter = new(_Command);
                    DataSet _DataSet = new();
                    _Adapter.Fill(_DataSet, "Table");

                    if (_DataSet.Tables.Count == 1)
                    {
                        _Table = _DataSet.Tables[0];
                    }
                }
                catch (Exception)
                {
                    _Table = new DataTable();
                }
            }
            return _Table;
        }
        public static DataTable GetTable(string UserName, string _Text, SQLiteParameter _ID)                      // Load Database with ID Parameter
        {
            DataTable _Table = new DataTable();
            if (_Text.Length > 0)
            {
                try
                {
                    SQLiteConnection MyConnection = ConnectionClass.AppConnection(UserName);
                    if (MyConnection.State != ConnectionState.Open) { MyConnection.Open(); }
                    SQLiteCommand _Command = new(_Text, MyConnection);
                    SQLiteDataAdapter _Adapter = new(_Command);
                    DataSet _DataSet = new();
                    _Command.Parameters.Add(_ID);
                    _Adapter.Fill(_DataSet);

                    if (_DataSet.Tables.Count == 1)
                    {
                        _Table = _DataSet.Tables[0];
                    }
                }
                catch (Exception)
                {
                    _Table = new DataTable();
                }
            }
            return _Table;
        }

        private void GetDataTable()
        {
            if (MyTableName == null) { return; }                 // Exit here if table name is not specified.

            try
            {
                var _CommandText = $"SELECT * FROM [{MyTableName}]";
                if (View_Filter.Length > 0) { _CommandText += " WHERE " + View_Filter; }
                SQLiteCommand _Command = new(_CommandText, GetConnection());
                SQLiteDataAdapter _Adapter = new(_Command);
                DataSet _DataSet = new();
                _Adapter.Fill(_DataSet, MyTableName);

                if (_DataSet.Tables.Count == 1)
                {

                    MyDataTable = _DataSet.Tables[0];
                    MyTableName = MyDataTable.TableName;
                    MyDataView = MyDataTable.AsDataView();
                    if (MyDataTable.Rows.Count > 0)
                    {
                        CurrentRow ??= MyDataTable.Rows[0];
                    }
                }
                else { MyDataTable = new DataTable(); MyConnection.Close(); }

            }
            catch (Exception e)
            {
                if (e.Message.Contains("no such table"))
                {
                    CreateTablesClass CreateTable = new(UserName, MyTableName);
                }
                MyMessage = e.Message;
            }
            return;
        }

        private void GetDataTable(string _Text)
        {
            MyTableName ??= "MyTable";                           // Exit here if table name is not specified.

            try
            {
                var _CommandText = _Text;
                SQLiteCommand _Command = new(_CommandText, GetConnection());
                SQLiteDataAdapter _Adapter = new(_Command);
                DataSet _DataSet = new();
                _Adapter.Fill(_DataSet, MyTableName);

                if (_DataSet.Tables.Count == 1)
                {

                    MyDataTable = _DataSet.Tables[0];
                    MyTableName = MyDataTable.TableName;
                    MyDataView = MyDataTable.AsDataView();
                    if (MyDataTable.Rows.Count > 0)
                    {
                        CurrentRow ??= MyDataTable.Rows[0];
                    }
                }
                else { MyDataTable = new DataTable(); MyConnection.Close(); }

            }
            catch (Exception e)
            {
                if (e.Message.Contains("no such table"))
                {
                    CreateTablesClass CreateTable = new(UserName, MyTableName);
                }
                MyMessage = e.Message;
            }
            return;
        }
        internal DataTable GetTable(string filter)
        {
            MyDataView.RowFilter = filter;
            return MyDataView.ToTable();
        }

        #endregion

        #region New Row / Refresh
        public DataRow NewRecord()
        {
            if(MyDataTable == null) { return null; }
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
            MyTableName ??= "MyTable";

            if (MyConnection == null) { IsError = true; }
            if (MyTableName == null) { IsError = true; }
            if (MyTableName.Length == 0) { IsError = true; }
            if (MyDataTable == null) { IsError = true; }
            if (MyDataView == null) { IsError = true; }
            if (CurrentRow == null) { IsError = true; }
        }
        public void Refresh()
        {
            var Filter = string.Empty;
            if (View_Filter.Length > 0) { Filter = $"WHERE {Filter}"; };
            var _CommandText = $"SELECT * FROM {MyTableName} {Filter}";
            var _Command = new SQLiteCommand(_CommandText, GetConnection());
            var _Adapter = new SQLiteDataAdapter(_Command);
            var _DataSet = new DataSet();
            _Adapter.Fill(_DataSet, MyTableName);
            if (_DataSet.Tables.Count > 0)
            {
                MyDataTable = _DataSet.Tables[0];
                MyDataView = MyDataTable.AsDataView();
            }
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
                _CommandString.Append(string.Concat('@', _Column.ColumnName));
                if (_ColumnName != _LastColumn)
                { _CommandString.Append(','); }
                else
                { _CommandString.Append(") "); }
            }

            _Command.CommandText = _CommandString.ToString();

            foreach (DataColumn _Column in _Columns)
            {
                if (_Column == null) { continue; }
                _ParameterName = string.Concat('@', _Column.ColumnName.Replace(" ", ""));
                _Command.Parameters.AddWithValue(_ParameterName, CurrentRow[_Column.ColumnName]);
            }

            Command_Insert = _Command;
            CurrentRow["ID"] = NewID();
            Command_Insert.Parameters["@ID"].Value = CurrentRow["ID"];
        }
        private void CommandUpdate()
        {
            var _Columns = CurrentRow.Table.Columns;
            var _Command = new SQLiteCommand(MyConnection);
            var _CommandString = new StringBuilder();
            var _LastColumn = _Columns[_Columns.Count - 1].ColumnName.ToString();
            var _TableName = MyTableName;

            _CommandString.Append(string.Concat("UPDATE [", _TableName, "] SET "));

            foreach (DataColumn _Column in _Columns)
            {
                if (_Column.ColumnName == "ID") { continue; }
                _CommandString.Append(string.Concat("[", _Column.ColumnName, "]"));
                _CommandString.Append('=');
                _CommandString.Append(string.Concat('@', _Column.ColumnName.Replace(" ", "")));

                if (_Column.ColumnName == _LastColumn)
                {
                    _CommandString.Append(string.Concat(" WHERE ID = @ID"));
                }
                else
                {
                    _CommandString.Append(',');
                }
            }

            _Command.CommandText = _CommandString.ToString();
            Command_Update = _Command;

            foreach (DataColumn _Column in _Columns)
            {
                if (_Column == null) { continue; }
                var _ParameterName = string.Concat('@', _Column.ColumnName.Replace(" ", ""));
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
            TableValidation = new(MyDataTable)
            {
                SQLAction = SQLAction.ToString(),
                MyVoucherType = postType
            };
            bool Result = TableValidation.Validation(Row);
            return Result;
        }

        #endregion

        #region Save / Delete / NewID

        public void Save()
        {
            Save(true);
        }


        public void Save(bool? Validate)
        {
            Validate ??= true;
            IsError = false;

            if (CurrentRow != null)
            {
                TableValidation = new TableValidationClass(CurrentRow.Table);

                if (TableValidation.SQLAction.Length == 0)
                {
                    TableValidation.SQLAction = CommandAction.Insert.ToString();
                    MyDataView.RowFilter = $"ID={CurrentRow["ID"]}";
                    if (MyDataView.Count > 0) { TableValidation.SQLAction = CommandAction.Update.ToString(); }
                }

                if ((bool)Validate)
                {
                    var IsValidated = TableValidation.Validation(CurrentRow);
                    if(!IsValidated)
                    {
                        IsError = true;
                        return;
                    }
                }

                MyDataView.RowFilter = "ID=" + CurrentRow["ID"].ToString();
                try
                {
                    if (MyDataView.Count == 0)
                    {
                        TableValidation.SQLAction = CommandAction.Insert.ToString();
                        CommandInsert();
                        Command_Insert.ExecuteNonQuery();
                        GetDataTable();
                        MyMessage = "Insert record in " + MyTableName;
                    }

                    if (MyDataView.Count > 0)
                    {
                        TableValidation.SQLAction = CommandAction.Update.ToString();
                        CommandUpdate();
                        Command_Update.ExecuteNonQuery();
                        GetDataTable();
                        MyMessage = "Update record in " + MyTableName;
                    }
                }
                catch (Exception e)
                {
                    TableValidation.MyMessages.Add(MessageClass.SetMessage(e.Message));
                    MyMessage = e.Message;
                    IsError = true;
                }
                //if (TableValidation.MyMessages.Count > 0) { IsError = true; }

            }
        }
        public bool Delete()
        {
            IsError = true;
            TableValidation.MyMessages = new List<Message>();
            CommandDelete();
            int records;
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

            return IsError;
        }
        public int NewID()
        {
            if (MyDataTable.Rows.Count > 0)
            {
                int _result = int.Parse(MyDataTable.Compute("MAX(ID)", "").ToString()) + 1;
                return _result;
            }
            else
            {
                return 1;
            }
        }
        public void Add()
        {
            IsError = false;
            TableValidation = new TableValidationClass(MyDataTable);


            if (CurrentRow != null)
            {
                try
                {
                    TableValidation.SQLAction = CommandAction.Insert.ToString();
                    if (TableValidation.Validation(CurrentRow))
                    {
                        CommandInsert();                                            // Create a command and assign new ID value.
                        Command_Insert.ExecuteNonQuery();
                        GetDataTable();
                        MyMessage = "Insert (Add) record in " + MyTableName;
                    }

                }
                catch (Exception e)
                {
                    TableValidation.MyMessages.Add(MessageClass.SetMessage(e.Message));
                    MyMessage = e.Message;
                }
                if (TableValidation.MyMessages.Count > 0) { IsError = true; }

            }
        }
        public static bool Replace(string UserName, Tables table, int _ID, string _Column, object _Value)
        {
            DataTableClass tb_table = new(UserName, table, "");
            return tb_table.Replace(_ID, _Column, _Value);
        }

        #endregion




        #region Static Methods

        public static DataTable GetTable(string UserName, Tables _Table, string _Filter)
        {
            return GetTable(UserName, _Table, _Filter, ConnectionClass.AppConnection(UserName));
        }

        public static DataTable GetTable(string UserName, Tables _Table, string _Filter, SQLiteConnection _Connection)
        {
            var _TableName = _Table.ToString();
            if (_Filter.Length > 0) { _Filter = $"WHERE {_Filter}"; }
            var _CommandText = $"SELECT * FROM {_TableName} {_Filter}";
            var _Command = new SQLiteCommand(_CommandText, _Connection);
            var _Adapter = new SQLiteDataAdapter(_Command);
            var _DataSet = new DataSet();
            _Adapter.Fill(_DataSet, _TableName);
            if (_DataSet.Tables.Count > 0) { return _DataSet.Tables[0]; }
            return new DataTable();
        }


        public static bool Delete(string UserName, Tables _Table, int ID)
        {
            var _Connection = ConnectionClass.AppConnection(UserName);
            var _TableName = _Table.ToString();
            var _CommandText = $"DELETE FROM {_TableName} WHERE ID={ID}";
            var _Command = new SQLiteCommand(_CommandText, _Connection);
            var _RecordNo = _Command.ExecuteNonQuery();
            if (_RecordNo == 0) { return false; }
            return true;

        }


        #endregion


        #region Max
        public int GetMaxTranID(VoucherType _VouType)
        {
            int Result = 0;
            if (Columns.Contains("TranID"))
            {
                var MaxNo = MyDataTable.Compute("MAX(TranID)", string.Format("Vou_Type='{0}'", _VouType.ToString()));
                if (MaxNo.Equals(DBNull.Value)) { MaxNo = 0; }
                Result = (int)MaxNo + 1;
            }
            return Result;
        }

        public void RemoveNull()
        {
            foreach (DataColumn Column in CurrentRow.Table.Columns)
            {
                if (CurrentRow[Column] == DBNull.Value)
                {
                    var _Type = Column.DataType;
                    if (_Type == typeof(int)) { CurrentRow[Column] = 0; }
                    if (_Type == typeof(string)) { CurrentRow[Column] = ""; }
                    if (_Type == typeof(DateTime)) { CurrentRow[Column] = new DateTime(); }
                    if (_Type == typeof(bool)) { CurrentRow[Column] = false; }
                }
            }
        }

        #endregion
        //======================================================== eof
    }
}

