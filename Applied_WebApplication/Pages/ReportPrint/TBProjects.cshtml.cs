using Applied_WebApplication.Pages.Sales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text;

namespace Applied_WebApplication.Pages.ReportPrint
{
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

            var _Query = SQLQuery.TBProject(_Filter);
            TBProjects = DataTableClass.GetTable(UserName,_Query);

            CboxAccounts = GetCboxAccounts();
            CboxProject = GetCBoxProject();

            var Stop = true;

        }

        private DataTable GetCBoxProject()
        {
            return DataTableClass.GetTable(UserName, Tables.Project);

            //DataTable _Table = new();
            //_Table.Columns.Add("ID", typeof(int));
            //_Table.Columns.Add("Title", typeof(string));
            //_Table.PrimaryKey = new[] { _Table.Columns[0] };

            //foreach (DataRow Row in TBProjects.Rows)
            //{
            //    if (Row["Project"] is not null)
            //    {
            //        if (_Table.Rows.Contains(Row["Project"])) { continue; }

            //        var newRow = _Table.NewRow();
            //        newRow["ID"] = Row["Project"];
            //        newRow["Title"] = Row["ProjectTitle"];
            //        _Table.Rows.Add(newRow);
            //    }
            //}
            //return _Table;
        }

        private DataTable GetCboxAccounts()
        {
            return DataTableClass.GetTable(UserName, Tables.COA);

            //DataTable _Table = new();
            //_Table.Columns.Add("ID", typeof(int));
            //_Table.Columns.Add("Title", typeof(string));
            //_Table.PrimaryKey = new[] { _Table.Columns[0] };

            //foreach (DataRow Row in TBProjects.Rows)
            //{
            //    if (Row["COA"] is not null)
            //    {
            //        if (_Table.Rows.Contains(Row["COA"])) { continue; }

            //        var newRow = _Table.NewRow();
            //        newRow["ID"] = Row["COA"];
            //        newRow["Title"] = Row["COATitle"];
            //        _Table.Rows.Add(newRow);
            //    }
            //}
            //return _Table;

        }

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


        #region Variables
        public class MyParameters
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public string ReportType { get; set; }
            public string ReportOption { get; set; }
            public decimal Tot_DR { get; set; }
            public decimal Tot_CR { get; set; }
            public int CboxProjectId { get; set; }
            public int CboxAccountsId { get; set; }
        }
        #endregion
    }
}
