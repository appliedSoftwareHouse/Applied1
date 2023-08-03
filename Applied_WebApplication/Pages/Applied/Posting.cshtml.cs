using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using NPOI.SS.Formula.Functions;
using System.Data;
using static Applied_WebApplication.Data.DataTableClass;
using static Applied_WebApplication.Data.MessageClass;
using static NPOI.HSSF.Util.HSSFColor;

namespace Applied_WebApplication.Pages.Applied
{
    public class PostingModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable PostTable;
        public bool IsError { get; set; }
        public List<Message> ErrorMessages { get; set; } = new();
        public string UserName => User.Identity.Name;
        private readonly string Submitted = VoucherStatus.Submitted.ToString();

        public void OnGet()
        {
            Variables = new()
            {
                PostingType = AppRegistry.GetNumber(UserName, "Post_Type"),
                Dt_From = AppRegistry.GetDate(UserName, "Post_dt_From"),
                Dt_To = AppRegistry.GetDate(UserName, "Post_dt_To")
            };

            string Filter;
            var Date1 = Variables.Dt_From.AddDays(-1).ToString(AppRegistry.DateYMD);
            var Date2 = Variables.Dt_To.AddDays(1).ToString(AppRegistry.DateYMD);


            switch (Variables.PostingType)
            {
                case 1:                                                                                                                                 // Cash Book
                    Filter = $"Vou_Date>'{Date1}' AND Vou_Date<'{Date2}'";
                    DataTableClass CashBook = new(UserName, Tables.PostCashBook, Filter);
                    PostTable = CashBook.MyDataTable;

                    break;
                case 2:                                                                                                                                 // Bank Book
                    Filter = $"Vou_Date>'{Date1}' AND Vou_Date<'{Date2}'";
                    PostTable = GetTable(UserName, SQLQuery.PostBook(Tables.BankBook, Filter, Submitted));
                    break;

                case 3:                                                                                                                                 // Write Cheques
                    Filter = $"Vou_Date>='{Date1}' AND Vou_Date<='{Date2}' AND Status='Submitted'";
                    var Query = SQLQuery.WriteCheque(Filter);

                    DataTableClass WriteCheque = new(UserName, Query);
                    PostTable = WriteCheque.MyDataTable;

                    break;

                case 4:     //   Bill Payable - Payment of purchases.
                    Filter = $"Date(Vou_Date) >=Date('{Date1}') AND Date(Vou_Date) <=Date('{Date2}') AND [Status]='{Submitted}'";
                    PostTable = GetTable(UserName, Tables.PostBillPayable, Filter);
                    break;

                case 5:    // Bill Receivable - Sales Invoice
                    Filter = $"Date(Vou_Date) >=Date('{Date1}') AND Date(Vou_Date) <=Date('{Date2}') AND [B1].[Status]='{Submitted}'";
                    PostTable = GetTable(UserName, SQLQuery.PostBillReceivable(Filter), "Vou_Date");

                    break;
                case 6:
                    break;

                case 7:
                    break;
                case 8:
                    break;
                case 9:             // Sales Return Transactions.
                    Filter = $"Date([BR].[Vou_Date]) > Date('{Date1}') AND Date([BR].[Vou_Date]) < Date('{Date2}') AND [ST].[Status]='{Submitted}'";
                    PostTable = GetTable(UserName, SQLQuery.PostSaleReturnList(Filter));


                    break;
                case 10:
                    break;


                default:
                    break;
            }
        }

        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "Post_Type", Variables.PostingType, KeyType.Number);
            AppRegistry.SetKey(UserName, "Post_dt_From", Variables.Dt_From, KeyType.Date);
            AppRegistry.SetKey(UserName, "Post_dt_To", Variables.Dt_To, KeyType.Date);

            return RedirectToPage();
        }

        public IActionResult OnPostPosting(int id, int PostingType)
        {
            Variables = new()
            {
                PostingType = AppRegistry.GetNumber(UserName, "Post_Type"),
                Dt_From = AppRegistry.GetDate(UserName, "Post_dt_From"),
                Dt_To = AppRegistry.GetDate(UserName, "Post_dt_To"),
            };

            if (PostingType == (int)PostType.CashBook) { ErrorMessages = PostingClass.PostCashBook(UserName, id); }
            if (PostingType == (int)PostType.BankBook) { ErrorMessages = PostingClass.PostBankBook(UserName, id); }
            if (PostingType == (int)PostType.BillPayable) { ErrorMessages = PostingClass.PostBillPayable(UserName, id); }
            if (PostingType == (int)PostType.BillReceivable) { ErrorMessages = PostingClass.PostBillReceivable(UserName, id); }
            if (PostingType == (int)PostType.SaleReturn) { ErrorMessages = PostingClass.PostSaleReturn(UserName, id); }

            if (ErrorMessages.Count > 0)
            {
                return Page();
            }

            return RedirectToPage();
        }

        public class MyParameters
        {
            public int PostingType { get; set; }
            public DateTime Dt_From { get; set; }
            public DateTime Dt_To { get; set; }
        }

    }
}
