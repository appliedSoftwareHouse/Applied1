using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages
{
    public class Voucher1Model : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public List<Message> ErrorMessages { get; set; } = new();
        public DataTable tb_Voucher { get; set; } = new();
        public string UserName => User.Identity.Name;




        public void OnGet(int TranID, string VouType, int RecNo)
        {
            //RecNo ??= 0;
            Variables = new();
            tb_Voucher = GetVoucher(TranID, VouType);
            if (tb_Voucher.Rows.Count > 0)
            {
                GetVariables(tb_Voucher, RecNo);
            }

        }

        public IActionResult OnPostEdit(int id)
        {
            DataTableClass tb_Ledger = new(UserName, Tables.Ledger);
            tb_Ledger.SeekRecord(id);

            int RecNo = (int)tb_Ledger.CurrentRow["ID"];
            int TranID = (int)tb_Ledger.CurrentRow["TranID"];
            string VouType = (string)tb_Ledger.CurrentRow["Vou_Type"];

            return RedirectToPage(routeValues: new { TranID, VouType, RecNo });
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

        private void GetVariables(DataTable _Table, int _RowID)
        {
            foreach (DataRow Row in _Table.Rows)
            {
                if (_RowID.Equals((int)Row["ID"]))
                {
                    Variables.ID = (int)Row["ID"];
                    Variables.TranID = (int)Row["TranID"];
                    Variables.Vou_Type = Row["Vou_Type"].ToString();
                    Variables.Vou_No = Row["Vou_No"].ToString();
                    Variables.Vou_Date = DateTime.Parse(Row["Vou_Date"].ToString());
                    Variables.Ref_No = Row["Ref_No"].ToString();
                    Variables.SR_NO = (int)Row["SR_NO"];
                    Variables.COA = (int)Row["COA"];
                    Variables.BookID = (int)Row["BookID"];
                    Variables.Description = Row["Description"].ToString();
                    Variables.Comments = Row["Comments"].ToString();
                    Variables.DR = decimal.Parse(Row["DR"].ToString());
                    Variables.CR = decimal.Parse(Row["CR"].ToString());
                    Variables.Employee = (int)Row["Employee"];
                    Variables.Customer = (int)Row["Customer"];
                    Variables.Project = (int)Row["Project"];
                }
            }

        }

        private DataTable GetVoucher(int tranID, string vouType)
        {
            DataTableClass _TableClass = new(UserName, Tables.Ledger);

            return _TableClass.GetTable("TranID=" + tranID.ToString() + " AND Vou_Type='" + vouType + "'");
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
