using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Text;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Data
{
    public class xTempTableClass
    {
        #region Setup

        public UserProfile UProfile { get; set; }
        public string UserName { get; set; }
        public bool IsNew { get; set; }
        public SQLiteConnection MyConnection { get; set; }
        public SQLiteConnection TempConnection { get; set; }
        public DataTable SourceTable { get; set; }
        public DataView SourceView { get; set; }
        public DataTable TempTable { get; set; }
        public DataView TempView { get; set; }
        public DataRow CurrentRow { get; set; }
        public string View_Filter { get; set; }
        public string MyTableName { get; set; }
        public Tables TableID { get; set; }
        public string ConnectionString { get; set; }
        public int CountSource => SourceTable.Rows.Count;
        public int CountTemp => TempTable.Rows.Count;
        private string CommandText { get; set; }
        private SQLiteCommand Command { get; set; }
        public TableValidationClass TableValidate { get; set; }
        public List<Message> ErrorMessages => TableValidate.MyMessages;

        private string VoucherNo { get; set; }
        private int TranID { get; set; }

        #endregion

        #region Constructor

        public xTempTableClass(string _UserName, Tables _TableID, string _VoucherNo, bool IsLoad)
        {
            VoucherNo = _VoucherNo;
            UserName = _UserName;
            MyTableName = _TableID.ToString();
            View_Filter = $"Vou_No='{VoucherNo}'";
            SourceTable = DataTableClass.GetTable(UserName, _TableID, View_Filter);
            TempTableIsExist(SourceTable);                              // Create a duplicate table in Temporary Database of App, if not exist.
            if (VoucherNo.ToUpper() == "NEW")
            {
                View_Filter = $"Vou_No='{VoucherNo}'";
                TempRefresh();
            }
            else
            {
                TempTable = CreateTempTable(UserName, SourceTable);
            }

            if (CurrentRow == null)
            {
                if (CountTemp > 0) { CurrentRow = TempTable.Rows[0]; } else { CurrentRow = NewRecord(); }
            }

            SourceView = SourceTable.AsDataView();
            TempView = TempTable.AsDataView();

            TableValidate = new(TempTable);
        }
        public xTempTableClass(string _UserName, Tables _TableID, int _TranID, bool? IsNew)
        {
            IsNew ??= false;
            TranID = _TranID;
            UserName = _UserName;
            MyTableName = _TableID.ToString();
            UProfile = new UserProfile(UserName);
            SourceTable = new();
            TempTable = new();

            SQLiteDataAdapter _Adapter;
            DataSet _DataSet;

            #region New Voucher

            if ((bool)IsNew)
            {
                View_Filter = $"TranID={TranID}";
                CommandText = $"SELECT * FROM [{MyTableName}] WHERE {View_Filter}";
                Command = new(CommandText, GetTempConnection());
                _Adapter = new(Command);
                _DataSet = new DataSet();
                _Adapter.Fill(_DataSet, MyTableName);
                if (_DataSet.Tables.Count > 0)
                {
                    TempTable = _DataSet.Tables[0];
                    SourceTable = TempTable.Clone();
                }
                else
                {
                    TempTable = new DataTable();
                    SourceTable = new DataTable();
                }
            }

            #endregion

            else
            {
                View_Filter = $"TranID={TranID}";
                SourceTable = DataTableClass.GetTable(UserName, _TableID, View_Filter);
                TempTableIsExist(SourceTable);                                               // Create a duplicate table in Temporary Database of App, if not exist.
                TempTable = CreateTempTable(UserName, SourceTable);
            }

            #region Current Row

            if (CurrentRow == null)
            {
                if (CountTemp > 0)
                {
                    CurrentRow = TempTable.Rows[0];
                }
                else
                {
                    CurrentRow = NewRecord();
                }
                #endregion
            }

            SourceView = SourceTable.AsDataView();
            TempView = TempTable.AsDataView();

            TableValidate = new(TempTable);
        }
        public xTempTableClass(string _UserName, Tables _TableID)
        {
            IsNew = true;
            TranID = 0;
            UserName = _UserName;
            MyTableName = _TableID.ToString();
            UProfile = new UserProfile(UserName);
            SourceTable = new();
            TempTable = new();

            // Source Table
            SourceTable = GetTable(UserName, _TableID, $"ID={TranID}");

            // Tamp Table
            CommandText = $"SELECT * FROM [{MyTableName}]";
            Command = new(CommandText, GetTempConnection());
            var _Adapter = new SQLiteDataAdapter(Command);
            var _DataSet = new DataSet();
            _Adapter.Fill(_DataSet, MyTableName);
            if (_DataSet.Tables.Count > 0) { TempTable = _DataSet.Tables[0]; } else { TempTable = new DataTable(); }

            // Source & Temp View
            SourceView = SourceTable.AsDataView();
            TempView = TempTable.AsDataView();

            //Current Row
            if (TempTable.Rows.Count > 0) { CurrentRow = TempTable.Rows[0]; } else { CurrentRow = TempTable.NewRow(); }
            TableValidate = new(TempTable);
        }
        #endregion

        #region Connection
        
        private SQLiteConnection GetTempConnection()
        {
            if (UserName == null) { return null; }
            TempConnection ??= ConnectionClass.AppTempConnection(UserName);
            if (TempConnection.State != ConnectionState.Open) { TempConnection.Open(); }
            return TempConnection;
        }


        #endregion


        #region Get Table / From Voucher No / Tran ID / & Refresh

        public static DataTable GetTable(string UserName, Tables _Table, string _Filter)
        {
            try
            {

                var _Connection = ConnectionClass.AppTempConnection(UserName);
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
            catch (Exception)
            {
                return new DataTable();
            }

        }
        public DataTable CreateTempTable(string _UserName, DataTable _Table)
        {
            UserName ??= _UserName;
            DataTable ResultTable = new DataTable();
            MyTableName = _Table.TableName;
            var _VouNo = "New";
            if (_Table.Columns.Contains("Vou_No"))
            {
                if (_Table.Rows.Count > 0)
                {
                    _VouNo = _Table.Rows[0]["Vou_No"].ToString();
                }
                ResultTable = FromVouNo(_Table, _VouNo);
            }
            else
            {
                var _TranID = 0;
                if (_Table.Columns.Contains("TranID"))
                {
                    if (_Table.Rows.Count > 0)
                    {
                        _TranID = (int)_Table.Rows[0]["TranID"];
                    }
                    ResultTable = FromTranID(_Table, _TranID);
                }
            }
            return ResultTable;
        }
        private DataTable FromVouNo(DataTable _Table, string _VouNo)
        {
            if (_VouNo.ToUpper() == "NEW") { return _Table; }

            var _Command = new SQLiteCommand(GetTempConnection());
            var _TableName = _Table.TableName;
            var _Filter = $"Vou_No='{_VouNo}'";
            var _CommandText = $"SELECT * FROM {_TableName} WHERE {_Filter}";

            DataTable TempTable = LoadTempTable(_CommandText);

            foreach (DataRow Row in TempTable.Rows)
            {
                _CommandText = $"DELETE FROM {_TableName} WHERE {(int)Row["ID"]}";
                _Command.CommandText = _CommandText;
                _Command.ExecuteNonQuery();
            }

            foreach (DataRow Row in _Table.Rows)
            {
                DataRow TempRow = TempTable.NewRow();
                TempRow.ItemArray = Row.ItemArray;
                _Command = CommandInsert(TempConnection, TempTable, TempRow);
                _Command.ExecuteNonQuery();
            }

            _CommandText = $"SELECT * FROM {_TableName} WHERE {_Filter}";
            var ResultTable = LoadTempTable(_CommandText);

            return ResultTable;
        }
        private DataTable FromTranID(DataTable _Table, int _TranID)
        {
            if (IsNew) { return _Table; }

            var _Command = new SQLiteCommand(GetTempConnection());
            var _TableName = _Table.TableName;
            var _Filter = $"TranID={_TranID}";
            var _CommandText = $"SELECT * FROM {_TableName} WHERE {_Filter}";
            var TempTable = LoadTempTable(_CommandText);

            foreach (DataRow Row in TempTable.Rows)
            {
                _CommandText = $"DELETE FROM {_TableName} WHERE {(int)Row["ID"]}";
                _Command.CommandText = _CommandText;
                _Command.ExecuteNonQuery();
            }

            foreach (DataRow Row in _Table.Rows)
            {
                DataRow TempRow = TempTable.NewRow();
                TempRow.ItemArray = Row.ItemArray;
                _Command = CommandInsert(TempConnection, TempTable, TempRow);
                _Command.ExecuteNonQuery();
            }

            _CommandText = $"SELECT * FROM {_TableName} WHERE {_Filter}";
            var ResultTable = LoadTempTable(_CommandText);

            return ResultTable;
        }



        public void TempRefresh()
        {
            var Filter = string.Empty;
            if (View_Filter.Length > 0) { Filter = View_Filter; };
            var _CommandText = $"SELECT * FROM {MyTableName} WHERE {Filter}";
            var _Command = new SQLiteCommand(_CommandText, GetTempConnection());
            var _Adapter = new SQLiteDataAdapter(_Command);
            var _DataSet = new DataSet();
            _Adapter.Fill(_DataSet, MyTableName);
            if (_DataSet.Tables.Count > 0)
            {
                TempTable = _DataSet.Tables[0];
                TempView = TempTable.AsDataView();
            }
        }

        #endregion

        #region Create or Load Table from Temp DataBase.

        private void TempTableIsExist(DataTable _Table)
        {
            var _TableName = _Table.TableName;

            var _CommandText = $"SELECT count(name) FROM sqlite_master WHERE type = 'table' AND name ='{_TableName}'";
            var _Command = new SQLiteCommand(_CommandText, GetTempConnection());

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

        #region Load Table

        private DataTable LoadTempTable(string _CommandText)
        {

            var _Command = new SQLiteCommand(_CommandText, TempConnection);
            var _Adapter = new SQLiteDataAdapter(_Command);
            var _DataSet = new DataSet();

            _Adapter.Fill(_DataSet, MyTableName);
            if (_DataSet.Tables.Count > 0)
            {
                return _DataSet.Tables[0];
            }
            return new DataTable();
        }

        #endregion`

        #region Save

        public void Save()
        {
            Save(true);
        }

        public void Save(bool Validate)
        {

            if (Validate)
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
                    _Command = CommandUpdate(TempConnection, TempTable, CurrentRow);
                }
                else
                {
                    CurrentRow["ID"] = MaxID();
                    _Command = CommandInsert(TempConnection, TempTable, CurrentRow);
                }
                try
                {
                    if (_Command.Connection.State != ConnectionState.Open) { _Command.Connection.Open(); }
                    var _Record = _Command.ExecuteNonQuery(); _Command.Connection.Close();
                    if (_Record == 0) { ErrorMessages.Add(MessageClass.SetMessage("No Record Saved", Color.Red)); }
                }
                catch (Exception e)
                {
                    ErrorMessages.Add(MessageClass.SetMessage(e.Message, Color.Red));

                }

            }
        }
        private int MaxID()
        {
            var MaxID = TempTable.Compute("MAX(ID)", "");
            if (MaxID == DBNull.Value) { MaxID = 0; }
            return (int)MaxID + 1;
        }

        #endregion

        #region Delete Record
        public void Delete()
        {
            //ErrorMessages = new();
            try
            {

                var _Command = CommandDelete(TempConnection, TempTable, CurrentRow);
                if (_Command.Connection.State != ConnectionState.Open) { _Command.Connection.Open(); }
                var _Records = _Command.ExecuteNonQuery(); _Command.Connection.Close();

                if (_Records == 0) { ErrorMessages.Add(MessageClass.SetMessage("No Record Delete...", 0, Color.Red)); }
                else { ErrorMessages.Add(MessageClass.SetMessage(_Records + "Record(s) Deleted...", _Records, Color.Red)); }

            }
            catch (Exception e)
            {
                ErrorMessages.Add(MessageClass.SetMessage(e.Message, Color.Red));
            }

        }

        public static bool Delete(string UserName, Tables _Table, int ID)
        {
            var _Connection = ConnectionClass.AppTempConnection(UserName);
            var _TableName = _Table.ToString();
            var _CommandText = $"DELETE FROM [{_TableName}] WHERE ID={ID}";
            var _Command = new SQLiteCommand(_CommandText, _Connection);
            var _RecordNo = _Command.ExecuteNonQuery();
            if (_RecordNo == 0) { return false; }
            return true;

        }

        #endregion

        public bool TempSeek(int ID)
        {
            TempView.RowFilter = string.Format("ID={0}", ID);
            if (TempView.Count == 1)
            {
                CurrentRow = TempView[0].Row;
                return true;
            }
            return false;
        }
        public DataRow NewRecord()
        {
            CurrentRow = TempTable.NewRow();

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

        #region Insert / Update / Delete

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

        internal DataRow Get_SrNo(int? Sr_No)
        {
            Sr_No ??= 1;
            if (TempView.Count > 0)
            {
                TempView.RowFilter = $"Sr_No={Sr_No}";
                if (TempView.Count > 1)
                {
                    return TempView[0].Row;
                }
            }
            return NewRecord();
        }

        internal static void Setup(string userName, Tables billReceivable, string vou_No)
        {
            throw new NotImplementedException();
        }

        internal static void Setup(string userName, Tables billReceivable, int TranID)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
