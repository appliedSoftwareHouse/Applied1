﻿using Microsoft.AspNetCore.Authorization;
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

        public IActionResult OnPostTest() => RedirectToPage("/Sales/SaleInvoice2", routeValues: new { ID = 4081, SRNO = 1 });
        public IActionResult OnPostCashBook() => RedirectToPage("/Accounts/Cashbook", routeValues: new { id = AppRegistry.GetNumber(UserName, "CashBookID") });
        public IActionResult OnPostBankBook() => RedirectToPage("/Accounts/Bankbook", routeValues: new { id = AppRegistry.GetNumber(UserName, "BankBookID") });
        public IActionResult OnPostWCheque() => RedirectToPage("/Accounts/WriteCheque", routeValues: new { ChqCode = "" });
        public IActionResult OnPostInvoice() => RedirectToPage("/Sales/SaleInvoice2");
        public IActionResult OnPostPosting() => RedirectToPage("/Applied/Posting");
        public IActionResult OnPostVouchers() => RedirectToPage("/Accounts/Vouchers");
        public IActionResult OnPostReceipt() => RedirectToPage("/Accounts/Receipt", routeValues: new { ID = 0 });
        public IActionResult OnPostPayable() => RedirectToPage("/Accounts/BillPayable");
        public IActionResult OnPostReceivable() => RedirectToPage("/Accounts/BillReceivable");
        public IActionResult OnPostLedgers() => RedirectToPage("/ReportPrint/ReportPrint/GeneralLedgers");
        public IActionResult OnPostJV() => RedirectToPage("/Accounts/JV");
        public IActionResult OnPostStock() => RedirectToPage("/Stock/StockPosition");



    }
}