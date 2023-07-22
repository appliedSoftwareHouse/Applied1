using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Sales
{
    public class SaleReturnModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public List<Message> MyMessages { get; set; }
        public string UserName => User.Identity.Name;
        public DataTable Receivable1 { get; set; }
        public DataTable Receivable2 { get; set; }

        #region Get

        public void OnGet()
        {
            MyMessages = new();
            GetVariables();
            var Date1 = Variables.Start.AddDays(-1).ToString(AppRegistry.DateYMD);
            var Date2 = Variables.End.AddDays(1).ToString(AppRegistry.DateYMD);

            var _Filter = $"Date([B1].[Vou_Date]) > Date('{Date1}') AND Date([B1].[Vou_Date]) < Date('{Date2}') AND Company={Variables.Company}";
            var _SQLSalesList = SQLQuery.SaleReturn(_Filter);
            var tb_SaleReturn = new DataTableClass(UserName, _SQLSalesList, Tables.SaleReturn);
            Receivable1 = tb_SaleReturn.MyDataTable;
        }
        public void OnGetEdit(int ID)
        {
            MyMessages = new();
            GetVariables();

            var Date1 = Variables.Start.AddDays(-1).ToString(AppRegistry.DateYMD);
            var Date2 = Variables.End.AddDays(1).ToString(AppRegistry.DateYMD);

            var _Filter = $"Date([B1].[Vou_Date]) > Date('{Date1}') AND Date([B1].[Vou_Date]) < Date('{Date2}') AND Company={Variables.Company}";
            //var _Filter = $"Vou_Date >= '{_Date1}' AND Vou_Date <= '{_Date2}' AND Company={Variables.Company}";
            var _SQLSalesList = SQLQuery.SaleReturn(_Filter);
            var _Table = new DataTableClass(UserName, _SQLSalesList);
            Receivable1 = _Table.MyDataTable;
            _Table.SeekRecord(ID);
            Variables.Vou_No = (string)_Table.CurrentRow["Vou_No"];
            Variables.Vou_Date = (DateTime)_Table.CurrentRow["Vou_Date"];
            Variables.StockTitle = (string)_Table.CurrentRow["StockTitle"];
            Variables.Batch = (string)_Table.CurrentRow["Batch"];
            Variables.TranID = ID;
            Variables.Qty = Conversion.ToDecimal(_Table.CurrentRow["Qty"]);
            Variables.RQty = Conversion.ToDecimal(_Table.CurrentRow["RQty"]);

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
            MyMessages = new();
            var tb_SaleReturn = new DataTableClass(UserName, Tables.SaleReturn);
            Receivable1 = tb_SaleReturn.MyDataTable;
            tb_SaleReturn.MyDataView.RowFilter = $"TranID={TranID}";
            if (tb_SaleReturn.CountView == 0)
            {
                tb_SaleReturn.NewRecord();
                tb_SaleReturn.CurrentRow["ID"] = 0;
                tb_SaleReturn.CurrentRow["Vou_No"] = "Generate";                // Generate a New Voucher No.
                tb_SaleReturn.CurrentRow["TranID"] = Variables.TranID;
            }
            else
            {
                tb_SaleReturn.CurrentRow = tb_SaleReturn.MyDataView[0].Row;
            }

            tb_SaleReturn.CurrentRow["Vou_No"] = "Generate";
            tb_SaleReturn.CurrentRow["Vou_Date"] = Variables.Vou_Date;
            tb_SaleReturn.CurrentRow["Qty"] = Variables.RQty;
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
                Vou_Date = DateTime.Now
            };

        }
        #endregion

        #region Parameters
        public class Parameters
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public int Company { get; set; }

            public int ID { get; set; }
            public string Vou_No { get; set; }
            public DateTime Vou_Date { get; set; }
            public int TranID { get; set; }
            public int Inventory { get; set; }
            public string StockTitle { get; set; }
            public string Batch { get; set; }
            public decimal Qty { get; set; }
            public decimal RQty { get; set; }

        }
        #endregion
    }
}
