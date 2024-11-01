using AppReportClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class SalesReturnReportModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public string UserName { get; set; }
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
            UserName ??= User.Identity.Name;
            GetKeys();
            LoadData(string.Empty);
        }

        public void LoadData(string Filter)
        {
            Customers = DataTableClass.GetTable(UserName, Tables.Customers, "", "[Title]");
            Inventory = DataTableClass.GetTable(UserName, Tables.Inventory, "", "[Title]");
            Cities = DataTableClass.GetTable(UserName, SQLQuery.Cities());

            var _Filter = GetFilter(Variables);
            var _SQLQuery = SQLQuery.SaleReturnReport(_Filter);
            SourceTable = DataTableClass.GetTable(UserName, _SQLQuery, "[Vou_Date],[Vou_No]");
        }

        public string GetFilter(Parameters variables)
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
            Text.Append($"Date([Vou_Date])>=Date('{Date1}') AND Date([Vou_Date])<=Date('{Date2}')");

            if (Text.ToString().Length > 0) { Text.Append(" AND "); }
            Text.Append("[Vou_No] not null");

            return Text.ToString();
        }

        public IActionResult OnPostRefresh()
        {
            UserName ??= User.Identity.Name;
            SetKeys();
            return RedirectToPage();

        }

        public IActionResult OnPostPrint(ReportType RptType)
        {
            UserName ??= User.Identity.Name;
            SetKeys();
            AppRegistry.SetKey(UserName, "srRptType", (int)RptType, KeyType.Number);
            return RedirectToPage("/ReportPrint/PrintReport", "SaleReturn");
        }
        private void SetKeys()
        {
            UserName ??= User.Identity.Name;
            if (Variables.CompanyID == 0) { Variables.AllCompany = true; } else { Variables.AllCompany = false; }
            if (Variables.InventoryID == 0) { Variables.AllInventory = true; } else { Variables.AllInventory = false; }

            var _Date1 = Variables.StartDate.ToString(AppRegistry.FormatDate);
            var _Date2 = Variables.EndDate.ToString(AppRegistry.FormatDate);

            var _Heading1 = $"Sale Return";
            var _Heading2 = $"From {_Date1} to {_Date2}";

            AppRegistry.SetKey(UserName, "srRptDate1", Variables.StartDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "srRptDate2", Variables.EndDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "srRptComAll", Variables.AllCompany, KeyType.Boolean);
            AppRegistry.SetKey(UserName, "srRptStockAll", Variables.AllInventory, KeyType.Boolean);
            AppRegistry.SetKey(UserName, "srRptCompany", Variables.CompanyID, KeyType.Number);
            AppRegistry.SetKey(UserName, "srRptCity", Variables.City, KeyType.Text);
            AppRegistry.SetKey(UserName, "srRptInventory", Variables.InventoryID, KeyType.Number);
            AppRegistry.SetKey(UserName, "srRptHeading1", _Heading1, KeyType.Text);
            AppRegistry.SetKey(UserName, "srRptHeading2", _Heading2, KeyType.Text);
        }

        public void GetKeys()
        {
            

            Variables ??= new();
            Variables.StartDate = AppRegistry.GetDate(UserName, "srRptDate1");
            Variables.EndDate = AppRegistry.GetDate(UserName, "srRptDate2");
            Variables.AllCompany = AppRegistry.GetBool(UserName, "srRptComAll");
            Variables.AllInventory = AppRegistry.GetBool(UserName, "srRptStockAll");
            Variables.CompanyID = AppRegistry.GetNumber(UserName, "srRptCompany");
            Variables.City = AppRegistry.GetText(UserName, "srRptCity");
            Variables.InventoryID = AppRegistry.GetNumber(UserName, "srRptInventory");
            Variables.Heading1 = AppRegistry.GetText(UserName, "srRptHeading1");
            Variables.Heading2 = AppRegistry.GetText(UserName, "srRptHeading2");
        }

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
