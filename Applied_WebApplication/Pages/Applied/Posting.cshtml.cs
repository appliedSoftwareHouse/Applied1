using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.DataTableClass;


namespace Applied_WebApplication.Pages.Applied
{
    public class PostingModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable PostTable;
        public bool IsError { get; set; }
        public List<Message> ErrorMessages { get; set; } = new();

        public void OnGet()
        {
            string UserName = User.Identity.Name;
            Variables = new();
            Variables.PostingType = (int)AppRegistry.GetKey(UserName, "Posting_Type", KeyType.Number);
            Variables.Dt_From = (DateTime)AppRegistry.GetKey(UserName, "Post_dt_From", KeyType.Date);
            Variables.Dt_To = (DateTime)AppRegistry.GetKey(UserName, "Post_dt_To", KeyType.Date);

            string Date1, Date2, Filter;

            switch (Variables.PostingType)
            {
                case 1:                                                                                                                                 // Cash Book
                    DataTableClass CashBook = new(UserName, Tables.PostCashBook);
                    Date1 = Variables.Dt_From.ToString(AppRegistry.DateYMD);
                    Date2 = Variables.Dt_To.ToString(AppRegistry.DateYMD);
                    Filter = string.Concat("Vou_Date>='", Date1, "' AND Vou_Date<='", Date2, "' ");
                    CashBook.MyDataView.RowFilter = Filter;
                    PostTable = CashBook.MyDataView.ToTable();

                    break;
                case 2:                                                                                                                                 // Bank Book

                    break;
                case 3:                                                                                                                                 // Write Cheques
                    PostTable = GetTable(UserName, Tables.PostWriteCheque);
                    break;

                case 4:                                                                                                                                 // 
                    Date1 = Variables.Dt_From.ToString(AppRegistry.DateYMD);
                    Date2 = Variables.Dt_To.ToString(AppRegistry.DateYMD);
                    Filter = string.Concat("(Vou_Date>='", Date1, "' AND Vou_Date<='", Date2, "') AND Status='", VoucherStatus.Submitted.ToString(), "' ");
                    PostTable = AppFunctions.GetRecords(UserName, Tables.PostBillPayable, Filter);
                    break;

                default:
                    break;
            }
        }

        public IActionResult OnPostRefresh()
        {
            string UserName = User.Identity.Name;

            if (Variables.PostingType == 0) { Variables.PostingType = int.Parse(Request.Form["PostingType"]); }

            AppRegistry.SetKey(UserName, "Post_Type", Variables.PostingType, KeyType.Number);
            AppRegistry.SetKey(UserName, "Post_dt_From", Variables.Dt_From, KeyType.Date);
            AppRegistry.SetKey(UserName, "Post_dt_To", Variables.Dt_To, KeyType.Date);

            string Date1, Date2, Filter;

            switch (Variables.PostingType)
            {
                case 1:                                                                                                                                 // Cash Book
                    DataTableClass CashBook = new(UserName, Tables.PostCashBook);
                    Date1 = Variables.Dt_From.ToString(AppRegistry.DateYMD);
                    Date2 = Variables.Dt_To.ToString(AppRegistry.DateYMD);
                    Filter = string.Concat("Vou_Date>='", Date1, "' AND Vou_Date<='", Date2, "' ");
                    CashBook.MyDataView.RowFilter = Filter;
                    PostTable = CashBook.MyDataView.ToTable();

                    break;
                case 2:                                                                                                                                 // Bank Book

                    break;
                case 3:                                                                                                                                 // Write Cheques
                    PostTable = GetTable(UserName, Tables.PostWriteCheque);
                    break;

                case 4:                                                                                                                                 // 
                    Date1 = Variables.Dt_From.ToString(AppRegistry.DateYMD);
                    Date2 = Variables.Dt_To.ToString(AppRegistry.DateYMD);
                    Filter = string.Concat("(Vou_Date>='", Date1, "' AND Vou_Date<='", Date2, "') AND Status='", VoucherStatus.Submitted.ToString(), "' ");
                    PostTable = AppFunctions.GetRecords(UserName, Tables.PostBillPayable, Filter);
                    break;

                default:
                    break;
            }
            return Page();
        }

        public IActionResult OnPostPosting(int id, int PostingType)
        {
            var UserName = User.Identity.Name;
            if (PostingType == (int)PostType.CashBook) { PostingClass.PostCashBook(UserName, id); }
            if (PostingType == (int)PostType.BillPayable) { ErrorMessages = PostingClass.PostBillPayable(UserName, id); }
            if (ErrorMessages.Count == 0) { IsError = false; } else { IsError = true; return Page(); }
            return RedirectToPage("Posting", "Refresh");
        }

        public class MyParameters
        {
            public int PostingType { get; set; }
            public DateTime Dt_From { get; set; } = DateTime.Now;
            public DateTime Dt_To { get; set; } = DateTime.Now;
        }

    }
}
