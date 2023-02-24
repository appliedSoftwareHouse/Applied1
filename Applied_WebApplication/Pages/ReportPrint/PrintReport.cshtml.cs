using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using System.Collections;
using System.Data;
using System.Reflection.PortableExecutable;
using System.ServiceModel.Security;
using static Applied_WebApplication.Data.AppFunctions;
using static Applied_WebApplication.Data.ReportClass;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class COAListModel : PageModel
    {
        [BindProperty]
        public string ReportLink { get; set; }
        public bool IsError { get; set; }
        public string MyMessage { get; set; }

        public void OnGet()
        {
        }

       

        public IActionResult OnGetCOAList()
        {
            string UserName = User.Identity.Name;
            ReportClass MyReport = new ReportClass
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
            MyReport.ReportData = MyReport.GetReportDataTable();
            MyReport.GetReport();                                                                       // Report is ready to print.
            ReportLink = MyReport.OutputFileLink;
            return Page();

            //return new FileContentResult(MyReport.MyBytes, "application/pdf");            Do not delete it.

        }

        public IActionResult OnGetGL()
        {
            var UserName = User.Identity.Name;
            ReportClass MyReport = new();

            ReportFilters Filters = new ReportFilters
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };

            var _FileType = (FileType)AppRegistry.GetKey(UserName, "ReportType", KeyType.Number);

            DataTable tb_Ledger = Ledger.GetGL(UserName, Filters);

            string Heading1 = "<<--     GENERAL LEDGER     -->>";
            string Heading2 = GetTitle(UserName, Tables.COA, Filters.N_COA);

            MyReport = new()
            {
                UserName = User.Identity.Name,
                RDLCDataSet = "dsname_Ledger",
                RDLCFileName = "Ledger.rdlc",
                ReportData = tb_Ledger,
                OutputFileName = "GeneralLedger",
                OutputFileType = _FileType
            };
            MyReport.Parameters.Add("UserName", UserName);
            MyReport.ReportParameters.Add("CompanyName", UserProfile.GetCompanyName(User));
            MyReport.ReportParameters.Add("Heading1", Heading1);
            MyReport.ReportParameters.Add("Heading2", Heading2);
            MyReport.GetReport();
            IsError = MyReport.IsError;
            MyMessage = MyReport.MyMessage;
            ReportLink = MyReport.OutputFileLink;

            return Page();
        }

        public IActionResult OnGetGLCompany()
        {
            var UserName = User.Identity.Name;
            ReportClass MyReport = new();
            ReportFilters Filters = new ReportFilters
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                N_Customer = (int)AppRegistry.GetKey(UserName, "GL_Company", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };

            var _Date1 = Filters.Dt_From.ToString(AppRegistry.FormatDate);
            var _Date2 = Filters.Dt_To.ToString(AppRegistry.FormatDate);

            var _FileType = (FileType)AppRegistry.GetKey(UserName, "ReportType", KeyType.Number);               // Get File Type from AppRegistry.
            var tb_Ledger = Ledger.GetGLCompany(UserName, Filters);
            var Heading1 = "General Ledger - " + GetTitle(UserName, Tables.Customers, Filters.N_Customer);
            var Heading2 = string.Concat("From ", _Date1, " To ", _Date2);

            MyReport = new()
            {
                UserName = User.Identity.Name,
                RDLCDataSet = "dsname_CompanyGL",
                RDLCFileName = "CompanyGL.rdlc",
                ReportData = tb_Ledger,
                OutputFileName = "CompanyGL",
                OutputFileType = _FileType
            };
            //MyReport.Parameters.Add("UserName", UserName);
            //MyReport.ReportParameters.Add("CompanyName", UserProfile.GetCompanyName(User));
            //MyReport.ReportParameters.Add("Heading1", Heading1);
            //MyReport.ReportParameters.Add("Heading2", Heading2);
            MyReport.GetReport();
            IsError = MyReport.IsError;
            MyMessage = MyReport.MyMessage;
            ReportLink = MyReport.OutputFileLink;

            return Page();
        }

        public IActionResult OnGetTB()
        {
            var UserName = User.Identity.Name;
            ReportClass MyReport = new();
            ReportFilters Filters = new ReportFilters
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                N_Customer = (int)AppRegistry.GetKey(UserName, "GL_Company", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };

            var _Date1 = Filters.Dt_From.ToString(AppRegistry.FormatDate);
            var _Date2 = Filters.Dt_To.ToString(AppRegistry.FormatDate);

            var _FileType = (FileType)AppRegistry.GetKey(UserName, "ReportType", KeyType.Number);               // Get File Type from AppRegistry.
            var tb_TB = Ledger.GetTB(UserName, Filters);
            var Heading1 = "TRIAL BALANCE";
            var Heading2 = string.Concat("Position as on ", _Date2);

            MyReport = new()
            {
                UserName = User.Identity.Name,
                RDLCDataSet = "dset_TB",
                RDLCFileName = "TB.rdlc",
                ReportData = tb_TB,
                OutputFileName = "TB",
                OutputFileType = _FileType
            };
            MyReport.Parameters.Add("UserName", UserName);
            MyReport.ReportParameters.Add("CompanyName", UserProfile.GetCompanyName(User));
            MyReport.ReportParameters.Add("Heading1", Heading1);
            MyReport.ReportParameters.Add("Heading2", Heading2);
            MyReport.GetReport();
            IsError = MyReport.IsError;
            MyMessage = MyReport.MyMessage;

            ReportLink = MyReport.OutputFileLink;

            return Page();
        }


    }
}
