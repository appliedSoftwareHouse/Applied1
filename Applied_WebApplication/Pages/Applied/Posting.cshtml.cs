using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Applied_WebApplication.Data;
using static Applied_WebApplication.Data.DataTableClass;
using System.ComponentModel.Design;

namespace Applied_WebApplication.Pages.Applied
{
    public class PostingModel : PageModel
    {
        [BindProperty]
        public Variables Me { get; set; }
        public DataTable PostTable;

        


        public void OnGet()
        {
            var UserName = User.Identity.Name;
            Me = new();
            Me.PostingType = (int)Registry.GetKey(UserName, "Post_Type", KeyType.Number);
            Me.Dt_From = (DateTime)Registry.GetKey(UserName, "Post_dt_From", KeyType.Date);
            Me.Dt_To = (DateTime)Registry.GetKey(UserName, "Post_dt_To", KeyType.Date);
        }

        public void OnPostRefresh()
        {
            string UserName = User.Identity.Name;
            switch (Me.PostingType)
            {
                case 1:                                                                                                                                 // Cash Book
                    
                    PostTable = GetDataView(UserName,Tables.PostCashBook);
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

        public void OnPostPosting(int id, int PostingType)
        {
            var UserName = User.Identity.Name;
            Registry.SetKey(UserName, "Post_Type", Me.PostingType, KeyType.Number);
            Registry.SetKey(UserName, "Post_dt_From", Me.PostingType, KeyType.Date);
            Registry.SetKey(UserName, "Post_dt_To", Me.PostingType, KeyType.Date);

            if(PostingType == (int)PostType.Bankbook)                                                   // Cash Book Posting
            {
                PostingClass.PostBashBook(UserName, id);
            }
            
        }




        public class Variables
        {
            public int PostingType { get; set; }
            public DateTime Dt_From { get; set; }
            public DateTime Dt_To { get; set; }
        }

    }
}
