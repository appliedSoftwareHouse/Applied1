using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text;
using System.Transactions;

namespace Applied_WebApplication.Pages.Accounts
{
    public class VouchersListModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; } = new();
        public DataTable Vouchers { get; set; } = new();

        public void OnGet()
        {
            string UserName = User.Identity.Name;
            
            Variables.VouType = (string)AppRegistry.GetKey(UserName, "Vou_VouType", KeyType.Text);
            Variables.DateFrom = (DateTime)AppRegistry.GetKey(UserName, "Vou_DtFrom", KeyType.Date);
            Variables.DateTo = (DateTime)AppRegistry.GetKey(UserName, "Vou_DtTo", KeyType.Date);

            if (string.IsNullOrEmpty(Variables.VouType)) { Variables.VouType = VoucherType.Cash.ToString(); }
           
            Vouchers = GetData();
            
        }

        public DataTable GetData()
        {
            string UserName = User.Identity.Name;
            DataTableClass _Table = new(UserName, Tables.Ledger);
            StringBuilder _Filter = new StringBuilder();

            string _Date1 = Variables.DateFrom.ToString(AppRegistry.DateYMD);
            string _Date2 = Variables.DateTo.ToString(AppRegistry.DateYMD);

            _Filter.Append("SR_No=1");
            _Filter.Append(" AND Vou_Type = '"+ Variables.VouType.ToString() + "'");
            _Filter.Append(" AND Vou_Date >= '" + _Date1 + "' ");
            _Filter.Append(" AND Vou_Date <= '" + _Date2 + "' ");

            _Table.MyDataView.RowFilter = _Filter.ToString();
            return _Table.MyDataView.ToTable();
        }


        public void OnPost() { }

        public void OnPostRefresh() 
        {
            string UserName = User.Identity.Name;
            Vouchers = GetData();


            AppRegistry.SetKey(UserName, "Vou_VouType", Variables.VouType, KeyType.Text);
            AppRegistry.SetKey(UserName, "Vou_DtFrom", Variables.DateFrom, KeyType.Date);
            AppRegistry.SetKey(UserName, "Vou_DtTo", Variables.DateTo, KeyType.Date);

        }

        public IActionResult OnPostEdit(int id)
        {
            string UserName = User.Identity.Name;
            DataTableClass _Table = new DataTableClass(UserName, Tables.Ledger);
            DataRow Row = _Table.SeekRecord(id);

            if(Row!=null)
            {
                int RecNo = (int)Row["ID"];
                int TranID = (int)Row["TranID"];
                string VouType = (string)Row["Vou_Type"];

                return RedirectToPage("./Voucher1", routeValues: new { TranID, VouType, RecNo });
                //return RedirectToPage("./Voucher1", pageHandler: "Edit", routeValues: new { TranID, VouType });
            }

            return Page();

        }
       


        public void OnGetDelete()
        {
            string UserName = User.Identity.Name;
        }
        public class MyParameters
        {
            public string VouType { get; set; } = VoucherType.Cash.ToString();
            public DateTime DateFrom { get; set; } = DateTime.Now;
            public DateTime DateTo { get; set; } = DateTime.Now;
        }

    }
}
