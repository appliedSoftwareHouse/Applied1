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
        public DataTableClass CashBook;
        public DataTable CashBookLedger;
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

        public void Refresh()
        {


        }

        public void OnPostRefresh(string UserName)
        {
            List<KeyValuePair<string, StringValues>> _Values = Request.Form.ToList();
            MyParameters.IsSelected = true;
            foreach (KeyValuePair<string, StringValues> _KeyValue in _Values)
            {
                if (_KeyValue.Key == "MinDate") { MyParameters.MinDate = DateTime.Parse(_KeyValue.Value); }
                if (_KeyValue.Key == "MaxDate") { MyParameters.MaxDate = DateTime.Parse(_KeyValue.Value); }
            }
        }

        public class ReportParameters
        {
            public int RecordID { get; set; }
            public bool IsSelected { get; set; }
            public int CashBookID { get; set; }
            public string BookTitle { get; set; }
            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }
        }
    }
}
