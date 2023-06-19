using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Accounts
{
    public class VouchersListModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable Vouchers { get; set; }
        public List<Message> ErrorMessages = new List<Message>();
        public string UserName => User.Identity.Name;

        public void OnGet()
        {
            try
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
            catch (Exception e)
            {
                ErrorMessages.Add(MessageClass.SetMessage(e.Message));
            }

            
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

            return Page();
        }

        public IActionResult OnPostEdit(int id)
        {
            var _Filter = $"ID={id}";
            DataTableClass _Table = new DataTableClass(UserName, Tables.Ledger, _Filter);
            if(_Table !=null)
            {
                if(_Table.Rows.Count>0)
                {
                    var Row = _Table.Rows[0];
                    int RecNo = (int)Row["ID"];
                    int TranID = (int)Row["TranID"];
                    string VouType = (string)Row["Vou_Type"];
                    return RedirectToPage("./Voucher", routeValues: new { id });
                }
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
