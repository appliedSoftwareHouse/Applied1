using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.AppFunctions;

namespace Applied_WebApplication.Pages.Accounts
{
    public class CashBookModel : PageModel
    {
        [BindProperty]
        public ReportParameters MyParameters { get; set; }
        public string UserName;
        public BookRecord MyRecord { get; set; }
        public DataTable Cashbook = new DataTable();
        public List<Message> ErrorMessages;

        #region Get / POST

        public void OnGet(int id)
        {
            UserName = User.Identity.Name;
            MyParameters = new ReportParameters
            {
                IsSelected = false,
                CashBookID = id,
                BookTitle = GetColumnValue(UserName, Tables.COA, "Title", id),
                MinDate = (DateTime.Now),
                MaxDate = (DateTime.Now)
            };

            DataTableClass _Table = new(UserName, Tables.CashBook);                                                                     // Create Temporary for Min and Max Date.
            if (_Table.MyDataTable.Rows.Count > 0)
            {
                MyParameters.MinDate = (DateTime)_Table.MyDataTable.Compute("Min(Vou_Date)", "");
                MyParameters.MaxDate = (DateTime)_Table.MyDataTable.Compute("Max(Vou_Date)", "");
            }

            if (id > 0) { MyParameters.IsSelected = true; }
        }
        public void OnPost()
        {

        }

        #endregion

        public void OnGetRefresh(int id)
        {
            UserName = User.Identity.Name;
            MyParameters = new ReportParameters
            {
                IsSelected = true,
                CashBookID = id,
                BookTitle = GetColumnValue(UserName, Tables.COA, "Title", id),
                MinDate = (DateTime.Now),
                MaxDate = (DateTime.Now)
            };

            DataTableClass _Table = new(UserName, Tables.CashBook);                                                                     // Create Temporary for Min and Max Date.
            if (_Table.MyDataTable.Rows.Count > 0)
            {
                MyParameters.MinDate = (DateTime)_Table.MyDataTable.Compute("Min(Vou_Date)", "");
                MyParameters.MaxDate = (DateTime)_Table.MyDataTable.Compute("Max(Vou_Date)", "");
            }
        }
        public IActionResult OnPostRefresh()
        {
            UserName = User.Identity.Name;
            MyParameters.IsSelected = true;
            MyParameters.Reload = true;
            MyParameters.BookTitle = GetColumnValue(UserName, Tables.COA, "Title", MyParameters.CashBookID);
            return Page();
        }
        public IActionResult OnPostAdd()
        {
            return RedirectToPage("CashBookRecord", new { id = MyParameters.CashBookID });
        }
        public IActionResult OnPostSave(int ID)
        {
            UserName = User.Identity.Name;

            DataTableClass CashBook = new(UserName, Tables.CashBook);
            CashBook.MyDataView.RowFilter = String.Concat("ID=", ID.ToString());
            if (CashBook.MyDataView.Count == 0) { CashBook.NewRecord(); }                  // Create a new record.
            else { CashBook.SeekRecord(ID); }                                                                   // Select a existing record.

            CashBook.CurrentRow["ID"] = MyRecord.ID;
            CashBook.CurrentRow["Vou_No"] = MyRecord.Vou_No;
            CashBook.CurrentRow["Vou_Date"] = MyRecord.Vou_Date;
            CashBook.CurrentRow["Ref_No"] = MyRecord.Ref_No;
            CashBook.CurrentRow["BookID"] = MyRecord.BookID;
            CashBook.CurrentRow["COA"] = MyRecord.COA;
            CashBook.CurrentRow["DR"] = MyRecord.DR;
            CashBook.CurrentRow["CR"] = MyRecord.CR;
            CashBook.CurrentRow["Description"] = MyRecord.Description;
            CashBook.CurrentRow["Comments"] = MyRecord.Comments;
            CashBook.CurrentRow["Customer"] = MyRecord.Customer;
            CashBook.CurrentRow["Project"] = MyRecord.Project;
            CashBook.CurrentRow["Employee"] = MyRecord.Employee;
            CashBook.Save();

            ErrorMessages = CashBook.TableValidation.MyMessages;
            if (ErrorMessages.Count == 0)
            {
                MyParameters.IsSelected = true;
                return Page();
            }
            else
            {
                MyParameters.IsSelected = false;
                MyParameters.IsAdd = true;
                MyParameters.IsError = true;
                MyParameters.Reload = true;
                return Page();
            }
        }


        [BindProperties]
        public class ReportParameters
        {
            public int RecordID { get; set; }
            public bool IsSelected { get; set; }
            public bool IsAdd { get; set; }
            public bool IsError { get; set; }
            public bool Reload { get; set; }
            public string BookTitle { get; set; }
            public int CashBookID { get; set; }
            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }

        }
        [BindProperties]
        public class BookRecord
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string Title { get; set; }
            public int BookID { get; set; }
            public string Vou_No { get; set; }
            public DateTime Vou_Date { get; set; } = DateTime.Now;
            public int COA { get; set; }
            public string Ref_No { get; set; }
            public decimal DR { get; set; }
            public decimal CR { get; set; }
            public int Customer { get; set; }
            public int Project { get; set; }
            public int Employee { get; set; }
            public string Description { get; set; }
            public string Comments { get; set; }

        }
    }
}
