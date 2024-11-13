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
        public Parameter Variables { get; set; }
        public Dictionary<int, string> tbAccounts { get; set; }
        public Dictionary<int, string> tbReceiptAcc { get; set; }
        public string UserName => User.Identity.Name;
        public DataRow PayerProfile { get; set; }

        public void OnGet(int ID)
        {

            if (ID == 0) { Variables = GetNewRecord(); }
            if (ID > 1) { Variables = Variables = GetRecord(ID); }

            if (Variables.Payer > 0)
            {
                GetPayerProfile();
            }
        }


        #region Get Payer Profile
        private void GetPayerProfile()
        {
            DataTable _DataTable = DataTableClass.GetTable(UserName, Tables.Customers, $"[ID]={Variables.Payer}");
            if (_DataTable.Rows.Count > 0) { PayerProfile = _DataTable.Rows[0]; }
        }
        #endregion

        #region Get Record

        private Parameter GetRecord(int ID)
        {
            DataTable _DataTable = DataTableClass.GetTable(UserName, Tables.Receipts, $"[ID]={ID}");
            if (_DataTable.Rows.Count > 0)
            {
                var _Row = _DataTable.Rows[0];
                Parameter _Variable = new()
                {
                    ID = (int)_Row["ID"],
                    Vou_No = (string)_Row["Vou_No"],
                    Vou_Date = (DateTime)_Row["Vou_Date"],
                    Payer = (int)_Row["Payer"],
                    COA = (int)_Row["COA"],
                    Project = (int)_Row["Project"],
                    Employee = (int)_Row["Employee"],
                    Account = (int)_Row["Account"],
                    Ref_No = (string)_Row["Ref_No"],
                    Amount = (decimal)_Row["Amount"],
                    Description = (string)_Row["Description"]
                };
                return _Variable;

            }
            else
            {
                return GetNewRecord();
            }
        }

        public Parameter GetNewRecord()
        {
            return new Parameter()
            {
                ID = 0,
                Vou_No = "New",
                Vou_Date = AppRegistry.GetDate(UserName, "rcptDate"),
                Ref_No = string.Empty,
                Payer = 0,
                Account = 0,
                Project = 0,
                Employee = 0,
                COA = 0,
                Amount = 0,
                Description = string.Empty
            };
        }
        #endregion

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
            _TableClass.CurrentRow["Ref_No"] = Variables.Ref_No;
            _TableClass.CurrentRow["Payer"] = Variables.Payer;
            _TableClass.CurrentRow["Account"] = Variables.Account;
            _TableClass.CurrentRow["COA"] = Variables.COA;
            _TableClass.CurrentRow["Project"] = Variables.Project;
            _TableClass.CurrentRow["Employee"] = Variables.Employee;
            _TableClass.CurrentRow["Amount"] = Variables.Amount;
            _TableClass.CurrentRow["Description"] = Variables.Description;

            if (Validation())
            {
                _TableClass.Save();
                Variables.ID = (int)_TableClass.CurrentRow["ID"];
                AppRegistry.SetKey(UserName, "rcptDate", Variables.Vou_Date, KeyType.Date);

                return RedirectToPage("Receipt", routeValues: new { ID = Variables.ID });
            }
            return Page();
        }

        public bool Validation()
        {
            var _Valid = true;
            if (Variables.Amount <= 0) { _Valid = false; }
            if (Variables.Payer == 0) { _Valid = false; }
            if (Variables.COA == 0) { _Valid = false; }
            if (Variables.Project == 0) { _Valid = false; }
            if (Variables.Description.Length == 0) { _Valid = false; }

            return _Valid;

        }
        #endregion

        #region Send Email
        public async Task SendEmailAsync()
        {
            await Task.Run(async () => { await Task.Delay(1000); });
        }
        #endregion


        #region Print
        public void OnPostPrint()
        {

        }
        #endregion

    }

    public class Parameter
    {
        public int ID { get; set; }
        public string Vou_No { get; set; }
        public DateTime Vou_Date { get; set; }
        public string Ref_No { get; set; } = string.Empty;
        public int COA { get; set; }
        public int Account { get; set; }
        public int Project { get; set; }
        public int Employee { get; set; }
        public int Payer { get; set; }
        public decimal Amount { get; set; } = 0.00M;
        public string Description { get; set; } = string.Empty;

    }
}
