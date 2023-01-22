using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using System.Collections;
using System.Reflection.PortableExecutable;
using static Applied_WebApplication.Data.AppFunctions;
using static Applied_WebApplication.Data.ReportClass;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class COAListModel : PageModel
    {
        [BindProperty]
        public ReportClass MyReport { get; set; } = new ReportClass();

        public void OnGet()
        {
        }

        public void OnGetPrint()
        {
        }

        public IActionResult OnGetCOAList()
        {
            string UserName = User.Identity.Name;
            MyReport = new ReportClass
            {
                OutputFileName = "PDFFile",
                RDLCFileName = "COAList.rdlc",
                OutputFileType = FileType.pdf,
                UserName = User.Identity.Name
            };
            //ReportFilter.dt_From = new DateTime(2022, 12, 1);
            //ReportFilter.dt_To = new DateTime(2022, 12, 31);
            MyReport.RDLCDataSet = "DataSet1";
            MyReport.ReportFilter.TableName = Tables.COA;
            MyReport.ReportFilter.Columns = " [ID],[Code],[Title] ";
            MyReport.CommandText = GetQueryText(MyReport.ReportFilter); //  "SELECT ID, CODE, TITLE FROM [COA]";
            MyReport.Parameters.Add("UserName", UserName);
            MyReport.ReportParameters.Add("CompanyName", UserProfile.GetCompanyName(User));
            MyReport.GetReport();                                                                       // Report is ready to print.
            return Page();

            //return new FileContentResult(MyReport.MyBytes, "application/pdf");

        }

        public IActionResult OnGetLedger(int COAID)
        {
            string UserName = User.Identity.Name;
            Ledger.UpdateLedger(UserName, COAID);

            MyReport = new ReportClass
            {
                OutputFileName = "CashBook_Ledger",
                RDLCFileName = "Ledger.rdlc",
                OutputFileType = FileType.pdf,
                UserName = User.Identity.Name
            };
            //ReportFilter.dt_From = new DateTime(2022, 12, 1);
            //ReportFilter.dt_To = new DateTime(2022, 12, 31);
            MyReport.RDLCDataSet = "dsname_Ledger";
            MyReport.ReportFilter.TableName = Tables.Ledger;
            MyReport.ReportFilter.Columns = "*";
            MyReport.ReportFilter.N_COA = COAID;
            MyReport.CommandText = GetQueryText(MyReport.ReportFilter);
            MyReport.Parameters.Add("UserName", UserName);
            MyReport.ReportParameters.Add("CompanyName", UserProfile.GetCompanyName(User));
            MyReport.ReportParameters.Add("Heading", "General Ledger - " + GetTitle(UserName, Tables.COA, COAID));
            MyReport.GetReport();                                                                       // Report is ready to print.

            return Page();
        }

    }
}
