using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Accounts
{
    public class VoucherModel : PageModel
    {
        public DataTable Tb_Voucher = new DataTable();

        public void OnGet()
        {
        }

        public void OnGetEdit(int TranID, string VouType)
        {
            string UserName = User.Identity.Name;
            DataTableClass _Table = new(UserName, Tables.Ledger);
            _Table.MyDataView.RowFilter = string.Concat("TranID=", TranID.ToString(), " AND Vou_Type='", VouType, "'"); 
            if(_Table.MyDataView.Count>0)
            {
                Tb_Voucher = _Table.MyDataView.ToTable();
            }


        }
            

    }
}
