using AppReportClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class PurchaseReportsModel : PageModel
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
        public DataTable Customers { get; set; }
        public DataTable Inventory { get; set; }
        public DataTable SourceTable { get; set; }
        public DataTable Cities { get; set; }

        public void OnGet()
        {
            Variables = new()
            {
                StartDate = AppRegistry.GetDate(UserName, "pRptDate1"),
                EndDate = AppRegistry.GetDate(UserName, "pRptDate2"),
                AllCompany = AppRegistry.GetBool(UserName, "pRptComAll"),
                AllInventory = AppRegistry.GetBool(UserName, "pRptStockAll"),
                CompanyID = AppRegistry.GetNumber(UserName, "pRptCompany"),
                City = AppRegistry.GetText(UserName, "pRptCity"),
                InventoryID = AppRegistry.GetNumber(UserName, "pRptInventory"),
            };

            Customers = DataTableClass.GetTable(UserName, Tables.Customers, "", "[Title]");
            Inventory = DataTableClass.GetTable(UserName, Tables.Inventory, "", "[Title]");
            Cities = DataTableClass.GetTable(UserName, SQLQuery.Cities());

            var _Filter = GetFilter(Variables);
            var _OrderBy = "[Vou_Date],[Vou_No]";
            var _SQLQuery = SQLQuery.PurchaseRegister(_Filter,_OrderBy);
            SourceTable = DataTableClass.GetTable(UserName, _SQLQuery);

            TempDBClass.CreateTempTable(UserName, SourceTable, "pRptTemp");

        }

        internal string GetFilter(Parameters variables)
        {
            var Date1 = AppRegistry.YMD(variables.StartDate);
            var Date2 = AppRegistry.YMD(variables.EndDate);

            var Text = new StringBuilder();

            if (variables.AllCompany == false)
            {
                Text.Append($"Company={variables.CompanyID}");
            }

            if (variables.AllInventory == false)
            {
                if (Text.ToString().Length > 0) { Text.Append(" AND "); }
                Text.Append($"Inventory={variables.InventoryID}");
            }

            if (Variables.City != "Select All" && Variables.City.Length > 0 )
            {
                if (Text.ToString().Length > 0) { Text.Append(" AND "); }
                Text.Append($"Upper([CityName])='{variables.City.ToUpper()}'");
            }

            if (Text.ToString().Length > 0) { Text.Append(" AND "); }
            Text.Append($"Date(Vou_Date)>=Date('{Date1}') AND Date(Vou_Date)<=Date('{Date2}')");

            return Text.ToString();
        }

        public IActionResult OnPostPrint(ReportType Option)
        {
            SetKeys();
            AppRegistry.SetKey(UserName, "pRptName", "PurchaseRegister", KeyType.Text);
            return RedirectToPage("/ReportPrint/PrintReport", "PurchaseRegister", new {RptType = Option});
        }

        public IActionResult OnPostPrintList(ReportType Option)
        {
            SetKeys();
            AppRegistry.SetKey(UserName, "pRptName", "PurchaseRegister3", KeyType.Text);
            return RedirectToPage("/ReportPrint/PrintReport", "PurchaseRegister", new { RptOption = Option });
        }


        public IActionResult OnPost()
        {

            SetKeys();
            return RedirectToPage();

        }

        private void SetKeys()
        {
            if (Variables.CompanyID == 0) { Variables.AllCompany = true; } else { Variables.AllCompany = false; }
            if (Variables.InventoryID == 0) { Variables.AllInventory = true; } else { Variables.AllInventory = false; }

            var _Date1 = Variables.StartDate.ToString(AppRegistry.FormatDate);
            var _Date2 = Variables.EndDate.ToString(AppRegistry.FormatDate);

            var _Heading1 = $"Purchase Register";
            var _Heading2 = $"From {_Date1} to {_Date2}";

            AppRegistry.SetKey(UserName, "pRptDate1", Variables.StartDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "pRptDate2", Variables.EndDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "pRptComAll", Variables.AllCompany, KeyType.Boolean);
            AppRegistry.SetKey(UserName, "pRptStockAll", Variables.AllInventory, KeyType.Boolean);
            AppRegistry.SetKey(UserName, "pRptCompany", Variables.CompanyID, KeyType.Number);
            AppRegistry.SetKey(UserName, "pRptCity", Variables.City, KeyType.Text);
            AppRegistry.SetKey(UserName, "pRptInventory", Variables.InventoryID, KeyType.Number);
            AppRegistry.SetKey(UserName, "pRptHeading1", _Heading1, KeyType.Text);
            AppRegistry.SetKey(UserName, "pRptHeading2", _Heading2, KeyType.Text);

        }


        public class Parameters
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public bool AllCompany { get; set; }
            public bool AllInventory { get; set; }
            public int CompanyID { get; set; }
            public int InventoryID { get; set; }
            public string Heading1 { get; set; }
            public string Heading2 { get; set; }
            public string ReportFile { get; set; }
            public string City { get; set; }
        }
    }
}
