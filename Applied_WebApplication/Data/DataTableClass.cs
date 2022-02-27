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

        //======================================================== eof
    }
}
