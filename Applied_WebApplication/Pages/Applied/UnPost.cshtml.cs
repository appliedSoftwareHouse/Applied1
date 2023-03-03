using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.DataTableClass;


namespace Applied_WebApplication.Pages.Applied
{
    public class UnPostModel : PageModel
    {

        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable UnpostTable;
        public string UserName => User.Identity.Name;
        public bool IsError { get; set; }
        public List<Message> ErrorMessages { get; set; } = new();

        public void OnGet()
        {
            
            Variables = new()
            {
                PostingType = (int)AppRegistry.GetKey(UserName, "Unpost_Type", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "Unpost_dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "Unpost_dt_To", KeyType.Date)
            };

            string Date1, Date2, Filter;

            switch (Variables.PostingType)
            {
                case 1:                                                                                                                                 // Cash Book
                    DataTableClass CashBook = new(UserName, Tables.UnpostCashBook);
                    Date1 = Variables.Dt_From.ToString(AppRegistry.DateYMD);
                    Date2 = Variables.Dt_To.ToString(AppRegistry.DateYMD);
                    Filter = string.Format("Vou_Date>='{0}' AND Vou_Date<='{1}' ", Date1, Date2);
                    CashBook.MyDataView.RowFilter = Filter;
                    UnpostTable = CashBook.MyDataView.ToTable();

                    break;
                case 2:                                                                                                                                 // Bank Book

                    break;
                case 3:                                                                                                                                 // Write Cheques
                    UnpostTable = GetDataView(UserName, Tables.PostWriteCheque);
                    break;

                case 4:                                                                                                                                 // 
                    Date1 = Variables.Dt_From.ToString(AppRegistry.DateYMD);
                    Date2 = Variables.Dt_To.ToString(AppRegistry.DateYMD);
                    Filter = string.Format("Vou_Date>='{0}' AND Vou_Date <='{1}' AND Status='{2}'", Date1, Date2, VoucherStatus.Posted.ToString());
                    //Filter = string.Concat("(Vou_Date>='", Date1, "' AND Vou_Date<='", Date2, "') AND Status='", VoucherStatus.Submitted.ToString(), "' ");
                    UnpostTable = AppFunctions.GetRecords(UserName, Tables.UnpostBillPayable, Filter);
                    break;

                default:
                    break;
            }
        }

        public IActionResult OnPostUnPost(int id, int PostingType)
        {
           
            if (PostingType == (int)PostType.CashBook) { IsError = UnpostClass.Unpost_CashBook(UserName, id); }
            if (PostingType == (int)PostType.BillPayable) {  IsError = UnpostClass.UnpostBillPayable(UserName, id); }
            if (ErrorMessages.Count == 0) { IsError = false; } else { IsError = true; return Page(); }
            return RedirectToPage("UnPost", "Refresh");
        }




        public IActionResult OnPostRefresh()
        {
            string UserName = User.Identity.Name;

            if (Variables.PostingType == 0) { Variables.PostingType = int.Parse(Request.Form["PostingType"]); }

            AppRegistry.SetKey(UserName, "Unpost_Type", Variables.PostingType, KeyType.Number);
            AppRegistry.SetKey(UserName, "Unpost_dt_From", Variables.Dt_From, KeyType.Date);
            AppRegistry.SetKey(UserName, "Unpost_dt_To", Variables.Dt_To, KeyType.Date);

            string Date1, Date2, Filter;

            switch (Variables.PostingType)
            {
                case 1:                                                                                                                                 // Cash Book
                    DataTableClass CashBook = new(UserName, Tables.UnpostCashBook);
                    Date1 = Variables.Dt_From.ToString(AppRegistry.DateYMD);
                    Date2 = Variables.Dt_To.ToString(AppRegistry.DateYMD);
                    Filter = string.Concat("Vou_Date>='", Date1, "' AND Vou_Date<='", Date2, "' ");
                    CashBook.MyDataView.RowFilter = Filter;
                    UnpostTable = CashBook.MyDataView.ToTable();

                    break;
                case 2:                                                                                                                                 // Bank Book

                    break;
                case 3:                                                                                                                                 // Write Cheques
                    UnpostTable = GetDataView(UserName, Tables.PostWriteCheque);
                    break;

                case 4:                                                                                                                                 // 
                    Date1 = Variables.Dt_From.ToString(AppRegistry.DateYMD);
                    Date2 = Variables.Dt_To.ToString(AppRegistry.DateYMD);
                    Filter = string.Concat("(Vou_Date>='", Date1, "' AND Vou_Date<='", Date2, "') AND Status='", VoucherStatus.Posted.ToString(), "' ");
                    UnpostTable = AppFunctions.GetRecords(UserName, Tables.UnpostBillPayable, Filter);
                    break;

                default:
                    break;
            }
            return Page();
        }

        #region Variables
        public class MyParameters
        {
            public int PostingType { get; set; }
            public DateTime Dt_From { get; set; } = DateTime.Now;
            public DateTime Dt_To { get; set; } = DateTime.Now;
        }
        #endregion
    }
}
