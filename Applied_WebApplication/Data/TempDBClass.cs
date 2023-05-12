using NPOI.HSSF.Record;
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
        public List<Message> MyMessages { get; set; }

        public int CountSource => SourceTable.Rows.Count;
        public int CountTemp => TempTable.Rows.Count;

        public string ViewFilter { get; set; }


        public string VoucherNo { get; set; }
        public int TranID { get; set; }

        public bool IsLoad { get; set; }
        public bool IsNew { get; set; }

        public TempDBClass(string _UserName, Tables _Table, string _Filter, bool _IsLoad)
        {
            UserName = _UserName;
            MyTableName = _Table.ToString();
            //VoucherNo = _VouNo;
            IsLoad = _IsLoad;
            ViewFilter = _Filter;
            MyMessages = new();

            

            if (ViewFilter.Length == 0)
            {
                MyMessages.Add(MessageClass.SetMessage("No Voucher Number assigned to proceed."));
                return;
            }

            MyConnection = ConnectionClass.AppConnection(UserName);
            MyTempConnection = ConnectionClass.AppTempConnection(UserName);

            SourceTable = DataTableClass.GetTable(UserName, _Table, ViewFilter);
        
            CreateTempTable(SourceTable);           // Create a Temp Table if not exist;
            TempTable = DataTableClass.GetTable(UserName, _Table, ViewFilter, MyTempConnection);

            if (CountSource > 0 && CountTemp > 0)
            {
                if (IsLoad)
                {
                    foreach (DataRow Row in TempTable.Rows)
                    {
                        MyCommandText = $"DELETE FROM {MyTableName} WHERE {ViewFilter}";
                        MyCommand = new(MyCommandText, MyTempConnection);
                        var _Records = MyCommand.ExecuteNonQuery();
                        MyMessages.Add(MessageClass.SetMessage($"{_Records} effected.", -1, Color.Green));
                        TempTable = DataTableClass.GetTable(UserName, _Table, ViewFilter, MyTempConnection);           // Refresh Temp Table.
                    }
                }
            }

            if(SourceTable.Rows.Count > 0) 
            {
            
            
            
            }




            SourceView = SourceTable.AsDataView();
            TempView = TempTable.AsDataView();

            if (CurrentRow == null)
            {
                if (SourceTable.Rows.Count > 0) { CurrentRow = SourceTable.Rows[0]; } else { CurrentRow = NewRecord(); }
            }
        }

        public TempDBClass(string _UserName, Tables _Table, int _TranID, bool _IsLoad)
        {

        }


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

        public bool Save()
        {

            return false;
        }

        public bool Delete()
        {

            return false;
        }

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

    }
}
