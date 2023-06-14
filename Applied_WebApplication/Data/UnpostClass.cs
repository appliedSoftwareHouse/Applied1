using System.Data;

namespace Applied_WebApplication.Data
{
    public class UnpostClass
    {
        public string MyMessage { get; set; }


        public static bool Unpost_CashBook(string UserName, int ID)
        {
            var _Result = false;
            DataTableClass Ledger = new(UserName, Tables.Ledger);
            DataTableClass CashBook = new(UserName, Tables.CashBook);
            CashBook.MyDataView.RowFilter = $"ID={ID}";
            string Filter = $"TranID={ID} AND Vou_Type='{VoucherType.Cash}'";
            DataTable tb_Ledger = DataTableClass.GetTable(UserName, Tables.Ledger, Filter);

            try
            {
                if (CashBook.Count == 1)
                {
                    if (tb_Ledger.Rows.Count == 2)
                    {
                        foreach (DataRow Row in tb_Ledger.Rows)
                        {
                            if (Ledger.Seek((int)Row["ID"]))
                            {
                                Ledger.SeekRecord((int)Row["ID"]);
                                Ledger.Delete();
                            }
                        }
                        if (CashBook.Seek(ID))
                        {
                            CashBook.SeekRecord(ID);
                            CashBook.Replace(ID, "Status", VoucherStatus.Submitted);
                        }
                        _Result = true;
                    }
                }

            }
            catch (Exception)
            {
                _Result = true;
            }

            return _Result;
        }

        public static bool UnpostBillPayable(string UserName, int ID)
        {
            var _Result = false;
            DataTableClass Ledger = new(UserName, Tables.Ledger);
            DataTableClass BillPayable = new(UserName, Tables.BillPayable);
            BillPayable.MyDataView.RowFilter = "ID=" + ID.ToString();
            string Filter = $"TranID={ID} AND Vou_Type='{VoucherType.Payable}'";
            DataTable tb_Ledger = DataTableClass.GetTable(UserName, Tables.Ledger, Filter);

            try
            {
                if (BillPayable.Count == 1)
                {
                    if (tb_Ledger.Rows.Count >= 2)
                    {
                        foreach (DataRow Row in tb_Ledger.Rows)
                        {
                            if (Ledger.Seek((int)Row["ID"]))
                            {
                                Ledger.SeekRecord((int)Row["ID"]);
                                Ledger.Delete();
                            }
                        }
                        if (BillPayable.Seek(ID))
                        {
                            BillPayable.SeekRecord(ID);
                            BillPayable.Replace(ID, "Status", VoucherStatus.Submitted);
                        }
                        _Result = true;
                    }
                }
            }
            catch (Exception)
            {
                _Result = true;
            }



            return _Result;
        }

        public static bool UnpostBillReceivable(string UserName, int ID)
        {
            var _Result = false;
            DataTableClass Ledger = new(UserName, Tables.Ledger);
            DataTableClass BillReceivable = new(UserName, Tables.BillReceivable);
            BillReceivable.MyDataView.RowFilter = "ID=" + ID.ToString();
            string Filter = $"TranID={ID} AND Vou_Type='{VoucherType.Receivable}'";
            DataTable tb_Ledger = DataTableClass.GetTable(UserName, Tables.Ledger, Filter);

            try
            {
                if (BillReceivable.Count == 1)
                {
                    if (tb_Ledger.Rows.Count >= 2)
                    {
                        foreach (DataRow Row in tb_Ledger.Rows)
                        {
                            if (Ledger.Seek((int)Row["ID"]))
                            {
                                Ledger.SeekRecord((int)Row["ID"]);
                                Ledger.Delete();
                            }
                        }
                        if (BillReceivable.Seek(ID))
                        {
                            BillReceivable.SeekRecord(ID);
                            BillReceivable.Replace(ID, "Status", VoucherStatus.Submitted);
                        }
                        _Result = true;
                    }
                }
            }
            catch (Exception)
            {
                _Result = true;
            }



            return _Result;

        }

    }
}
