using AppReporting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.AppFunctions;

using DataTable = System.Data.DataTable;


namespace Applied_WebApplication.Pages.ReportPrint
{
    public class COAListModel : PageModel
    {
        //private HtmlToPdf converter;

        [BindProperty]
        public string ReportLink { get; set; }
        public bool IsError { get; set; }
        public string MyMessage { get; set; }
        public DataTable Preview { get; set; } = new();
        public List<Message> ErrorMessages { get; set; } = new();
        public bool IsShowPdf { get; set; } = false;
        public string UserName => User.Identity.Name;

        #region Get Reports.

        public void OnGet()
        {
        }

        public IActionResult OnGetCOAList()
        {
            //OnPostCOAExcel();

            //return Page();
            //=================================================================
            ReportClass reports = new ReportClass();
            DataTableClass COA = new(UserName, Tables.COA);
            AppGlobals.AppUser = User;

            reports.AppUser = User;
            reports.ReportFilePath = AppGlobals.ReportPath;
            reports.ReportFile = "COAList.rdlc";
            reports.ReportDataSet = "DataSet1";
            reports.ReportData = COA.MyDataTable;
            reports.RecordSort = "Title";

            reports.OutputFilePath = AppGlobals.PrintedReportPath;
            reports.OutputFile = "COAList";
            reports.OutputFileLinkPath = AppGlobals.PrintedReportPathLink;
            
            //reports.ReportParameters.Add("CompanyName", UserProfile.GetUserClaim(User, "Company"));
            //reports.ReportParameters.Add("SortOrder", reports.RecordSort);

            ReportLink = reports.GetReportLink();
            return Page();
        }

        

        public IActionResult OnGetGL()
        {

            ReportClass.ReportFilters Filters = new ReportClass.ReportFilters
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };
            DataTable tb_Ledger = Ledger.GetGL(UserName, Filters);
            //string _Heading1 = "GENERAL LEDGER";
            //string _Heading2 = GetTitle(UserName, Tables.COA, Filters.N_COA);

            ReportClass reports = new ReportClass
            {
                AppUser = User,
                ReportFilePath = AppGlobals.ReportPath,
                ReportFile = "Ledger.rdlc",
                ReportDataSet = "dsname_Ledger",
                ReportData = tb_Ledger,
                RecordSort = "Vou_Date",

                OutputFilePath = AppGlobals.PrintedReportPath,
                OutputFile = "GeneralLedger",
                OutputFileLinkPath = AppGlobals.PrintedReportPathLink

            };

            ReportLink = reports.GetReportLink();
            IsShowPdf = !reports.IsError;

            return Page();
        }

        public IActionResult OnGetGLCompany()
        {

            // ReportClass MyReport = new();
            ReportClass.ReportFilters Filters = new ReportClass.ReportFilters
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                N_Customer = (int)AppRegistry.GetKey(UserName, "GL_Company", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };

            DataTable tb_Ledger = Ledger.GetGL(UserName, Filters);
            //string _Heading1 = "GENERAL LEDGER : " + GetTitle(UserName, Tables.COA, Filters.N_COA);
            //string _Heading2 = GetTitle(UserName, Tables.Customers, Filters.N_Customer);

            ReportClass reports = new ReportClass
            {
                AppUser = User,
                ReportFilePath = AppGlobals.ReportPath,
                ReportFile = "CompanyGL.rdlc",
                ReportDataSet = "dsname_CompanyGL",
                ReportData = tb_Ledger,
                RecordSort = "Vou_Date",

                OutputFilePath = AppGlobals.PrintedReportPath,
                OutputFile = "CompanyGL",
                OutputFileLinkPath = AppGlobals.PrintedReportPathLink

            };

            var _Heading1 = "";
            var _Heading2 = string.Concat("From ",Filters.Dt_From.ToString(AppRegistry.FormatDate), " To ", Filters.Dt_To.ToString(AppRegistry.FormatDate));

            reports.ReportParameters.Add("CompanyName", UserProfile.GetUserClaim(User, "Company"));
            reports.ReportParameters.Add("Heading1", _Heading1);
            reports.ReportParameters.Add("Heading2", _Heading2);


            ReportLink = reports.GetReportLink();
            IsShowPdf = !reports.IsError;
            return Page();
        }

        public IActionResult OnGetTB()
        {

            //ReportClass MyReport = new();
            ReportClass.ReportFilters Filters = new ReportClass.ReportFilters
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                N_Customer = (int)AppRegistry.GetKey(UserName, "GL_Company", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };

            //var _Date1 = Filters.Dt_From.ToString(AppRegistry.FormatDate);
            //var _Date2 = Filters.Dt_To.ToString(AppRegistry.FormatDate);

            DataTableClass TB = new(UserName, Tables.TB);
            
            ReportClass reports = new ReportClass
            {
                AppUser = User,
                ReportFilePath = AppGlobals.ReportPath,
                ReportFile = "TB.rdlc",
                ReportDataSet = "dset_TB",
                ReportData = TB.MyDataTable,
                RecordSort = "Code",

                OutputFilePath = AppGlobals.PrintedReportPath,
                OutputFile = "TB",
                OutputFileLinkPath = AppGlobals.PrintedReportPathLink

            };

            ReportLink = reports.GetReportLink();
            IsShowPdf = !reports.IsError;
            return Page();
        }

        #endregion






    }
}
