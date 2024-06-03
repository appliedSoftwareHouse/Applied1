using System.Data.SQLite;
using System.Data;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Data
{
    public class TempDBClass2
    {
        public AppliedTemp.ConnectionClass TempConnectionClass { get; set; }
        public SQLiteConnection TempConnection => TempConnectionClass.TempDBConnection;
        public AppliedTemp.UserProfileModel UserProfileTemp { get; set; }
        public ClaimsPrincipal AppUser { get; set; }
        public DataTable SourceTable { get; set; } = new();
        public Tables SourceTableID { get; set; }
        public DataTable TempTable { get; set; } = new();
        public DataRow CurrentRow { get; set; }
        public Guid TempGUID { get; set; }
        public string TableName => SourceTable.TableName;
        public string UserName => UserProfileTemp.UserName;
        public string RegistryKey { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsTempTableCreated { get; set; } = false;
        public bool IsTempTableDelete { get; set; } = false;
        public decimal Tot_DR { get; set; }
        public decimal Tot_CR { get; set; }

        #region Constructor

        public TempDBClass2() { }

        public TempDBClass2(ClaimsPrincipal _AppUser, DataTable _SourceTable, string _RegistryKey)
        {
            try
            {
                AppUser = _AppUser;
                SourceTable = _SourceTable;
                UserProfileTemp = GetUserProfile();
                TempConnectionClass = new(UserProfileTemp);
                RegistryKey = _RegistryKey;
                TempGUID = new Guid();

                var _KeyValue = AppRegistry.GetText(UserProfileTemp.UserName, RegistryKey);
                if(_KeyValue.Length==0) { TempGUID = Guid.NewGuid(); } 
                else {  TempGUID = new Guid(_KeyValue); }

                CreateTempTable();

                if (TempTable.Rows.Count > 0)
                {
                    CurrentRow = TempTable.Rows[0];
                    Totals();
                }

            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }

        public TempDBClass2(ClaimsPrincipal _AppUser, string _RegistryKey)
        {
            try
            {
                AppUser = _AppUser;
                UserProfileTemp = GetUserProfile();
                TempConnectionClass = new(UserProfileTemp);
                RegistryKey = _RegistryKey;
                var _TempGUID = AppRegistry.GetText(UserProfileTemp.UserName, RegistryKey);
                TempGUID = new(_TempGUID);
                TempTable = LoadTempTable();

                if (TempTable.Rows.Count > 0)
                {
                    CurrentRow = TempTable.Rows[0];
                }
                else
                {
                    CurrentRow = TempTable.NewRow();
                }


            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }


        #endregion

        #region Get User Profile from Claim Principals
        private AppliedTemp.UserProfileModel GetUserProfile()
        {
            var _UserProfileTemp = new AppliedTemp.UserProfileModel()
            {
                UserName = AppUser.Identity.Name,
                TempFolder = AppFunctions.AppGlobals.AppDBTempPath,
                TempFile = "AppliedTemp.db",
                UserRole = UserProfile.GetUserRole(AppUser)
            };

            return _UserProfileTemp;

        }
        #endregion

        #region Create & Load -  a GUID Temp Table / Drop GUID Temp Table.
        public void CreateTempTable()
        {
            if (RegistryKey.Length == 0) { return; }
            if (TempGUID.Equals(null)) { TempGUID = Guid.NewGuid(); }

            #region Delete Temp File
            // Delete GUID Temp Table is already exist....
            //var _GUIDTable = AppRegistry.GetText(UserName, RegistryKey);
            if (TempGUID.ToString().Length > 0)
            {
                DropTempTable(TempGUID.ToString());
            }
            // End deletion of Temp Table....
            #endregion


            if (SourceTable is not null)
            {
                try
                {
                    AppRegistry.SetKey(UserName, RegistryKey, TempGUID.ToString(), KeyType.Text);

                    var _Columns = SourceTable.Columns;
                    var _LastColumn = _Columns[_Columns.Count - 1];
                    var _Text = new StringBuilder();
                    _Text.Append($"CREATE TABLE [{TempGUID.ToString()}] (");

                    foreach (DataColumn _Column in _Columns)
                    {
                        var _Name = _Column.ColumnName;
                        var _Type = _Column.DataType;
                        var _ColumnType = string.Empty;

                        if (_Type.Equals(typeof(int))) { _ColumnType = "INT"; }
                        if (_Type.Equals(typeof(string))) { _ColumnType = "NVARCHAR"; }
                        if (_Type.Equals(typeof(DateTime))) { _ColumnType = "DATETIME"; }
                        if (_Type.Equals(typeof(decimal))) { _ColumnType = "DECIMAL"; }
                        if (_Type.Equals(typeof(double))) { _ColumnType = "DOUBLE"; }

                        _Text.Append($"{_Name} {_ColumnType}");

                        if (_Column != _LastColumn) { _Text.Append(", "); } else { _Text.Append(") "); }

                    }

                    _Text.Append(";");


                    var _Command = new SQLiteCommand(_Text.ToString(), TempConnection);
                    var _effacted = _Command.ExecuteNonQuery();
                    TempTable = LoadTempTable();
                    var _Colummns = SourceTable.Columns;

                    foreach (DataRow Row in SourceTable.Rows)
                    {
                        var _NewRow = TempTable.NewRow();
                        foreach (DataColumn _Column in _Columns)
                        {
                            _NewRow[_Column.ColumnName] = Row[_Column.ColumnName];
                        }

                        _Command = CreateInsertCommand(_NewRow, TempGUID.ToString());
                        var _Effected = _Command.ExecuteNonQuery();
                    }

                    TempTable = LoadTempTable();
                    IsTempTableCreated = false;


                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                }
            }
        }

        public bool DropTempTable(string TempTable)
        {
            var Text = $"SELECT name FROM [sqlite_master] WHERE type='table' AND name = '{TempTable}'";
            SQLiteCommand _Command = new(Text, TempConnection);

            var _TempTableIsExist = _Command.ExecuteScalar();

            if (_TempTableIsExist != null)
            {
                _Command.CommandText = $"DROP TABLE [{TempTable}]";
                _Command.ExecuteNonQuery();
                Message = $"Temp Table {TempTable} Deleted.";
                return true;
            }
            else
            {
                Message = $"Temp Table {TempTable} NOTTTTTT Deleted.";
                return false;
            }
        }
        #endregion

        #region Insert / Update / Delete
        public SQLiteCommand CreateInsertCommand(DataRow _Row, string _GUID)
        {
            if (SourceTable == null) { return null; }
            if (TempTable == null) { return null; }

            DataColumnCollection _Columns = TempTable.Columns;
            SQLiteCommand _Command = new SQLiteCommand(TempConnection);

            StringBuilder _CommandString = new StringBuilder();
            string _LastColumn = _Columns[_Columns.Count - 1].ColumnName.ToString();
            string _ParameterName;

            _CommandString.Append($"INSERT INTO [{_GUID}] VALUES (");

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
                _Command.Parameters.AddWithValue(_ParameterName, _Row[_Column.ColumnName]);
            }

            return _Command;
        }

        public SQLiteCommand CreateUpdateCommand(DataRow _Row, string _GUID)
        {
            if (TempTable is null) { return null; }

            DataColumnCollection _Columns = TempTable.Columns;
            SQLiteCommand _Command = new SQLiteCommand(TempConnection);

            StringBuilder _CommandString = new StringBuilder();
            string _LastColumn = _Columns[_Columns.Count - 1].ColumnName.ToString();
            string _ParameterName;

            _CommandString.Append($"UPDATE [{_GUID}] SET ");

            foreach (DataColumn _Column in _Columns)
            {
                string _ColumnName = _Column.ColumnName.ToString();
                if (_ColumnName == "ID") { continue; }                 // Skip, if Column Name is 'ID'
                _CommandString.Append($"{_Column.ColumnName}=");
                _CommandString.Append(string.Concat('@', _Column.ColumnName));
                if (_ColumnName != _LastColumn)
                { _CommandString.Append(','); }
            }

            _CommandString.Append(" WHERE ID=@ID");
            _Command.CommandText = _CommandString.ToString();

            foreach (DataColumn _Column in _Columns)
            {
                if (_Column == null) { continue; }
                _ParameterName = string.Concat('@', _Column.ColumnName.Replace(" ", ""));
                _Command.Parameters.AddWithValue(_ParameterName, _Row[_Column.ColumnName]);
            }
            return _Command;
        }

        public SQLiteCommand CreateDeleteCommand(DataRow _Row, string _GUID)
        {
            if (TempTable is null) { return null; }
            SQLiteCommand _Command = new SQLiteCommand(TempConnection);

            _Command.CommandText = $"DELETE FROM [{_GUID}] WHERE ID=@ID;";
            _Command.Parameters.AddWithValue("@ID", _Row["ID"]);

            return _Command;
        }

        #endregion

        #region Load / Get Temp Table
        public DataTable LoadTempTable()
        {
            try
            {
                var _SQLText = $"SELECT * FROM [{TempGUID}]";
                var _Command = new SQLiteCommand(_SQLText, TempConnection);
                var _Adapter = new SQLiteDataAdapter(_Command);
                var _DataSet = new DataSet();
                _Adapter.Fill(_DataSet, $"[{TempGUID}]");
                if (_DataSet.Tables.Count > 0)
                {
                    return _DataSet.Tables[0];
                }

            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }


            return null;

        }
        #endregion

        #region Voucher DR & CR Total
        public bool Totals()
        {
            //[Status]<>'Deleted'

            var IsDR = decimal.TryParse(TempTable.Compute("SUM(DR)", "").ToString(), out decimal Tot_DR);
            var IsCR = decimal.TryParse(TempTable.Compute("SUM(CR)", "").ToString(), out decimal Tot_CR);
            if (Tot_DR.Equals(Tot_CR)) { return true; }
            return false;

        }
        #endregion

        #region Get CurrentRow 
        public bool GetRow(int ID)
        {
            TempTable.DefaultView.RowFilter = $"ID={ID}";
            if (TempTable.DefaultView.Count == 1)
            {
                CurrentRow = TempTable.DefaultView[0].Row;
                return true;
            }
            return false;
        }

        #endregion

        #region Add a new record in Temp Table
        public bool Insert()
        {
            int _MinID = (int)TempTable.Compute("Min(ID)", "");
            if (_MinID >= 0) { _MinID = -1; } else { _MinID = _MinID - 1; }
            CurrentRow["ID"] = _MinID;
            CurrentRow["Status"] = "Insert";

            var _Command = CreateInsertCommand(CurrentRow, TempGUID.ToString());
            var _Effacted = _Command.ExecuteNonQuery();

            if (_Effacted > 0)
            {
                if (_Effacted == 1)
                {
                    Message = "Record inserted successfully."; return true;
                }
                else
                {
                    Message = "More than one record inserted";
                }
            }
            return false;
        }
        #endregion

        #region Save a Current Row in the Temp Table
        public bool Update()
        {
            CurrentRow["Status"] = "Update";

            var _Command = CreateUpdateCommand(CurrentRow, TempGUID.ToString());
            var _Effacted = _Command.ExecuteNonQuery();
            if (_Effacted > 0) { return true; }
            return false;
        }
        #endregion

        #region Delete a Currrent Row from the Temp Table

        public bool Delete()
        {
            CurrentRow["Status"] = "Delete";
            var _Command = CreateUpdateCommand(CurrentRow, TempGUID.ToString());
            var _Effacted = _Command.ExecuteNonQuery();
            if (_Effacted > 0) { return true; }
            return false;

            // Here Row marked as Deleted and When SAve All then it will be delete from 
            // Source Table and create a new Temp GUID File.

        }


        #endregion


        #region New Row and Removal Null Vales
        public void NewRow()
        {
            CurrentRow = TempTable.NewRow();
            RemoveDBNull();
        }

        public void RemoveDBNull()
        {
            if(CurrentRow == null) { return; }
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

        }
        #endregion

        #region Save / Update Temp Table to Source Table
        public bool SaveToSource()
        {
            if (TempTable is null) { return false; }
            if (UserName is null) { return false; }
            if (UserName.Length == 0) { return false; }

            var SourceClass = new DataTableClass(UserName, SourceTableID);

            foreach (DataRow Row in TempTable.Rows)
            {
                var _Status = Row["Status"].ToString();

                if (_Status != "Deleted")
                {
                    if ((int)Row["ID"] < 0) { Row["ID"] = 0; }
                    Row["Status"] = "Submitted";
                    var _Columns = Row.Table.Columns;
                    SourceClass.NewRecord();
                    foreach (DataColumn _Column in _Columns)
                    {
                        SourceClass.CurrentRow[_Column.ColumnName] = Row[_Column.ColumnName];
                    }

                    SourceClass.Save();
                }

                if((_Status=="Deleted"))
                {
                    var _Columns = Row.Table.Columns;
                    SourceClass.NewRecord();
                    foreach (DataColumn _Column in _Columns)
                    {
                        SourceClass.CurrentRow[_Column.ColumnName] = Row[_Column.ColumnName];
                    }
                    SourceClass.Delete();
                }
            }

            return false;
        }
        #endregion

    }
}
