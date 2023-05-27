using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Data;

namespace Applied_WebApplication.Pages.Accounts
{
    public class BillReceivableListModel : PageModel
    {
        public string UserName => User.Identity.Name;
        public DataTable BillReceivable { get; set; } = new();

        public void OnGet()
        {
            DataTableClass _Table = new(UserName, Tables.BillReceivable);
            //_Table.MyDataView.RowFilter = "";
            BillReceivable = _Table.MyDataView.ToTable();
        }

        public void OnGetEdit()
        {
        }
        public IActionResult OnPostAdd()
        {
            return RedirectToPage("../Sales/SaleInvoice");
        }

        public IActionResult OnPostShow(int ID)
        {
            DataTableClass _Table = new(UserName, Tables.BillReceivable, $"ID={ID}");
            if (_Table.Count > 0)
            {
                var Vou_No = _Table.CurrentRow["Vou_No"];
                var Sr_No = 1;
                return RedirectToPage("../Sales/SaleInvoice", routeValues: new { Vou_No, Sr_No });
            }
            return RedirectToPage("../Sales/SaleInvoice");
        }

        public IActionResult OnPostDelete(int ID)
        {
            DataTableClass _Table = new(UserName, Tables.BillReceivable, $"ID={ID}");
            if (_Table.Count > 0)
            {
                // Code for delete a sales invoices 
            }
                return Page();
        }

        public IActionResult OnPostPrint(int ID)
        {
            var TranID = ID;
            return RedirectToPage("../ReportPrint/PrintReport", pageHandler: "SaleInvoice", routeValues: new { TranID });
        }
    }
}
