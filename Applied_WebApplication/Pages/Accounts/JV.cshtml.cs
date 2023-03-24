using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Accounts
{
    public class JVModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public List<Message> ErrorMessages { get; set; } = new();

        public string UserName => User.Identity.Name;
        public DataTable Voucher { get; set; }


        public void OnGet(string? Vou_No, int? Sr_No)
        {
            Variables = new();              // Setup of Variables
            if (Vou_No == null)
            {
                Variables.Vou_No = NewVoucherID();
                Variables.Vou_Date = DateTime.Now;
                Variables.Sr_No = 1;

                var TempVoucher = new TempTableClass(UserName, Tables.Ledger, "");
                Voucher = TempVoucher.MyDataTable;
            }
            else
            {
                var TempVoucher = new TempTableClass(UserName, Tables.Ledger, Vou_No);
                Voucher = TempVoucher.MyDataTable;

                Sr_No ??= 0;
                TempVoucher.MyDataView.RowFilter = string.Format("SR_No={0}", (int)Sr_No);
                if(TempVoucher.MyDataView.Count ==1) 
                {
                    TempVoucher.CurrentRow = TempVoucher.MyDataView[0].Row;
                    Variables.ID = (int)TempVoucher.CurrentRow["ID"];
                    Variables.TranID = (int)TempVoucher.CurrentRow["TranID"];
                    Variables.Vou_Type = (string)TempVoucher.CurrentRow["Vou_Type"];
                    Variables.Vou_Date = (DateTime)TempVoucher.CurrentRow["Vou_Date"];
                    Variables.Vou_No = (string)TempVoucher.CurrentRow["Vou_No"];
                    Variables.Sr_No = (int)TempVoucher.CurrentRow["Sr_No"];
                    Variables.Ref_No = (string)TempVoucher.CurrentRow["Ref_No"];
                    Variables.COA = (int)TempVoucher.CurrentRow["COA"];
                    Variables.DR = (int)TempVoucher.CurrentRow["DR"];
                    Variables.CR = (int)TempVoucher.CurrentRow["CR"];
                    Variables.Customer = (int)TempVoucher.CurrentRow["Customer"];
                    Variables.Project = (int)TempVoucher.CurrentRow["Project"];
                    Variables.Employee = (int)TempVoucher.CurrentRow["Employee"];
                    Variables.Inventory = (int)TempVoucher.CurrentRow["Inventory"];
                    Variables.Description = (string)TempVoucher.CurrentRow["Description"];
                    Variables.Comments = (string)TempVoucher.CurrentRow["Comments"];
                }

            }

        }

        private static string NewVoucherID()
        {
            return "JV2303-0001";
        }

        public IActionResult OnPostSave()
        {
            ErrorMessages = new();
            var TempVoucher = new TempTableClass(UserName, Tables.Ledger, Variables.Vou_No);
            TempVoucher.MyDataView.RowFilter = string.Format("Sr_No={0}", Variables.Sr_No);
            if(TempVoucher.MyDataView.Count==1) { TempVoucher.CurrentRow = TempVoucher.MyDataView[0].Row; }
            else { TempVoucher.CurrentRow = TempVoucher.MyDataTable.NewRow(); }

            TempVoucher.CurrentRow["ID"] = Variables.ID;
            TempVoucher.CurrentRow["TranID"] = Variables.TranID;
            TempVoucher.CurrentRow["Vou_Type"] = Variables.Vou_Type;
            TempVoucher.CurrentRow["Vou_Date"] = Variables.Vou_Date;
            TempVoucher.CurrentRow["Vou_No"] = Variables.Vou_No;
            TempVoucher.CurrentRow["Sr_No"] = Variables.Sr_No;
            TempVoucher.CurrentRow["Ref_No"] = Variables.Ref_No;
            TempVoucher.CurrentRow["COA"] = Variables.COA;
            TempVoucher.CurrentRow["DR"] = Variables.DR;
            TempVoucher.CurrentRow["CR"] = Variables.CR;
            TempVoucher.CurrentRow["Customer"] = Variables.Customer;
            TempVoucher.CurrentRow["Employee"] = Variables.Employee;
            TempVoucher.CurrentRow["Project"] = Variables.Project;
            TempVoucher.CurrentRow["Inventory"] = Variables.Inventory;
            TempVoucher.CurrentRow["Description"] = Variables.Description;
            TempVoucher.CurrentRow["Comments"] = Variables.Comments;
            TempVoucher.Save();
            if(TempVoucher.ErrorMessages.Count>0)
            {
                return Page();
            }

            var Vou_No = TempVoucher.CurrentRow["Vou_No"].ToString();
            var Sr_No = TempVoucher.CurrentRow["Sr_No"].ToString();


            return RedirectToPage("JV", routeValues: new {Vou_No, Sr_No});
        }
        public class MyParameters
        {
            public int ID { get; set; }
            public int TranID { get; set; }
            public string Vou_Type { get; set; }
            public DateTime Vou_Date { get; set; }
            public string Vou_No { get; set; }
            public int Sr_No { get; set; }
            public string Ref_No { get; set; }
            public int COA { get; set; }
            public int BookID { get; set; }
            public decimal DR { get; set; }
            public decimal CR { get; set; }
            public int Customer { get; set; }
            public int Project { get; set; }
            public int Employee { get; set; }
            public int Inventory { get; set; }
            public string Description { get; set; }
            public string Comments { get; set; }
        }

    }
}
