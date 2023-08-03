using AppReporting;
using AppReportClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using System.Data;
using System.Data.SQLite;
using static Applied_WebApplication.Data.AppFunctions;
using static Applied_WebApplication.Data.MessageClass;
using NPOI.OpenXmlFormats.Dml.Chart;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class COAListModel : PageModel
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
        #endregion

        #region Get Reports.

        public void OnGet()
        {
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

        #region Supplier / Vendor Ledger

        public IActionResult OnGetGLCompany(ReportType _ReportType)
        {
            #region Report Filter Variables
            ReportFilters Filters = new()
            {
                N_Customer = (int)AppRegistry.GetKey(UserName, "GL_Company", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };
            #endregion
            var _COAs = AppRegistry.GetText(UserName, "CompanyGLs");
            var _Filter = $"COA IN({_COAs}) AND {Filters.FilterText()} ORDER BY Customer, Vou_Date, COA";
            var _Table = DataTableClass.GetTable(UserName, SQLQuery.Ledger(_Filter));

            if (_Table.Rows.Count > 0)
            {
                var _Title = GetTitle(UserName, Tables.Customers, Filters.N_Customer);
                var _Status = GetColumnValue(UserName, Tables.Customers, "Status", Filters.N_Customer);
                var _StatusTitle = "";
                if (_Status.Length > 0)
                {
                    _StatusTitle = DirectoryClass.GetDirectoryValue(UserName, "CompanyStatus", Convert.ToInt32(_Status));
                }
                var _Heading1 = string.Concat(_Title, " (", _StatusTitle, ")");
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

            ErrorMessages.Add(MessageClass.SetMessage("No Record / File found to generate Report........", ConsoleColor.Yellow));
            return Page();
        }

        #endregion

        #region Employees Ledger

        public IActionResult OnGetGLEmployee(ReportType _ReportType)
        {
            #region Report Filter Variables
            ReportFilters Filters = new()
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                N_Employee = (int)AppRegistry.GetKey(UserName, "GL_Employee", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };
            #endregion

            var _Table = DataTableClass.GetTable(UserName, SQLQuery.Ledger(Filters.FilterText()+" ORDER BY Customer, COA, Vou_Date"));

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

        #region Trial Balance
        public IActionResult OnGetTBPrint(string _ReportLink, Boolean _IsShowPdf)
        {
            ReportLink = _ReportLink;
            IsShowPdf = _IsShowPdf;
            return Page();
        }

        public async Task<IActionResult> OnGetOBTBAsync()
        {
            var OBDate = AppRegistry.GetDate(UserName, "OBDate");
            var DateFormat = AppRegistry.FormatDate;
            TrialBalanceClass TB = new(User);
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
            var _SalesReportName = AppRegistry.GetText(UserName, "SalesReportRDL");

            if(_SalesReportName.Length==0) { _SalesReportName = "SalesInvoiceST"; }

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
            #region Check Error
            if (_ReportType.ToString().Length == 0)
            {
                ErrorMessages.Add(SetMessage("Report Type not defined", ConsoleColor.Red));
            }
            #endregion

            #region Get Data Table
            var _SheetNo = AppRegistry.GetText(UserName, "Sheet_No");

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

    }
}
