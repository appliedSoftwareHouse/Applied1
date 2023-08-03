using AppReportClass;
using AppReporting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace Applied_WebApplication.Data
{
    public class TrialBalanceClass
    {
        #region Setup
        public DataTable MyDataTable { get; set; }
        public DateTime TB_From { get; set; }
        public DateTime TB_To { get; set; }
        public DateTime OB_Date { get; set; }
        public string UserName { get; set; }
        public string ReportPath => AppFunctions.AppGlobals.ReportPath;
        public UserProfile uProfile { get; set; }
        public string CompanyName { get; set; }
        public ClaimsPrincipal UserPrincipal { get; set; }
        public string Heading1 { get; set; }
        public string Heading2 { get; set; }
        public string MyMessage { get; set; }
        public ReportClass MyReportClass { get; set; }

        #endregion

        public TrialBalanceClass(ClaimsPrincipal _UserClaims)
        {
            UserPrincipal = _UserClaims;
            UserName = UserPrincipal.Identity.Name;
            uProfile = new(UserName);
            MyDataTable = new DataTable();
            TB_From = AppRegistry.GetFiscalFrom();
            TB_To = AppRegistry.GetFiscalTo();
            CompanyName = uProfile.Company;
            Heading1 = "Trial Balance";
            Heading2 = "---";
            //SetParameters();
        }


        public void SetParameters()
        {
            MyReportClass = new ReportClass
            {
                AppUser = UserPrincipal,
                ReportFilePath = ReportPath,
                ReportFile = "TB.rdl",
                ReportDataSet = "dset_TB",
                ReportSourceData = MyDataTable,
                RecordSort = "Code",

                OutputFilePath = AppFunctions.AppGlobals.PrintedReportPath,
                OutputFile = "TB",
                OutputFileLinkPath = AppFunctions.AppGlobals.PrintedReportPathLink

            };

            List<ReportParameter> _Parameters = new List<ReportParameter>
            {
                new ReportParameter("CompanyName", CompanyName),
                new ReportParameter("Heading1", Heading1),
                new ReportParameter("Heading2", Heading2),
                new ReportParameter("Footer", AppFunctions.AppGlobals.ReportFooter)
            };

        }

        public DataTable TBOB_Data()
        {
            DataTable _Table;
            DateTime OBalDate = AppRegistry.GetDate(UserName, "OBDate");
            var _Date = OBalDate.ToString(AppRegistry.DateYMD);
            var _Filter = $"Date([Ledger].[Vou_Date]) = Date('{_Date}')";
            var _OrderBy = "Code";
            _Table = DataTableClass.GetTable(UserName, SQLQuery.TrialBalance(_Filter, _OrderBy ));
            SetParameters();
            MyReportClass.ReportSourceData = _Table;
            return _Table;
        }

        public DataTable TB_Dates(DateTime Date1, DateTime Date2)
        {
            DataTable _Table;
            
            var _Start = Date1.ToString(AppRegistry.DateYMD);
            var _End = Date2.ToString(AppRegistry.DateYMD);
            var _Filter = $"Date([Ledger].[Vou_Date]) >= Date('{_Start}') AND Date([Ledger].[Vou_Date]) <= Date('{_End}') ";
            var _OrderBy = "Code";
            _Table = DataTableClass.GetTable(UserName, SQLQuery.TrialBalance(_Filter,_OrderBy));
            SetParameters();
            MyReportClass.ReportSourceData = _Table;
            return _Table;
        }

        public DataTable TB_All()
        {
            DataTable _Table;
            SetParameters();
            var _Filter = String.Empty;
            var _OrderBy = "Code";
            _Table = DataTableClass.GetTable(UserName, SQLQuery.TrialBalance(_Filter, _OrderBy));
            return _Table;
        }

        public  void Download(ReportType _ReportType)
        {
            SetParameters();

            List<ReportParameter> _Parameters = new List<ReportParameter>
            {
                new ReportParameter("CompanyName", CompanyName),
                new ReportParameter("Heading1", Heading1),
                new ReportParameter("Heading2", Heading2),
                new ReportParameter("Footer", AppFunctions.AppGlobals.ReportFooter)
            };

            var _ReportFile = string.Concat(MyReportClass.ReportFilePath, MyReportClass.ReportFile);
            ReportDataSource _DataSource = new(MyReportClass.ReportDataSet, MyReportClass.ReportSourceData);
            LocalReport report = new();
            var _ReportStream = new StreamReader(_ReportFile);
            report.LoadReportDefinition(_ReportStream);
            report.DataSources.Add(_DataSource);
            report.SetParameters(_Parameters);

            var _RenderFormat = ExportReport.GetRenderFormat(_ReportType);
            var _RenderedReport = report.Render(_RenderFormat);
            var _mimeType = ExportReport.GetReportMime(_ReportType);
            var _Extention = "." + ExportReport.GetReportExtention(_ReportType);

            //return File(_RenderedReport, _mimeType, MyReportClass.OutputFile + _Extention);
        }

    }
}
