using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.DataTableClass;


namespace Applied_WebApplication.Pages.Applied
{
    public class PostingModel : PageModel
    {
        [BindProperty]
        public PostingData My { get; set; }
        public DataTable PostTable;

        public void OnGet()
        {
            var UserName = User.Identity.Name;
            My = new()
            {
                PostingType = (int)AppRegistry.GetKey(UserName, "Post_Type", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "Post_dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "Post_dt_To", KeyType.Date)
            };
        }

        public void OnPostRefresh()
        {
            string UserName = User.Identity.Name;
            AppRegistry.SetKey(UserName, "Post_Type", My.PostingType, KeyType.Number);
            AppRegistry.SetKey(UserName, "Post_dt_From", My.Dt_From, KeyType.Date);
            AppRegistry.SetKey(UserName, "Post_dt_To", My.Dt_To, KeyType.Date);

            string Date1, Date2, Filter;

            switch (My.PostingType)
            {
                case 1:                                                                                                                                 // Cash Book
                    DataTableClass CashBook = new(UserName, Tables.PostCashBook);
                    Date1 = My.Dt_From.ToString(AppRegistry.DateYMD);
                    Date2 = My.Dt_To.ToString(AppRegistry.DateYMD);
                    Filter = string.Concat("Vou_Date>='", Date1, "' AND Vou_Date<='", Date2, "' ");
                    CashBook.MyDataView.RowFilter = Filter;
                    PostTable = CashBook.MyDataView.ToTable();

                    break;
                case 2:                                                                                                                                 // Bank Book

                    break;
                case 3:                                                                                                                                 // Write Cheques
                    PostTable = GetDataView(UserName, Tables.PostWriteCheque);
                    break;

                case 4:                                                                                                                                 // 
                    Date1 = My.Dt_From.ToString(AppRegistry.DateYMD);
                    Date2 = My.Dt_To.ToString(AppRegistry.DateYMD);
                    Filter = string.Concat("(Vou_Date>='", Date1, "' AND Vou_Date<='", Date2, "') AND Status='",VoucherStatus.Submitted.ToString() ,"' ");
                    PostTable = AppFunctions.GetRecords(UserName, Tables.PostBillPayable, Filter);
                    break;

                default:
                    break;
            }
        }

        public IActionResult OnPostPosting(int id, int PostingType)
        {
            var UserName = User.Identity.Name;

            if (PostingType == (int)PostType.Bankbook) { PostingClass.PostCashBook(UserName, id);}
            if (PostingType == (int)PostType.BillPayable) { PostingClass.PostBillPayable(UserName, id); }

            return Page();
        }

        public class PostingData
        {
            public int PostingType { get; set; }
            public DateTime Dt_From { get; set; }
            public DateTime Dt_To { get; set; }
        }

    }
}
