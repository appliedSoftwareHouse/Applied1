using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Accounts
{
    [Authorize]
    public class ReceiptModel : PageModel
    {
        [BindProperty]
        public Receipt Variables { get; set; }
        public Dictionary<int, string> tbAccounts { get; set; }
        public Dictionary<int, string> tbReceiptAcc { get; set; }
        public string UserName => User.Identity.Name;

        public void OnGet()
        {
            Variables = GetNewRecord();
        }

        public static Receipt GetNewRecord()
        {
            return new Receipt()
            {
                ID = 0,
                Vou_No = "New",
                Vou_Date = DateTime.Now,
                IsCash = false,
                Payer = 0,
                Account = 0,
                Project = 0,
                Employee = 0,
                COA = 0,
                Amount = 0,
                Description = ""
            };
        }

        #region Save and Validation

        public IActionResult OnPostSave()
        {
            var _TableClass = new DataTableClass(UserName, Tables.Receipts);
            _TableClass.CurrentRow = _TableClass.MyDataTable.NewRow();

            if (Variables.Vou_No.ToUpper() == "NEW")
            {
                Variables.Vou_No = NewVoucher.GetNewVoucher(_TableClass.MyDataTable, "RT");
            }

            _TableClass.CurrentRow["ID"] = Variables.ID;
            _TableClass.CurrentRow["Vou_No"] = Variables.Vou_No;
            _TableClass.CurrentRow["Vou_Date"] = Variables.Vou_Date;
            _TableClass.CurrentRow["Payer"] = Variables.Payer;
            _TableClass.CurrentRow["Account"] = Variables.Account;
            _TableClass.CurrentRow["COA"] = Variables.COA;
            _TableClass.CurrentRow["Project"] = Variables.Project;
            _TableClass.CurrentRow["Employee"] = Variables.Employee;
            _TableClass.CurrentRow["Amount"] = Variables.Account;
            _TableClass.CurrentRow["Description"] = Variables.Description;

            if(Validation())
            {
                _TableClass.Save();
            }
            return Page();
        }
        
        public bool Validation()
        {
            return true;
        }
        #endregion
    }

    public class Receipt
    {
        public int ID { get; set; }
        public string Vou_No { get; set; }
        public DateTime Vou_Date { get; set; }
        public bool IsCash { get; set; } = false;
        public int COA { get; set; }
        public int Account { get; set; }
        public int Project { get; set; }
        public int Employee { get; set; }
        public int Payer { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;

    }
}
