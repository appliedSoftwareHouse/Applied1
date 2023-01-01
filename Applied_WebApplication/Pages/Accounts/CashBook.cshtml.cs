using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using System.Data;
using System.Globalization;
using System.ServiceModel.Security;

namespace Applied_WebApplication.Pages.Accounts
{
    public class CashBookModel : PageModel
    {
        public DataTable Cashbook = new DataTable();
        public ReportParameters MyParameters = new();


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

        public IActionResult  OnPostRefresh(string UserName)
        {
            
            List<KeyValuePair<string, StringValues>> _Values = Request.Form.ToList();
            foreach (KeyValuePair<string, StringValues> _KeyValue in _Values)
            {
                if (_KeyValue.Key == "CashBookID") { MyParameters.CashBookID = int.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "MinDate") { MyParameters.MinDate = DateTime.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "MaxDate") { MyParameters.MaxDate = DateTime.Parse(_KeyValue.Value); }
            }
            MyParameters.IsSelected = true;
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

        public IActionResult OnPostSave(string UserName, int id, ReportParameters? Paramaters)
        {
            return Page();
        }

        public class ReportParameters
        {
            public int RecordID { get; set; }
            public bool IsSelected { get; set; } = false;
            public bool IsAdd { get; set; } = false;
            public int CashBookID { get; set; }
            public string BookTitle { get; set; } = "Select....";
            public DateTime MinDate { get; set; } = DateTime.Now;
            public DateTime MaxDate { get; set; } = DateTime.Now;
            public DataRow Row { get; set; }
        }
    }
}
