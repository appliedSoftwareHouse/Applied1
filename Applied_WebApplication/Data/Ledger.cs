using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;

namespace Applied_WebApplication.Data
{
    public class Ledger
    {
        public string UserName { get; set; }
        public Tables TableName { get; set; }
        public DateTime Date_From { get; set; }
        public DateTime Date_To { get; set; }
        public string Sort { get; set; }
        public DataTable Records { get => GetRecords(); }


        public Ledger(string _UserName)
        {
            UserName = _UserName;


        }

        private DataTable GetRecords()                                                                                                      // Get Records for Ledger.
        {
            if (UserName == null || string.IsNullOrEmpty(UserName)) { return new DataTable(); }
            if (TableName.ToString() == "CashBook") { return GetLedger_CashBook(); }                                // Get Ledger Record from CashBook
            return new DataTable();
        }

        private DataTable GetLedger_CashBook()
        {
            DataTableClass _Temp = new(UserName, Tables.view_Ledger);
            DataTable _Ledger = _Temp.MyDataTable.Clone();
            _Temp = new(UserName, Tables.CashBook);
            _Temp.MyDataView.Sort = Sort;
            decimal Balance = 0M;
            bool IsBalance = false;
            bool IsFirstOBal = false;
            decimal Debit = 0M;
            decimal Credit = 0M;
            DataRow _NewRow;

            foreach (DataRow _Row in _Temp.MyDataView.ToTable().Rows)
            {
                if ((DateTime)_Row["Vou_Date"] < Date_From)
                {
                    Balance = Balance + ((decimal)_Row["DR"] - (decimal)_Row["CR"]);
                    IsFirstOBal = true;
                }
                else
                {
                    if ((DateTime)_Row["Vou_Date"] <= Date_To)
                    {
                        if (!IsBalance)
                        {
                            if (IsFirstOBal)
                            {
                                _NewRow = _Ledger.NewRow();
                                _NewRow["ID"] = 0;
                                _NewRow["Vou_Type"] = "Cash";
                                _NewRow["Vou_Date"] = _Row["Vou_Date"];
                                _NewRow["Vou_No"] = "OBal";
                                _NewRow["Description"] = "Opening Balannce";
                                if (Balance >= 0)
                                {
                                    _NewRow["DR"] = Balance;
                                    _NewRow["CR"] = 0;
                                    _NewRow["BAL"] = Balance;
                                }
                                else
                                {
                                    _NewRow["DR"] = 0;
                                    _NewRow["CR"] = Balance;
                                    _NewRow["BAL"] = Balance;
                                }

                                _Ledger.Rows.Add(_NewRow);
                                IsBalance = true;
                            }
                        }

                        Debit = decimal.Parse(_Row["DR"].ToString());
                        Credit = decimal.Parse(_Row["CR"].ToString());
                        Balance = Balance + (Debit - Credit);

                        _NewRow = _Ledger.NewRow();
                        _NewRow["ID"] = _Row["ID"];
                        _NewRow["Vou_Type"] = "Cash";
                        _NewRow["Vou_Date"] = _Row["Vou_Date"];
                        _NewRow["Vou_No"] = _Row["Vou_No"];
                        _NewRow["Description"] = _Row["Description"];
                        _NewRow["DR"] = _Row["DR"];
                        _NewRow["CR"] = _Row["CR"];
                        _NewRow["BAL"] = Balance;
                        _Ledger.Rows.Add(_NewRow);
                    }
                }
            }
            return _Ledger;
        }
    }
}
