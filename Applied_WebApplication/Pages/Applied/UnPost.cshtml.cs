using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.DataTableClass;
using static Applied_WebApplication.Data.MessageClass;

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
                PostingType = (int)AppRegistry.GetNumber(UserName, "Unpost_Type"),
                Dt_From = AppRegistry.GetDate(UserName, "Unpost_dt_From"),
                Dt_To = AppRegistry.GetDate(UserName, "Unpost_dt_To")
            };

            string Filter;
            var Date1 = Variables.Dt_From.AddDays(-1).ToString(AppRegistry.DateYMD);
            var Date2 = Variables.Dt_To.AddDays(1).ToString(AppRegistry.DateYMD);

            switch (Variables.PostingType)
            {
                case 1:                                                                                                                                 // Cash Book
                    Filter = $"Date(Vou_Date)>Date('{Date1}') AND Date(Vou_Date) < Date('{Date2}')";
                    UnpostTable = DataTableClass.GetTable(UserName, SQLQuery.PostBook(Tables.CashBook, Filter, VoucherStatus.Posted.ToString()));

                    break;
                case 2:                                                                                                                                 // Bank Book
                    Filter = $"Date(Vou_Date)>Date('{Date1}') AND Date(Vou_Date) < Date('{Date2}')";
                    UnpostTable = DataTableClass.GetTable(UserName, SQLQuery.PostBook(Tables.BankBook, Filter, VoucherStatus.Posted.ToString()));
                    break;
                case 3:                                                                                                                                 // Write Cheques
                    //UnpostTable = GetTable(UserName, Tables.PostWriteCheque);
                    Filter = $"Date(Vou_Date)>Date('{Date1}') AND Date(Vou_Date) < Date('{Date2}')";
                    Filter += $"";
                    UnpostTable = GetTable(UserName, SQLQuery.PostWriteCheques(Filter));
                    break;

                case 4:                                                                                                                                 // Bill Payable
                    Filter = $"Date([B].[Vou_Date])>Date('{Date1}') AND Date([B].[Vou_Date]) < Date('{Date2}') AND [B].[Status]='{VoucherStatus.Posted}'";
                    UnpostTable = DataTableClass.GetTable(UserName, SQLQuery.UnpostBillPayable(Filter));
                    break;

                case 5:                                                                                                                                 // Bill Receivable (Sales Invoices) 
                    Filter = $"Date([B].[Vou_Date])>Date('{Date1}') AND Date([B].[Vou_Date]) <Date('{Date2}') AND [B].[Status]='{VoucherStatus.Posted}'";
                    UnpostTable = DataTableClass.GetTable(UserName, SQLQuery.UnpostBillReceivable(Filter));
                    break;

                case 6:             // Payment
                    break;

                case 7:             // Receipt
                    break;

                case 8:             // JV
                    break;

                case 9:             // Sale Return
                    Filter = $"Date([SR].[Vou_Date])>Date('{Date1}') AND Date([SR].[Vou_Date]) <Date('{Date2}') AND [SR].[Status]='{VoucherStatus.Posted}'";
                    UnpostTable = GetTable(UserName, SQLQuery.PostSaleReturnList(Filter));
                    break;

                case 10:            // BOM
                    break;

                default:
                    break;
            }
        }

        public IActionResult OnPostUnPost(int id, int PostingType)
        {

            if (PostingType == (int)PostType.CashBook) { IsError = UnpostClass.Unpost_CashBook(UserName, id); }
            if (PostingType == (int)PostType.BankBook) { IsError = UnpostClass.Unpost_BankBook(UserName, id); }
            if (PostingType == (int)PostType.BillPayable) { IsError = UnpostClass.UnpostBillPayable(UserName, id); }
            if (PostingType == (int)PostType.BillReceivable) { IsError = UnpostClass.UnpostBillReceivable(UserName, id); }
            if (ErrorMessages.Count == 0) { IsError = false; } else { IsError = true; return Page(); }
            return RedirectToPage("UnPost", "Refresh");
        }

        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "Unpost_Type", Variables.PostingType, KeyType.Number);
            AppRegistry.SetKey(UserName, "Unpost_dt_From", Variables.Dt_From, KeyType.Date);
            AppRegistry.SetKey(UserName, "Unpost_dt_To", Variables.Dt_To, KeyType.Date);
            return RedirectToPage();
        }

        #region Refresh
        //public IActionResult OnPostRefresh()
        //{
        //    string UserName = User.Identity.Name;

        //    if (Variables.PostingType == 0) { Variables.PostingType = int.Parse(Request.Form["PostingType"]); }

        //    AppRegistry.SetKey(UserName, "Unpost_Type", Variables.PostingType, KeyType.Number);
        //    AppRegistry.SetKey(UserName, "Unpost_dt_From", Variables.Dt_From, KeyType.Date);
        //    AppRegistry.SetKey(UserName, "Unpost_dt_To", Variables.Dt_To, KeyType.Date);

        //    string Filter;
        //    var Date1 = Variables.Dt_From.AddDays(-1).ToString(AppRegistry.DateYMD);
        //    var Date2 = Variables.Dt_To.AddDays(1).ToString(AppRegistry.DateYMD);

        //    switch (Variables.PostingType)
        //    {
        //        case (int)PostType.CashBook:                                                                                                                                    // Cash Book
        //            Filter = $"Vou_Date>'{Date1}' AND Vou_Date<'{Date2}'";
        //            UnpostTable = DataTableClass.GetTable(UserName, SQLQuery.PostBook(Tables.CashBook, Filter, VoucherStatus.Posted.ToString()));
        //            break;

        //        case (int)PostType.BankBook:                                                                                                                         // Bank Book
        //            Filter = $"Vou_Date>'{Date1}' AND Vou_Date<'{Date2}'";
        //            UnpostTable = DataTableClass.GetTable(UserName, SQLQuery.PostBook(Tables.BankBook, Filter, VoucherStatus.Posted.ToString()));

        //            //DataTableClass BankBook = new(UserName, Tables.UnpostBankBook);
        //            //Date1 = Variables.Dt_From.ToString(AppRegistry.DateYMD);
        //            //Date2 = Variables.Dt_To.ToString(AppRegistry.DateYMD);
        //            //Filter = string.Concat("Vou_Date>='", Date1, "' AND Vou_Date<='", Date2, "' ");
        //            //CashBook.MyDataView.RowFilter = Filter;
        //            //UnpostTable = CashBook.MyDataView.ToTable();
        //            break;

        //        case 3:                                                                                                                                 // Write Cheques
        //            UnpostTable = GetTable(UserName, Tables.PostWriteCheque);
        //            break;

        //        case 4:                                                                                                                                 // 
        //            Date1 = Variables.Dt_From.ToString(AppRegistry.DateYMD);
        //            Date2 = Variables.Dt_To.ToString(AppRegistry.DateYMD);
        //            Filter = string.Concat("(Vou_Date>='", Date1, "' AND Vou_Date<='", Date2, "') AND Status='", VoucherStatus.Posted.ToString(), "' ");
        //            UnpostTable = DataTableClass.GetTable(UserName, Tables.UnpostBillPayable, Filter);
        //            break;

        //        default:
        //            break;
        //    }
        //    return Page();
        //}
        #endregion

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
