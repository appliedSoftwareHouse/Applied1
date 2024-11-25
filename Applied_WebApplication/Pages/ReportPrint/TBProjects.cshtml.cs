using AppReportClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;


namespace Applied_WebApplication.Pages.ReportPrint

{
    [Authorize]
    public class TBProjectsModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public string ReportLink { get; set; }
        public bool IsShowPdf { get; set; } = false;

        public DataTable TBProjects = new();
        public DataTable DataProject = new();
        public DataTable CboxProject { get; set; }
        public DataTable CboxAccounts { get; set; }
        public decimal Tot_DR { get; set; } = 0.00M;
        public decimal Tot_CR { get; set; } = 0.00M;
        public string UserName => User.Identity.Name;
        public List<Message> ErrorMessages { get; set; }

        public void OnGet()
        {
            Variables = new()
            {
                DateTo = AppRegistry.GetDate(UserName, "tbPrj_Date"),
                CboxProjectId = AppRegistry.GetNumber(UserName, "tbPrj_Project"),
                CboxAccountsId = AppRegistry.GetNumber(UserName, "tbPrj_Accounts")
            };

            int CashBookNature = AppRegistry.GetNumber(UserName, "CashBkNature");
            int BankBookNature = AppRegistry.GetNumber(UserName, "BankBkNature");
            string _Nature = $"{CashBookNature},{BankBookNature}";

            var _Date = Variables.DateTo.ToString(AppRegistry.DateYMD);
            var _Filter = $"[C].[Nature] NOT IN({_Nature}) ";
            _Filter += $"AND Date([Vou_Date]) <= Date('{_Date}') ";
            if (Variables.CboxProjectId == 0) { _Filter += "AND [L].[Project] > 0 "; }
            if (Variables.CboxProjectId > 0) { _Filter += $"AND [Project] = {Variables.CboxProjectId} "; }
            if (Variables.CboxAccountsId > 0) { _Filter += $"AND [COA] = {Variables.CboxAccountsId} "; }

            var _Query = SQLQuery.TBProject(_Filter, "[Project], [COA]");
            TBProjects = DataTableClass.GetTable(UserName, _Query);

            CboxAccounts = GetCboxAccounts();
            CboxProject = GetCBoxProject();

            var Stop = true;

        }

        #region Get List of Project and Accounts for ComboBox
        private DataTable GetCBoxProject()
        {
            return DataTableClass.GetTable(UserName, Tables.Project);
        }

        private DataTable GetCboxAccounts()
        {
            return DataTableClass.GetTable(UserName, Tables.COA);
        }
        #endregion

        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "tbPrj_Date", Variables.DateTo, KeyType.Date);
            AppRegistry.SetKey(UserName, "tbPrj_Project", Variables.CboxProjectId, KeyType.Number);
            AppRegistry.SetKey(UserName, "tbPrj_Accounts", Variables.CboxAccountsId, KeyType.Number);
            return RedirectToPage();
        }

        public IActionResult OnPostReSet()
        {
            AppRegistry.SetKey(UserName, "tbPrj_Date", DateTime.Now, KeyType.Date);
            AppRegistry.SetKey(UserName, "tbPrj_Project", 0, KeyType.Number);
            AppRegistry.SetKey(UserName, "tbPrj_Accounts", 0, KeyType.Number);
            return RedirectToPage();
        }

        public IActionResult OnPostPrint(ReportType Option)
        {
            AppRegistry.SetKey(UserName, "tbpFrom", Variables.DateFrom, KeyType.Date);
            AppRegistry.SetKey(UserName, "tbpTo", Variables.DateTo, KeyType.Date);
            AppRegistry.SetKey(UserName, "tbpProject", Variables.CboxProjectId, KeyType.Number);
            AppRegistry.SetKey(UserName, "tbpAccount", Variables.CboxAccountsId, KeyType.Number);
            AppRegistry.SetKey(UserName, "tbpFormat", Variables.ReportFormat, KeyType.Number);

            return RedirectToPage("../ReportPrint/PrintReport", "ProjectTB", new { RptType = Option });
        }




        public void ReportStyle()
        {
            //if(_Style == 1) { Variables.ReportStyle = 1; }
            //if(_Style == 2) { Variables.ReportStyle = 2; }
            //if(_Style == 3) { Variables.ReportStyle = 3; }


        }
    }

    #region Variables
    public class MyParameters
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string ReportType { get; set; }
        public int ReportFormat { get; set; }
        public string ReportOption { get; set; }
        public decimal Tot_DR { get; set; }
        public decimal Tot_CR { get; set; }
        public int CboxProjectId { get; set; }
        public int CboxAccountsId { get; set; }
    }
    #endregion

}
