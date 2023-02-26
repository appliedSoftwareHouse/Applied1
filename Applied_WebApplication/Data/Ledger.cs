using System.Data;

namespace Applied_WebApplication.Data
{
    public class Ledger
    {
        public string UserName { get; set; }
        public int COA { get; set; }
        public Tables TableName { get; set; }
        public DateTime Date_From { get; set; }
        public DateTime Date_To { get; set; }
        public string Sort { get; set; }
        public string Filter { get; set; }
        public DataTable Records { get => GetRecords(); }



        public Ledger(string _UserName)
        {
            UserName = _UserName;
        }
        private DataTable GetRecords()                                                                                                      // Get Records for Ledger.
        {


            if (UserName == null || string.IsNullOrEmpty(UserName)) { return new DataTable(); }

            LedgerParamaters _Paramaters = new();

            _Paramaters.UserName = UserName;
            _Paramaters.Filter = Filter;
            _Paramaters.Sort = Sort;
            _Paramaters.Date_From = Date_From;
            _Paramaters.Date_To = Date_To;


            if (TableName.ToString() == "CashBook") { return LedgerCashBook(_Paramaters); }                                // Get Ledger Record from CashBook
            return GetEmptyLedger();
        }
        public static DataTable ConvertLedger(string UserName, DataTable _Table)
        {
            DataTable _Ledger = DataTableClass.GetDataView(UserName, Tables.view_Ledger).Clone();
            decimal Balance = 0M;

            #region CashBook
            if (_Table.TableName == "CashBook")
            {
                DataView _DataView = _Table.AsDataView();
                _DataView.Sort = "Vou_Date";

                foreach (DataRow _Row in _DataView.ToTable().Rows)
                {
                    decimal Debit = decimal.Parse(_Row["DR"].ToString());
                    decimal Credit = decimal.Parse(_Row["CR"].ToString());
                    Balance += (Debit - Credit);

                    DataRow _NewRow = _Ledger.NewRow();
                    _NewRow["ID"] = _Row["ID"];
                    _NewRow["Vou_Type"] = "Cash";
                    _NewRow["Vou_Date"] = _Row["Vou_Date"];
                    _NewRow["Vou_No"] = _Row["Vou_No"];
                    _NewRow["Description"] = _Row["Description"];
                    _NewRow["DR"] = Debit;
                    _NewRow["CR"] = Credit;
                    _NewRow["BAL"] = Balance;
                    _Ledger.Rows.Add(_NewRow);
                }
            }
            #endregion

            return _Ledger;
        }

        internal static void UpdateLedger(string UserName, int COA)
        {
            DataTableClass tb_CashBook = new(UserName, Tables.CashBook);
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);


            foreach (DataRow Row in tb_CashBook.MyDataTable.Rows)
            {
                if ((int)Row["BookID"] != COA) { continue; }

                tb_Ledger.CurrentRow = tb_Ledger.NewRecord();
                tb_Ledger.CurrentRow["ID"] = Row["ID"];
                tb_Ledger.CurrentRow["Vou_Type"] = "Cash";
                tb_Ledger.CurrentRow["Vou_No"] = Row["Vou_No"];
                tb_Ledger.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                tb_Ledger.CurrentRow["SR_No"] = 0;
                tb_Ledger.CurrentRow["BookID"] = Row["BookID"];
                tb_Ledger.CurrentRow["COA"] = Row["COA"];
                tb_Ledger.CurrentRow["DR"] = Row["DR"];
                tb_Ledger.CurrentRow["CR"] = Row["CR"];
                tb_Ledger.CurrentRow["Customer"] = Row["Customer"];
                tb_Ledger.CurrentRow["Employee"] = Row["Employee"];
                tb_Ledger.CurrentRow["Inventory"] = 0;
                tb_Ledger.CurrentRow["Project"] = Row["Project"];
                tb_Ledger.CurrentRow["Description"] = Row["Description"];
                tb_Ledger.CurrentRow["Comments"] = Row["Ref_No"];
                tb_Ledger.Save();

            }
        }

        private static DataTable LedgerCashBook(LedgerParamaters Param)
        {
            DataTableClass _Table = new(Param.UserName, Tables.view_Ledger);
            DataTable _Ledger = _Table.MyDataTable.Clone();
            _Table = new(Param.UserName, Tables.CashBook);
            _Table.MyDataView.Sort = Param.Sort;
            _Table.MyDataView.RowFilter = Param.Filter;
            decimal Balance = 0M;
            bool IsBalance = false;
            bool IsFirstOBal = false;
            decimal DR = 0.00M;
            decimal CR = 0.00M;
            DataRow _NewRow;

            foreach (DataRow _Row in _Table.MyDataView.ToTable().Rows)
            {
                if ((DateTime)_Row["Vou_Date"] < Param.Date_From)                                               // Skip vouchers if less than from From Date
                {
                    Balance += ((decimal)_Row["DR"] - (decimal)_Row["CR"]);
                    IsFirstOBal = true;
                }
                else
                {
                    if ((DateTime)_Row["Vou_Date"] <= Param.Date_To)
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
                                _NewRow["Description"] = "Opening Balance";
                                _NewRow["Status"] = "Posted";
                                if (Balance >= 0)                                                                       // Debit Amount of Voucher
                                {
                                    _NewRow["DR"] = Balance;
                                    _NewRow["CR"] = 0;
                                    _NewRow["BAL"] = Balance;
                                }
                                // Credit Amout of voucher
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

                        DR = decimal.Parse(_Row["DR"].ToString());
                        CR = decimal.Parse(_Row["CR"].ToString());
                        Balance += (DR - CR);

                        _NewRow = _Ledger.NewRow();
                        _NewRow["ID"] = _Row["ID"];
                        _NewRow["Vou_Type"] = "Cash";
                        _NewRow["Vou_Date"] = _Row["Vou_Date"];
                        _NewRow["Vou_No"] = _Row["Vou_No"];
                        _NewRow["Description"] = _Row["Description"];
                        _NewRow["DR"] = _Row["DR"];
                        _NewRow["CR"] = _Row["CR"];
                        _NewRow["BAL"] = Balance;
                        _NewRow["Status"] = _Row["Status"];
                        _Ledger.Rows.Add(_NewRow);
                    }
                }
            }
            return _Ledger;
        }

        private DataTable GetEmptyLedger()
        {
            DataTableClass _Table = new(UserName, Tables.view_Ledger);
            DataTable _Ledger = _Table.MyDataTable.Clone();
            return _Ledger;
        }

        internal static DataTable GetGL(string userName, ReportClass.ReportFilters paramaters)
        {
            DataTableClass tb_Ledger = new(userName, Tables.Ledger);
            DataTable Result = tb_Ledger.MyDataTable.Clone();

            tb_Ledger.MyDataView.RowFilter = string.Concat("COA=", paramaters.N_COA);
            tb_Ledger.MyDataView.Sort = "Vou_Date,Vou_No";


            return Generate_LedgerTable(userName, tb_Ledger.MyDataView.ToTable(), paramaters);
        }

        internal static DataTable GetGLCompany(string userName, ReportClass.ReportFilters paramaters)
        {
            DataTableClass tb_Ledger = new(userName, Tables.Ledger);
            DataTable tb_Report = tb_Ledger.MyDataTable.Clone();
            string _Filter = string.Concat("Customer=", paramaters.N_Customer.ToString());

            if (!paramaters.All_COA)
            {
                _Filter = string.Concat(_Filter, " AND COA=", paramaters.N_COA.ToString());
            }

            tb_Ledger.MyDataView.RowFilter = _Filter;

            return Generate_LedgerTable(userName, tb_Ledger.MyDataView.ToTable(), paramaters);
        }

        internal static DataTable Generate_LedgerTable(string userName, DataTable _Table, ReportClass.ReportFilters paramaters)
        {
            DataTableClass tb_Ledger = new(userName, Tables.Ledger);
            DataTable Result = tb_Ledger.MyDataTable.Clone();
            tb_Ledger.MyDataView.Sort = "Vou_Date";
            decimal Balance = 0.00M;
            bool IsOBalEntry = false;
            DataRow NewRow;

            foreach (DataRow Row in _Table.Rows)
            {
                DateTime RowDate = DateTime.Parse(Row["Vou_Date"].ToString());
                decimal RowDR = decimal.Parse(Row["DR"].ToString());
                decimal RowCR = decimal.Parse(Row["CR"].ToString());

                if (RowDate < paramaters.Dt_From)
                {
                    Balance += RowDR - RowCR;
                    continue;
                }      // Skip if date is less from [From]
                else
                {
                    if (RowDate > paramaters.Dt_To) { continue; }                      // Skip if date if more than [To]
                    if (!IsOBalEntry)
                    {
                        NewRow = Result.NewRow();
                        NewRow["Vou_Date"] = paramaters.Dt_From.AddDays(-1);
                        NewRow["Vou_No"] = "OBAL";
                        NewRow["Description"] = "Opening Balance as on " + paramaters.Dt_From.AddDays(-1).ToShortDateString();
                        if (Balance >= 0) { NewRow["DR"] = Balance; NewRow["CR"] = 0; } else { NewRow["DR"] = 0; NewRow["CR"] = Balance * (-1); }
                        Result.Rows.Add(NewRow);
                        IsOBalEntry = true;
                        //continue;
                    }

                    NewRow = Result.NewRow();
                    NewRow["ID"] = Row["ID"];
                    NewRow["TranID"] = Row["TranID"];
                    NewRow["Vou_Date"] = Row["Vou_Date"];
                    NewRow["Vou_No"] = Row["Vou_No"];
                    NewRow["Vou_Type"] = Row["Vou_Type"];
                    NewRow["SR_No"] = Row["SR_No"];
                    NewRow["Ref_No"] = Row["Ref_No"];
                    NewRow["BookID"] = Row["BookID"];
                    NewRow["COA"] = Row["COA"];
                    NewRow["DR"] = Row["DR"];
                    NewRow["CR"] = Row["CR"];
                    NewRow["Description"] = Row["Description"];
                    NewRow["Comments"] = Row["Comments"];
                    NewRow["Customer"] = Row["Customer"];
                    NewRow["Project"] = Row["Project"];
                    NewRow["Employee"] = Row["Employee"];
                    NewRow["Inventory"] = Row["Inventory"];
                    Result.Rows.Add(NewRow);
                }
            }
            return Result;


        }

        internal static DataTable GetTB(string UserName, ReportClass.ReportFilters filters)
        {
            DataTableClass tc_TB = new(UserName, Tables.TB);
            DataTable tb_TrialBal = tc_TB.MyDataTable.Clone();

            foreach (DataRow Row in tc_TB.MyDataTable.Rows)
            {
                decimal _Amount = decimal.Parse(Row["BAL"].ToString());
                decimal _Debit = 0.00M;
                decimal _Credit = 0.00M;
                if (_Amount >= 0) { _Debit = _Amount; _Credit = 0; }
                if (_Amount < 0) { _Debit = 0; _Credit = Math.Abs(_Amount); }

                DataRow RowTB = tb_TrialBal.NewRow();
                RowTB["COA"] = Row["COA"];
                RowTB["Code"] = Row["Code"];
                RowTB["Title"] = Row["Title"];
                RowTB["DR"] = _Debit;
                RowTB["CR"] = _Credit;
                RowTB["Bal"] = _Amount;
                tb_TrialBal.Rows.Add(RowTB);
            }

            return tb_TrialBal;
        }

        public class LedgerParamaters
        {
            public string UserName { get; set; }
            public Tables Table { get; set; }
            public string Sort { get; set; }
            public string Filter { get; set; }
            public DateTime Date_From { get; set; }
            public DateTime Date_To { get; set; }
            public string Status { get; set; }
        }

    }
}
