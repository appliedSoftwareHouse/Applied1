using AppReportClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Drawing;
using System.Net;
using static Applied_WebApplication.Data.AppRegistry;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Accounts
{
    [Authorize]
    public class BankBookModel : PageModel
    {

        [BindProperty]
        public MyParameters Variables { get; set; }
        public BookRecord MyRecord { get; set; }
        public DataTable Bankbook = new DataTable();
        public int BookNature { get; set; }
        public Dictionary<int, string> BookTitles { get; set; }
        public List<Message> ErrorMessages = new();
        public string FMTNumber { get; set; }
        public string FMTCurrency { get; set; }
        public string FMTDate { get; set; }
        public string UserName => User.Identity.Name;


        #region Get

        public void OnGet(int? id)
        {

            BookNature = GetNumber(UserName, "BankBkNature");
            BookTitles = AppFunctions.Titles(UserName, SQLQuery.BookTitles(BookNature));

            id ??= 0;
            if(id==0) { id = GetNumber(UserName, "BankBookID"); }
            Variables = new()
            {
                IsSelected = false,
                BankBookID = (int)id,
                MinDate = (DateTime)GetKey(UserName, "BankBookFrom", KeyType.Date),
                MaxDate = (DateTime)GetKey(UserName, "BankBookTo", KeyType.Date),
                IsPosted1 = (int)GetKey(UserName, "BankBookPost", KeyType.Number),
            };

            


            var LedgerClass = new Ledger(UserName);                                                                                     // Create a Class for ledger style record.
            var BankCode = GetNumber(UserName, "BankBkNature");
            var BankBookTitle = DataTableClass.GetTable(UserName, Tables.COA, $"Nature={BankCode}");
            var Date1 = Variables.MinDate.AddDays(-1).ToString("yyyy-MM-dd");
            var Date2 = Variables.MaxDate.AddDays(1).ToString("yyyy-MM-dd");
            LedgerClass.TableName = Tables.BankBook;
            LedgerClass.Date_From = DateTime.Parse(Date1);
            LedgerClass.Date_To = DateTime.Parse(Date2);
            LedgerClass.Sort = "Vou_Date";
            LedgerClass.Filter = $"BookID={id} AND Date(Vou_Date)>Date('{Date1}') AND Date(Vou_Date)<Date('{Date2}')";
            Bankbook = LedgerClass.Records;               // Fill Bankbook as ledger style
            if (Variables.IsPosted1 == 1)                        // Not posted.
            {
                DataView BookView = Bankbook.AsDataView();
                BookView.RowFilter = "Status='Posted'";
                Bankbook = BookView.ToTable();
            }

            if (Variables.IsPosted1 == 2)                        // Not posted.
            {
                DataView BookView = Bankbook.AsDataView();
                BookView.RowFilter = "Status<>'Posted'";
                Bankbook = BookView.ToTable();
            }

            if (Bankbook.Rows.Count == 0)
            {
                ErrorMessages.Add(SetMessage("No record Found.....!"));
            }

            FMTNumber = GetText(UserName, "FMT");
        }

        #endregion

        #region Post
        public IActionResult OnPostEdit(int ID)
        {
            return RedirectToPage("BankBookRecord", routeValues: new { ID, BookID = Variables.BankBookID });
        }
        public IActionResult OnPostRefresh(int id)
        {

            SetKey(UserName, "BankBookFrom", Variables.MinDate, KeyType.Date);
            SetKey(UserName, "BankBookTo", Variables.MaxDate, KeyType.Date);
            SetKey(UserName, "BankBookPost", Variables.IsPosted1, KeyType.Number);
            SetKey(UserName, "BankBookID", Variables.BankBookID, KeyType.Number);


            return RedirectToPage("BankBook", routeValues: new { id = Variables.BankBookID });
        }
        public IActionResult OnPostAdd()
        {
            return RedirectToPage("BankBookRecord", new { ID = 0, _BookID = Variables.BankBookID });
        }
        public IActionResult OnPostSave(int ID)
        {
            DataTableClass BankBook = new(UserName, Tables.BankBook);
            BankBook.MyDataView.RowFilter = $"ID={ID}";
            if (BankBook.MyDataView.Count == 0) { BankBook.NewRecord(); }                  // Create a new record.
            else { BankBook.SeekRecord(ID); }                                                                   // Select a existing record.

            BankBook.CurrentRow["ID"] = MyRecord.ID;
            BankBook.CurrentRow["Vou_No"] = MyRecord.Vou_No;
            BankBook.CurrentRow["Vou_Date"] = MyRecord.Vou_Date;
            BankBook.CurrentRow["Ref_No"] = MyRecord.Ref_No;
            BankBook.CurrentRow["Sheet_No"] = MyRecord.Sheet_No;
            BankBook.CurrentRow["BookID"] = MyRecord.BookID;
            BankBook.CurrentRow["COA"] = MyRecord.COA;
            BankBook.CurrentRow["DR"] = MyRecord.DR;
            BankBook.CurrentRow["CR"] = MyRecord.CR;
            BankBook.CurrentRow["Description"] = MyRecord.Description;
            BankBook.CurrentRow["Comments"] = MyRecord.Comments;
            BankBook.CurrentRow["Customer"] = MyRecord.Customer;
            BankBook.CurrentRow["Project"] = MyRecord.Project;
            BankBook.CurrentRow["Employee"] = MyRecord.Employee;
            BankBook.Save();

            ErrorMessages = BankBook.TableValidation.MyMessages;
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
            DataTableClass _Table = new(UserName, Tables.BankBook);
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
            return RedirectToPage("BankBook");
        }
        public IActionResult OnPostPrint(int ID)
        {
            if (ID > 0)
            {

                var _Table = DataTableClass.GetTable(UserName, Tables.BankBook, $"ID={ID}");

                if (_Table.Rows.Count > 0)
                {
                    var _BookID = (int)_Table.Rows[0]["BookID"];
                    var _Booktitle = AppFunctions.GetTitle(UserName, Tables.COA, _BookID);
                    AppRegistry.SetKey(UserName, "cbbVouID", ID, KeyType.Number);
                    AppRegistry.SetKey(UserName, "cbbHeading1", "Bank Book", KeyType.Text);
                    AppRegistry.SetKey(UserName, "cbbHeading2", _Booktitle, KeyType.Text);
                    AppRegistry.SetKey(UserName, "cbbBook", "BankBook", KeyType.Text);

                    var Option = ReportType.Preview;
                    return RedirectToPage("../ReportPrint/PrintReport", "CashBankBook", new { _ReportType = Option });

                }
            }
            return Page();


            //SetKey(UserName, "cbVouNo", Vou_No, KeyType.Text);
            //SetKey(UserName, "Heading1", "Bank Voucher", KeyType.Text);

            //return RedirectToPage("../ReportPrint/PrintReport", "Voucher", new { _ReportType = ReportType.Preview });
        }
        public IActionResult OnPostShow(int ID)
        {
            return Page();
        }
        #endregion
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
            public int BankBookID { get; set; }
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
