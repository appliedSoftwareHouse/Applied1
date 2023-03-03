using System.Data;
using System.Drawing;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Data
{
    public class TableValidationClass
    {

        public string SQLAction { get; set; }
        public DataTable MyDataTable { get; set; }

        public List<Message> MyMessages = new List<Message>();
        public PostType MyVoucherType { get; set; }
        private DataView MyDataView { get; set; }
        public int Count => MyMessages.Count;



        private static DateTime FiscalFrom => AppRegistry.GetFiscalFrom();
        private static DateTime FiscalTo => AppRegistry.GetFiscalTo();


        public TableValidationClass()
        {
            MyDataTable = new DataTable();
            MyMessages = new List<Message>();
            MyDataView = MyDataTable.AsDataView();
        }

        public TableValidationClass(DataTable table)
        {
            if(table!=null)
            { 
            MyDataTable = table;
            MyDataView = MyDataTable.AsDataView();
            MyMessages = new List<Message>();
            SQLAction = string.Empty;
            }
        }



        public bool Validation(DataRow Row, CommandAction _SQLAction)
        {
            SQLAction = _SQLAction.ToString();
            return Validation(Row);
        }

        public bool Validation(DataRow Row)
        {

            MyMessages = new List<Message>();
            if (Row == null)
            {
                MyMessages.Add(new Message() { Success = false, ErrorID = 10, Msg = "Current row is null" });
                return false;
            }   // Return false if row is null
            if (SQLAction == null)
            {
                MyMessages.Add(new Message() { Success = false, ErrorID = 10, Msg = "Database query action is not defined." });
                return false;
            }
            if (MyDataTable == null)
            {
                MyMessages.Add(new Message() { Success = false, ErrorID = 10, Msg = "DataTable is null. define Datatable to validate the record." });
                return false;
            }
            //if (MyDataView == null) { if (MyDataTable != null) { MyDataView = MyDataTable.AsDataView(); } }
            if (Row.Table.TableName == Tables.COA.ToString()) { ValidateTable_COA(Row); }
            if (Row.Table.TableName == Tables.Customers.ToString()) { ValidateTable_Customer(Row); }
            if (Row.Table.TableName == Tables.Inventory.ToString()) { ValidateTable_Inventory(Row); }
            if (Row.Table.TableName == Tables.CashBook.ToString()) { ValidateTable_CashBook(Row); }
            if (Row.Table.TableName == Tables.WriteCheques.ToString()) { ValidateTable_WriteChq(Row); }
            if (Row.Table.TableName == Tables.Ledger.ToString()) { ValidateTable_Ledger(Row); }
            if (Row.Table.TableName == Tables.BillPayable.ToString()) { ValidateTable_BillPayable(Row); }
            if (Row.Table.TableName == Tables.BillPayable2.ToString()) { ValidateTable_BillPayable2(Row); }
            if (Row.Table.TableName == Tables.TempLedger.ToString()) { ValidateTable_TempLedger(Row); }
            if (Row.Table.TableName == Tables.FinishedGoods.ToString()) { ValidateTable_FinishedGoods(Row); }
            if (Row.Table.TableName == Tables.BillReceivable.ToString()) { ValidateTable_BillReceivable(Row); }
            if (Row.Table.TableName == Tables.BillReceivable2.ToString()) { ValidateTable_BillReceivable2(Row); }
            if (Row.Table.TableName == Tables.view_BillReceivable.ToString()) { ValidateTable_view_BillReceivable(Row); }
            if (MyMessages.Count > 0) { return false; } else { return true; }
        }




        #region Methods => Seek / Sucess
        private bool Seek(string _Column, string _Value)
        {
            if (MyDataTable != null)
            {
                DataView _DataView = MyDataTable.AsDataView();
                _DataView.RowFilter = _Column + "='" + _Value + "'";
                if (_DataView.Count > 0) { return true; } else { return false; }
            }
            return false;               // Default value    
        }
        public bool Success()
        {
            if (MyMessages.Count > 0)
            { return true; }
            else
            { return false; }
        }

        #endregion

        #region Validation of Tables

        private void ValidateTable_COA(DataRow Row)
        {
            MyMessages = new List<Message>();
            if (SQLAction == CommandAction.Insert.ToString())
            {
                if (Row["Code"].ToString().Trim().Length != 6)                  // Code
                {

                    MyMessages.Add(new Message() { Success = false, ErrorID = 101, Msg = "Code Length must be 6 Character" });
                }

                if (Row["Title"].ToString().Trim().Length == 0)             // Title
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 102, Msg = "Title Length can not be null" });
                }
            }
            if (SQLAction == CommandAction.Update.ToString())
            {
                if (Row["Code"].ToString().Trim().Length != 6)                  // Code
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 101, Msg = "Code Length must be 6 Character" });
                }

                if (Row["Title"].ToString().Trim().Length == 0)             // Title
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 102, Msg = "Title Length can not be null" });
                }
            }
        }
        private void ValidateTable_Customer(DataRow Row)
        {
            MyMessages = new List<Message>();

            #region Insert
            if (SQLAction == CommandAction.Insert.ToString())
            {
                if (Row["Code"] == DBNull.Value)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 103, Msg = "Null value of Code is not allowed." });
                }

                if (Seek("Code", Row["Code"].ToString()))
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 104, Msg = "Code is already assigned. Duplicate value not allowed." });
                }

                if (Row["Title"] == DBNull.Value)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 105, Msg = "Null value of Code is not allowed." });
                }


                if (Seek("Title", Row["Title"].ToString()))
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 106, Msg = "Title is already assigned. duplicate value not allowed." });
                }
            }
            #endregion

            #region Update
            if (SQLAction == CommandAction.Delete.ToString())
            {


            }
            #endregion
        }
        private void ValidateTable_CashBook(DataRow Row)
        {
            MyMessages = new List<Message>();

            if (SQLAction == CommandAction.Insert.ToString())
            {
                if (Seek("ID", Row["ID"].ToString()))
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10501, Msg = "Duplicate of ID found." });
                }

                if (Row["Vou_No"].ToString().Length == 0)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10507, Msg = "Enter Valid Voucher Number." });
                }

                if (Seek("Vou_No", Row["Vou_No"].ToString()))
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10502, Msg = "Duplicate of Voucher No found." });
                }

                if ((int)Row["COA"] == 0)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10505, Msg = "Accounts Head must be selected." });
                }

                if ((decimal)Row["DR"] > 0 && (decimal)Row["CR"] > 0)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10503, Msg = "Only Debit or Credit must be more more than Zero." });
                }

                if ((decimal)Row["DR"] == 0 && (decimal)Row["CR"] == 0)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10504, Msg = "Must be enter Debit or Credit Amount" });
                }


                if (Row["Description"].ToString().Length == 0)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 10506, Msg = "Description must be some charactors." });
                }
            }
        }
        private void ValidateTable_WriteChq(DataRow Row)
        {
            MyMessages = new List<Message>();
            //if (SQLAction == CommandAction.Insert.ToString())
            //{
            //    if (Seek("ID", Row["ID"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 10801, Msg = "Duplicate of ID found." }); }
            //}
            //if (SQLAction == CommandAction.Update.ToString())
            //{
            //    if (!Seek("ID", Row["ID"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 10802, Msg = "Cheque Reccord not found." }); }
            //}

            //if (Row["Code"].ToString() == "New Code") { MyMessages.Add(new Message() { Success = false, ErrorID = 10803, Msg = "Cheque Transaction code is not assigned." }); }
            //if ((int)Row["TranType"] == 1)
            //{
            //    if ((decimal)Row["ChqAmount"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 10802, Msg = "Cheque amount can not be null" }); }
            //}
            //if ((int)Row["Bank"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 10803, Msg = "Bank must be selected." }); }
            //if ((int)Row["Company"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 10804, Msg = "Bank must be selected." }); }
            //if (Row["Status"].ToString() == "") { MyMessages.Add(new Message() { Success = false, ErrorID = 1080, Msg = "Cheque Status must be selected." }); }
        }
        private void ValidateTable_Ledger(DataRow Row)
        {
            MyMessages = new List<Message>();
            int VoucherID = (int)Row["TranID"];

            if (MyVoucherType == PostType.CashBook)
            {
                MyDataView.RowFilter = string.Concat("[Vou_Type]='Cash' AND [TranID]=", VoucherID.ToString());
                if (MyDataView.Count > 0)
                {
                    MyMessages.Add(new Message() { Success = false, ErrorID = 101, Msg = "Voucher is already posted. Unpost voucher and post again." });
                }
            }
        }
        private void ValidateTable_BillPayable(DataRow Row)
        {
            MyMessages = new List<Message>();
            if (SQLAction == CommandAction.Insert.ToString())
            {
                if ((int)Row["ID"] != 0)
                {
                    MyDataView.RowFilter = String.Concat("ID=", Row["ID"].ToString());
                    if (MyDataView.Count > 0)
                    {
                        MyMessages.Add(new Message() { Success = false, ErrorID = 11101, Msg = "Duplicate of ID found." + Row.Table.TableName });
                    }
                }
            }

            if (string.IsNullOrEmpty(Row["Vou_No"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 11101, Msg = "Voucher Number is not define." }); }
            if (string.IsNullOrEmpty(Row["Description"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 11101, Msg = "Description must be some charactors." }); }

            if ((DateTime)Row["Vou_Date"] < AppRegistry.GetFiscalFrom()) { MyMessages.Add(new Message() { Success = false, ErrorID = 11101, Msg = "Voucher Date is less than Fiscal Year Date." }); }
            if ((DateTime)Row["Vou_Date"] > AppRegistry.GetFiscalTo()) { MyMessages.Add(new Message() { Success = false, ErrorID = 11101, Msg = "Voucher Date is higher than Fiscan year end date." }); }
            if ((string.IsNullOrEmpty(Row["Inv_No"].ToString()))) { MyMessages.Add(new Message() { Success = false, ErrorID = 11101, Msg = "Voucher Number value is null, not allowed." }); }
            if ((DateTime)Row["Inv_Date"] < AppRegistry.MinDate) { MyMessages.Add(new Message() { Success = false, ErrorID = 11101, Msg = "Voucher Date is higher than Fiscan year end date." }); }
            if ((DateTime)Row["Pay_Date"] < (DateTime)Row["Inv_Date"]) { MyMessages.Add(new Message() { Success = false, ErrorID = 11101, Msg = "Payment Date is less than invoice date, Enter Valid Date." }); }
            if ((int)Row["Company"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 11101, Msg = "Comapny is not selected. select any one." }); }

        }
        private void ValidateTable_BillPayable2(DataRow Row)
        {
            MyMessages = new List<Message>();

            if (SQLAction == CommandAction.Insert.ToString())
            {
                if ((int)Row["ID"] != 0)
                {
                    MyDataView.RowFilter = String.Concat("ID=", Row["ID"].ToString());
                    if (MyDataView.Count > 0)
                    {
                        MyMessages.Add(new Message() { Success = false, ErrorID = 11201, Msg = "Duplicate of ID found." + Row.Table.TableName });
                    }
                }
            }
            if ((int)Row["Inventory"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 11201, Msg = "Inventory is not selected. Select any one." }); }
            if (string.IsNullOrEmpty(Row["Batch"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 11201, Msg = "Inventory Batch is not define. Nil value not allowed." }); }
            if ((int)Row["Tax"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 11201, Msg = "Tax Category is not selected. Select any one." }); }
            if ((decimal)Row["Qty"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 11201, Msg = "Quantity value is zero.  Zero value is not allowed." }); }
            if ((decimal)Row["Rate"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 11201, Msg = "Rate is zero.  Zero value is not allowed." }); }
        }
        private void ValidateTable_Inventory(DataRow Row)
        {
            MyMessages = new List<Message>();
            if (SQLAction == CommandAction.Insert.ToString())
            {
                if (Seek("Code", Row["Code"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 104, Msg = "Code is already assigned. Duplicate value not allowed." }); }
                if (Seek("Title", Row["Title"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 106, Msg = "Title is already assigned. duplicate value not allowed." }); }
            }
            if (Row["ID"] == null) { MyMessages.Add(new Message() { Success = false, ErrorID = 104, Msg = "Record ID is null. Error in database record. contact to Administrator" }); }
            if (Row["Code"] == DBNull.Value) { MyMessages.Add(new Message() { Success = false, ErrorID = 103, Msg = "Null value of Code is not allowed." }); }
            if (Row["Title"] == DBNull.Value) { MyMessages.Add(new Message() { Success = false, ErrorID = 105, Msg = "Null value of Title is not allowed." }); }
            if (Row["SubCategory"] == DBNull.Value) { MyMessages.Add(new Message() { Success = false, ErrorID = 105, Msg = "Null value of Sub Category is not allowed." }); }
            if ((int)Row["SubCategory"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 105, Msg = "Zero value of Sub Category  is not allowed." }); }
        }
        private void ValidateTable_TempLedger(DataRow Row)
        {
            //MyMessages = new List<Message>();
            //if (SQLAction == CommandAction.Insert.ToString())
            //{
            //    if (Seek("TranID", Row["TranID"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 99901, Msg = "Transaction ID is already assigned. Duplicate value not allowed." }); }
            //}
            //if (Row["ID"] == null) { MyMessages.Add(new Message() { Success = false, ErrorID = 99902, Msg = "Record ID is null. Error in database record. contact to Administrator" }); }
            //if (Row["ID2"] == null) { MyMessages.Add(new Message() { Success = false, ErrorID = 99903, Msg = "Record ID-2 is null. Error in database record. contact to Administrator" }); }
            //if (Row["TranID"] == null) { MyMessages.Add(new Message() { Success = false, ErrorID = 99904, Msg = "Transaction ID is null. Error in database record. contact to Administrator" }); }
            //if (Row["Vou_No"] == null) { MyMessages.Add(new Message() { Success = false, ErrorID = 99905, Msg = "Voucher No is null. Error in database record. contact to Administrator" }); }
        }
        private void ValidateTable_FinishedGoods(DataRow Row)
        {
            MyMessages = new List<Message>();
            if (SQLAction == CommandAction.Insert.ToString())
            {
                if (Seek("ID", Row["ID"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 30601, Msg = "Record ID is already assigned. Duplicate value not allowed." }); }
            }
            if (Row["Batch"] == null) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Batch value is null." }); }
            if (Row["Process"] == null) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Process value is null." }); }

            if (string.IsNullOrEmpty(Row["Batch"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Batch value is null." }); }
            if ((int)Row["Product"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Finished Goods is not selected." }); }
            if ((int)Row["Process"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Process is not selected." }); }
            if ((decimal)Row["Qty"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Quantity is zero, not allowed." }); }
        }
        private void ValidateTable_BillReceivable(DataRow Row)
        {
            MyMessages = new List<Message>();
            if (SQLAction == CommandAction.Insert.ToString())
            {
                if (Seek("ID", Row["ID"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 30601, Msg = "Record ID is already assigned. Duplicate value not allowed." }); }

            }
            if (string.IsNullOrEmpty(Row["Inv_No"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Invoice Number value is null, contact to Administrator." }); }
            if ((int)Row["Company"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Company ID is Zero, not allowed." }); }
            if (string.IsNullOrEmpty(Row["Description"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Description is Zero, not allowed." }); }
            if (string.IsNullOrEmpty(Row["Status"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Status is not defined, must be a valid value." }); }
            if ((DateTime)Row["Vou_Date"] < FiscalFrom) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Voucher Date is below the fiscal year date" }); }
            if ((DateTime)Row["Vou_Date"] > FiscalTo) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Voucher Date is above the fiscal year date" }); }
            if ((DateTime)Row["Vou_Date"] < (DateTime)Row["Inv_Date"]) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Voucher Date is below the invoice Date, not allowed" }); }
            if ((DateTime)Row["Pay_Date"] < (DateTime)Row["Inv_Date"]) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Payment Date is below the invoice Date, not allowed" }); }
        }
        private void ValidateTable_BillReceivable2(DataRow Row)
        {
            MyMessages = new List<Message>();
            if (SQLAction == CommandAction.Insert.ToString())
            {
                if (Seek("ID", Row["ID"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 30601, Msg = "Record ID is already assigned. Duplicate value not allowed." }); }

            }
            if ((int)Row["Sr_No"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Serial Number is Zero, not allowed." }); }
            //if ((int)Row["TranID"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Transaction ID is Zero, not allowed." }); }
            if ((int)Row["Inventory"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Product ID is Zero, not allowed." }); }
            if (string.IsNullOrEmpty(Row["Batch"].ToString())) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Batch Number is zero, not allowed." }); }
            if ((decimal)Row["Qty"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Quantity is Zero, not allowed." }); }
            if ((decimal)Row["Rate"] == 0) { MyMessages.Add(new Message() { Success = false, ErrorID = 30602, Msg = "Rate is Zero, not allowed." }); }


        }
        private void ValidateTable_view_BillReceivable(DataRow Row)
        {
            MyMessages = new List<Message>();
            if (SQLAction == CommandAction.Insert.ToString())
            {
                if (Seek("ID", Row["ID"].ToString())) { MyMessages.Add(SetMessage("ID is already exist in Data Base. Contact to Administrator.")); }
                if (Seek("Vou_No", Row["Vou_No"].ToString())) { MyMessages.Add(SetMessage("Voucher No is already exist in Data Base. Contact to Administrator.")); }
                if (Seek("TranID", Row["TranID"].ToString())) { MyMessages.Add(SetMessage("Bill Receivable No is already exist in Data Base. Contact to Administrator.")); }
            }

            if (string.IsNullOrEmpty(Row["Description"].ToString())) { MyMessages.Add(SetMessage("Description is null, must be some value.")); }
            if (string.IsNullOrEmpty(Row["Ref_No"].ToString())) { MyMessages.Add(SetMessage("Reference No. is null, must be some value.")); }
            if (string.IsNullOrEmpty(Row["Inv_No"].ToString())) { MyMessages.Add(SetMessage("Bill No is null, must be some value.")); }
            if (string.IsNullOrEmpty(Row["Batch"].ToString())) { MyMessages.Add(SetMessage("Batch No is null, must be some value.")); }

            if ((int)Row["Company"] == 0) { MyMessages.Add(SetMessage("Company is not selected. select one.")); }
            if ((int)Row["Inventory"] == 0) { MyMessages.Add(SetMessage("Product is not selected. select one.")); }
            if ((int)Row["Sr_No"] == 0) { MyMessages.Add(SetMessage("Serial No. of bill is zero. must be more than zero.")); }

            if ((decimal)Row["Qty"] == 0) { MyMessages.Add(SetMessage("Quantity is zero, not allowed.", Color.Red)); }
            if ((decimal)Row["Rate"] == 0) { MyMessages.Add(SetMessage("Quantity is zero, not allowed.", Color.Red)); }
            if ((decimal)Row["Tax"] == 0) { MyMessages.Add(SetMessage("Quantity is zero, not allowed.", Color.Red)); }

        }


        #endregion

    }
}