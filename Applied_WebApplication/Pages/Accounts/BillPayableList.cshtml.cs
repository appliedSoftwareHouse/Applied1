using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Accounts
{
    [Authorize]
    public class BillPayableListModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public DataTable BillPayable { get; set; }
        public string UserName => User.Identity.Name;
        public void OnGet()
        {
            Variables = new()
            {
                MinDate = AppRegistry.GetDate(UserName, "bp_Date1"),
                MaxDate = AppRegistry.GetDate(UserName, "bp_Date2"),
                Company = AppRegistry.GetNumber(UserName, "bp_Company")
            };

            var _Date1 = AppFunctions.MinDate(Variables.MinDate, Variables.MaxDate).ToString(AppRegistry.DateYMD);
            var _Date2 = AppFunctions.MaxDate(Variables.MinDate, Variables.MaxDate).ToString(AppRegistry.DateYMD);

            var _Filter = $"Date([Vou_Date]) BETWEEN Date('{_Date1}') AND Date('{_Date2}') ";
            if(Variables.Company > 0) { _Filter += $" AND Company = {Variables.Company}"; }

            BillPayable = DataTableClass.GetTable(UserName, SQLQuery.BillPayableList(_Filter));
        }

        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "bp_Date1", Variables.MinDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "bp_Date2", Variables.MaxDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "bp_Company", Variables.Company, KeyType.Number);


            return RedirectToPage();
        }


        #region Variables
        public class Parameters
        {
            public int ID { get; set; }
            public string Vou_No { get; set; }
            public DateTime Vou_Date { get; set; }
            public DateTime Pay_Date { get; set; }
            public int Company { get; set; }
            public string Ref_No { get; set; }
            public string Inv_No { get; set; }
            public DateTime Inv_Date { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public string Comments { get; set; }
            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }
        }
        #endregion
    }
}
