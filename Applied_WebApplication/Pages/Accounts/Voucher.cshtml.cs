using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Accounts
{
    public class Voucher1Model : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public List<Message> ErrorMessages { get; set; } = new();
        public DataTable tb_Voucher { get; set; } = new();
        public string UserName => User.Identity.Name;




        public void OnGet(int? ID)
        {
            if (ID == null)
            {
                Variables = new();
            }
            else
            {
                var _Filter = $"ID={ID}";
                DataTableClass tb_Ledger = new(UserName, Tables.Ledger, _Filter);
                tb_Ledger.SeekRecord((int)ID);

                Variables = new()
                {
                    ID = (int)tb_Ledger.CurrentRow["ID"],
                    TranID = (int)tb_Ledger.CurrentRow["TranID"],
                    Vou_Type = tb_Ledger.CurrentRow["Vou_Type"].ToString(),
                    Vou_No = tb_Ledger.CurrentRow["Vou_No"].ToString(),
                    Vou_Date = DateTime.Parse(tb_Ledger.CurrentRow["Vou_Date"].ToString()),
                    Ref_No = tb_Ledger.CurrentRow["Ref_No"].ToString(),
                    SR_NO = (int)tb_Ledger.CurrentRow["SR_NO"],
                    COA = (int)tb_Ledger.CurrentRow["COA"],
                    BookID = (int)tb_Ledger.CurrentRow["BookID"],
                    Description = tb_Ledger.CurrentRow["Description"].ToString(),
                    Comments = tb_Ledger.CurrentRow["Comments"].ToString(),
                    DR = decimal.Parse(tb_Ledger.CurrentRow["DR"].ToString()),
                    CR = decimal.Parse(tb_Ledger.CurrentRow["CR"].ToString()),
                    Employee = (int)tb_Ledger.CurrentRow["Employee"],
                    Customer = (int)tb_Ledger.CurrentRow["Customer"],
                    Project = (int)tb_Ledger.CurrentRow["Project"]
                };

                tb_Voucher = GetVoucher(Variables.TranID, Variables.Vou_Type);

            }


        }

        public IActionResult OnPostEdit(int ID)
        {
            //DataTableClass tb_Ledger = new(UserName, Tables.Ledger);
            //tb_Ledger.SeekRecord(id);
            return RedirectToPage("Voucher1", routeValues: new { ID });
        }

        public IActionResult OnPostSave(int id)
        {

            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);
            tb_Ledger.SeekRecord(id);

            int RecNo = (int)tb_Ledger.CurrentRow["ID"];
            int TranID = (int)tb_Ledger.CurrentRow["TranID"];
            string VouType = (string)tb_Ledger.CurrentRow["Vou_Type"];

            return RedirectToPage(routeValues: new { TranID, VouType, RecNo });
        }


        private DataTable GetVoucher(int TranID, string VouType)
        {
            DataTableClass _TableClass = new(UserName, Tables.Ledger);
            _TableClass.MyDataView.RowFilter = string.Format("TranID={0} AND Vou_Type='{1}'", TranID, VouType);
            if (_TableClass.Count > 0)
            {
                return _TableClass.MyDataView.ToTable();
            }
            return _TableClass.MyDataTable.Clone();
        }

        public class MyParameters
        {
            public int ID { get; set; }
            public int ID2 { get; set; }
            public int TranID { get; set; }
            public string Vou_Type { get; set; }
            public string Vou_No { get; set; }
            public DateTime Vou_Date { get; set; }
            public int SR_NO { get; set; }
            public string Ref_No { get; set; }
            public int BookID { get; set; }
            public decimal DR { get; set; }
            public decimal CR { get; set; }
            public string Description { get; set; }
            public string Comments { get; set; }

            public int COA { get; set; }
            public int Customer { get; set; }
            public int Employee { get; set; }
            public int Project { get; set; }
            public string Created { get; set; }
        }
    }
}
