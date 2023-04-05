using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static NPOI.HSSF.Util.HSSFColor;
using System.Text.RegularExpressions;

namespace Applied_WebApplication.Pages.Accounts
{
    public class JVListModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable JVList { get; set; }
        public string UserName => User.Identity.Name;

        public void OnGet()
        {
            Variables = new()
            {
                DateFrom = AppRegistry.GetDate(UserName, "JVList_From"),
                DateTo = AppRegistry.GetDate(UserName, "JVList_To")
            };

            var Date1 = Variables.DateFrom.ToString(AppRegistry.DateYMD);
            var Date2 = Variables.DateTo.ToString(AppRegistry.DateYMD);
            var Filter = string.Format("Vou_Date >='{0}' AND Vou_Date <='{1}'", Date1, Date2);

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
            return RedirectToPage("JV", routeValues: new { Vou_No });
        }

        public class MyParameters
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }

        }

    }
}
