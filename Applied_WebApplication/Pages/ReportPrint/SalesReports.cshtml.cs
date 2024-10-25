using AppReportClass;
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
        public string City { get; set; }
        public int InventoryID { get; set; }
        public DataTable SourceTable { get; set; }
        public DataTable Customers { get; set; }
        public DataTable Inventory { get; set; }
        public DataTable Cities { get; set; }


        public void OnGet()
        {
            Variables = new()
            {
                StartDate = AppRegistry.GetDate(UserName, "sRptDate1"),
                EndDate = AppRegistry.GetDate(UserName, "sRptDate2"),
                AllCompany = AppRegistry.GetBool(UserName, "sRptComAll"),
                AllInventory = AppRegistry.GetBool(UserName, "sRptStockAll"),
                CompanyID = AppRegistry.GetNumber(UserName, "sRptCompany"),
                City = AppRegistry.GetText(UserName, "sRptCity"),
                InventoryID = AppRegistry.GetNumber(UserName, "sRptInventory"),

            };

            Customers = DataTableClass.GetTable(UserName, Tables.Customers, "", "[Title]");
            Inventory = DataTableClass.GetTable(UserName, Tables.Inventory, "", "[Title]");
            Cities = DataTableClass.GetTable(UserName, SQLQuery.Cities());

            var _Filter = GetFilter(Variables);
            var _SQLQuery = SQLQuery.SaleReturn(_Filter);
            SourceTable = DataTableClass.GetTable(UserName, _SQLQuery, "[Vou_Date],[Vou_No]");

        }
        public IActionResult OnPostRefresh()
        {
            SetKeys();
            return RedirectToPage();
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

            if (Variables.City.ToUpper() != "SELECT ALL" && Variables.City.Length > 0)
            {
                if (Text.ToString().Length > 0) { Text.Append(" AND "); }
                Text.Append($"Upper([CityName])='{variables.City.ToUpper()}'");
            }


            if (Text.ToString().Length > 0) { Text.Append(" AND "); }
            Text.Append($"Date(Vou_Date)>=Date('{Date1}') AND Date(Vou_Date)<=Date('{Date2}')");

            return Text.ToString();
        }
        public IActionResult OnPostPrint(ReportType RptType)
        {
            SetKeys();
            AppRegistry.SetKey(UserName, "sRptName", "SaleRegister.rdl", KeyType.Text);
            AppRegistry.SetKey(UserName, "sRptType", (int)RptType, KeyType.Number);
            return RedirectToPage("/ReportPrint/PrintReport", "SaleRegister");
        }
        public IActionResult OnPostPrintList(ReportType RptType)
        {
            SetKeys();
            AppRegistry.SetKey(UserName, "sRptName", "SaleRegister3.rdl", KeyType.Text);
            AppRegistry.SetKey(UserName, "sRptType", (int)RptType, KeyType.Number);
            return RedirectToPage("/ReportPrint/PrintReport", "SaleRegister");
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

            var _Heading1 = $"Sale Register";
            var _Heading2 = $"From {_Date1} to {_Date2}";

            AppRegistry.SetKey(UserName, "sRptDate1", Variables.StartDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "sRptDate2", Variables.EndDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "sRptComAll", Variables.AllCompany, KeyType.Boolean);
            AppRegistry.SetKey(UserName, "sRptStockAll", Variables.AllInventory, KeyType.Boolean);
            AppRegistry.SetKey(UserName, "sRptCompany", Variables.CompanyID, KeyType.Number);
            AppRegistry.SetKey(UserName, "sRptCity", Variables.City, KeyType.Text);
            AppRegistry.SetKey(UserName, "sRptInventory", Variables.InventoryID, KeyType.Number);
            AppRegistry.SetKey(UserName, "sRptHeading1", _Heading1, KeyType.Text);
            AppRegistry.SetKey(UserName, "sRptHeading2", _Heading2, KeyType.Text);
            

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
            public int ReportType { get; set; }
        }
    }
}
