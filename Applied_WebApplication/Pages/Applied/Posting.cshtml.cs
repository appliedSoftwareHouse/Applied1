using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
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
        public string UserName => User.Identity.Name;

        public void OnGet()
        {
            
            Variables = new()
            {
                PostingType = (int)AppRegistry.GetKey(UserName, "Post_Type", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "Post_dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "Post_dt_To", KeyType.Date)
            };

            string Filter;
            var Date1 = Variables.Dt_From.ToString(AppRegistry.DateYMD);
            var Date2 = Variables.Dt_To.ToString(AppRegistry.DateYMD);

            switch (Variables.PostingType)
            {
                case 1:                                                                                                                                 // Cash Book
                    Filter = string.Concat("Vou_Date>='", Date1, "' AND Vou_Date<='", Date2, "' ");
                    DataTableClass CashBook = new(UserName, Tables.PostCashBook, Filter);
                    //CashBook.MyDataView.RowFilter = Filter;
                    PostTable = CashBook.MyDataTable;

                    break;
                case 2:                                                                                                                                 // Bank Book

                    break;
                case 3:                                                                                                                                 // Write Cheques
                    PostTable = GetTable(UserName, Tables.PostWriteCheque);
                    break;

                case 4:                                                                                                                                 // 
                    Filter = string.Format("Vou_Date >='{0}' AND Vou_Date <='{1}' AND Status='{2}'", Date1, Date2, VoucherStatus.Submitted.ToString());
                    PostTable = AppFunctions.GetRecords(UserName, Tables.PostBillPayable, Filter);
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
            if (PostingType == (int)PostType.CashBook) { PostingClass.PostCashBook(UserName, id); }
            if (PostingType == (int)PostType.BillPayable) { ErrorMessages = PostingClass.PostBillPayable(UserName, id); }
            
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
