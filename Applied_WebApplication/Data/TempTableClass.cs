using Applied_WebApplication.Pages;
using NPOI.HSSF.Model;
using NPOI.OpenXmlFormats.Dml.Chart;
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
        public DataTable MyDataTable { get; set; }
        public DataTable TempVoucher { get; set; }
        public DataView MyDataView { get; set; }
        public string MyTableName { get; set; }
        public Tables TableID { get; set; }
        public UserProfile UProfile { get; set; }
        public string DBPath => UProfile.DBFilePath;
        public string ConnectionString { get; set; }
        public SQLiteConnection MyConnection { get; set; }
        public int Count => MyDataTable.Rows.Count;
        public DataRow CurrentRow;
        public List<Message> ErrorMessages { get; set; }

        private string CommandText { get; set; }
        private SQLiteCommand Command { get; set; }
        private TableValidationClass TableValidate { get; set; }

        #endregion

        public TempTableClass(string _UserName, Tables _TableID, string _VoucherNo)
        {
            UserName = _UserName;
            TableID = _TableID;
            UProfile = new UserProfile(UserName);
            ConnectionString = string.Concat("Data Source=", DBPath);
            MyConnection = new SQLiteConnection(ConnectionString);
            CommandText = string.Format("SELECT * FROM [{0}] WHERE Vou_No='{1}'", TableID.ToString(), _VoucherNo);
            Command = new SQLiteCommand(CommandText, MyConnection);
            MyDataTable = GetDataTable();
            MyDataView = MyDataTable.AsDataView();
            TempVoucher = CreateTempTable(UserName, MyDataTable);
            if (Count > 0) { CurrentRow = MyDataTable.Rows[0]; }
        }
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

        public static DataTable CreateTempTable(string UserName, DataTable _Table)
        {
            if (_Table.Rows.Count == 0) { return new DataTable(); }                    // return  null if voucher not found.

            var MyTableName = _Table.TableName;
            var VoucherNo = _Table.Rows[0]["Vou_No"];
            var ReturnTable = new DataTable();
            var _Connection = AppFunctions.GetTempConnection(UserName);
            var _CommandText = "SELECT count(name) FROM sqlite_master WHERE type = 'table' AND name ='" + MyTableName + "'";
            var _Command = new SQLiteCommand(_CommandText, _Connection);


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

            var CommandText = string.Format("SELECT * FROM [{0}] WHERE Vou_No = '{1}'", MyTableName, VoucherNo);
            _Command.CommandText = new(CommandText);                // Get a Voucher from Temp Table.
            var _Adapter = new SQLiteDataAdapter(_Command);
            var _DataSet = new DataSet();
            _Adapter.Fill(_DataSet, MyTableName);                               // Created a Temp Table


            if (_DataSet.Tables.Count == 1)
            {
                ReturnTable = _DataSet.Tables[0];
                if (ReturnTable.Rows.Count > 0)
                {
                    _Command.CommandText = string.Format("DELETE FROM [{0}] WHERE Vou_No='{1}'", ReturnTable.TableName, VoucherNo);
                    _Command.ExecuteNonQuery();
                }
                foreach (DataRow Row in _Table.Rows)
                {
                    var ThisCommand = CommandInsert(_Connection, _Table, Row);
                    ThisCommand.ExecuteNonQuery();
                }
            }

            // Refresh Table after update records.
            CommandText = string.Format("SELECT * FROM [{0}] WHERE Vou_No = '{1}'", MyTableName, VoucherNo);
            _Command.CommandText = new(CommandText);                // Get a Voucher from Temp Table.
            _Adapter = new SQLiteDataAdapter(_Command);
            _DataSet = new DataSet();
            _Adapter.Fill(_DataSet, MyTableName);
            if (_DataSet.Tables.Count == 1)
            {
                ReturnTable = _DataSet.Tables[0];
            }
            _Command.Connection.Close();                    // Close Database Connection.
            return ReturnTable;
        }

        internal void Save()
        {
            TableValidate = new(MyDataTable);
            TableValidate.Validation(CurrentRow);
            ErrorMessages = TableValidate.MyMessages;
            if (ErrorMessages.Count == 0)
            {
                SQLiteCommand _Command; 
                MyDataView.RowFilter = string.Format("ID={0}", CurrentRow["ID"]);
                if (MyDataView.Count == 1)
                {
                    _Command = CommandUpdate(MyConnection, MyDataTable, CurrentRow);
                }
                else
                {
                    _Command = CommandInsert(MyConnection, MyDataTable, CurrentRow);
                }
                try
                {
                   if(_Command.Connection.State!=ConnectionState.Open) { _Command.Connection.Open(); }
                    var _Record = _Command.ExecuteNonQuery(); _Command.Connection.Close();
                    if (_Record==0) { ErrorMessages.Add(MessageClass.SetMessage("No Record Saved", Color.Red)); }
                }
                catch (Exception e)
                {
                    ErrorMessages.Add(MessageClass.SetMessage(e.Message, Color.Red));
                    
                }

            }
        }

        internal void Delete()
        {
            try
            {
                
                var _Command = CommandDelete(MyConnection, MyDataTable, CurrentRow);
                if (_Command.Connection.State != ConnectionState.Open) { _Command.Connection.Open(); }
                var _Records = _Command.ExecuteNonQuery(); _Command.Connection.Close();
                if (_Records == 0) { ErrorMessages.Add(MessageClass.SetMessage("No Record Delete...", Color.Red)); }

            }
            catch (Exception e)
            {
                ErrorMessages.Add(MessageClass.SetMessage(e.Message, Color.Red));
            }

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
            var _Columns = _Table.Columns;
            var _Command = new SQLiteCommand(_Connection);
            var _CommandString = new StringBuilder();
            var _LastColumn = _Columns[_Columns.Count - 1].ColumnName.ToString();
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
