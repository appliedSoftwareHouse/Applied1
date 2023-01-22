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
                PostingType = (int)Registry.GetKey(UserName, "Post_Type", KeyType.Number),
                Dt_From = (DateTime)Registry.GetKey(UserName, "Post_dt_From", KeyType.Date),
                Dt_To = (DateTime)Registry.GetKey(UserName, "Post_dt_To", KeyType.Date)
            };
        }

        public void OnPostRefresh()
        {
            string UserName = User.Identity.Name;
            Registry.SetKey(UserName, "Post_Type", My.PostingType, KeyType.Number);
            Registry.SetKey(UserName, "Post_dt_From", My.Dt_From, KeyType.Date);
            Registry.SetKey(UserName, "Post_dt_To", My.Dt_To, KeyType.Date);

            switch (My.PostingType)
            {
                case 1:                                                                                                                                 // Cash Book
                    DataTableClass CashBook = new(UserName, Tables.PostCashBook);
                    string Date1 = My.Dt_From.ToString(Registry.DateYMD);
                    string Date2 = My.Dt_To.ToString(Registry.DateYMD);
                    CashBook.MyDataView.RowFilter = string.Concat("Vou_Date>='", Date1, "' AND Vou_Date<='", Date2, "' ");
                    PostTable = CashBook.MyDataView.ToTable();
                    
                        break;
                case 2:
                    PostTable = GetDataView(UserName, Tables.PostBankBook);
                    break;
                case 3:
                    PostTable = GetDataView(UserName, Tables.PostWriteCheque);
                    break;

                default:
                    break;
            }
        }

        public IActionResult OnPostPosting(int id, int PostingType)
        {
            var UserName = User.Identity.Name;
            
            if(PostingType == (int)PostType.Bankbook)                                                   // Cash Book Posting
            {
                PostingClass.PostCashBook(UserName, id);
                
            }
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
