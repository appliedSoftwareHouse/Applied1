using AppReporting;
using AppReportClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using System.Data;
using System.Data.SQLite;
using static Applied_WebApplication.Data.AppRegistry;
using static Applied_WebApplication.Data.AppFunctions;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.ReportPrint
{
    //public class COAListModel : PageModel
    public class ReportPrintModel : PageModel
    {
        #region Setup
        [BindProperty]
        public string ReportLink { get; set; }
        public bool IsError { get; set; }
        public string MyMessage { get; set; }
        public DataTable Preview { get; set; } = new();
        public List<Message> ErrorMessages { get; set; } = new();
        public bool IsShowPdf { get; set; } = false;
        public string UserName => User.Identity.Name;
        public string CompanyName => UserProfile.GetUserClaim(User, "Company");
        public string ReportFooter => GetReportFooter(User.Identity.Name);
        public string AppPath => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public string ReportPath = ".\\wwwroot\\Reports\\";
        public string PrintedReportsPath = ".\\wwwroot\\PrintedReports\\";
        public string PrintedReportsPathLink = "~/PrintedReports/";


        #endregion

        #region Get

        public void OnGet()
        {
        }
        #endregion

        #region Production Report
        public IActionResult OnGetProductionReport()
        {
            string _Filter = "";
            string _Flow = GetText(UserName, "pdRptFlow");
            int _Stock = GetNumber(UserName, "pdRptStock");
            DateTime _Date1 = GetDate(UserName, "pdRptDateFrom");
            DateTime _Date2 = GetDate(UserName, "pdRptDateTo");
            string ReportName = GetText(UserName, "pdRptName");

            string __Date1 = _Date1.ToString(DateYMD);
            string __Date2 = _Date2.ToString(DateYMD);

            if (_Flow.Length > 0) { _Filter = $"Flow='{_Flow}' "; }

            if (_Stock > 0)
            {
                if (_Filter.Length > 0) { _Filter += " AND "; }
                _Filter = $"Flow={_Flow} ";
            }
            if (_Filter.Length > 0) { _Filter += " AND "; }
            _Filter += $"Date([Vou_Date]) >= Date('{__Date1}') AND ";
            _Filter += $"Date([Vou_Date]) <= Date('{__Date2}')";


            string _Heading2 = $"From {_Date1.ToString(FormatDate)} to {_Date2.ToString(FormatDate)}";
            ReportClass reports = new ReportClass();
            DataTable SourceData = DataTableClass.GetTable(UserName, SQLQuery.ProductionReport(_Filter));

            reports.AppUser = User;
            reports.ReportFilePath = AppGlobals.ReportPath;
            reports.ReportFile = ReportName;
            reports.ReportDataSet = "ds_Production";
            reports.ReportSourceData = SourceData;
            reports.RecordSort = "Vou_Date, Vou_No";

            reports.OutputFilePath = AppGlobals.PrintedReportPath;
            reports.OutputFile = "ProductionReport";
            reports.OutputFileLinkPath = AppGlobals.PrintedReportPathLink;

            reports.RptParameters.Add("CompanyName", CompanyName);
            reports.RptParameters.Add("Heading1", "Production Report");
            reports.RptParameters.Add("Heading2", _Heading2);
            reports.RptParameters.Add("Footer", AppGlobals.ReportFooter);

            ReportLink = reports.GetReportLink();
            IsShowPdf = !reports.IsError;
            return Page();
        }
        #endregion

        #region Chart of Accounts
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

        #region General Ledger - GL
        public IActionResult OnGetGL(ReportType _ReportType)
        {

            ReportFilters Filters = new ReportFilters()
            {
                N_COA = (int)GetKey(UserName, "GL_COA", KeyType.Number),
                Dt_From = (DateTime)GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };
            DataTable _Table = Ledger.GetGL(UserName, Filters);

            if (_Table.Rows.Count > 0)
            {
                var FMTDate = GetFormatDate(UserName);
                var Date1 = MinDate(Filters.Dt_From, Filters.Dt_To).ToString(FMTDate);
                var Date2 = MaxDate(Filters.Dt_From, Filters.Dt_To).ToString(FMTDate);
                var _Heading1 = $"GENERAL LEDGER: {GetTitle(UserName, Tables.COA, Filters.N_COA)}";
                var _Heading2 = $"From {Date1} to {Date2}";
                var _CompanyName = UserProfile.GetCompanyName(User);


                List<ReportParameter> _Parameters = new List<ReportParameter>
                {
                    new ReportParameter("CompanyName", CompanyName),
                    new ReportParameter("Heading1", _Heading1),
                    new ReportParameter("Heading2", _Heading2),
                    new ReportParameter("Footer", AppGlobals.ReportFooter)
                };

                var Variables = new ReportParameters()
                {
                    ReportPath = AppGlobals.ReportPath,
                    ReportFile = "Ledger.rdl",
                    OutputPath = AppGlobals.PrintedReportPath,
                    OutputPathLink = AppGlobals.PrintedReportPathLink,
                    OutputFile = "Ledger",
                    CompanyName = _CompanyName,
                    Heading1 = _Heading1,
                    Heading2 = _Heading2,
                    Footer = AppGlobals.ReportFooter,
                    ReportType = _ReportType,
                    DataSetName = "dsname_Ledger",
                    ReportData = _Table,
                    DataParameters = _Parameters
                };

                var ReportClass = new ExportReport(Variables);
                ReportClass.Render();

                if (_ReportType == ReportType.Preview)
                {
                    ReportLink = ReportClass.Variables.GetFileLink();
                    IsShowPdf = true;
                    return Page();
                }
                else
                {
                    return File(ReportClass.Variables.FileBytes, ReportClass.Variables.MimeType, ReportClass.Variables.OutputFileFullName);
                }

            }

            return Page();


        }
        #endregion

        #region General Ledger - GL Supplier / Vendor Ledger

        public IActionResult OnGetGLCompany(ReportType _ReportType)
        {
            #region Report Filter Variables
            ReportFilters Filters = new()
            {
                N_Customer = (int)GetKey(UserName, "GL_Company", KeyType.Number),
                Dt_From = (DateTime)GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };
            #endregion
            var _COAs = GetText(UserName, "CompanyGLs");
            var _Filter = $"COA IN ({_COAs}) AND Customer = {Filters.N_Customer} ";
            var _Dates = new string[] { Filters.Dt_From.ToString(DateYMD), Filters.Dt_To.ToString(DateYMD) };
            var _OrderBy = "[Vou_Date], [Vou_No]";
            var _Table = DataTableClass.GetTable(UserName, SQLQuery.Ledger2(_Filter, _Dates, _OrderBy));

            if (_Table.Rows.Count > 0)
            {
                var _Title = GetTitle(UserName, Tables.Customers, Filters.N_Customer);
                var _Status = GetColumnValue(UserName, Tables.Customers, "Status", Filters.N_Customer);
                var _StatusTitle = "";
                if (_Status.Length > 0)
                {
                    _StatusTitle = DirectoryClass.GetDirectoryValue(UserName, "CompanyStatus", Convert.ToInt32(_Status));
                    if (_StatusTitle.Length == 0) { _StatusTitle = "Un-assigned"; }
                }
                var _Heading1 = string.Concat(_Title, " (", _StatusTitle, ")");
                var _Heading2 = string.Concat("From ", Filters.Dt_From.ToString(FormatDate), " To ", Filters.Dt_To.ToString(FormatDate));

                List<ReportParameter> _Parameters = new List<ReportParameter>
                {
                    new ReportParameter("CompanyName", CompanyName),
                    new ReportParameter("Heading1", _Heading1),
                    new ReportParameter("Heading2", _Heading2),
                    new ReportParameter("Footer", AppGlobals.ReportFooter)
                };

                var Variables = new ReportParameters()
                {
                    ReportPath = AppGlobals.ReportPath,
                    ReportFile = "CompanyGL2.rdl",
                    OutputPath = AppGlobals.PrintedReportPath,
                    OutputPathLink = AppGlobals.PrintedReportPathLink,
                    OutputFile = "CompanyGL2",
                    CompanyName = UserProfile.GetCompanyName(User),
                    Heading1 = "General Ledger",
                    Heading2 = "Vender / Supplier / Customer",
                    Footer = AppGlobals.ReportFooter,
                    ReportType = _ReportType,
                    DataSetName = "dsname_CompanyGL",
                    ReportData = _Table,
                    DataParameters = _Parameters
                };

                var ReportClass = new ExportReport(Variables);
                ReportClass.Render();

                if (_ReportType == ReportType.Preview)
                {
                    ReportLink = ReportClass.Variables.GetFileLink();
                    IsShowPdf = true;
                    return Page();
                }
                else
                {
                    return File(ReportClass.Variables.FileBytes, ReportClass.Variables.MimeType, ReportClass.Variables.OutputFileFullName);
                }
            }

            ErrorMessages.Add(SetMessage("No Record / File found to generate Report........", ConsoleColor.Yellow));
            return Page();
        }

        #endregion

        #region General Ledger - Employees

        public IActionResult OnGetGLEmployee(ReportType _ReportType)
        {
            #region Report Filter Variables
            ReportFilters Filters = new()
            {
                N_COA = 0, // (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                N_Employee = (int)AppRegistry.GetKey(UserName, "GL_Employee", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };
            #endregion

            var _Table = DataTableClass.GetTable(UserName, SQLQuery.Ledger(Filters.FilterText() + " ORDER BY Customer, COA, Vou_Date"));

            if (_Table.Rows.Count > 0)
            {
                var _Title = GetTitle(UserName, Tables.Employees, Filters.N_Employee);
                var _Heading1 = string.Concat(_Title);
                var _Heading2 = string.Concat("From ", Filters.Dt_From.ToString(AppRegistry.FormatDate), " To ", Filters.Dt_To.ToString(AppRegistry.FormatDate));

                List<ReportParameter> _Parameters = new List<ReportParameter>
                {
                    new ReportParameter("CompanyName", CompanyName),
                    new ReportParameter("Heading1", _Heading1),
                    new ReportParameter("Heading2", _Heading2),
                    new ReportParameter("Footer", AppGlobals.ReportFooter)
                };

                var Variables = new ReportParameters()
                {
                    ReportPath = AppGlobals.ReportPath,
                    ReportFile = "EmployeeGL.rdl",
                    OutputPath = AppGlobals.PrintedReportPath,
                    OutputPathLink = AppGlobals.PrintedReportPathLink,
                    OutputFile = "EmployeeGL",
                    CompanyName = UserProfile.GetCompanyName(User),
                    Heading1 = "General Ledger",
                    Heading2 = "Vender / Supplier / Customer",
                    Footer = AppGlobals.ReportFooter,
                    ReportType = _ReportType,
                    DataSetName = "ds_EmployeeGL",
                    ReportData = _Table,
                    DataParameters = _Parameters
                };

                var ReportClass = new ExportReport(Variables);
                ReportClass.Render();

                if (_ReportType == ReportType.Preview)
                {
                    ReportLink = ReportClass.Variables.GetFileLink();
                    IsShowPdf = true;
                    return Page();
                }
                else
                {
                    return File(ReportClass.Variables.FileBytes, ReportClass.Variables.MimeType, ReportClass.Variables.OutputFileFullName);
                }
            }

            ErrorMessages.Add(MessageClass.SetMessage("No Record / File found to generate Report........", ConsoleColor.Yellow));
            return Page();
        }

        #endregion

        #region General Ledger - Projects
        public IActionResult OnGetGLProject(ReportType RptType)
        {
            try
            {

                string _Nature = GetText(UserName, "GLp_Nature");
                int _Project = GetNumber(UserName, "GL_Project");
                DateTime _Date1 = GetDate(UserName, "GL_Dt_From");
                DateTime _Date2 = GetDate(UserName, "GL_Dt_To");
                string __Date1 = _Date1.ToString(DateYMD);
                string __Date2 = _Date2.ToString(DateYMD);
                string _Sort = "Vou_Date, Vou_No";

                string _Filter1 = $"[L].[Project] = {_Project} AND NOT [C].[NATURE] IN ({_Nature}) AND ";
                _Filter1 += $"Date([Vou_Date]) < '{__Date1}'";

                string _Filter2 = $"[L].[Project] = {_Project} AND NOT [C].[NATURE] IN ({_Nature}) AND ";
                _Filter2 += $"Date([Vou_Date]) >= '{__Date1}' AND Date([Vou_Date] <= '{__Date2}')";

                string _Query = SQLQuery.GLProject(_Filter1, _Filter2, _Sort);

                ReportClass reports = new ReportClass();
                DataTable _SourceTable = DataTableClass.GetTable(UserName, _Query);

                string _ReportFile = "GLProject";
                string _Heading1 = "No Records Found...";
                string _Heading2 = $"From {_Date1.ToString(FormatDate)} to {_Date2.ToString(FormatDate)}";

                if (_SourceTable.Rows.Count > 0)
                {
                    _Heading1 = $"PROJECT LEDGER - {_SourceTable.Rows[1]["ProTitle"]}";
                }

                ReportModel Reportmodel = new();
                // Input Parameters  (.rdl report file)
                Reportmodel.InputReport.FilePath = ReportPath;
                Reportmodel.InputReport.FileName = _ReportFile;
                Reportmodel.InputReport.FileExtention = "rdl";
                // output Parameters (like pdf, excel, word, html, tiff)
                Reportmodel.OutputReport.FilePath = PrintedReportsPath;
                Reportmodel.OutputReport.FileLink = PrintedReportsPathLink;
                Reportmodel.OutputReport.FileName = _ReportFile;
                Reportmodel.OutputReport.ReportType = RptType;
                // Reports Parameters
                Reportmodel.AddReportParameter("CompanyName", CompanyName);
                Reportmodel.AddReportParameter("Heading1", _Heading1);
                Reportmodel.AddReportParameter("Heading2", _Heading2);
                Reportmodel.AddReportParameter("Footer", ReportFooter);

                Reportmodel.ReportData.DataSetName = "ds_Project";
                Reportmodel.ReportData.ReportTable = _SourceTable; // Data Filter will apply by registry variables. FYI

                if (Reportmodel.ReportRenderAsync().Result)         // Render a report for preview or download...
                {
                    if (Reportmodel.OutputReport.ReportType == ReportType.HTML || Reportmodel.OutputReport.ReportType == ReportType.Preview)
                    {
                        ReportLink = Reportmodel.OutputReport.FileLink;
                        IsShowPdf = true;
                        return Page();
                    }
                    else
                    {
                        var FileName = $"{Reportmodel.OutputReport.FileName}{Reportmodel.OutputReport.FileExtention}";
                        return File(Reportmodel.ReportBytes, Reportmodel.OutputReport.MimeType, FileName);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return Page();
        }
        #endregion

        #region Trial Balance
        public IActionResult OnGetTBPrint(string _ReportLink, Boolean _IsShowPdf)
        {
            ReportLink = _ReportLink;
            IsShowPdf = _IsShowPdf;
            return Page();
        }

        public async Task<IActionResult> OnGetOBTBAsync()
        {
            var OBDate = GetDate(UserName, "OBDate");
            var DateFormat = FormatDate;
            TrialBalanceClass TB = new(User);
            TB.Heading2 = "Opening Balances as on " + OBDate.ToString(DateFormat);
            TB.MyDataTable = TB.TBOB_Data();
            await Task.Run(() => (ReportLink = TB.MyReportClass.GetReportLink()));

            IsShowPdf = !TB.MyReportClass.IsError;

            if (!IsShowPdf) { ErrorMessages.Add(SetMessage(TB.MyReportClass.MyMessage)); }
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
            var _SalesReportName = GetText(UserName, "SalesReportRDL");

            if (_SalesReportName.Length == 0) { _SalesReportName = "SalesInvoiceST"; }

            _Command.Parameters.AddWithValue("@ID", TranID);
            _Adapter.Fill(_DataSet, "SalesInvoice");
            if (_DataSet.Tables.Count > 0)
            {
                var _Table = _DataSet.Tables[0];
                var SaleInvoice = new ReportClass
                {
                    AppUser = User,
                    ReportFilePath = AppGlobals.ReportPath,
                    ReportFile = _SalesReportName + ".rdl",
                    ReportDataSet = "ds_SaleInvoice",
                    ReportSourceData = _Table,
                    RecordSort = "Sr_No",

                    OutputFilePath = AppGlobals.PrintedReportPath,
                    OutputFile = _SalesReportName,
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
                if (!IsShowPdf) { ErrorMessages.Add(SetMessage(SaleInvoice.MyMessage)); }
                return Page();
            }

            #endregion

            return Page();
        }
        #endregion

        #region Sale Return
        public IActionResult OnGetSaleReturn()
        {
            try
            {
                var RptType = (ReportType)GetNumber(UserName, "srRptType");
                var _PageModel = new SalesReturnReportModel();
                _PageModel.UserName = UserName;

                //_PageModel.UserName = UserName;
                _PageModel.GetKeys();
                _PageModel.LoadData(_PageModel.GetFilter(_PageModel.Variables));

                var _Heading1 = _PageModel.Variables.Heading1;
                var _Heading2 = _PageModel.Variables.Heading2;

                ReportModel Reportmodel = new();
                // Input Parameters  (.rdl report file)
                Reportmodel.InputReport.FilePath = ReportPath;
                Reportmodel.InputReport.FileName = "SalesReturn";
                Reportmodel.InputReport.FileExtention = "rdl";
                // output Parameters (like pdf, excel, word, html, tiff)
                Reportmodel.OutputReport.FilePath = PrintedReportsPath;
                Reportmodel.OutputReport.FileLink = PrintedReportsPathLink;
                Reportmodel.OutputReport.FileName = "SalesReturn";
                Reportmodel.OutputReport.ReportType = RptType;
                // Reports Parameters
                Reportmodel.AddReportParameter("CompanyName", CompanyName);
                Reportmodel.AddReportParameter("Heading1", _Heading1);
                Reportmodel.AddReportParameter("Heading2", _Heading2);
                Reportmodel.AddReportParameter("Footer", ReportFooter);

                Reportmodel.ReportData.DataSetName = "ds_SalesReturn";
                Reportmodel.ReportData.ReportTable = _PageModel.SourceTable; // Data Filter will apply by registry variables. FYI

                var _Result = Reportmodel.ReportRenderAsync().Result;

                if (_Result)         // Render a report for preview or download...
                {
                    if (Reportmodel.OutputReport.ReportType == ReportType.HTML || Reportmodel.OutputReport.ReportType == ReportType.Preview)
                    {
                        ReportLink = Reportmodel.OutputReport.FileLink;
                        IsShowPdf = true;
                        return Page();
                    }
                    else
                    {
                        var FileName = $"{Reportmodel.OutputReport.FileName}{Reportmodel.OutputReport.FileExtention}";
                        return File(Reportmodel.ReportBytes, Reportmodel.OutputReport.MimeType, FileName);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMessages.Add(SetMessage($"ERROR: {e.Message}", ConsoleColor.Red));
            }

            return Page();

        }
        #endregion

        #region Sales Register Report

        public IActionResult OnGetSaleRegister(ReportType _ReportType)
        {
            #region Check Error
            if (_ReportType.ToString().Length == 0)
            {
                ErrorMessages.Add(SetMessage("Report Type not defined", ConsoleColor.Red));
                return Page();
            }
            #endregion

            #region Create Report Data Class
            SalesReportsModel model = new();
            model.Variables = new()
            {
                StartDate = GetDate(UserName, "sRptDate1"),
                EndDate = GetDate(UserName, "sRptDate2"),
                AllCompany = GetBool(UserName, "sRptComAll"),
                AllInventory = GetBool(UserName, "sRptStockAll"),
                CompanyID = GetNumber(UserName, "sRptCompany"),
                City = GetText(UserName, "sRptCity"),
                InventoryID = GetNumber(UserName, "sRptInventory"),
                Heading1 = GetText(UserName, "sRptHeading1"),
                Heading2 = GetText(UserName, "sRptHeading2"),
                ReportFile = GetText(UserName, "sRptName"),
                ReportType = GetNumber(UserName, "sRptType")
            };

            var _Filter = model.GetFilter(model.Variables);
            var _SQLQuery = SQLQuery.SaleRegister(_Filter);
            var _Table = DataTableClass.GetTable(UserName, _SQLQuery, "[Vou_Date],[Vou_No]");
            #endregion

            #region Report's Data Parameters

            var Heading1 = "SALE REGISTER";
            var Heading2 = $"From {model.Variables.StartDate.ToString(FormatDate)} " +
                $"To {model.Variables.EndDate.ToString(FormatDate)}";

            List<ReportParameter> _Parameters = new List<ReportParameter>
            {
                new ReportParameter("CompanyName", CompanyName),
                new ReportParameter("Heading1", Heading1),
                new ReportParameter("Heading2", Heading2),
                new ReportParameter("Footer", AppGlobals.ReportFooter)
            };
            #endregion

            #region Report Setup

            var SaleRegisterReport = new ReportParameters()
            {
                ReportPath = AppGlobals.ReportPath,
                ReportFile = model.Variables.ReportFile,
                ReportType = (ReportType)model.Variables.ReportType,
                ReportData = _Table,
                DataParameters = _Parameters,

                OutputPath = AppGlobals.PrintedReportPath,
                OutputPathLink = AppGlobals.PrintedReportPathLink,
                OutputFile = "SaleRegister",

                DataSetName = "ds_SalesRegister",
                Footer = AppGlobals.ReportFooter,
                Heading1 = Heading1,
                Heading2 = Heading2,
            };

            #region Generae Report
            try
            {
                var _Download = new ExportReport(SaleRegisterReport);
                _Download.Render();
                if (_Download.Variables.IsSaved)
                {
                    ReportLink = _Download.Variables.GetFileLink();
                    IsShowPdf = true;
                    return Page();
                }
                return File(_Download.Variables.FileBytes, _Download.Variables.MimeType, _Download.Variables.OutputFileName);
            }
            catch (Exception e)
            {
                ErrorMessages.Add(SetMessage(e.Message, ConsoleColor.Red));
            }
            #endregion

            return Page();

            //var SaleRegister = new ReportClass
            //{
            //    AppUser = User,
            //    ReportFilePath = AppGlobals.ReportPath,
            //    ReportFile = model.Variables.ReportFile,

            //    ReportDataSet = "ds_SalesRegister",
            //    ReportSourceData = _SourceTable,
            //    RecordSort = "Company, Vou_Date",

            //    OutputFilePath = AppGlobals.PrintedReportPath,
            //    OutputFile = "SaleRegister",
            //    OutputFileLinkPath = AppGlobals.PrintedReportPathLink
            //};


            //SaleRegister.RptParameters.Add("CompanyName", CompanyName);
            //SaleRegister.RptParameters.Add("Heading1", model.Variables.Heading1);
            //SaleRegister.RptParameters.Add("Heading2", model.Variables.Heading2);
            //SaleRegister.RptParameters.Add("Footer", AppGlobals.ReportFooter);
            //ReportLink = SaleRegister.GetReportLink();
            //IsShowPdf = !SaleRegister.IsError;
            //if (!IsShowPdf) { ErrorMessages.Add(SetMessage(SaleRegister.MyMessage)); }
            //return Page();
            #endregion
        }
        #endregion

        #region Purchase Register
        public IActionResult OnGetPurchaseRegister(ReportType RptType)
        {
            try
            {
                PurchaseReportsModel model = new();

                // Generate / Obtain Report Data from Temp Table....
                var _TempTable = GetText(UserName, "pRptTemp");
                var _SourceTable = TempDBClass.LoadTempTableAsync(UserName, _TempTable).Result;
                if (_SourceTable.DataSet == null) { return Page(); }
                // End Generate Report Data

                var _Heading1 = GetText(UserName, "pRptHeading1");
                var _Heading2 = GetText(UserName, "pRptHeading2");


                ReportModel Reportmodel = new ReportModel();
                // Input Parameters  (.rdl report file)
                Reportmodel.InputReport.FilePath = ReportPath;
                Reportmodel.InputReport.FileName = GetText(UserName, "pRptName");
                Reportmodel.InputReport.FileExtention = "rdl";
                // output Parameters (like pdf, excel, word, html, tiff)
                Reportmodel.OutputReport.FilePath = PrintedReportsPath;
                Reportmodel.OutputReport.FileLink = PrintedReportsPathLink;
                Reportmodel.OutputReport.FileName = "PurchaseRegister";
                Reportmodel.OutputReport.ReportType = RptType;
                // Reports Parameters
                Reportmodel.AddReportParameter("CompanyName", CompanyName);
                Reportmodel.AddReportParameter("Heading1", _Heading1);
                Reportmodel.AddReportParameter("Heading2", _Heading2);
                Reportmodel.AddReportParameter("Footer", ReportFooter);

                var StockClass = new StockLedgersClass(UserName);

                Reportmodel.ReportData.DataSetName = "ds_PurchaseRegister";
                Reportmodel.ReportData.ReportTable = _SourceTable;

                if (Reportmodel.ReportRender())         // Render a report for preview or download...
                {
                    if (Reportmodel.OutputReport.ReportType == ReportType.HTML || Reportmodel.OutputReport.ReportType == ReportType.Preview)
                    {
                        ReportLink = Reportmodel.OutputReport.GetFileLink();
                        IsShowPdf = true;
                        return Page();
                    }
                    else
                    {
                        var FileName = $"{Reportmodel.OutputReport.FileName}{Reportmodel.OutputReport.FileExtention}";
                        return File(Reportmodel.ReportBytes, Reportmodel.OutputReport.MimeType, FileName);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMessages.Add(SetMessage($"ERROR: {e.Message}", ConsoleColor.Red));
            }

            return Page();
        }

        #endregion

        #region ExpenseSheet

        public IActionResult OnGetExpenseSheet(ReportType _ReportType)
        {
            #region Check Error
            if (_ReportType.ToString().Length == 0)
            {
                ErrorMessages.Add(SetMessage("Report Type not defined", ConsoleColor.Red));
                return Page();
            }
            #endregion

            #region Get Data Table
            var _SheetNo = GetText(UserName, "Sheet_No");

            if (_SheetNo.Length == 0)
            {
                ErrorMessages.Add(SetMessage("Expense sheet is not defined.", ConsoleColor.Red));
                return Page();
            }

            var _Filter = $"Sheet_No='{_SheetNo}'";
            var _Table = DataTableClass.GetTable(UserName, SQLQuery.ExpenseSheet(_Filter), "");

            #endregion

            #region Report's Data Parameters

            var Heading1 = "PROJECT EXPENSES SHEET";
            var Heading2 = $"Project Sheet # {_SheetNo}";

            List<ReportParameter> _Parameters = new List<ReportParameter>
            {
                new ReportParameter("CompanyName", CompanyName),
                new ReportParameter("Heading1", Heading1),
                new ReportParameter("Heading2", Heading2),
                new ReportParameter("Footer", AppGlobals.ReportFooter)
            };
            #endregion

            #region Report Setup

            var ReportParameters = new ReportParameters()
            {
                ReportPath = AppGlobals.ReportPath,
                ReportFile = "ExpenseSheet.rdl",
                ReportType = _ReportType,
                ReportData = _Table,
                DataParameters = _Parameters,

                OutputPath = AppGlobals.PrintedReportPath,
                OutputPathLink = AppGlobals.PrintedReportPathLink,
                OutputFile = "ExpenseSheet",

                DataSetName = "ds_ExpenseSheet",
                Footer = AppGlobals.ReportFooter,
                Heading1 = Heading1,
                Heading2 = Heading2,
            };

            #endregion

            #region Generae Report
            try
            {
                var _Download = new ExportReport(ReportParameters);
                _Download.Render();
                if (_Download.Variables.IsSaved)
                {
                    ReportLink = _Download.Variables.GetFileLink();
                    IsShowPdf = true;
                    return Page();
                }
                return File(_Download.Variables.FileBytes, _Download.Variables.MimeType, _Download.Variables.OutputFileName);
            }
            catch (Exception e)
            {
                ErrorMessages.Add(SetMessage(e.Message, ConsoleColor.Red));
            }
            #endregion

            return Page();

        }
        #endregion

        #region Expense Group Report
        public IActionResult OnGetExpenseGroup(ReportType _ReportType)
        {
            #region Get Data Table
            var _SheetNo = AppRegistry.GetText(UserName, "Sheet_No");

            if (_SheetNo.Length == 0)
            {
                ErrorMessages.Add(SetMessage("Expense sheet is not defined.", ConsoleColor.Red));
                return Page();
            }

            var _Filter = $"Sheet_No='{_SheetNo}'";
            var _Table = DataTableClass.GetTable(UserName, SQLQuery.ExpenseSheetGroup(_Filter), "");

            #endregion

            #region Report's Data Parameters

            var Heading1 = "PROJECT SUMMARY EXPENSES SHEET";
            var Heading2 = $"Project Sheet # {_SheetNo}";


            List<ReportParameter> _Parameters = new List<ReportParameter>
            {
                new ReportParameter("CompanyName", CompanyName),
                new ReportParameter("Heading1", Heading1),
                new ReportParameter("Heading2", Heading2),
                new ReportParameter("Footer", AppGlobals.ReportFooter)
            };
            #endregion

            #region Report Setup

            var ReportParameters = new ReportParameters()
            {
                ReportPath = AppGlobals.ReportPath,
                ReportFile = "ExpenseSheetGroup.rdl",
                ReportType = _ReportType,
                ReportData = _Table,
                DataParameters = _Parameters,

                OutputPath = AppGlobals.PrintedReportPath,
                OutputPathLink = AppGlobals.PrintedReportPathLink,
                OutputFile = "ExpenseGroup",

                DataSetName = "ds_ExpenseGroup",
                Footer = AppGlobals.ReportFooter,
                Heading1 = Heading1,
                Heading2 = Heading2,
            };

            #endregion

            #region Print Report
            try
            {
                var _Download = new ExportReport(ReportParameters);
                _Download.Render();
                if (_Download.Variables.IsSaved)
                {
                    ReportLink = _Download.Variables.GetFileLink();
                    IsShowPdf = true;
                    return Page();
                }
                return File(_Download.Variables.FileBytes, _Download.Variables.MimeType, _Download.Variables.OutputFileName);
            }
            catch (Exception e)
            {
                ErrorMessages.Add(SetMessage(e.Message, ConsoleColor.Red));
            }
            return Page();

            #endregion

        }

        #endregion

        #region Voucher
        public IActionResult OnGetVoucher(ReportType _ReportType)
        {

            #region Get Data Table
            var _VoucherNo = AppRegistry.GetText(UserName, "cbVouNo");
            var _Filter = $"Vou_No='{_VoucherNo}'";
            var _Table = DataTableClass.GetTable(UserName, SQLQuery.Voucher(_Filter));
            if (_Table.Rows.Count == 0)
            {
                ErrorMessages.Add(SetMessage($"Vouche No {_VoucherNo} not Found.", ConsoleColor.Red));
                return Page();
            }
            #endregion

            #region Report's Data Parameters

            var _VoucherType = _Table.Rows[0]["Vou_Type"];
            string Heading1 = _VoucherType switch
            {
                VoucherType.Cash => "Cash Voucher",
                VoucherType.Bank => "Bank Voucher",
                VoucherType.Payment => "Payment Voucher",
                VoucherType.Receivable => "Sales Invoices Voucher",
                VoucherType.OBalance => "Opening Balance Voucher",
                VoucherType.Cheque => "Bank Payment Voucher",
                VoucherType.JV => "Journal Voucher",
                VoucherType.OBalCom => "Opening Balance Voucher",
                VoucherType.OBalStock => "Stock Opening Balance Voucher",
                VoucherType.Receipt => "Receipt Voucher",
                _ => "Journal Voucher"
            };

            var Heading2 = $"Voucher # {_VoucherNo}";

            List<ReportParameter> _Parameters = new List<ReportParameter>
            {
                new ReportParameter("CompanyName", CompanyName),
                new ReportParameter("Heading1", Heading1),
                new ReportParameter("Heading2", Heading2),
                new ReportParameter("Footer", AppGlobals.ReportFooter)
            };
            #endregion

            #region Report Setup

            var ReportParameters = new ReportParameters()
            {
                ReportPath = AppGlobals.ReportPath,
                ReportFile = "Voucher.rdl",
                ReportType = _ReportType,
                ReportData = _Table,
                DataParameters = _Parameters,

                OutputPath = AppGlobals.PrintedReportPath,
                OutputPathLink = AppGlobals.PrintedReportPathLink,
                OutputFile = "Voucher",

                DataSetName = "ds_Voucher",
                Footer = AppGlobals.ReportFooter,
                Heading1 = Heading1,
                Heading2 = Heading2,
            };

            #endregion

            try
            {
                var _Download = new ExportReport(ReportParameters);
                _Download.Render();
                if (_Download.Variables.IsSaved)
                {
                    ReportLink = _Download.Variables.GetFileLink();
                    IsShowPdf = true;
                    return Page();
                }
                return File(_Download.Variables.FileBytes, _Download.Variables.MimeType, _Download.Variables.OutputFileName);
            }
            catch (Exception e)
            {
                ErrorMessages.Add(SetMessage(e.Message, ConsoleColor.Red));
            }
            return Page();

        }

        #endregion

        #region Company Balances
        public IActionResult OnGetComBalances(ReportType _ReportType)
        {
            try
            {
                var _COA_List = GetText(UserName, "CompanyGLs");
                var _Heading1 = GetText(UserName, "cRptHead1");
                var _Heading2 = GetText(UserName, "cRptHead2");
                var _RptQuery = GetText(UserName, "cRptQuery");

                ReportModel Reportmodel = new ReportModel();
                // Input Parameters  (.rdl report file)
                Reportmodel.InputReport.FilePath = ReportPath;
                Reportmodel.InputReport.FileName = "CompanyBalances";
                Reportmodel.InputReport.FileExtention = "rdl";
                // output Parameters (like pdf, excel, word, html, tiff)
                Reportmodel.OutputReport.FilePath = PrintedReportsPath;
                Reportmodel.OutputReport.FileName = "ComBalance";
                Reportmodel.OutputReport.ReportType = _ReportType;
                // Reports Parameters
                Reportmodel.AddReportParameter("CompanyName", CompanyName);
                Reportmodel.AddReportParameter("Heading1", _Heading1);
                Reportmodel.AddReportParameter("Heading2", _Heading2);
                Reportmodel.AddReportParameter("Footer", ReportFooter);

                Reportmodel.ReportData.DataSetName = "ds_ComBalance";
                Reportmodel.ReportData.ReportTable = DataTableClass.GetTable(UserName, SQLQuery.CompanyBalances(_RptQuery, _COA_List));

                if (Reportmodel.ReportRender())
                {
                    if (Reportmodel.OutputReport.ReportType == ReportType.HTML || Reportmodel.OutputReport.ReportType == ReportType.Preview)
                    {

                        ReportLink = $"~/PrintedReports/{Reportmodel.OutputReport.FileName}{Reportmodel.OutputReport.FileExtention}";
                        IsShowPdf = true;
                        return Page();



                    }
                    else
                    {
                        var FileName = $"{Reportmodel.OutputReport.FileName}{Reportmodel.OutputReport.FileExtention}";
                        return File(Reportmodel.ReportBytes, Reportmodel.OutputReport.MimeType, FileName);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMessages.Add(SetMessage($"ERROR: {e.Message}", ConsoleColor.Red));
            }

            return Page();
        }

        #endregion

        #region Stock in Hand
        public IActionResult OnGetStockInHand(ReportType RptOption)
        {
            try
            {
                var _Heading1 = GetText(UserName, "cSIHHead1");
                var _Heading2 = GetText(UserName, "cSIHHead2");


                ReportModel Reportmodel = new ReportModel();
                // Input Parameters  (.rdl report file)
                Reportmodel.InputReport.FilePath = ReportPath;
                Reportmodel.InputReport.FileName = "StockInHand";
                Reportmodel.InputReport.FileExtention = "rdl";
                // output Parameters (like pdf, excel, word, html, tiff)
                Reportmodel.OutputReport.FilePath = PrintedReportsPath;
                Reportmodel.OutputReport.FileLink = PrintedReportsPathLink;
                Reportmodel.OutputReport.FileName = "StockInHand";
                Reportmodel.OutputReport.ReportType = RptOption;
                // Reports Parameters
                Reportmodel.AddReportParameter("CompanyName", CompanyName);
                Reportmodel.AddReportParameter("Heading1", _Heading1);
                Reportmodel.AddReportParameter("Heading2", _Heading2);
                Reportmodel.AddReportParameter("Footer", ReportFooter);

                var _SourceData = TempDBClass.LoadTempTableAsync(UserName, GetText(UserName, "stkInHand")).Result;

                Reportmodel.ReportData.DataSetName = "ds_StockInHand";
                Reportmodel.ReportData.ReportTable = _SourceData;

                if (Reportmodel.ReportRenderAsync().Result)         // Render a report for preview or download...
                {
                    if (Reportmodel.OutputReport.ReportType == ReportType.HTML || Reportmodel.OutputReport.ReportType == ReportType.Preview)
                    {
                        ReportLink = Reportmodel.OutputReport.GetFileLink();
                        IsShowPdf = true;
                        return Page();
                    }
                    else
                    {
                        var FileName = $"{Reportmodel.OutputReport.FileName}{Reportmodel.OutputReport.FileExtention}";
                        return File(Reportmodel.ReportBytes, Reportmodel.OutputReport.MimeType, FileName);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMessages.Add(SetMessage($"ERROR: {e.Message}", ConsoleColor.Red));
            }

            return Page();
        }

        #endregion

        #region Stock Ledger
        public IActionResult OnGetStockLedger(ReportType RptType)
        {
            try
            {
                // Generate / Obtain Report Data from Temp Table....
                var _TempTable = GetText(UserName, "stkLedData");
                var _SourceTable = TempDBClass.LoadTempTableAsync(UserName, _TempTable).Result;
                var _ReportFile = GetText(UserName, "stkLedger");
                if (_SourceTable == null) { return Page(); }

                var _Heading1 = GetText(UserName, "stkLedHead1");
                var _Heading2 = GetText(UserName, "stkLedHead2");

                ReportModel Reportmodel = new();
                // Input Parameters  (.rdl report file)
                Reportmodel.InputReport.FilePath = ReportPath;
                Reportmodel.InputReport.FileName = _ReportFile;
                Reportmodel.InputReport.FileExtention = "rdl";
                // output Parameters (like pdf, excel, word, html, tiff)
                Reportmodel.OutputReport.FilePath = PrintedReportsPath;
                Reportmodel.OutputReport.FileLink = PrintedReportsPathLink;
                Reportmodel.OutputReport.FileName = _ReportFile;
                Reportmodel.OutputReport.ReportType = RptType;
                // Reports Parameters
                Reportmodel.AddReportParameter("CompanyName", CompanyName);
                Reportmodel.AddReportParameter("Heading1", _Heading1);
                Reportmodel.AddReportParameter("Heading2", _Heading2);
                Reportmodel.AddReportParameter("Footer", ReportFooter);

                Reportmodel.ReportData.DataSetName = "ds_StockLedger";
                Reportmodel.ReportData.ReportTable = _SourceTable; // Data Filter will apply by registry variables. FYI

                if (Reportmodel.ReportRenderAsync().Result)         // Render a report for preview or download...
                {
                    if (Reportmodel.OutputReport.ReportType == ReportType.HTML || Reportmodel.OutputReport.ReportType == ReportType.Preview)
                    {
                        ReportLink = Reportmodel.OutputReport.GetFileLink();
                        IsShowPdf = true;
                        return Page();
                    }
                    else
                    {
                        var FileName = $"{Reportmodel.OutputReport.FileName}{Reportmodel.OutputReport.FileExtention}";
                        return File(Reportmodel.ReportBytes, Reportmodel.OutputReport.MimeType, FileName);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMessages.Add(SetMessage($"ERROR: {e.Message}", ConsoleColor.Red));
            }

            return Page();
        }

        #endregion

    }
}
