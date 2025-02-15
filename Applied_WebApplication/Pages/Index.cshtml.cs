using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public string MyMessage = string.Empty;
        public List<Message> ErrorMessages { get; set; }
        public string UserName => User.Identity.Name;
        public bool IsDBCreate { get; set; } = true;

        public void OnGet()
        {
            if (IsDBCreate)                                                                             // Create Data Tables and Views if they not existed.
            {
                CreateTablesClass CreateDataTables = new(User);
                CreateDataTables.CreateTables();
                ErrorMessages = CreateDataTables.MyMessages;
            }
        }

        public IActionResult OnPostTest() => RedirectToPage("/ReportPrint/PrintReport", "Ledger", routeValues: new { COAID = 2 });
        public IActionResult OnPostCashBook() => RedirectToPage("/Accounts/Cashbook", routeValues: new { id = AppRegistry.GetNumber(UserName, "CashBookID") });
        public IActionResult OnPostBankBook() => RedirectToPage("/Accounts/Bankbook", routeValues: new { id = AppRegistry.GetNumber(UserName, "BankBookID") });
        public IActionResult OnPostWCheque() => RedirectToPage("/Accounts/WriteCheque", routeValues: new { ChqCode = "" });
        public IActionResult OnPostInvoice() => RedirectToPage("/Sales/SaleInvoice2");
        public IActionResult OnPostPosting() => RedirectToPage("/Applied/Posting");
        public IActionResult OnPostVouchers() => RedirectToPage("/Accounts/Vouchers");
        public IActionResult OnPostReceipt() => RedirectToPage("/Accounts/Receipt", routeValues: new { ID = 0 });



    }
}