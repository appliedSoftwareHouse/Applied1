using System.Data;

namespace Applied_WebApplication.Data
{
    public class UnpostClass
    {
        public string MyMessage { get; set; }


        #region Cash book
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
                    if (tb_Ledger.Rows.Count > 0)
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
        #endregion

        #region Bank book
        public static bool Unpost_BankBook(string UserName, int ID)
        {
            var _Result = false;
            DataTableClass Ledger = new(UserName, Tables.Ledger);
            DataTableClass BankBook = new(UserName, Tables.BankBook);
            BankBook.MyDataView.RowFilter = $"ID={ID}";
            string Filter = $"TranID={ID} AND Vou_Type='{VoucherType.Bank}'";
            DataTable tb_Ledger = DataTableClass.GetTable(UserName, Tables.Ledger, Filter);

            try
            {
                if (BankBook.Count == 1)
                {
                    if (tb_Ledger.Rows.Count > 0)
                    {
                        foreach (DataRow Row in tb_Ledger.Rows)
                        {
                            if (Ledger.Seek((int)Row["ID"]))
                            {
                                Ledger.SeekRecord((int)Row["ID"]);
                                Ledger.Delete();
                            }
                        }
                        if (BankBook.Seek(ID))
                        {
                            BankBook.SeekRecord(ID);
                            BankBook.Replace(ID, "Status", VoucherStatus.Submitted);
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
        #endregion

        #region Bill Payable
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
        #endregion

        #region Bill Receivable
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
                if (BillReceivable.CountView == 1)
                {
                    if (tb_Ledger.Rows.Count > 0)
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

        internal static bool UnpostProduction(string UserName, int ID)
        {
            var _Result = false;
            var _VouType = VoucherType.Production.ToString();
            var _Filter = $"[Vou_Type] = '{_VouType}' AND [TranID] = {ID}";
            var _LedgerClass = new DataTableClass(UserName, Tables.Ledger, _Filter);
            var _LedgerTable = _LedgerClass.MyDataTable;

            if (_LedgerTable.Rows.Count >= 2)
            {
                var _VouNo = _LedgerTable.Rows[0]["Vou_No"].ToString();
                foreach (DataRow Row in _LedgerTable.Rows)
                {
                    if (Row["Vou_No"].ToString() == _VouNo)
                    {
                        _LedgerClass.CurrentRow = Row;
                        _LedgerClass.Delete();

                    }
                    _Result = true;
                }
            }

            return _Result;

            //try
            //{
            //    if (BillReceivable.CountView == 1)
            //    {
            //        if (tb_Ledger.Rows.Count > 0)
            //        {
            //            foreach (DataRow Row in tb_Ledger.Rows)
            //            {
            //                if (Ledger.Seek((int)Row["ID"]))
            //                {
            //                    Ledger.SeekRecord((int)Row["ID"]);
            //                    Ledger.Delete();
            //                }
            //            }
            //            if (BillReceivable.Seek(ID))
            //            {
            //                BillReceivable.SeekRecord(ID);
            //                BillReceivable.Replace(ID, "Status", VoucherStatus.Submitted);
            //            }
            //            _Result = true;
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    _Result = true;
            //}



            //return _Result;
        }
        #endregion

        #region Sale Retrurn
        public static bool UnpostSaleReturn(string UserName, int ID)
        {
            var _Result = false;
            DataTableClass Ledger = new(UserName, Tables.Ledger);
            DataTableClass SaleReturn = new(UserName, Tables.SaleReturn);
            SaleReturn.MyDataView.RowFilter = "ID=" + ID.ToString();
            if (SaleReturn.MyDataView.Count > 0)
            {
                string Filter = $"TranID={(int)SaleReturn.MyDataView[0]["TranID"]} AND Vou_Type='{VoucherType.SaleReturn}'";
                DataTable tb_Ledger = DataTableClass.GetTable(UserName, Tables.Ledger, Filter);

                try
                {
                    if (SaleReturn.Count == 1)
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
                            if (SaleReturn.Seek(ID))
                            {
                                SaleReturn.SeekRecord(ID);
                                SaleReturn.Replace(ID, "Status", VoucherStatus.Submitted);
                            }
                            _Result = true;
                        }
                    }
                }
                catch (Exception)
                {
                    _Result = true;
                }
            }
            return _Result;
        }

        internal static bool UnpostReceipt(string UserName, int ID)
        {
            var _Result = false;
            DataTableClass Ledger = new(UserName, Tables.Ledger);
            DataTableClass Receipt = new(UserName, Tables.Receipts);
            Receipt.MyDataView.RowFilter = $"ID={ID}";
            string Filter = $"TranID={ID} AND Vou_Type='{VoucherType.Receipt}'";
            DataTable tb_Ledger = DataTableClass.GetTable(UserName, Tables.Ledger, Filter);

            try
            {
                if (Receipt.Count == 1)
                {
                    if (tb_Ledger.Rows.Count > 0)
                    {
                        foreach (DataRow Row in tb_Ledger.Rows)
                        {
                            if (Ledger.Seek((int)Row["ID"]))
                            {
                                Ledger.SeekRecord((int)Row["ID"]);
                                Ledger.Delete();
                            }
                        }
                        if (Receipt.Seek(ID))
                        {
                            Receipt.SeekRecord(ID);
                            Receipt.Replace(ID, "Status", VoucherStatus.Submitted);
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
        #endregion
    }
}
