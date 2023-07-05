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
            Variables = new()
            {
                Start = AppRegistry.GetDate(UserName, "SRtn_Start"),
                End = AppRegistry.GetDate(UserName, "SRtn_End"),
                Company = AppRegistry.GetNumber(UserName, "SRtnCompany")
            };

            var _Date1 = Variables.Start.ToString(AppRegistry.DateYMD);
            var _Date2 = Variables.End.ToString(AppRegistry.DateYMD);

            var _Filter = $"Vou_Date >= '{_Date1}' AND Vou_Date <= '{_Date2}' AND Company={Variables.Company}";
            var _SQLSalesList = SQLQuery.SaleRegister(_Filter);
            Receivable1 = DataTableClass.GetTable(UserName, _SQLSalesList);
        }
        public void OnGetEdit(int ID)
        {
            MyMessages = new();
            Variables = new()
            {
                Start = AppRegistry.GetDate(UserName, "SRtn_Start"),
                End = AppRegistry.GetDate(UserName, "SRtn_End"),
                Company = AppRegistry.GetNumber(UserName, "SRtnCompany")
            };

            var _Date1 = Variables.Start.ToString(AppRegistry.DateYMD);
            var _Date2 = Variables.End.ToString(AppRegistry.DateYMD);

            var _Filter = $"Vou_Date >= '{_Date1}' AND Vou_Date <= '{_Date2}' AND Company={Variables.Company}";
            var _SQLSalesList = SQLQuery.SaleRegister(_Filter);
            Receivable1 = DataTableClass.GetTable(UserName, _SQLSalesList);
        }
        #endregion

        #region Post

        public IActionResult OnPostRefresh()
        {
            MyMessages = new();
            AppRegistry.SetKey(UserName, "SRtn_Start", Variables.Start, KeyType.Date);
            AppRegistry.SetKey(UserName, "SRtn_End", Variables.End, KeyType.Date);
            AppRegistry.SetKey(UserName, "SRtnCompany", Variables.Company, KeyType.Number);
            return RedirectToPage();
        }

        public IActionResult OnPostStockReturn(int? id)
        {
            id ??= 0;
            var _ID = Conversion.ToInteger(id);
            return RedirectToPage("SaleReturn", "Edit", new { ID = _ID });
        }
        #endregion

        #region Parameters
        public class Parameters
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public int Company { get; set; }

            public int ID { get; set; }
            public int TranID { get; set; }
            public int Inventory { get; set; }
            public int Batch { get; set; }
            public decimal Qty { get; set; }

        }
        #endregion
    }
}
