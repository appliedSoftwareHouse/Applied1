using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NPOI.SS.Formula.Functions;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class TrialBalanceModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable TB = new();                                                        // Trial Balance Code class
        public decimal Tot_DR { get; set; } = 0.00M;
        public decimal Tot_CR { get; set; } = 0.00M;
        public string UserName => User.Identity.Name;


        public void OnGet()
        {
            
            string UserName = User.Identity.Name;

            if (Variables == null)
            {
                Variables = new()
                {
                    DateFrom = AppRegistry.GetDate(UserName, "TBDate1"),
                    DateTo = AppRegistry.GetDate(UserName, "TBDate2"),
                    ReportType = "ALL",
                    Tot_DR = 0.00M,
                    Tot_CR = 0.00M
                };
            }

            TrialBalance TBal = new(User);
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


        public IActionResult OnPostOBTBAsync()
        {
            var OBDate = AppRegistry.GetDate(UserName, "OBDate");
            AppRegistry.SetKey(UserName, "TBDate1", OBDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "TBDate2", OBDate, KeyType.Date);
            return RedirectToPage();

            //return RedirectToPage("./PrintReport", pageHandler: "OBTB");
        }

        public IActionResult OnPostTBALLAsync()
        {
            DataTableClass Ledger = new(UserName, Tables.Ledger);

            var MinDate = Ledger.MyDataTable.Compute("MIN(Vou_Date)", "");
            var MaxDate = Ledger.MyDataTable.Compute("MAX(Vou_Date)", "");
            AppRegistry.SetKey(UserName, "TBDate1", MinDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "TBDate2", MaxDate, KeyType.Date);


            return RedirectToPage();

            //return RedirectToPage("./PrintReport", pageHandler: "OBTB");
        }
        public IActionResult OnPostTBPrint()
        {
            var Date1 = Variables.DateFrom;
            var Date2 = Variables.DateTo;
            
            return RedirectToPage("./PrintReport", pageHandler: "TBPrint", routeValues: new {Date1, Date2});                  // PrintReport Folder is not include in Project.
        }


        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "TBDate1", Variables.DateFrom, KeyType.Date);
            AppRegistry.SetKey(UserName, "TBDate2", Variables.DateTo, KeyType.Date);
            return RedirectToPage();
        }
        public class MyParameters
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public string ReportType { get; set; }
            public decimal Tot_DR { get; set; }
            public decimal Tot_CR { get; set; }

        }


    }
}
