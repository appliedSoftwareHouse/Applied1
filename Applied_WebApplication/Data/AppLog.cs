using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;

namespace Applied_WebApplication.Data
{
    public class AppLog
    {
        public string UserName { get; set; }
        public LogModel Model { get; set; } = new();
        public List<LogModel> LogList { get; set; }
        public SQLiteConnection MyConnection { get; set; }


        public AppLog(string _UserName)
        {
            UserName = _UserName;
            MyConnection = ConnectionClass.AppConnection(UserName);

        }
        public VoucherType GetVouTypeValue(string Vou_Type)
        {
            if (Vou_Type == VoucherType.None.ToString()) { return VoucherType.None; }
            if (Vou_Type == VoucherType.Cash.ToString()) { return VoucherType.Cash; }
            if (Vou_Type == VoucherType.Bank.ToString()) { return VoucherType.Bank; }
            if (Vou_Type == VoucherType.OBalance.ToString()) { return VoucherType.OBalance; }
            if (Vou_Type == VoucherType.OBalCom.ToString()) { return VoucherType.OBalCom; }
            if (Vou_Type == VoucherType.OBalStock.ToString()) { return VoucherType.OBalStock; }
            if (Vou_Type == VoucherType.Payable.ToString()) { return VoucherType.Payable; }
            if (Vou_Type == VoucherType.Receivable.ToString()) { return VoucherType.Receivable; }
            if (Vou_Type == VoucherType.Production.ToString()) { return VoucherType.Production; }
            if (Vou_Type == VoucherType.Cheque.ToString()) { return VoucherType.Cheque; }
            if (Vou_Type == VoucherType.JV.ToString()) { return VoucherType.JV; }
            if (Vou_Type == VoucherType.Payment.ToString()) { return VoucherType.Payment; }
            if (Vou_Type == VoucherType.Receipt.ToString()) { return VoucherType.Receipt; }
            return VoucherType.None;

        }

        public bool Insert()
        {
            if(Model.SourceTableName == Tables.Registry.ToString()) { return true; }

            int _MaxID;
            int _Records;
            SQLiteCommand _Command = new(MyConnection)
            {
                CommandText = "SELECT MAX(ID) AS [MAXID] FROM [Log]"
            };

            try
            {
                _Records = int.Parse(_Command.ExecuteScalar().ToString());

                if (_Records > 0) { _MaxID = _Records; }
                else { _MaxID = 1; }

            

            _Command.CommandText = "INSERT INTO [Log] VALUES (@ID,@User,@LogDate,@LogCat,@VouType,@LogMessage,@SourceTableName,@Comments)";
            _Command.Parameters.AddWithValue("@ID", _MaxID + 1);
            _Command.Parameters.AddWithValue("@User", Model.User);
            _Command.Parameters.AddWithValue("@LogDate", DateTime.Now);
            _Command.Parameters.AddWithValue("@LogCat", (int)Model.LogCat);
            _Command.Parameters.AddWithValue("@VouType", (int)Model.VouType);
            _Command.Parameters.AddWithValue("@LogMessage", Model.LogMessage);
            _Command.Parameters.AddWithValue("@SourceTableName", Model.SourceTableName);
            _Command.Parameters.AddWithValue("@Comments", Model.Comments);
            _Records = _Command.ExecuteNonQuery();

            if (_Records > 0) { return true; }
            }
            catch (Exception)
            {
                _MaxID = 1;
            }
            return false;
        }

        public string GetMessage(DataRow _Row)
        {
            var _Text = new StringBuilder();

            foreach(DataColumn Column in _Row.Table.Columns)
            {
                _Text.Append($"[{Column.ColumnName}:");
                _Text.Append($"{_Row[Column.ColumnName]}] ");
            }

            return _Text.ToString();
        }

    }

    public class LogModel
    {
        public int ID { get; set; }
        public string User { get; set; }
        public LogCategory LogCat { get; set; } = LogCategory.none;
        public VoucherType VouType { get; set; }
        public DateTime LogDate { get; set; }
        public string LogMessage { get; set; } = string.Empty;
        public string SourceTableName { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;

    }

    public enum LogCategory
    {
        none,
        Login,
        RecordDelete,
        RecordUpdate,
        RecordInsert
    }



}
