using Microsoft.VisualBasic;
using System.Data;
using System.Data.SQLite;

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

        private string Get_InsertText()
        {
                Command_Insert.Parameters.AddWithValue("@ID", 0);
                Command_Insert.Parameters.AddWithValue("@Title", "");
                return "INSERT INTO [" + MyTableName + "] VALUES (@ID, @Title) WHERE ID=@ID";
        }

        private string Get_DeleteText()
        {
            throw new NotImplementedException();
        }

        private string Get_UpdateText()
        {
           Command_Update.Parameters.AddWithValue("@ID", 0);
            Command_Update.Parameters.AddWithValue("@Title", "");
            return "UPDATE [" + MyTableName + "] SET ID=@ID, Title=@Title WHERE ID=@ID";
        }

        public DataRow? UserRow()
        {
            if(ViewRecordCount() == 1) 
            {
                return MyDataView[0].Row;
            }
            else
            {
                return null; 
            }
        }

        public int ViewRecordCount() { return MyDataView.Count; }

        private void CheckError()
        {
            if (MyConnectionClass.AppliedConnection == null) { IsError = true; }
            if (MyTableName == null) { IsError = true; }
            if (MyTableName.Length == 0) { IsError = true; }
            if(MyDataTable==null) { IsError = true; }
            if(MyDataView==null) { IsError = true; }
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

        public bool Seek(Double _ID)
        {
            DataView _TableView = MyDataView;
            _TableView.RowFilter = "ID=" + _ID.ToString();

            if(_TableView.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SaveCurrentRow()
        {
            MyDataView.RowFilter = "ID=" + CurrentRow["ID"].ToString();

            if (MyDataView.Count > 0)      
            {

                Command_Update.ExecuteNonQuery(); 
            }
            else  
            {
                Command_Insert_Set(CurrentRow);
                Command_Insert.Connection.Open();
                Command_Insert.ExecuteNonQuery();
            }

            return true;
        }


        public void Command_Insert_Set(DataRow _Row)
        {
            Command_Insert.Parameters.AddWithValue("@ID", CurrentRow["ID"]);
            Command_Insert.Parameters.AddWithValue("@Title", CurrentRow["Title"]);
            Command_Insert.CommandText = "INSERT INTO [" + MyTableName + "] VALUES (@ID, @Title)";
        }
        //======================================================== eof
    }
}
