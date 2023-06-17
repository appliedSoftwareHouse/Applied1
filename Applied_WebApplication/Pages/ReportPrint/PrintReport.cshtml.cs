using AppReporting;
using AppReportClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using System.Data;
using System.Data.SQLite;
using static Applied_WebApplication.Data.AppFunctions;
using System.IO;

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
            reports.ReportFile = "COAList.rdl";
            reports.ReportDataSet = "DataSet1";
            reports.ReportSourceData = COA.MyDataTable;
            reports.RecordSort = "Title";

            reports.OutputFilePath = AppGlobals.PrintedReportPath;
            reports.OutputFile = "COAList";
            reports.OutputFileLinkPath = AppGlobals.PrintedReportPathLink;

            reports.RptParameters.Add("CompanyName", CompanyName);
            reports.RptParameters.Add("Heading1", "Chart of Accounts");
            reports.RptParameters.Add("Footer", AppGlobals.ReportFooter);

            ReportLink = reports.GetReportLink();
            IsShowPdf = !reports.IsError;
            return Page();
        }
        #endregion

        #region General Ledger
        public IActionResult OnGetGL()
        {

            ReportFilters Filters = new ReportFilters()
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
                ReportFile = "Ledger.rdl",
                ReportDataSet = "dsname_Ledger",
                ReportSourceData = tb_Ledger,
                RecordSort = "Vou_Date",

                OutputFilePath = AppGlobals.PrintedReportPath,
                OutputFile = "GeneralLedger",
                OutputFileLinkPath = AppGlobals.PrintedReportPathLink

            };

            string _Heading1 = "GENERAL LEDGER";
            string _Heading2 = GetTitle(UserName, Tables.COA, Filters.N_COA);

            reports.RptParameters.Add("CompanyName", CompanyName);
            reports.RptParameters.Add("Heading1", _Heading1);
            reports.RptParameters.Add("Heading2", _Heading2);
            reports.RptParameters.Add("Footer", AppGlobals.ReportFooter);

            ReportLink = reports.GetReportLink();
            IsShowPdf = !reports.IsError;

            return Page();
        }
        #endregion

        #region Supplier / Vendore Ledger
        public IActionResult OnGetGLCompany(ReportType _ReportType)
        {
            ReportFilters Filters = new()
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                N_Customer = (int)AppRegistry.GetKey(UserName, "GL_Company", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };

            var ReportFile = "CompanyGL.rdl";

            DataTable tb_Ledger = Ledger.GetGLCompany(UserName, Filters);


            ReportClass GLCompany = new ReportClass
            {
                AppUser = User,
                ReportFilePath = AppGlobals.ReportPath,
                ReportFile = ReportFile,
                ReportDataSet = "dsname_CompanyGL",
                ReportSourceData = tb_Ledger,
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

            if (_ReportType == ReportType.PDF)
            {
                GLCompany.RptParameters.Add("CompanyName", CompanyName);
                GLCompany.RptParameters.Add("Heading1", _Heading1);
                GLCompany.RptParameters.Add("Heading2", _Heading2);
                GLCompany.RptParameters.Add("Footer", AppGlobals.ReportFooter);

                ReportLink = GLCompany.GetReportLink();                     // Create a report and provide link of pdf file location.
                IsShowPdf = !GLCompany.IsError;                                   // Show PDF id no error found.
                return Page();
            }
            else
            {
                List<ReportParameter> _Parameters = new List<ReportParameter>
                {
                    new ReportParameter("CompanyName", CompanyName),
                    new ReportParameter("Heading1", _Heading1),
                    new ReportParameter("Heading2", _Heading2),
                    new ReportParameter("Footer", AppGlobals.ReportFooter)
                };
                var _ReportParamaters = new ReportParameters()
                {
                    ReportPath = AppGlobals.ReportPath,
                    ReportFile = ReportFile,
                    OutputPath = AppGlobals.PrintedReportPath,
                    OutputFile = "CompanyGL",
                    DataSetName = "dsname_CompanyGL",
                    ReportData = tb_Ledger,
                    DataParameters = _Parameters,
                    ReportType = _ReportType

                };
                var reportData = ExportReport.Render(_ReportParamaters);
                var _mimeType = ExportReport.GetReportMime(_ReportType);
                var _Extention = ExportReport.GetReportExtention(_ReportType);
                return File(reportData, _mimeType, GLCompany.OutputFile + _Extention);
            }
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
        public async Task<IActionResult> OnGetSaleInvoiceAsync(int TranID)
        {

            #region Get Data Table
            var _CommandText = SQLQuery.SalesInvoice();
            var _Command = new SQLiteCommand(_CommandText, ConnectionClass.AppConnection(UserName));

            var _Adapter = new SQLiteDataAdapter(_Command);
            var _DataSet = new DataSet();

            _Command.Parameters.AddWithValue("@ID", TranID);
            _Adapter.Fill(_DataSet, "SalesInvoice");
            if (_DataSet.Tables.Count > 0)
            {
                var _Table = _DataSet.Tables[0];
                var SaleInvoice = new ReportClass
                {
                    AppUser = User,
                    ReportFilePath = AppGlobals.ReportPath,
                    ReportFile = "SalesInvoiceST.rdl",
                    ReportDataSet = "ds_SaleInvoice",
                    ReportSourceData = _Table,
                    RecordSort = "Sr_No",

                    OutputFilePath = AppGlobals.PrintedReportPath,
                    OutputFile = "SaleInvoiceST",
                    OutputFileLinkPath = AppGlobals.PrintedReportPathLink
                };

                var Heading1 = "Sales Invoice";
                var Heading2 = "Commercial";

                SaleInvoice.RptParameters.Add("CompanyName", CompanyName);
                SaleInvoice.RptParameters.Add("Heading1", Heading1);
                SaleInvoice.RptParameters.Add("Heading2", Heading2);
                SaleInvoice.RptParameters.Add("Footer", AppGlobals.ReportFooter);
                await Task.Run(() => (ReportLink = SaleInvoice.GetReportLink()));
                IsShowPdf = !SaleInvoice.IsError;
                if (!IsShowPdf) { ErrorMessages.Add(MessageClass.SetMessage(SaleInvoice.MyMessage)); }
                return Page();
            }

            #endregion

            return Page();
        }
        #endregion

        #region Sale Register

        public IActionResult OnGetSaleRegister()
        {

            SalesReportsModel model = new();
            model.Variables = new()
            {
                StartDate = AppRegistry.GetDate(UserName, "sRptDate1"),
                EndDate = AppRegistry.GetDate(UserName, "sRptDate2"),
                AllCompany = AppRegistry.GetBool(UserName, "sRptComAll"),
                AllInventory = AppRegistry.GetBool(UserName, "sRptStockAll"),
                CompanyID = AppRegistry.GetNumber(UserName, "sRptCompany"),
                InventoryID = AppRegistry.GetNumber(UserName, "sRptInventory"),
                Heading1 = AppRegistry.GetText(UserName, "sRptHeading1"),
                Heading2 = AppRegistry.GetText(UserName, "sRptHeading2"),
                ReportFile = AppRegistry.GetText(UserName, "sRptName"),
            };

            var _Filter = model.GetFilter(model.Variables);
            var _SQLQuery = SQLQuery.SaleRegister(_Filter);
            var _SourceTable = DataTableClass.GetTable(UserName, _SQLQuery);
            var SaleRegister = new ReportClass
            {
                AppUser = User,
                ReportFilePath = AppGlobals.ReportPath,
                ReportFile = model.Variables.ReportFile,
                ReportDataSet = "ds_SalesRegister",
                ReportSourceData = _SourceTable,
                RecordSort = "Company, Vou_Date",

                OutputFilePath = AppGlobals.PrintedReportPath,
                OutputFile = "SaleRegister",
                OutputFileLinkPath = AppGlobals.PrintedReportPathLink
            };

            SaleRegister.RptParameters.Add("CompanyName", CompanyName);
            SaleRegister.RptParameters.Add("Heading1", model.Variables.Heading1);
            SaleRegister.RptParameters.Add("Heading2", model.Variables.Heading2);
            SaleRegister.RptParameters.Add("Footer", AppGlobals.ReportFooter);
            ReportLink = SaleRegister.GetReportLink();
            IsShowPdf = !SaleRegister.IsError;
            if (!IsShowPdf) { ErrorMessages.Add(MessageClass.SetMessage(SaleRegister.MyMessage)); }
            return Page();
        }
        #endregion

        #region ExpenseSheet

        public IActionResult OnGetExpenseSheet(ReportType _ReportType)
        {
            #region Get Data Table
            var _SheetNo = AppRegistry.GetText(UserName, "Sheet_No");
            var _Filter = $"Sheet_No='{_SheetNo}'";
            var _Table = DataTableClass.GetTable(UserName, SQLQuery.ExpenseSheet(_Filter), "");

            #endregion

            var ExpenseSheet = new ReportClass
            {
                AppUser = User,
                ReportFilePath = AppGlobals.ReportPath,
                ReportFile = "ExpenseSheet.rdl",
                ReportDataSet = "ds_ExpenseSheet",
                ReportSourceData = _Table,
                RecordSort = "Vou_No",

                OutputFilePath = AppGlobals.PrintedReportPath,
                OutputFile = "ExpenseSheet",
                OutputFileLinkPath = AppGlobals.PrintedReportPathLink
            };
            var Heading1 = "PROJECT EXPENSES SHEET";
            var Heading2 = $"Project Sheet # {_SheetNo}";

            List<ReportParameter> _Parameters = new List<ReportParameter>
            {
                new ReportParameter("CompanyName", CompanyName),
                new ReportParameter("Heading1", Heading1),
                new ReportParameter("Heading2", Heading2),
                new ReportParameter("Footer", AppGlobals.ReportFooter)
            };


            var _ReportFile = string.Concat(ExpenseSheet.ReportFilePath, ExpenseSheet.ReportFile);
            ReportDataSource _DataSource = new("ds_ExpenseSheet", _Table);
            LocalReport report = new();
            var _ReportStream = new StreamReader(_ReportFile);
            report.LoadReportDefinition(_ReportStream);
            report.DataSources.Add(_DataSource);
            report.SetParameters(_Parameters);

            var _RenderFormat = ExportReport.GetRenderFormat(_ReportType);
            var _RenderedReport = report.Render(_RenderFormat);
            var _mimeType = ExportReport.GetReportMime(_ReportType);
            var _Extention = "." + ExportReport.GetReportExtention(_ReportType);

            if(_ReportType == ReportType.PDF)
            {
                ReportLink = CreateFile(_RenderedReport, ExpenseSheet.OutputFile + ".pdf" );
                IsShowPdf = !ExpenseSheet.IsError;
                if (!IsShowPdf) { ErrorMessages.Add(MessageClass.SetMessage(ExpenseSheet.MyMessage)); }
                return Page();
            }


            return File(_RenderedReport, _mimeType, ExpenseSheet.OutputFile + _Extention);


        }

        #endregion


        public string CreateFile(byte[] FileBtyes, string FileName)
        {
            if (FileBtyes.Length > 1)
            {
                FileStream MyFileStream;
                string OutPutFile = $"{AppGlobals.PrintedReportPath}{FileName}";
                string OutPutFileLink = $"{AppGlobals.PrintedReportPathLink}{FileName}";


                try
                {
                    if (System.IO.File.Exists(OutPutFile)) { System.IO.File.Delete(OutPutFile); }

                    using (FileStream fstream = new FileStream(OutPutFile, FileMode.Create))
                    {
                        fstream.Write(FileBtyes, 0, FileBtyes.Length);
                        MyFileStream = fstream;
                    }


                    if (System.IO.File.Exists(OutPutFile))
                    {
                        IsError = false;
                        MyMessage = "File has been created sucessfully.";
                        return OutPutFileLink;
                    }
                  
                }
                catch (Exception e) { MyMessage = e.Message; IsError = true; }

                
            }
            return "";

        }

    }
}
