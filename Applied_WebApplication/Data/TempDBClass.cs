using Applied_WebApplication.Pages;
using NPOI.HSSF.Record;
using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Spreadsheet;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Text;

namespace Applied_WebApplication.Data
{
    public class TempDBClass
    {
        public UserProfile Profile { get; set; }
        public string UserName { get; set; }
        public string MyTableName { get; set; }
        public string MyCommandText { get; set; }
        public SQLiteConnection MyConnection { get; set; }
        public SQLiteConnection MyTempConnection { get; set; }
        public SQLiteCommand MyCommand { get; set; }
        public SqlDataAdapter MyAdapter { get; set; }
        public DataSet MyDataSet { get; set; }
        public DataTable SourceTable { get; set; }
        public DataTable TempTable { get; set; }
        public DataView SourceView { get; set; }
        public DataView TempView { get; set; }
        public DataRow CurrentRow { get; set; }
        public TableValidationClass TableValidate { get; set; }
        public List<Message> MyMessages { get; set; }
        public List<Message> ErrorMessages { get; set; }

        public int CountSource => SourceTable.Rows.Count;
        public int CountTemp => TempTable.Rows.Count;
        public string ViewFilter { get; set; }
        public string VoucherNo { get; set; }
        public int TranID { get; set; }
        public bool Refresh { get; set; }
        public bool IsNew { get; set; }

        #region Constructor

        public TempDBClass()
        {
        }

        public TempDBClass(string _UserName, Tables _Table, string _Filter, bool _Refresh)
        {
            UserName = _UserName;
            MyTableName = _Table.ToString();
            Refresh = _Refresh;
            ViewFilter = _Filter;
            MyMessages = new();
            ErrorMessages = new();

            if (ViewFilter.Length == 0)
            {
                MyMessages.Add(MessageClass.SetMessage("No Voucher Number assigned to proceed."));
                return;
            }

            MyConnection = ConnectionClass.AppConnection(UserName);
            MyTempConnection = ConnectionClass.AppTempConnection(UserName);

            if(Refresh) {DeleteRecords(_Table);}
            SourceTable = GetTable(_Table, ViewFilter, MyConnection);
            CreateTempTable(SourceTable);           // Create a Temp Table if not exist;
            TempTable = GetTable(_Table, ViewFilter, MyTempConnection);

            SourceView = SourceTable.AsDataView();
            TempView = TempTable.AsDataView();
            TableValidate = new(TempTable);

            if (Refresh)
            {
                if (CountSource > 0)
                {
                    foreach (DataRow Row in SourceTable.Rows)
                    {
                        CurrentRow = TempTable.NewRow();
                        CurrentRow.ItemArray = Row.ItemArray;
                        Save(MyTempConnection, false);
                    }
                    TempTable = GetTable(MyTableName, ViewFilter, MyTempConnection);                // Refresh Table 
                }
            }

            if (CurrentRow == null)
            {
                if (TempTable.Rows.Count > 0) { CurrentRow = TempTable.Rows[0]; } else { CurrentRow = NewRecord(); }
            }
        }

        private void DeleteRecords(Tables _Table)
        {
            var TempTable1 = GetTable(_Table, ViewFilter, MyTempConnection);
            if(TempTable1.Rows.Count>0)
            {
                foreach (DataRow Row in TempTable1.Rows)
                {
                    MyCommand = CommandDelete(MyTempConnection, Row.Table, Row);
                    var _Records = MyCommand.ExecuteNonQuery();
                    MyMessages.Add(MessageClass.SetMessage($"{_Records} effected.", -1, Color.Green));
                }
            }
        }

        #endregion

        #region Get Table


        public static DataTable GetTable(Tables _Table, string _Filter, SQLiteConnection _Connection)
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

        public static DataTable GetTable(string _Table, string _Filter, SQLiteConnection _Connection)
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


        #endregion

        #region New Record
        public DataRow NewRecord()
        {
            CurrentRow = SourceTable.NewRow();

            foreach (DataColumn _Column in CurrentRow.Table.Columns)                                                                // DBNull remove and assign a Data Type Empty Value.
            {
                if (CurrentRow[_Column.ColumnName] == DBNull.Value)
                {
                    if (_Column.DataType == typeof(string)) { CurrentRow[_Column.ColumnName] = string.Empty; }
                    if (_Column.DataType == typeof(short)) { CurrentRow[_Column.ColumnName] = 0; }
                    if (_Column.DataType == typeof(int)) { CurrentRow[_Column.ColumnName] = 0; }
                    if (_Column.DataType == typeof(long)) { CurrentRow[_Column.ColumnName] = 0; }
                    if (_Column.DataType == typeof(decimal)) { CurrentRow[_Column.ColumnName] = 0.00M; }
                    if (_Column.DataType == typeof(DateTime)) { CurrentRow[_Column.ColumnName] = DateTime.Now; }
                }
            }
            return CurrentRow;
        }
        #endregion`

        #region Save
        public void Save()
        {
            Save(MyConnection, true);
        }
        public void Save(SQLiteConnection _Connection)
        {
            Save(_Connection, true);
        }
        public void Save(SQLiteConnection _Connection, bool? _Validate)
        {
            _Connection ??= MyConnection;
            _Validate ??= true;
            ErrorMessages ??= new();

            if ((bool)_Validate)
            {
                TableValidate = new(TempTable);
                TableValidate.Validation(CurrentRow);
            }

            if (ErrorMessages.Count == 0)
            {
                SQLiteCommand _Command;
                var _View = TempView;
                _View.RowFilter = string.Format("ID={0}", CurrentRow["ID"]);

                if (_View.Count == 1)
                {
                    _Command = CommandUpdate(_Connection, TempTable, CurrentRow);
                }
                else
                {
                    if ((int)CurrentRow["ID"] == 0) { CurrentRow["ID"] = MaxID(); }
                    _Command = CommandInsert(_Connection, TempTable, CurrentRow);
                }
                try
                {
                    if (_Command.Connection.State != ConnectionState.Open) { _Command.Connection.Open(); }
                    var _Record = _Command.ExecuteNonQuery(); _Command.Connection.Close();
                    if (_Record == 0) { ErrorMessages.Add(MessageClass.SetMessage("No Record Saved", Color.Red)); }
                    //else
                    //{
                    //    TempTable = TempRefresh(_Connection);
                    //}

                }
                catch (Exception e)
                {
                    ErrorMessages.Add(MessageClass.SetMessage(e.Message, Color.Red));

                }

            }
        }

        private DataTable TempRefresh(SQLiteConnection _Connection)
        {
            return GetTable(MyTableName, ViewFilter, _Connection);
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            return true;
        }

        public bool DeleteAll()
        {
            MyCommand = CommandDeleteAll(MyTempConnection, MyTableName);
            var _Records = MyCommand.ExecuteNonQuery();
            MyMessages.Add(MessageClass.SetMessage($"{_Records}(s) have been effected."));
            if (_Records == 0) { return false; }
            return true;
        }
        #endregion

        #region Create a Temp Table if not exist

        private void CreateTempTable(DataTable _Table)
        {
            var _TableName = _Table.TableName;

            var _CommandText = $"SELECT count(name) FROM sqlite_master WHERE type = 'table' AND name ='{_TableName}'";
            var _Command = new SQLiteCommand(_CommandText, MyTempConnection);

            long TableExist = (long)_Command.ExecuteScalar();
            if (TableExist == 0)
            {
                //================================================== Create Table if not exist.
                StringBuilder _Text = new();

                _Text.Append(string.Concat("CREATE TABLE [", _TableName, "] ("));
                string _LastColumn = _Table.Columns[_Table.Columns.Count - 1].ToString();
                foreach (DataColumn Column in _Table.Columns)
                {
                    _Text.Append(" ["); _Text.Append(Column.ColumnName); _Text.Append("] ");
                    _Text.Append(Column.DataType.Name);
                    if (Column.ColumnName != _LastColumn) { _Text.Append(", "); } else { _Text.Append(") "); }
                }
                _Command.CommandText = _Text.ToString();
                _Command.ExecuteNonQuery();
            }
        }

        #endregion

        #region Command Update, Insert & Delete

        private static SQLiteCommand CommandInsert(SQLiteConnection _Connection, DataTable _Table, DataRow _Row)
        {
            var _Columns = _Table.Columns;
            var _Command = new SQLiteCommand(_Connection);
            var _CommandString = new StringBuilder();
            var _LastColumn = _Columns[_Columns.Count - 1].ColumnName.ToString();
            var _TableName = _Table.TableName;
            //var _ParameterName;

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

            _Command.CommandText = _CommandString.ToString();           // Generate Command string from string builder.

            foreach (DataColumn _Column in _Columns)
            {
                if (_Column == null) { continue; }
                var _ParameterName = string.Concat('@', _Column.ColumnName.Replace(" ", ""));
                _Command.Parameters.AddWithValue(_ParameterName, _Row[_Column.ColumnName]);
            }

            return _Command;

        }

        private static SQLiteCommand CommandUpdate(SQLiteConnection _Connection, DataTable _Table, DataRow _Row)
        {
            var _Columns = _Table.Columns;
            var _Command = new SQLiteCommand(_Connection);
            var _CommandString = new StringBuilder();
            var _LastColumn = _Columns[_Columns.Count - 1].ColumnName.ToString();
            var _TableName = _Table.TableName;

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

            _Command.CommandText = _CommandString.ToString();           // Generate Command string from string builder.

            foreach (DataColumn _Column in _Columns)
            {
                if (_Column == null) { continue; }
                var _ParameterName = string.Concat('@', _Column.ColumnName.Replace(" ", ""));
                _Command.Parameters.AddWithValue(_ParameterName, _Row[_Column.ColumnName]);
            }


            return _Command;
        }

        private static SQLiteCommand CommandDelete(SQLiteConnection _Connection, DataTable _Table, DataRow _Row)
        {
            var _Command = new SQLiteCommand(_Connection);
            var _TableName = _Table.TableName;

            if ((int)_Row["ID"] > 0)
            {
                _Command.Parameters.AddWithValue("@ID", _Row["ID"]);
                _Command.CommandText = "DELETE FROM [" + _TableName + "]  WHERE ID=@ID";
            }
            return _Command;
        }

        public static SQLiteCommand CommandDeleteAll(SQLiteConnection _Connection, string _Table)
        {
            if (_Connection.State != ConnectionState.Open) { _Connection.Open(); }
            var _CommandText = $"DELETE FROM [{_Table}]  WHERE ID>0";
            var _Command = new SQLiteCommand(_CommandText, _Connection);
            return _Command;
        }

        #endregion

        #region GetID and NewID
        internal DataRow Get_ID(int? ID)
        {
            if (TempView.Count > 0)
            {
                TempView.RowFilter = $"ID={ID}";
                if (TempView.Count > 1)
                {
                    return TempView[0].Row;
                }
            }
            return NewRecord();
        }
        internal int MaxID()
        {
            var MaxID = TempTable.Compute("MAX(ID)", "");
            if (MaxID == DBNull.Value) { MaxID = 0; }
            return (int)MaxID + 1;
        }

        internal int MaxTableID()
        {
            var _CommandText = $"SELECT MAX(ID) AS MaxID FROM [{MyTableName}]";
            var _Command = new SQLiteCommand(_CommandText, MyConnection);
            var MaxID = _Command.ExecuteScalar();
            if (MaxID == DBNull.Value) { return 1; }
            var MaxTranID = Conversion.ToInteger(MaxID) + 1;
            return MaxTranID;
        }

        #endregion

        // END
    }
}
