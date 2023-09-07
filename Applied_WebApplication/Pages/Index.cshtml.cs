using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ExcelClass;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public string MyMessage = string.Empty;
        public List<Message> ErrorMessages { get; set; }
        public string UserName => User.Identity.Name;
        public bool IsDBCreate  {get; set;} = true;

        public void OnGet()
        {
            if (IsDBCreate)
            {
                CreateTablesClass CreateDataTables = new(User);
                CreateDataTables.CreateTables();
                ErrorMessages = CreateDataTables.MyMessages;
            }

        }



        public IActionResult OnPostTest()
        {
            return RedirectToPage("/ReportPrint/PrintReport", "Ledger", routeValues: new { COAID = 2 });
        }

        public IActionResult OnPostCashBook()
        {
            var id = AppRegistry.GetNumber(UserName, "CashBookID");
            return RedirectToPage("/Accounts/Cashbook", routeValues: new { id });
        }

        public IActionResult OnPostBankBook()
        {
            var id = AppRegistry.GetNumber(UserName, "BankBookID");
            return RedirectToPage("/Accounts/Bankbook", routeValues: new { id });
        }


        public IActionResult OnPostWCheque()
        {
            return RedirectToPage("/Accounts/WriteCheque", routeValues: new { ChqCode = "" });
        }

        public IActionResult OnPostPosting()
        {
            return RedirectToPage("/Applied/Posting");
        }

        public IActionResult OnPostVouchers()
        {
            return RedirectToPage("/Accounts/Vouchers");
        }


        #region Excel Class
        //public void OnPostExcel()
        //{
        //    ExcelClass.AppExcel excel = new();

        //    MyMessage = excel.CreateExcel();
        //}
        #endregion
    }
}