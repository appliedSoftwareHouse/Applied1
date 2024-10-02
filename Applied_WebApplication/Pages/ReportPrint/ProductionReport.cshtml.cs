using AppReportClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class ProductionReportModel : PageModel
    {
        [BindProperty]
        public ReportFilters Parameters { get; set; }
        public string UserName => User.Identity.Name;
        public DataTable Inventory { get; set; }

        public void OnGet()
        {
            Parameters = new()
            {
                Flow = AppRegistry.GetText(UserName, "pdRptFlow"),
                Dt_From = AppRegistry.GetDate(UserName, "pdRptDateFrom"),
                Dt_To = AppRegistry.GetDate(UserName, "pdRptDateTo"),
                N_Inventory = AppRegistry.GetNumber(UserName, "pdRptStock"),
            };


            Inventory = DataTableClass.GetTable(UserName, Tables.Inventory, "", "[Title]");

        }


        public IActionResult OnPostPrint(ReportType RptType)
        {
            SetKeys();
            AppRegistry.SetKey(UserName, "pdRptName", "ProductionReport.rdl", KeyType.Text);
            AppRegistry.SetKey(UserName, "pdRptType", (int)RptType, KeyType.Number);
            return RedirectToPage("/ReportPrint/PrintReport", "ProductionReport");
        }



        
        private void SetKeys()
        {
            AppRegistry.SetKey(UserName, "pdRptDateFrom", Parameters.Dt_From, KeyType.Date);
            AppRegistry.SetKey(UserName, "pdRptDateTo", Parameters.Dt_To, KeyType.Date);
            AppRegistry.SetKey(UserName, "pdRptFlow", Parameters.Flow, KeyType.Text);
            AppRegistry.SetKey(UserName, "pdRptStock", Parameters.N_Inventory, KeyType.Number);
        }
    }

   
}
