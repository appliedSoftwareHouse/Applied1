using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Sales
{
    public class SaleReturnModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public List<Message> MyMessages { get; set; }
        public string UserName => User.Identity.Name;
        public string UserRole => UserProfile.GetUserRole(User);
        public DataTable Receivable1 { get; set; }
        public DataTable Receivable2 { get; set; }

        #region Get

        public void OnGet()
        {
            MyMessages = new();
            GetVariables();
            var Date1 = AppFunctions.MinDate(Variables.Start, Variables.End).AddDays(-1).ToString(AppRegistry.DateYMD);
            var Date2 = AppFunctions.MaxDate(Variables.Start, Variables.End).AddDays(1).ToString(AppRegistry.DateYMD);
            var _Filter = $"Date([Vou_Date]) > Date('{Date1}') AND Date([Vou_Date]) < Date('{Date2}') ";
            if (Variables.Company > 0) { _Filter += $"AND Company={Variables.Company}"; }
            var _SQLSalesList = SQLQuery.SaleReturn(_Filter) + " order by [Vou_Date]";
            var tb_SaleReturn = new DataTableClass(UserName, _SQLSalesList, Tables.SaleReturn);
            Receivable1 = tb_SaleReturn.MyDataTable;
        }
        public void OnGetEdit(int ID)
        {
            MyMessages = new();
            GetVariables();
            var Date1 = AppFunctions.MinDate(Variables.Start, Variables.End).AddDays(-1).ToString(AppRegistry.DateYMD);
            var Date2 = AppFunctions.MaxDate(Variables.Start, Variables.End).AddDays(1).ToString(AppRegistry.DateYMD);
            var _Filter = $"Date([Vou_Date]) > Date('{Date1}') AND Date([Vou_Date]) < Date('{Date2}') ";
            if (Variables.Company > 0) { _Filter += $"AND Company={Variables.Company}"; }
            var _SQLSalesList = SQLQuery.SaleReturn(_Filter) + " order by [Vou_Date]";
            var tb_SaleReturn = new DataTableClass(UserName, _SQLSalesList, Tables.SaleReturn);
            Receivable1 = tb_SaleReturn.MyDataTable;

            tb_SaleReturn.SeekRecord(ID);

            Variables.Vou_No = (string)tb_SaleReturn.CurrentRow["Vou_No"];
            Variables.Vou_Date = (DateTime)tb_SaleReturn.CurrentRow["Vou_Date"];
            Variables.SaleVou_No = (string)tb_SaleReturn.CurrentRow["SaleVou_No"];
            Variables.StockTitle = (string)tb_SaleReturn.CurrentRow["StockTitle"];
            Variables.Batch = (string)tb_SaleReturn.CurrentRow["Batch"];
            Variables.TranID = ID;
            Variables.Qty = Conversion.ToDecimal(tb_SaleReturn.CurrentRow["Qty"]);
            Variables.RQty = Conversion.ToDecimal(tb_SaleReturn.CurrentRow["RQty"]);

            if (Variables.Vou_No.Length == 0)
            {
                //var _Vou_Date = (DateTime)tb_SaleReturn.CurrentRow["Vou_Date"];
                //Variables.Vou_No = GetNewVouNo(_Vou_Date, "SR");
                Variables.Vou_No = "New";
                Variables.Vou_Date = DateTime.Now;
            }

        }
        #endregion

        #region Post

        public IActionResult OnPostRefresh()
        {
            MyMessages = new();
            SetVariables();
            return RedirectToPage();
        }

        public IActionResult OnPostStockReturn(int? id)
        {
            SetVariables();
            id ??= 0;
            var _ID = Conversion.ToInteger(id);
            return RedirectToPage("SaleReturn", "Edit", new { ID = _ID });
        }

        public IActionResult OnPostSave(int TranID)
        {
            if(UserRole=="Viewer") { return Page(); }
            MyMessages = new();
            var tb_SaleReturn = new DataTableClass(UserName, Tables.SaleReturn);
            Receivable1 = tb_SaleReturn.MyDataTable;
            tb_SaleReturn.MyDataView.RowFilter = $"TranID={TranID}";
            if (tb_SaleReturn.CountView == 0)
            {
                tb_SaleReturn.NewRecord();
                tb_SaleReturn.CurrentRow["ID"] = 0;
                tb_SaleReturn.CurrentRow["Vou_No"] = GetNewVouNo((DateTime)tb_SaleReturn.CurrentRow["Vou_Date"],"SR");                // Generate a New Voucher No.
                tb_SaleReturn.CurrentRow["TranID"] = Variables.TranID;
                
            }
            else
            {
                tb_SaleReturn.CurrentRow = tb_SaleReturn.MyDataView[0].Row;
            }

            //tb_SaleReturn.CurrentRow["Vou_No"] = Variables.Vou_No;
            tb_SaleReturn.CurrentRow["Vou_Date"] = Variables.Vou_Date;
            tb_SaleReturn.CurrentRow["Qty"] = Variables.RQty;
            tb_SaleReturn.CurrentRow["Status"] = VoucherStatus.Submitted.ToString();
            tb_SaleReturn.Save();
            if (tb_SaleReturn.IsError)
            {
                MyMessages.Add(SetMessage("ERROR: Sale Return Record do not save. Contact to Administrator"));
            }
            else
            {
                MyMessages.Add(SetMessage("Sale Return Save successfully.", ConsoleColor.Green));
            }

            return RedirectToPage();
        }

        #endregion

        #region Get Set Variables
        private void SetVariables()
        {
            AppRegistry.SetKey(UserName, "SRtn_Start", Variables.Start, KeyType.Date);
            AppRegistry.SetKey(UserName, "SRtn_End", Variables.End, KeyType.Date);
            AppRegistry.SetKey(UserName, "SRtnCompany", Variables.Company, KeyType.Number);
        }

        private void GetVariables()
        {
            Variables = new()
            {
                Start = AppRegistry.GetDate(UserName, "SRtn_Start"),
                End = AppRegistry.GetDate(UserName, "SRtn_End"),
                Company = AppRegistry.GetNumber(UserName, "SRtnCompany"),
                Vou_No = "New",
                Vou_Date = DateTime.Now,
                Status = VoucherStatus.Submitted.ToString()
            };

        }
        #endregion

        #region Get New Voucher No.
        private string GetNewVouNo(DateTime _Date, string _VouTag)
        {
            string NewNum;
            StringBuilder _Text = new StringBuilder();
            _Text.Append("SELECT [Vou_No], ");
            _Text.Append("substr([Vou_No],1,2) AS Tag, ");
            _Text.Append("SubStr([Vou_No],3,2) AS Year, ");
            _Text.Append("SubStr([Vou_No],5,2) AS Month, ");
            _Text.Append("Cast((SubStr([Vou_No],8,4)) as integer) AS num ");
            _Text.Append("FROM [SaleReturn]");

            DataTable _Table = DataTableClass.GetTable(UserName, _Text.ToString(), "");
            DataView _View = _Table.AsDataView();

            var _Year = _Date.ToString("yy");
            var _Month = _Date.ToString("MM");
            _View.RowFilter = $"Vou_No like '{_VouTag}{_Year}{_Month}%'";
            DataTable _VouList = _View.ToTable();
            var MaxNum = _VouList.Compute("Max(num)", "");
            if (MaxNum == DBNull.Value)
            {
                NewNum = string.Concat(_VouTag, _Year, _Month, "-", "0001");
            }
            else
            {
                var _MaxNum = int.Parse(MaxNum.ToString()) + 1;
                NewNum = string.Concat(_VouTag, _Year, _Month, "-", _MaxNum.ToString("0000"));
            }
            return NewNum;
        }
        #endregion

        #region Parameters
        public class Parameters
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public int Company { get; set; }

            public int ID { get; set; }
            public string SaleVou_No { get; set; }
            public string Vou_No { get; set; }
            public DateTime Vou_Date { get; set; }
            public int TranID { get; set; }
            public int Inventory { get; set; }
            public string StockTitle { get; set; }
            public string Batch { get; set; }
            public decimal Qty { get; set; }
            public decimal RQty { get; set; }
            public string Status { get; set; } 

        }
        #endregion

    }
}
