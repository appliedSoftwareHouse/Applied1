using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Security.Cryptography;

namespace Applied_WebApplication.Data
{
    public class DataTableClass
    {

        private ConnectionClass MyConnectionClass = new ConnectionClass();
        public DataTable MyDataTable = new();
        public DataView MyDataView = new();
        public SQLiteConnection MyConnection = new();
        public string MyTableName = "";
        public bool IsError = false;
        public string View_Filter { get; set; } = "";
        public DataRow CurrentRow { get; set; }

        private SQLiteCommand Command_Update;
        private SQLiteCommand Command_Delete;
        private SQLiteCommand Command_Insert;


        public DataTableClass()
        {
            MyConnection = MyConnectionClass.AppliedConnection;
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

        public DataTableClass(string _TableName, int _ID)
        {
            MyConnection = MyConnectionClass.AppliedConnection;
            MyTableName = _TableName;
            GetDataTable();                                                                                   // Load DataTable and View
            MyDataView.RowFilter = View_Filter;                                                  // Set a view filter for table view.
            CheckError();

            Command_Update = new SQLiteCommand(MyConnection);
            Command_Delete = new SQLiteCommand(MyConnection);
            Command_Insert = new SQLiteCommand(MyConnection);

            CurrentRow = SeekRecord(_ID);
        }



        public DataRow NewRecord()
        {
            CurrentRow = MyDataTable.NewRow();
            return CurrentRow;
        }

        #region GET Commands

        private void CommandInsert()
        {
            Command_Insert.Parameters.AddWithValue("@ID", CurrentRow["ID"]);
            Command_Insert.Parameters.AddWithValue("@Title", CurrentRow["Title"]);
            Command_Insert.CommandText = "INSERT INTO [" + MyTableName + "] VALUES (@ID, @Title)";
        }
        private void CommandUpdate()
        {
            Command_Update.Parameters.AddWithValue("@ID", CurrentRow["ID"]);
            Command_Update.Parameters.AddWithValue("@Title", CurrentRow["Title"]);
            Command_Update.CommandText = "UPDATE [" + MyTableName + "] SET ID=@ID, Title=@Title WHERE ID=@ID";
        }
        private void CommandDelete()
        {
            Command_Update.Parameters.AddWithValue("@ID", 0);
            Command_Update.CommandText = "DELETE FROM [" + MyTableName + "]  WHERE ID=@ID";
        }


        #endregion

        public DataRow? UserRow()
        {
            if (ViewRecordCount() == 1)
            {
                return MyDataView[0].Row;
            }
            else
            {
                return null;
            }
        }

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

            SQLiteCommand _Command = new("SELECT * FROM [" + MyTableName + "]", MyConnection);
            SQLiteDataAdapter _Adapter = new(_Command);
            DataSet _DataSet = new();
            _Adapter.Fill(_DataSet, MyTableName);

            if (_DataSet.Tables.Count == 1)
            {
                MyDataTable = _DataSet.Tables[0];
                MyDataView.Table = MyDataTable;
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
            { MyDataView.RowFilter = Filter; return true; }
            else
            { MyDataView.RowFilter = Filter; return false; }
        }

        public DataRow SeekRecord(int _ID)
        {
            DataRow row = null;
            string Filter = MyDataView.RowFilter;
            MyDataView.RowFilter = "ID=" + _ID.ToString();

            if (MyDataView.Count > 0)
            { row = MyDataView[0].Row; }
            else
            { row = MyDataTable.NewRow(); }

            CurrentRow = row;
            MyDataView.RowFilter = Filter; return row;

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


        public int ViewRecordCount() { return MyDataView.Count; }

        public bool Save()
        {
            if (CurrentRow != null)
            {

                MyDataView.RowFilter = "ID=" + CurrentRow["ID"].ToString();

                if (MyDataView.Count > 0)
                {
                    CommandUpdate();
                    Command_Update.Connection.Open();
                    Command_Update.ExecuteNonQuery();
                    GetDataTable();

                }
                else
                {
                    CommandInsert();
                    Command_Insert.Connection.Open();
                    Command_Insert.ExecuteNonQuery();
                    GetDataTable();
                }
            }

            return true;
        }

        #endregion

        #region SQLite Insert and Update Command

        public bool TableRowInsert(DataRow _Row)
        {
            DataColumnCollection _Columns = _Row.Table.Columns;
            SQLiteCommand _Command = new SQLiteCommand(MyConnection);

            StringBuilder _CommandString = new StringBuilder();
            string _LastColumn = _Columns[_Columns.Count - 1].ColumnName.ToString();
            string _TableName =  _Row.Table.TableName;

            _CommandString.Append("INSERT INTO ");
           _CommandString.Append(_TableName);
           _CommandString.Append(" ( ");

            foreach (DataColumn _Column in _Columns)
            {
                string _ColumnName = _Column.ColumnName.ToString();
                _CommandString.Append(string.Concat("[",_Column.ColumnName,"]"));

                if(_ColumnName != _LastColumn)
                { _CommandString.Append(",");  }
                else
                { _CommandString.Append(") ");  }
            }

            _CommandString.Remove(_CommandString.ToString().Trim().Length - 1, 1);


            return true;
        }

        #endregion

       
        //    If _ColumnName<> _LastColumn Then
        //        _CommandString.Append(",")
        //    Else
        //        _CommandString.Append(") ")
        //    End If
        //Next

        //_CommandString.Remove(_CommandString.ToString().Trim().Length - 1, 1)
        //_CommandString.Append(") VALUES (")

        //For Each _Column As DataColumn In _Columns
        //    Dim _ColumnName As String = _Column.ColumnName
        //    _CommandString.Append(String.Concat("@", _Column.ColumnName.Replace(" ", "")))

        //    If _ColumnName<> _LastColumn Then
        //        _CommandString.Append(",")
        //    Else
        //        _CommandString.Append(") ")
        //    End If
        //Next

        //Return _CommandString.ToString


        //Public Function SQLiteInsert(_DataRow As DataRow, _Connection As SQLiteConnection) As SQLiteCommand

        //        'Dim _Connection As Data.sql

        //        Dim _Columns As DataColumnCollection = _DataRow.Table.Columns                       ' Assign Columns to create SQLite Command.
        //        Dim _Command As New SQLiteCommand(General.SQLInsert(_Columns), _Connection)
        //        Dim _ParmaterName As String

        //        For Each _Column As DataColumn In _Columns
        //            If _Column Is Nothing Then
        //                Continue For
        //            End If

        //            _ParmaterName = String.Concat("@" & _Column.ColumnName.Replace(" ", ""))
        //            _Command.Parameters.AddWithValue(_ParmaterName, _DataRow(_Column.ColumnName))
        //        Next

        //        Return _Command
        //    End Function



        //======================================================== eof
    }
}

public class MySQLiteCommand
{


    //    Public Function SQLiteUpdate(ByVal _DataRow As DataRow, _PrimaryKeyName As String, _Connection As SQLiteConnection) As SQLiteCommand

    //        Dim _Command As New SQLiteCommand(General.SQLUpdate(_DataRow.Table.Columns, _PrimaryKeyName), _Connection)
    //        Dim _ParmaterName As String

    //        For Each _Column As DataColumn In _DataRow.Table.Columns
    //            If _Column Is Nothing Then
    //                Continue For
    //            End If

    //            _ParmaterName = String.Concat("@" & _Column.ColumnName.Replace(" ", ""))
    //            _Command.Parameters.AddWithValue(_ParmaterName, _DataRow(_Column.ColumnName))
    //        Next

    //        Return _Command
    //    End Function


}
