using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class SalesReportsModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }

        public string UserName => User.Identity.Name;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AllCompany { get; set; }
        public bool AllInventory { get; set; }
        public int CompanyID { get; set; }
        public int InventoryID { get; set; }
        public DataTable SourceTable { get; set; }


        public void OnGet()
        {
            Variables = new()
            {
                StartDate = AppRegistry.GetDate(UserName, "sRptDate1"),
                EndDate = AppRegistry.GetDate(UserName, "sRptDate2"),
                AllCompany = AppRegistry.GetBool(UserName, "sRptComAll"),
                AllInventory = AppRegistry.GetBool(UserName, "sRptStockAll"),
                CompanyID = AppRegistry.GetNumber(UserName, "sRptCompany"),
                InventoryID = AppRegistry.GetNumber(UserName, "sRptInventory"),
            };



            var _Filter = GetFilter(Variables);
            var _SQLQuery = SQLQuery.SaleRegister(_Filter);
            SourceTable = DataTableClass.GetTable(UserName, _SQLQuery);


        }

        internal string  GetFilter(Parameters variables)
        {
            var Date1 = AppRegistry.YMD(variables.StartDate);
            var Date2 = AppRegistry.YMD(variables.EndDate);

            var Text = new StringBuilder();
            
            if(variables.AllCompany == false)
            {
                Text.Append($"Company={variables.CompanyID}");
            }
            
            if(variables.AllInventory == false)
            {
                if(Text.ToString().Length > 0) { Text.Append(" AND "); }
                Text.Append($"Inventory={variables.InventoryID}");
            }

            if (Text.ToString().Length > 0) { Text.Append(" AND "); }
            Text.Append($"Vou_Date>='{Date1}' AND Vou_Date<='{Date2}'");

            return Text.ToString();
        }

        public IActionResult OnPostPrint()
        {
            return RedirectToPage("/ReportPrint/PrintReport", "SaleRegister");
        }

        public IActionResult OnPost()
        {
            if(Variables.CompanyID == 0) { Variables.AllCompany = true; } else { Variables.AllCompany = false; }
            if(Variables.InventoryID == 0) { Variables.AllInventory = true; } else if(Variables.AllInventory = false)

            AppRegistry.SetKey(UserName, "sRptDate1", Variables.StartDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "sRptDate2", Variables.EndDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "sRptComAll", Variables.AllCompany, KeyType.Boolean);
            AppRegistry.SetKey(UserName, "sRptStockAll", Variables.AllInventory, KeyType.Boolean);
            AppRegistry.SetKey(UserName, "sRptCompany", Variables.CompanyID, KeyType.Number);
            AppRegistry.SetKey(UserName, "sRptInventory", Variables.InventoryID, KeyType.Number);

            return RedirectToPage();

        }


        public class Parameters
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public bool AllCompany { get; set; }
            public bool AllInventory { get; set; }
            public int CompanyID { get; set; }
            public int InventoryID { get; set; }
        }
    }
}
