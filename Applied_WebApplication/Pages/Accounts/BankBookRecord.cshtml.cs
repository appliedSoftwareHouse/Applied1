using AppReportClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Pages.Accounts.BankBookModel;


namespace Applied_WebApplication.Pages.Accounts
{
    [Authorize]
    public class BankBookRecordModel : PageModel
    {
        [BindProperty]
        public BookRecord BookRecord { get; set; }
        public bool IsAdd = true;
        public bool IsError = false;
        public List<Message> ErrorMessages = new();
        public string UserName => User.Identity.Name;
        public int BookID { get; set; }
        public string MyVouTag = "BB";
        public void OnGet(int ID, int _BookID)
        {
            BookID = _BookID;
            DataTableClass _Table = new(UserName, Tables.BankBook);
            DataRow Row = _Table.NewRecord();
            BookRecord = new();

            if (ID < 1)
            {
                Row = _Table.CurrentRow;
                if (BookRecord == null) { BookRecord = new(); }
                //Row["Vou_No"] = GetNewVouNo(DateTime.Now);
                Row["Vou_No"] = "New";
                Row["Vou_Date"] = AppRegistry.GetDate(UserName, "bbook-dt");
                Row["BookID"] = BookID;
            }

            if (ID > 0)
            {
                _Table.SeekRecord(ID);
                Row = _Table.CurrentRow;
                if (BookRecord == null) { BookRecord = new(); }
                if (Row["Vou_Date"] == DBNull.Value) { Row["Vou_Date"] = DateTime.Now; }
            }

            IsAdd = false;
            BookRecord.BookID = (int)Row["BookID"];
            BookRecord.ID = (int)Row["ID"];
            BookRecord.Vou_Date = DateTime.Parse(Row["Vou_Date"].ToString());
            BookRecord.Vou_No = (string)Row["Vou_No"];
            BookRecord.COA = (int)Row["COA"];
            BookRecord.Ref_No = Row["Ref_No"].ToString();
            BookRecord.Sheet_No = Row["Sheet_No"].ToString();
            BookRecord.DR = (decimal)Row["DR"];
            BookRecord.CR = (decimal)Row["CR"];
            BookRecord.Customer = (int)Row["Customer"];
            BookRecord.Description = Row["Description"].ToString();
            BookRecord.Comments = Row["Comments"].ToString();
            BookRecord.Project = (int)Row["Project"];
            BookRecord.Employee = (int)Row["Employee"];
        }

        public IActionResult OnPostSave()
        {
            DataTableClass Table = new(UserName, Tables.BankBook);
            DataRow Row = Table.NewRecord();

            Row["ID"] = BookRecord.ID;
            Row["BookID"] = BookRecord.BookID;
            Row["Vou_Date"] = BookRecord.Vou_Date;
            Row["Vou_No"] = BookRecord.Vou_No;
            Row["COA"] = BookRecord.COA;
            Row["Ref_No"] = BookRecord.Ref_No;
            Row["Sheet_No"] = BookRecord.Sheet_No;
            Row["DR"] = BookRecord.DR;
            Row["CR"] = BookRecord.CR;
            Row["Customer"] = BookRecord.Customer;
            Row["Description"] = BookRecord.Description;
            Row["Comments"] = BookRecord.Comments;
            Row["Project"] = BookRecord.Project;
            Row["Employee"] = BookRecord.Employee;
            Row["Status"] = VoucherStatus.Submitted;
            // Generate a new voucher number
            if (BookRecord.Vou_No.ToUpper()=="NEW") { Row["Vou_No"] = NewVoucher.GetNewVoucher(Row.Table, "BB", BookRecord.Vou_Date); ; }  

            Table.Save();

            AppRegistry.SetKey(UserName, "bbook-dt", BookRecord.Vou_Date, KeyType.Date);

            if (Table.IsError)
            {
                IsError = Table.IsError;
                ErrorMessages = Table.TableValidation.MyMessages;
                return Page();
            }
            else
            {
                var _MaxDate = AppRegistry.GetDate(UserName, "BankBookTo");

                if (_MaxDate < BookRecord.Vou_Date)            // Save the Voucher date for Bank Book List Page
                {
                    AppRegistry.SetKey(UserName, "BankBookTo", BookRecord.Vou_Date, KeyType.Date);
                }

                return RedirectToPage("BankBook", routeValues: new { id = BookID });
            }
        }

        public IActionResult OnPostBack()
        {
            return RedirectToPage("BankBook", routeValues: new { id = BookID });
        }

        public IActionResult OnPostAutoVoucher()
        {
            var _Table = DataTableClass.GetTable(UserName, Tables.BankBook);
            BookRecord.Vou_No = NewVoucher.GetNewVoucher(_Table, "BB", DateTime.Now); ;
            return Page();
        }

        public IActionResult OnPostPrint()
        {
            if (BookRecord.Vou_No.ToUpper() == "NEW") { return Page(); }

            var _Booktitle = AppFunctions.GetTitle(UserName, Tables.COA, BookID);
            AppRegistry.SetKey(UserName, "cbbVouID", BookRecord.ID, KeyType.Number);
            AppRegistry.SetKey(UserName, "cbbHeading1", "Bank Book", KeyType.Text);
            AppRegistry.SetKey(UserName, "cbbHeading2", _Booktitle, KeyType.Text);
            AppRegistry.SetKey(UserName, "cbbBook", "Bank", KeyType.Text);

            var Option = ReportType.Preview;
            return RedirectToPage("./ReportPrint/PrintReport", "CashBankBook", new { _ReportType = Option });
        }

    }
}
