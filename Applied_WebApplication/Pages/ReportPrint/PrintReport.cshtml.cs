using AppReporting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
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
        public string CompanyName => UserProfile.GetUserClaim(User, "Company");

        #region Get Reports.

        public void OnGet()
        {
        }
        #endregion

        #region Chart of Accpounts
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

            reports.ReportParameters.Add("CompanyName", CompanyName);
            reports.ReportParameters.Add("Heading1", "Chart of Accounts");
            reports.ReportParameters.Add("Footer", AppGlobals.ReportFooter);

            ReportLink = reports.GetReportLink();
            IsShowPdf = !reports.IsError;
            return Page();
        }
        #endregion

        #region General Ledger
        public IActionResult OnGetGL()
        {

            ReportClass.ReportFilters Filters = new ReportClass.ReportFilters
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };
            DataTable tb_Ledger = Ledger.GetGL(UserName, Filters);

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

            string _Heading1 = "GENERAL LEDGER";
            string _Heading2 = GetTitle(UserName, Tables.COA, Filters.N_COA);

            reports.ReportParameters.Add("CompanyName", CompanyName);
            reports.ReportParameters.Add("Heading1", _Heading1);
            reports.ReportParameters.Add("Heading2", _Heading2);
            reports.ReportParameters.Add("Footer", AppGlobals.ReportFooter);

            ReportLink = reports.GetReportLink();
            IsShowPdf = !reports.IsError;

            return Page();
        }
        #endregion

        #region Supplier / Vendore Ledger
        public IActionResult OnGetGLCompany()
        {
            ReportClass.ReportFilters Filters = new ReportClass.ReportFilters
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                N_Customer = (int)AppRegistry.GetKey(UserName, "GL_Company", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };

            DataTable tb_Ledger = Ledger.GetGLCompany(UserName, Filters);

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

            var _Title = GetTitle(UserName, Tables.Customers, Filters.N_Customer);
            var _Status = GetColumnValue(UserName, Tables.Customers, "Status", Filters.N_Customer);
            var _StatusTitle = "";
            if (_Status.Length > 0)
            {
                _StatusTitle = DirectoryClass.GetDirectoryValue(UserName, "CompanyStatus", Convert.ToInt32(_Status));
            }
            var _Heading1 = string.Concat(_Title, " (", _StatusTitle, ")");
            var _Heading2 = string.Concat("From ", Filters.Dt_From.ToString(AppRegistry.FormatDate), " To ", Filters.Dt_To.ToString(AppRegistry.FormatDate));

            reports.ReportParameters.Add("CompanyName", CompanyName);
            reports.ReportParameters.Add("Heading1", _Heading1);
            reports.ReportParameters.Add("Heading2", _Heading2);
            reports.ReportParameters.Add("Footer", AppGlobals.ReportFooter);

            ReportLink = reports.GetReportLink();                     // Create a report and provide link of pdf file location.
            IsShowPdf = !reports.IsError;                                   // Show PDF id no error found.
            return Page();
        }
        #endregion

        #region Trial Balance
        public async Task<IActionResult> OnGetTBPrintAsync(DateTime Date1, DateTime Date2)
        {
            TrialBalance TB = new(User);
            TB.MyDataTable = TB.TB_Dates(Date1, Date2);
            TB.Heading2 = string.Format("From {0} to {1}", Date1.ToString(AppRegistry.FormatDate), Date2.ToString(AppRegistry.FormatDate));
            TB.SetParameters();
            await Task.Run(() => (ReportLink = TB.MyReportClass.GetReportLink()));
            IsShowPdf = !TB.MyReportClass.IsError;
            if (!IsShowPdf) { ErrorMessages.Add(MessageClass.SetMessage(TB.MyReportClass.MyMessage)); }
            return Page();
        }

        public async Task<IActionResult> OnGetOBTBAsync()
        {
            var OBDate = AppRegistry.GetDate(UserName, "OBDate");
            var DateFormat = AppRegistry.FormatDate;
            TrialBalance TB = new(User);
            TB.Heading2 = "Opening Balances as on " + OBDate.ToString(DateFormat);
            TB.MyDataTable = TB.TBOB_Data();
            await Task.Run(() => (ReportLink = TB.MyReportClass.GetReportLink()));

            IsShowPdf = !TB.MyReportClass.IsError;

            if (!IsShowPdf) { ErrorMessages.Add(MessageClass.SetMessage(TB.MyReportClass.MyMessage)); }
            return Page();
        }



        #endregion

        #region Sales Invoice
        public IActionResult OnGetSaleInvoice(int TranID)
        {
            return Page();
        }
        #endregion





    }
}
