using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class TrialBalanceModel : PageModel
    {
        public DataTable TB = new();
        public decimal Tot_DR { get; set; } = 0.00M;
        public decimal Tot_CR { get; set; } = 0.00M;
      

        public void OnGet()
        {
            string UserName = User.Identity.Name;
            DataTableClass _Table = new(UserName, Tables.TB);
            _Table.MyDataView.Sort = "Code";
            TB = _Table.MyDataView.ToTable();

            //Tot_DR = 0.00M; Tot_CR = 0.00M;

            foreach (DataRow Row in TB.Rows)
            {
                decimal _Amount = decimal.Parse(Row["DR"].ToString()) - decimal.Parse(Row["CR"].ToString());
                if (_Amount >= 0) { Tot_DR += _Amount; }
                if (_Amount < 0) { Tot_CR += Math.Abs(_Amount); }
            }


        }


        public IActionResult OnPostTB()
        {
            return RedirectToPage("./PrintReport", pageHandler: "TB");

        }

    }
}
