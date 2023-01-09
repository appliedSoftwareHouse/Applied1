using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using System.Data;
using static Applied_WebApplication.Data.TableValidationClass;

namespace Applied_WebApplication.Pages.Accounts
{
    public class CashBookModel : PageModel
    {
        public Record MyRecord = new(); 
        public DataTable Cashbook = new DataTable();
        public ReportParameters MyParameters = new();
        public List<Message> ErrorMessages;

        public void OnGet(string UserName, int id, ReportParameters? Paramaters)
        {
            MyParameters.IsSelected = true;
            MyParameters.CashBookID = id;
            MyParameters.BookTitle = DataTableClass.GetColumnValue(UserName, Tables.COA, "Title", id);
            DataTableClass _Table = new(UserName, Tables.CashBook);                                                                     // Create Temporary for Min and Max Date.
            MyParameters.MinDate = (DateTime)_Table.MyDataTable.Compute("Min(Vou_Date)", "");
            MyParameters.MaxDate = (DateTime)_Table.MyDataTable.Compute("Max(Vou_Date)", "");
            _Table = null;                                                                                                                                                   // dispose off

        }

        public IActionResult OnPostRefresh(string UserName)
        {
            MyParameters.IsSelected = true;
            MyParameters.Reload = true;
            MyParameters.BookTitle = DataTableClass.GetColumnValue(UserName, Tables.COA, "Title", MyParameters.CashBookID);
            return Page();
        }

        public IActionResult OnPostAdd(string UserName, ReportParameters? Paramaters)
        {
            MyParameters.IsAdd = true;
            MyParameters.IsSelected = false;
            List<KeyValuePair<string, StringValues>> _Values = Request.Form.ToList();
            foreach (KeyValuePair<string, StringValues> _KeyValue in _Values)
            {
                if (_KeyValue.Key == "CashBookID") { MyParameters.CashBookID = int.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "MinDate") { MyParameters.MinDate = DateTime.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "MaxDate") { MyParameters.MaxDate = DateTime.Parse(_KeyValue.Value); }
            }
            MyParameters.BookTitle = DataTableClass.GetColumnValue(UserName, Tables.COA, "Title", MyParameters.CashBookID);
            return Page();
        }

        public IActionResult OnPostEdit(string UserName, int id, ReportParameters? Paramaters)
        {


            return Page();
        }

        public IActionResult OnPostDelete(string UserName, int id, ReportParameters? Paramaters)
        {


            return Page();
        }

        public IActionResult OnPostSave(string UserName)
        {
            List<KeyValuePair<string, StringValues>> _Values = Request.Form.ToList();
            foreach (KeyValuePair<string, StringValues> _KeyValue in _Values)
            {
                if (_KeyValue.Key == "CashBookID") { MyRecord.BookID = int.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "ID") { MyRecord.ID = int.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "Code") { MyRecord.Code = _KeyValue.Value; }
                if (_KeyValue.Key == "Vou_No") { MyRecord.Vou_No = _KeyValue.Value; }
                if (_KeyValue.Key == "Vou_Date") { MyRecord.Vou_Date = DateTime.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "Ref_No") { MyRecord.Ref_No = _KeyValue.Value; }
                if (_KeyValue.Key == "DR") { MyRecord.DR = decimal.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "CR") { MyRecord.CR = decimal.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "COA") { MyRecord.COA = int.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "Customer") { MyRecord.Customer = int.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "Project") { MyRecord.Project = int.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "Employee") { MyRecord.Employee = int.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "Description") { MyRecord.Description = _KeyValue.Value; }
                if (_KeyValue.Key == "Comments") { MyRecord.Comments = _KeyValue.Value; }

                if (_KeyValue.Key == "MinDate") { MyParameters.MinDate = DateTime.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "MaxDate") { MyParameters.MaxDate = DateTime.Parse(_KeyValue.Value); }
            }

            DataTableClass CashBook = new(UserName, Tables.CashBook);
            CashBook.NewRecord();
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
            if(ErrorMessages.Count == 0)
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

        

        public class ReportParameters
        {
            public int RecordID { get; set; }
            public bool IsSelected { get; set; } = false;
            public bool IsAdd { get; set; } = false;
            public bool IsError { get; set; } = false;
            public bool Reload { get; set; } = false;
            public int CashBookID { get; set; }
            public string BookTitle { get; set; } = "Select....";
            public DateTime MinDate { get; set; } = DateTime.Now;
            public DateTime MaxDate { get; set; } = DateTime.Now;
            
        }

        public class Record
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
