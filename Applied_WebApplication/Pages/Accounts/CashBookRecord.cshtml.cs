using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.AppFunctions;
using static Applied_WebApplication.Pages.Accounts.CashBookModel;

namespace Applied_WebApplication.Pages.Accounts
{
    public class CashBookRecordModel : PageModel
    {
        [BindProperty]
        public BookRecord BookRecord { get; set; }
        public bool IsAdd = true;
        public bool IsError = false;
        public List<Message> ErrorMessages = new();

        public void OnGet(int id)
        {
            string UserName = User.Identity.Name;
            DataTableClass Table = new(UserName, Tables.CashBook);
            DataRow Row = Table.NewRecord();
            if (BookRecord == null) { BookRecord = new(); }
            if (Row["Vou_Date"] == DBNull.Value) { Row["Vou_Date"] = DateTime.Now; }

            IsAdd = false;
            BookRecord.BookID = id;
            BookRecord.ID = (int)Row["ID"];
            BookRecord.Vou_Date = DateTime.Parse(Row["Vou_Date"].ToString());
            BookRecord.Vou_No = GetNewCashVoucher(UserName);
            BookRecord.COA = (int)Row["COA"];
            BookRecord.Ref_No = Row["Ref_No"].ToString();
            BookRecord.DR = (decimal)Row["DR"];
            BookRecord.CR = (decimal)Row["CR"];
            BookRecord.Customer = (int)Row["Customer"];
            BookRecord.Description = Row["Description"].ToString();
            BookRecord.Comments = Row["Comments"].ToString();
            BookRecord.Project = (int)Row["Project"];
            BookRecord.Employee = (int)Row["Employee"];
        }

        public void OnGetEdit(int ID)
        {
            string UserName = User.Identity.Name;
            DataTableClass Table = new(UserName, Tables.CashBook);
            DataRow Row = Table.NewRecord();
            if (BookRecord == null) { BookRecord = new(); }

            if (ID > 0) { Row = Table.SeekRecord(ID); }

            IsAdd = false;
            BookRecord.ID = (int)Row["ID"];
            BookRecord.BookID = (int)Row["BookID"];
            BookRecord.Vou_Date = DateTime.Parse(Row["Vou_Date"].ToString());
            BookRecord.Vou_No = Row["Vou_No"].ToString();
            BookRecord.COA = (int)Row["COA"];
            BookRecord.Ref_No = Row["Ref_No"].ToString();
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
            string UserName = User.Identity.Name;
            DataTableClass Table = new(UserName, Tables.CashBook);
            DataRow Row = Table.NewRecord();

            Row["ID"] = BookRecord.ID;
            Row["BookID"] = BookRecord.BookID;
            Row["Vou_Date"] = BookRecord.Vou_Date;
            Row["Vou_No"] = BookRecord.Vou_No;
            Row["COA"] = BookRecord.COA;
            Row["Ref_No"] = BookRecord.Ref_No;
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
                return RedirectToPage("CashBook", "Refresh", new { id = BookRecord.BookID });
            }
        }

        public IActionResult OnPostBack()
        {
            return RedirectToPage("CashBook", "Refresh", new { BookRecord.BookID });
        }

        public IActionResult OnPostAutoVoucher()
        {
            string UserName = HttpContext.User.Identity.Name;
            BookRecord.Vou_No = GetNewCashVoucher(UserName);
            return Page();
        }


    }
}
