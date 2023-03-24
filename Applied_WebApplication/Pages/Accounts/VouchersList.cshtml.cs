using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Applied_WebApplication.Pages.Accounts
{
    public class VouchersListModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; } = new();
        public DataTable Vouchers { get; set; } = new();
        public string UserName => User.Identity.Name;

        public void OnGet()
        {
            

            Variables = new()
            {
                VouType = (string)AppRegistry.GetKey(UserName, "Vou_VouType", KeyType.Text),
                DateFrom = (DateTime)AppRegistry.GetKey(UserName, "Vou_DtFrom", KeyType.Date),
                DateTo = (DateTime)AppRegistry.GetKey(UserName, "Vou_DtTo", KeyType.Date)
            };

            if (string.IsNullOrEmpty(Variables.VouType)) { Variables.VouType = VoucherType.Cash.ToString(); }

            Vouchers = GetData();

        }

        public DataTable GetData()
        {
            
            DataTableClass _Table = new(UserName, Tables.Ledger);
            StringBuilder _Filter = new StringBuilder();

            string _Date1 = Variables.DateFrom.ToString(AppRegistry.DateYMD);
            string _Date2 = Variables.DateTo.ToString(AppRegistry.DateYMD);

            _Filter.Append("SR_No=1");
            _Filter.Append(" AND Vou_Type = '" + Variables.VouType.ToString() + "'");
            _Filter.Append(" AND Vou_Date >= '" + _Date1 + "' ");
            _Filter.Append(" AND Vou_Date <= '" + _Date2 + "' ");

            _Table.MyDataView.RowFilter = _Filter.ToString();
            return _Table.MyDataView.ToTable();
        }


        public void OnPost() { }

        public IActionResult OnPostRefresh()
        {
         
            AppRegistry.SetKey(UserName, "Vou_VouType", Variables.VouType, KeyType.Text);
            AppRegistry.SetKey(UserName, "Vou_DtFrom", Variables.DateFrom, KeyType.Date);
            AppRegistry.SetKey(UserName, "Vou_DtTo", Variables.DateTo, KeyType.Date);

            return RedirectToPage("VoucherList");
        }

        public IActionResult OnPostEdit(int id)
        {
            DataTableClass _Table = new DataTableClass(UserName, Tables.Ledger);
            DataRow Row = _Table.SeekRecord(id);

            if (Row != null)
            {
                int RecNo = (int)Row["ID"];
                int TranID = (int)Row["TranID"];
                string VouType = (string)Row["Vou_Type"];

                return RedirectToPage("./Voucher1", routeValues: new { TranID, VouType, RecNo });
            }

            return Page();

        }



        public void OnGetDelete()
        {
           
        }
        public class MyParameters
        {
            public string VouType { get; set; } = VoucherType.Cash.ToString();
            public DateTime DateFrom { get; set; } = DateTime.Now;
            public DateTime DateTo { get; set; } = DateTime.Now;
        }

    }
}
