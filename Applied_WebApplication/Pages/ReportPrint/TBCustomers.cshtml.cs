using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Linq;
using System.Text;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class TBCustomersModel : PageModel
    {

        [BindProperty]
        public MyParameters Variables { get; set; }
        public string ReportLink { get; set; }
        public bool IsShowPdf { get; set; } = false;

        public DataTable TBCustomer = new();
        public DataTable DataCustomer = new();
        public DataTable CboxCusotmer { get; set; }
        public DataTable CboxAccounts { get; set; }
        public decimal Tot_DR { get; set; } = 0.00M;
        public decimal Tot_CR { get; set; } = 0.00M;
        public string UserName => User.Identity.Name;
        public List<Message> ErrorMessages { get; set; }

        

        public void OnGet()
        {
            Variables = new()
            {
                DateTo = AppRegistry.GetDate(UserName, "tbCus_Date"),
                CboxCustomerId = AppRegistry.GetNumber(UserName, "tbCus_Customer"),
                CboxAccountsId = AppRegistry.GetNumber(UserName, "tbCus_Accounts")
            };
                        
            var _Date = Variables.DateTo.ToString(AppRegistry.DateYMD);
            var _Filter = $"Date(Vou_Date) <= Date('{_Date}')";
            DataCustomer = DataTableClass.GetTable(UserName, SQLQuery.TBCustomer(UserName, _Filter));
            var _View = DataCustomer.AsDataView();
            #region Filter
            var _Text = new StringBuilder();
            if (Variables.CboxAccountsId > 0)
            {
                if (_Text.ToString().Length > 0) { _Text.Append(" AND "); }
                _Text.Append($"COA = {Variables.CboxAccountsId}");
            }

            if (Variables.CboxCustomerId > 0)
            {
                if (_Text.ToString().Length > 0) { _Text.Append(" AND "); }
                _Text.Append($"Customer = {Variables.CboxCustomerId}");
            }
            _View.RowFilter = _Text.ToString();
            #endregion

            TBCustomer = _View.ToTable();
            var Customers = GetcBoxTable();
            var Accounts = GetcBoxTable();

            foreach(DataRow Row in DataCustomer.Rows)
            {
                if(Customers.Rows.Contains(Row["Customer"]))
                {
                    continue;
                }
                var newRow = Customers.NewRow();
                newRow["ID"] = Row["Customer"];
                newRow["Title"] = Row["CustomerName"];
                Customers.Rows.Add(newRow);
            }

            foreach (DataRow Row in DataCustomer.Rows)
            {
                if (Accounts.Rows.Contains(Row["COA"]))
                {
                    continue;
                }
                var newRow = Accounts.NewRow();
                newRow["ID"] = Row["COA"];
                newRow["Title"] = Row["Account"];
                Accounts.Rows.Add(newRow);
            }

            CboxCusotmer = Customers;
            CboxAccounts = Accounts;
        }

        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "tbCus_Date", Variables.DateTo, KeyType.Date);
            AppRegistry.SetKey(UserName, "tbCus_Customer", Variables.CboxCustomerId, KeyType.Number);
            AppRegistry.SetKey(UserName, "tbCus_Accounts", Variables.CboxAccountsId, KeyType.Number);
            return RedirectToPage();
        }

        public IActionResult OnPostReSet()
        {
            AppRegistry.SetKey(UserName, "tbCus_Date", DateTime.Now, KeyType.Date);
            AppRegistry.SetKey(UserName, "tbCus_Customer", 0, KeyType.Number);
            AppRegistry.SetKey(UserName, "tbCus_Accounts", 0, KeyType.Number);
            return RedirectToPage();
        }

        private DataTable GetcBoxTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Title", typeof(string));
            dt.PrimaryKey = new[] { dt.Columns[0] };
            return dt;
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
            public int CboxCustomerId {  get; set; }
            public int CboxAccountsId {  get; set; }
        }
        #endregion
    }
}
