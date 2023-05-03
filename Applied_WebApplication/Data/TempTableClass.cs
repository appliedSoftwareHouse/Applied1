using Applied_WebApplication.Pages;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Model;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.OpenXmlFormats.Spreadsheet;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Text;

namespace Applied_WebApplication.Data
{
    public class TempTableClass
    {
        #region Setup

        public string UserName { get; set; }
        public DataTable SourceTable { get; set; }
        public DataView SourceView { get; set; }
        public DataTable TempTable { get; set; }
        public DataView TempView { get; set; }
        public string MyTableName { get; set; }
        public Tables TableID { get; set; }
        public UserProfile UProfile { get; set; }
        public string DBPath => UProfile.DataBaseFile;
        public string ConnectionString { get; set; }
        public SQLiteConnection MyConnection { get; set; }
        public SQLiteConnection TempConnection { get; set; }
        public int CountSource => SourceTable.Rows.Count;
        public int CountTemp => TempTable.Rows.Count;

        public DataRow CurrentRow;


        private string CommandText { get; set; }
        private SQLiteCommand Command { get; set; }
        public TableValidationClass TableValidate { get; set; }
        public List<Message> ErrorMessages => TableValidate.MyMessages;

        private string VoucherNo { get; set; }
        private int TranID { get; set; }

        #endregion

        #region Constructors

        public TempTableClass(string _UserName, Tables _TableID, string _VoucherNo)
        {
            VoucherNo = _VoucherNo;
            TranID = 0;
            UserName = _UserName;
            TableID = _TableID;
            UProfile = new UserProfile(UserName);
            ConnectionString = string.Concat("Data Source=", DBPath);
            MyConnection = new SQLiteConnection(ConnectionString);
            CommandText = string.Format("SELECT * FROM [{0}] WHERE Vou_No='{1}'", TableID.ToString(), VoucherNo);
            Command = new SQLiteCommand(CommandText, MyConnection);
            SourceTable = GetDataTable();
            TempTable = CreateTempTable(UserName, SourceTable);
            if (CurrentRow == null)
            {
                if (CountTemp > 0) { CurrentRow = TempTable.Rows[0]; } else { CurrentRow = NewRecord(); }
            }

            SourceView = SourceTable.AsDataView();
            TempView = TempTable.AsDataView();

            TableValidate = new(TempTable);
        }

        public TempTableClass(string _UserName, Tables _TableID, int _TranID)
        {
            VoucherNo = "";
            TranID = _TranID;
            UserName = _UserName;
            TableID = _TableID;
            UProfile = new UserProfile(UserName);
            ConnectionString = string.Concat("Data Source=", DBPath);
            MyConnection = new SQLiteConnection(ConnectionString);
            CommandText = string.Format("SELECT * FROM [{0}] WHERE TranID={1}", TableID.ToString(), TranID);
            Command = new SQLiteCommand(CommandText, MyConnection);
            SourceTable = GetDataTable();
            TempTable = CreateTempTable(UserName, SourceTable);
            if (CurrentRow == null)
            {
                if (CountTemp > 0) { CurrentRow = TempTable.Rows[0]; } else { CurrentRow = NewRecord(); }
            }

            SourceView = SourceTable.AsDataView();
            TempView = TempTable.AsDataView();

            TableValidate = new(TempTable);
        }

        #endregion

        private DataTable GetDataTable()
        {
            SQLiteDataAdapter _Adapter = new(Command);
            DataSet _DataSet = new();
            _Adapter.Fill(_DataSet, TableID.ToString());
            if (_DataSet.Tables.Count == 1)
            {
                return _DataSet.Tables[0];
            }
            return new DataTable();
        }
        public DataTable CreateTempTable(string UserName, DataTable _Table)
        {
            DataTable ResultTable = new DataTable();
            TempConnection = ConnectionClass.AppTempConnection(UserName);
            MyTableName = _Table.TableName;

            TempTableIsExist(_Table);

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

        #region Get Table From Voucher Number / TranID

        private DataTable FromVouNo(DataTable _Table, string _VouNo)
        {
            
            var _Command = new SQLiteCommand(TempConnection);
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
            
            var _Command = new SQLiteCommand(TempConnection);
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

        #endregion


        #region Refresh Data Tables

        public void TempRefresh()
        {
            var _CommandText = "";
            if (TempTable.Columns.Contains("Vou_No"))
            {
                _CommandText = $"SELECT * FROM {MyTableName} WHERE Vou_No='{VoucherNo}'";
            }
            
            if (TempTable.Columns.Contains("TranID"))
            {
                _CommandText = $"SELECT * FROM {MyTableName} WHERE TranID={TranID}";
            }
            var _Command = new SQLiteCommand(_CommandText, TempConnection);
            var _Adapter = new SQLiteDataAdapter(_Command);
            var _DataSet = new DataSet();

            _Adapter.Fill(_DataSet, MyTableName);
            if (_DataSet.Tables.Count == 1)
            {
                TempTable = _DataSet.Tables[0];
            }
        }

        #endregion

        #region Create or Load Table from Temp DataBase.

        private void TempTableIsExist(DataTable _Table)
        {
            var _TableName = _Table.TableName;

            var _CommandText = $"SELECT count(name) FROM sqlite_master WHERE type = 'table' AND name ='{_TableName}'";
            var _Command = new SQLiteCommand(_CommandText, TempConnection);

            long TableExist = (long)_Command.ExecuteScalar();
            if (TableExist == 0)
            {
                //================================================== Create Table if not exist.
                StringBuilder _Text = new();

                _Text.Append(string.Concat("CREATE TABLE [", MyTableName, "] ("));
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

        #endregion
    }
}
