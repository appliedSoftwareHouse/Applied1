using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic;
using System.Data;
using System.Data.SQLite;
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
            { MyDataView.RowFilter = Filter;  return true; }
            else
            { MyDataView.RowFilter = Filter; return false; }
        }

        public DataRow SeekRecord(int _ID)
        {
            DataRow row = null;
            string Filter = MyDataView.RowFilter;
            MyDataView.RowFilter = "ID=" + _ID.ToString();

            if (MyDataView.Count > 0)
            { row= MyDataView[0].Row;  }
            else
            { row=MyDataTable.NewRow(); }

            CurrentRow = row;
            MyDataView.RowFilter=Filter; return row;

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

        //======================================================== eof
    }
}
