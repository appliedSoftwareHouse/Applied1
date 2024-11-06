using AppReportClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Reporting.NETCore;
using System.Data;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.ReportPrint
{
    [Authorize]
    public class TrialBalanceModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public string ReportLink { get; set; }
        public bool IsShowPdf { get; set; } = false;

        public DataTable TB = new();                                                        // Trial Balance Code class
        public decimal Tot_DR { get; set; } = 0.00M;
        public decimal Tot_CR { get; set; } = 0.00M;
        public string UserName => User.Identity.Name;
        public List<Message> ErrorMessages { get; set; } = new();

        public void OnGet()
        {
            ErrorMessages = new();

            try
            {
                if (Variables == null)
                {
                    Variables = new()
                    {
                        DateFrom = AppRegistry.GetDate(UserName, "TBDate1"),
                        DateTo = AppRegistry.GetDate(UserName, "TBDate2"),
                        ReportOption = AppRegistry.GetText(UserName, "TBRptOption"),
                        Tot_DR = 0.00M,
                        Tot_CR = 0.00M
                    };
                }

                TrialBalanceClass TBal = new(User);
                TB = TBal.TB_Dates(Variables.DateFrom, Variables.DateTo);

                foreach (DataRow Row in TB.Rows)
                {
                    decimal _Amount = decimal.Parse(Row["DR"].ToString()) - decimal.Parse(Row["CR"].ToString());
                    if (_Amount >= 0) { Tot_DR += _Amount; }
                    if (_Amount < 0) { Tot_CR += Math.Abs(_Amount); }

                    Variables.Tot_DR = Tot_DR;
                    Variables.Tot_CR = Tot_CR;
                }
            }
            catch (Exception e)
            {
                ErrorMessages.Add(SetMessage(e.Message));
            }
        }

        public IActionResult OnPostPrint(ReportType _ReportType)
        {
            ErrorMessages = new();
            var _Date1 = AppRegistry.GetDate(UserName, "TBDate1");
            var _Date2 = AppRegistry.GetDate(UserName, "TBDate2");
            var __Date1 = _Date1.AddDays(-1).ToString(AppRegistry.DateYMD);
            var __Date2 = _Date2.AddDays(1).ToString(AppRegistry.DateYMD);
            var _Filter = string.Empty;
            var _OrderBy = "Code";

            if(Variables.ReportOption == TBOption.All.ToString()) { _Filter = string.Empty; }
            if(Variables.ReportOption == TBOption.UptoDate.ToString()) { _Filter = $"Date(Vou_Date) < '{__Date2}'"; }
            if(Variables.ReportOption == TBOption.Monthly.ToString()) { _Filter = $"Date(Vou_Date) > '{__Date1}' AND Date(Vou_Date) < '{__Date2}'"; }

            //_Filter += " ORDER BY Vou_Date,Vou_No";
            
            var _Table = DataTableClass.GetTable(UserName, SQLQuery.TrialBalance(_Filter, _OrderBy));

            if (_Table.Rows.Count>0)
            {
                var CompanyName = UserProfile.GetCompanyName(User);

                var _Heading1 = "TRIAL BALANCE";
                var _Heading2 = string.Empty;
                var _Footer = AppFunctions.AppGlobals.ReportFooter;

                if (this.Variables.ReportOption == TBOption.All.ToString()) { _Heading2 = $"Upto {DateTime.Now.ToString(AppRegistry.FormatDate)}"; }
                if (this.Variables.ReportOption == TBOption.UptoDate.ToString()) { _Heading2 = $"Upto {_Date2.ToString(AppRegistry.FormatDate)}"; }
                if (this.Variables.ReportOption == TBOption.Monthly.ToString()) { _Heading2 = $"From {_Date1.ToString(AppRegistry.FormatDate)} To {_Date2.ToString(AppRegistry.FormatDate)}"; }

               

                var Variables = new ReportParameters()
                {
                    ReportPath = AppFunctions.AppGlobals.ReportPath,
                    ReportFile = "TB.rdl",
                    OutputPath = AppFunctions.AppGlobals.PrintedReportPath,
                    OutputPathLink = AppFunctions.AppGlobals.PrintedReportPathLink,
                    OutputFile = "TB",
                    CompanyName = string.IsNullOrEmpty(CompanyName) ? ".." : CompanyName,
                    Heading1 = string.IsNullOrEmpty(_Heading1) ? ".." : _Heading1,
                    Heading2 = string.IsNullOrEmpty(_Heading2) ? ".." : _Heading2,
                    Footer = string.IsNullOrEmpty(_Footer) ? ".." : _Footer,
                    ReportType = _ReportType,
                    DataSetName = "dset_TB",
                    ReportData = _Table,
                    
                };

                List<ReportParameter> _Parameters = new List<ReportParameter>
                {
                    new ReportParameter("CompanyName", Variables.CompanyName),
                    new ReportParameter("Heading1", Variables.Heading1),
                    new ReportParameter("Heading2", Variables.Heading2),
                    new ReportParameter("Footer", AppFunctions.AppGlobals.ReportFooter)
                };

                Variables.DataParameters = _Parameters;
                var ReportClass = new ExportReport(Variables);
                ReportClass.Render();

                if (_ReportType == ReportType.Preview)
                {
                    ReportLink = ReportClass.Variables.GetFileLink();
                    IsShowPdf = true;
                    return RedirectToPage("PrintReport", "TBPrint", routeValues: new { _ReportLink=ReportLink, _IsShowPdf=IsShowPdf });
                }
                else
                {
                    
                    return File(ReportClass.Variables.FileBytes, ReportClass.Variables.MimeType, ReportClass.Variables.OutputFileFullName);
                }
            }
            else
            {
                ErrorMessages.Add(SetMessage("No Record found...", ConsoleColor.Yellow));
                return Page();
            }
        }

        public IActionResult OnPostTBOpening()
        {
            var OBDate = AppRegistry.GetDate(UserName, "OBDate");
            SetKeys();
            return RedirectToPage();
        }
        public IActionResult OnPostTBALL()
        {
            DataTableClass Ledger = new(UserName, Tables.Ledger);

            var MinDate = Ledger.MyDataTable.Compute("MIN(Vou_Date)", "");
            var MaxDate = Ledger.MyDataTable.Compute("MAX(Vou_Date)", "");
            SetKeys();
            return RedirectToPage();
        }
        public IActionResult OnPostReload()
        {
            SetKeys();
            return RedirectToPage();
        }

        private void SetKeys()
        {
            AppRegistry.SetKey(UserName, "TBDate1", Variables.DateFrom, KeyType.Date);
            AppRegistry.SetKey(UserName, "TBDate2", Variables.DateTo, KeyType.Date);
            AppRegistry.SetKey(UserName, "TBRptOption", Variables.ReportOption.ToString(), KeyType.Text);

        }

        public class MyParameters
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public string ReportType { get; set; } = "Preview";
            public string ReportOption { get; set; }
            public decimal Tot_DR { get; set; }
            public decimal Tot_CR { get; set; }

        }
    }
}
