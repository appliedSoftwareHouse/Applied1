using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace Applied_WebApplication.Pages.Accounts
{
    [Authorize]
    public class JVListModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable JVList { get; set; }
        public string UserName => User.Identity.Name;
        public int Sr_No { get; set; }
        public bool Refresh { get; set; }

        public void OnGet()
        {
            Variables = new()
            {
                DateFrom = AppRegistry.GetDate(UserName, "JVList_From"),
                DateTo = AppRegistry.GetDate(UserName, "JVList_To")
            };

            var Date1 = Variables.DateFrom.AddDays(-1).ToString(AppRegistry.DateYMD);
            var Date2 = Variables.DateTo.AddDays(1).ToString(AppRegistry.DateYMD);
            var Filter = $"Date(Vou_Date) > Date('{Date1}') AND Date(Vou_Date) < Date('{Date2}')";

            DataTableClass tb_Class = new(UserName, Tables.JVList, Filter);
            tb_Class.MyDataView.Sort = "Vou_Date, Vou_No";
            JVList = tb_Class.MyDataView.ToTable();
        }

        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "JVList_From", Variables.DateFrom, KeyType.Date);
            AppRegistry.SetKey(UserName, "JVList_To", Variables.DateTo, KeyType.Date);
            return RedirectToPage("JVList");
        }

        public IActionResult OnPostJV(string Vou_No)
        {
            Sr_No = -1; Refresh = true;

            return RedirectToPage("JV", routeValues: new { Vou_No, Sr_No, Refresh });
        }

        public IActionResult OnPostJV2(string Vou_No)
        {
            AppRegistry.SetKey(UserName, "JV2VouNo", Vou_No, KeyType.Text);
            return RedirectToPage("JV2");
        }

        public IActionResult OnPostNew()
        {
            return RedirectToPage("JV", "New");
        }


        public class MyParameters
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }

        }

    }
}
