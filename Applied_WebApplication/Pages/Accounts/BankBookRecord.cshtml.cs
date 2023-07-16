using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.AppFunctions;
using static Applied_WebApplication.Data.MessageClass;
using static Applied_WebApplication.Pages.Accounts.BankBookModel;


namespace Applied_WebApplication.Pages.Accounts
{
    public class BankBookRecordModel : PageModel
    {
        [BindProperty]

        public BookRecord BookRecord { get; set; }
        public bool IsAdd = true;
        public bool IsError = false;
        public List<Message> ErrorMessages = new();
        public string UserName => User.Identity.Name;
        public int BookID { get; set; }
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
                Row["Vou_No"] = GetNewBankVoucher(UserName);
                Row["Vou_Date"] = DateTime.Now;
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
            Table.Save();

            if (Table.IsError)
            {
                IsError = Table.IsError;
                ErrorMessages = Table.TableValidation.MyMessages;
                return Page();
            }
            else
            {
                var _MaxDate = AppRegistry.GetDate(UserName, "BankBookTo");

                if (_MaxDate < BookRecord.Vou_Date)                      // Save the Voucher date for Bank Book List Page
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
            BookRecord.Vou_No = GetNewBankVoucher(UserName);
            return Page();
        }

    }
}
