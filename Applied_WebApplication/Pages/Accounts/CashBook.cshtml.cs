using AppReportClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Drawing;
using static Applied_WebApplication.Data.AppRegistry;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Accounts
{
    [Authorize]
    public class CashBookModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public BookRecord MyRecord { get; set; }
        public DataTable Cashbook = new DataTable();

        public List<Message> ErrorMessages = new();
        public string FMTNumber { get; set; }
        public string FMTCurrency  { get; set; }
        public string FMTDate { get; set; }
        public string UserName => User.Identity.Name;

        #region Get / POST

        public void OnGet(int? id)
        {
            Variables = new()
            {
                IsSelected = false,
                CashBookID = GetNumber(UserName, "CashBookID"),
                MinDate = (DateTime)GetKey(UserName, "CashBookFrom", KeyType.Date),
                MaxDate = (DateTime)GetKey(UserName, "CashBookTo", KeyType.Date),
                IsPosted1 = (int)GetKey(UserName, "CashBookPost", KeyType.Number),
            };
            
            id ??= Variables.CashBookID;
            if (id == 0) { id = GetNumber(UserName, "CashBookID"); }

            var _Date1 = AppFunctions.MinDate(Variables.MinDate, Variables.MaxDate).ToString(DateYMD);
            var _Date2 = AppFunctions.MaxDate(Variables.MinDate, Variables.MaxDate).ToString(DateYMD);

            var _Filter = $"COA={id}";
            var _Dates = new string[] { _Date1, _Date2 };
            var _Book = "Cash";

            Cashbook = DataTableClass.GetTable(UserName, SQLQuery.BookLedger(_Filter, _Dates, _Book));

            if (Variables.IsPosted1 == 1)                        // Not posted.
            {
                DataView BookView = Cashbook.AsDataView();
                BookView.RowFilter = "Status='Posted'";
                Cashbook = BookView.ToTable();
            }

            if (Variables.IsPosted1 == 2)                        // Not posted.
            {
                DataView BookView = Cashbook.AsDataView();
                BookView.RowFilter = "Status<>'Posted'";
                Cashbook = BookView.ToTable();
            }

            if (Cashbook.Rows.Count == 0)
            {
                ErrorMessages.Add(SetMessage("No record Found.....!"));
            }

            FMTNumber = GetText(UserName, "FMT");
        }
    
        #endregion

        public IActionResult OnPostEdit(int ID)
        {
            return RedirectToPage("CashBookRecord", routeValues: new { ID, BookID = Variables.CashBookID });
        }

        public IActionResult OnPostRefresh(int id)
        {

            SetKey(UserName, "CashBookFrom", Variables.MinDate, KeyType.Date);
            SetKey(UserName, "CashBookTo", Variables.MaxDate, KeyType.Date);
            SetKey(UserName, "CashBookPost", Variables.IsPosted1, KeyType.Number);
            SetKey(UserName, "CashBookID", Variables.CashBookID, KeyType.Number);


            return RedirectToPage("CashBook", routeValues: new { id = Variables.CashBookID });
        }
        public IActionResult OnPostAdd()
        {
            return RedirectToPage("CashBookRecord", new { ID = 0, _BookID = Variables.CashBookID });
        }
        public IActionResult OnPostSave(int ID)
        {
            DataTableClass CashBook = new(UserName, Tables.CashBook);
            CashBook.MyDataView.RowFilter = $"ID={ID}";
            if (CashBook.MyDataView.Count == 0) { CashBook.NewRecord(); }                  // Create a new record.
            else { CashBook.SeekRecord(ID); }                                                                   // Select a existing record.

            CashBook.CurrentRow["ID"] = MyRecord.ID;
            CashBook.CurrentRow["Vou_No"] = MyRecord.Vou_No;
            CashBook.CurrentRow["Vou_Date"] = MyRecord.Vou_Date;
            CashBook.CurrentRow["Ref_No"] = MyRecord.Ref_No;
            CashBook.CurrentRow["Sheet_No"] = MyRecord.Sheet_No;
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
                Variables.IsSelected = true;
                return RedirectToPage();
            }
            else
            {
                Variables.IsSelected = false;
                Variables.IsAdd = true;
                Variables.IsError = true;
                Variables.Reload = true;
                return Page();
            }
        }
        public IActionResult OnPostDelete(int ID)
        {
            ErrorMessages = new();
            DataTableClass _Table = new(UserName, Tables.CashBook);
            _Table.MyDataView.RowFilter = string.Format("ID={0}", ID);
            if (_Table.Count == 1)
            {
                try
                {
                    _Table.SeekRecord(ID);
                    _Table.Delete();
                    ErrorMessages.Add(MessageClass.SetMessage("Record has been deleted.", Color.Red));
                }
                catch (Exception e)
                {
                    ErrorMessages.Add(MessageClass.SetMessage(e.Message, Color.Red));
                }
            }
            return RedirectToPage("CashBook");
        }
        public IActionResult OnPostPrint(string Vou_No)
        {
            SetKey(UserName, "cbVouNo", Vou_No, KeyType.Text);
            return RedirectToPage("../ReportPrint/PrintReport", "Voucher", new { _ReportType = ReportType.Preview });
        }
        public IActionResult OnPostShow(int ID)
        {
            return Page();
        }
        public static string GetSelectPosted(int _Posted)
        {
            if (_Posted == 1) { return "All"; }
            if (_Posted == 2) { return "Posted"; }
            if (_Posted == 3) { return "Submitted"; }
            return "";
        }

        #region Variables

        [BindProperties]
        public class MyParameters
        {
            public int ID { get; set; }
            public bool IsSelected { get; set; }
            public bool IsAdd { get; set; }
            public bool IsError { get; set; }
            public bool Reload { get; set; }
            public int CashBookID { get; set; }
            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }
            public int IsPosted1 { get; set; }

        }
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
            public string Sheet_No { get; set; }
            public decimal DR { get; set; }
            public decimal CR { get; set; }
            public int Customer { get; set; }
            public int Project { get; set; }
            public int Employee { get; set; }
            public string Description { get; set; }
            public string Comments { get; set; }

        }
        #endregion
    }
}
