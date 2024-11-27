using AppReportClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text;

namespace Applied_WebApplication.Pages.Accounts
{
    [Authorize]
    public class ReceiptListModel : PageModel
    {
        [BindProperty]
        public ReceiptsFilter Variables { get; set; }
        public string UserName => User.Identity.Name;
        public DataTable ReceiptList { get; set; }


        public void OnGet()
        {
            GetKeys();
            var _Query = SQLQuery.ReceiptsList(GetFiler());
            ReceiptList = DataTableClass.GetTable(UserName, _Query, "Vou_Date");
            

        }

        private string GetFiler()
        {
            var _Text = new StringBuilder();

            var _Date1 = Variables.From.ToString(AppRegistry.DateYMD);
            var _Date2 = Variables.To.ToString(AppRegistry.DateYMD);
            var _CloseBracket = false;
            
            _Text.Append($"Date([Vou_Date]) BETWEEN Date('{_Date1}') AND Date('{_Date2}') ");
            if(Variables.Search.Length > 0)
            {
                _Text.Append($"AND ( AccountTitle like '%{Variables.Search}%' ");
                _CloseBracket = true;
            }
            if (Variables.Payer > 0)
            {
                _Text.Append($"OR PayerTitle like '%{Variables.Payer}%' ");
            }
            if (Variables.Employee > 0)
            {
                _Text.Append($"OR EmployeeTitle like '%{Variables.Employee}%' ");
            }
            if (Variables.Employee > 0)
            {
                _Text.Append($"OR ProjectTitle like '%{Variables.Project}%' ");
            }
            if(_CloseBracket) { _Text.Append(" )"); }

            return _Text.ToString();

        }

        public IActionResult OnPostRefresh()
        {
            SetKeys();
            return RedirectToPage();
        }

        public IActionResult OnPostNew()
        {
            return RedirectToPage("./Receipt");
        }

        public IActionResult OnPostEdit(int ID)
        {
            return RedirectToPage("Receipt", routeValues: new { ID });
        }

        public IActionResult OnPostDelete(int ID)
        {
            return Page();
        }

        public IActionResult OnPostPrint(int ID, string Vou_No)
        {
            var _ReportType = ReportType.Preview;
            var _ShowImages = true;

            AppRegistry.SetKey(UserName, "rcptID", ID, KeyType.Text);
            AppRegistry.SetKey(UserName, "rcptHead2", Vou_No, KeyType.Text);
            AppRegistry.SetKey(UserName, "rcptShowImg", !_ShowImages, KeyType.Boolean);

            return RedirectToPage("../ReportPrint/PrintReport", "Receipt", new { RptType = _ReportType });
        }

        public IActionResult OnPostUnPost(int ID, string Vou_No)
        {
            return Page();
        }


        #region Set / Get Keys
        public void SetKeys()
        {
            AppRegistry.SetKey(UserName, "rcptFrom", Variables.From, KeyType.Date);
            AppRegistry.SetKey(UserName, "rcptTo", Variables.To, KeyType.Date);
            AppRegistry.SetKey(UserName, "rcptSearch", Variables.Search, KeyType.Text);
            
        }

        public void GetKeys()
        {
            Variables = new()
            {
                From = AppRegistry.GetDate(UserName, "rcptFrom"),
                To = AppRegistry.GetDate(UserName, "rcptTo"),
                Search = AppRegistry.GetText(UserName, "rcptSearch")
            };

        }
        #endregion

    }

    public class ReceiptsFilter
    {
        
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Search { get; set; }
        public int Payer { get; set; }
        public int Account { get; set; }
        public int Project { get; set; }
        public int Employee { get; set; }
        
        
    }
}
