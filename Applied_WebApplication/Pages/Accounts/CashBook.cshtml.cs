using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Drawing;
using static Applied_WebApplication.Data.AppFunctions;
using static Applied_WebApplication.Data.AppRegistry;



namespace Applied_WebApplication.Pages.Accounts
{
    public class CashBookModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public BookRecord MyRecord { get; set; }
        public DataTable Cashbook = new DataTable();
        public List<Message> ErrorMessages = new();
        public string UserName => User.Identity.Name;

        #region Get / POST

        public void OnGet(int? id)
        {

            if (id == null)
            {
                Variables = new() 
                {
                    IsSelected = false,
                    CashBookID = (int)GetKey(UserName, "CashBookID", KeyType.Number),
                    MinDate = (DateTime)GetKey(UserName, "CashBookFrom", KeyType.Date),
                    MaxDate = (DateTime)GetKey(UserName, "CashBookTo", KeyType.Date),
                    IsPosted = (int)GetKey(UserName, "CashBookPost", KeyType.Number)
                };
            }
            else
            {
                Variables = new()
                {
                    IsSelected = false,
                    CashBookID = (int)id,
                    MinDate = (DateTime)GetKey(UserName, "CashBookFrom", KeyType.Date),
                    MaxDate = (DateTime)GetKey(UserName, "CashBookTo", KeyType.Date),
                    IsPosted = (int)GetKey(UserName, "CashBookPost", KeyType.Number),
                };
            }

            id ??= Variables.CashBookID;
            var LedgerClass = new Ledger(UserName);                                                                                     // Create a Class for ledger style record.
            var Date1 = Variables.MinDate.ToString("yyyy-MM-dd");
            var Date2 = Variables.MaxDate.ToString("yyyy-MM-dd");
            LedgerClass.TableName = Tables.CashBook;
            LedgerClass.Date_From = DateTime.Parse(Date1);
            LedgerClass.Date_To = DateTime.Parse(Date2);
            LedgerClass.Sort = "Vou_Date";
            LedgerClass.Filter = string.Concat("BookID=", (int)id);
            Cashbook = LedgerClass.Records;               // Fill cashbook as ledger style
            if (Variables.IsPosted==1)                        // Not posted.
            {
                DataView BookView = Cashbook.AsDataView();
                BookView.RowFilter = "Status='Posted'";
                Cashbook = BookView.ToTable();
            }

            if (Variables.IsPosted == 2)                        // Not posted.
            {
                DataView BookView = Cashbook.AsDataView();
                BookView.RowFilter = "Status<>'Posted'";
                Cashbook = BookView.ToTable();
            }

            if (Cashbook.Rows.Count==0)
            {
                ErrorMessages.Add(MessageClass.SetMessage("No record Found.....!"));
            }


            //Variables.BookTitle = GetTitle(UserName, Tables.COA, Variables.CashBookID);
            
        }
       
        #endregion

        
        public IActionResult OnPostRefresh(int id)
        {
           
           SetKey(UserName, "CashBookFrom", Variables.MinDate, KeyType.Date);
           SetKey(UserName, "CashBookTo", Variables.MaxDate, KeyType.Date);
           SetKey(UserName, "CashBookPost", Variables.IsPosted, KeyType.Number);
           SetKey(UserName, "CashBookID", Variables.CashBookID, KeyType.Number);


            return RedirectToPage("CashBook", routeValues: new { id = Variables.CashBookID });
        }
        public IActionResult OnPostAdd()
        {
            return RedirectToPage("CashBookRecord", new { id = Variables.CashBookID });
        }
        public IActionResult OnPostSave(int ID)
        {


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
                Variables.IsSelected = true;
                return Page();
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
            if(_Table.Count==1)
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

        [BindProperties]
        public class MyParameters
        {
            public int RecordID { get; set; }
            public bool IsSelected { get; set; }
            public bool IsAdd { get; set; }
            public bool IsError { get; set; }
            public bool Reload { get; set; }
            public int CashBookID { get; set; }
            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }
            public int IsPosted { get; set; }

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
