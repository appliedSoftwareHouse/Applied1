using AppReportClass;
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
        public string MyMessage { get; set; } = string.Empty;
        public List<Message> MyMessages { get; set; } = new();

        public void OnGet(int ID)
        {
           
            MyMessages = new();
            if (ID == 0) { Variables = GetNewRecord(); }
            if (ID > 0) { Variables = GetRecord(ID); }

            if (Variables.Payer > 0)
            {
                GetPayerProfile();
            }


            if (MyMessage.Length == 0)
            {
                MyMessage = AppRegistry.GetText(UserName, "rcptMessage");
                AppRegistry.SetKey(UserName, "rcptMessage", "", KeyType.Text);
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

                Parameter _Variable = new(); ;
                {
                    _Variable.ID = (int)_Row["ID"];
                    _Variable.Vou_No = _Row["Vou_No"].ToString();
                    _Variable.Vou_Date = (DateTime)_Row["Vou_Date"];
                    _Variable.Payer = (int)_Row["Payer"];
                    _Variable.COACash = (int)_Row["COACash"];
                    _Variable.COA = (int)_Row["COA"];
                    _Variable.Project = (int)_Row["Project"];
                    _Variable.Employee = (int)_Row["Employee"];
                    _Variable.Ref_No = _Row["Ref_No"].ToString();
                    _Variable.Amount = (decimal)_Row["Amount"];
                    _Variable.Description = _Row["Description"].ToString();
                    _Variable.Status = _Row["Status"].ToString();
                }
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
                COACash = 0,
                COA = 0,
                Project = 0,
                Employee = 0,
                Amount = 0,
                Description = string.Empty,
                Status = string.Empty
            };
        }
        #endregion

        #region Save and Validation

        public IActionResult OnPostSave()
        {
            MyMessages = new();
            var _TableClass = new DataTableClass(UserName, Tables.Receipts);
            _TableClass.CurrentRow = _TableClass.MyDataTable.NewRow();

            if (Variables.Vou_No.ToUpper() == "NEW")
            {
                Variables.Vou_No = NewVoucher.GetNewVoucher(_TableClass.MyDataTable, "RN");

            }

            _TableClass.CurrentRow["ID"] = Variables.ID;
            _TableClass.CurrentRow["Vou_No"] = Variables.Vou_No;
            _TableClass.CurrentRow["Vou_Date"] = Variables.Vou_Date;
            _TableClass.CurrentRow["Ref_No"] = Variables.Ref_No;
            _TableClass.CurrentRow["Payer"] = Variables.Payer;
            _TableClass.CurrentRow["COACash"] = Variables.COACash;
            _TableClass.CurrentRow["COA"] = Variables.COA;
            _TableClass.CurrentRow["Project"] = Variables.Project;
            _TableClass.CurrentRow["Employee"] = Variables.Employee;
            _TableClass.CurrentRow["Amount"] = Variables.Amount;
            _TableClass.CurrentRow["Description"] = Variables.Description;
            _TableClass.CurrentRow["Status"] = "Submitted";

            _TableClass.Save();
            MyMessages = _TableClass.ErrorMessages;
            if(MyMessages.Count > 0)
            {
                return Page();
            }
            else
            {
                MyMessages.Add(MessageClass.SetMessage($"{Variables.Vou_No} has been saved."));
                Variables.ID = (int)_TableClass.CurrentRow["ID"];
                AppRegistry.SetKey(UserName, "rcptDate", Variables.Vou_Date, KeyType.Date);
                AppRegistry.SetKey(UserName, "rcptMessage", $"Receipt {Variables.Vou_No} has been saved", KeyType.Text);
                return RedirectToPage("Receipt", routeValues: new { ID = Variables.ID });
            }

        }


        #endregion

        #region Send Email
        public async Task SendEmailAsync()
        {
            await Task.Run(async () => { await Task.Delay(1000); });
        }
        #endregion

        #region Print
        public IActionResult OnPostPrint(ReportType _ReportType)
        {
            var _ShowImages = true;
            if (_ReportType == ReportType.Preview) { _ShowImages = false; }

            AppRegistry.SetKey(UserName, "rcptID", Variables.ID, KeyType.Text);
            AppRegistry.SetKey(UserName, "rcptHead2", Variables.Vou_No, KeyType.Text);
            AppRegistry.SetKey(UserName, "rcptShowImg", !_ShowImages, KeyType.Boolean);

            return RedirectToPage("../ReportPrint/PrintReport", "Receipt", new { RptType = _ReportType });
        }
        #endregion

        #region New Receipt
        public IActionResult OnPostNew()
        {
            return RedirectToPage(0);
        }
        #endregion

        #region Back Button
        public IActionResult OnPostBack()
        {
            return RedirectToPage("Receiptlist");
        }
        #endregion

        #region Delete Button
        public IActionResult OnPostDelete()
        {
            if (Variables.Status == "Submitted")
            {
                var _TableClass = new DataTableClass(UserName, Tables.Receipts);
                _TableClass.Delete(Variables.ID);
                AppRegistry.SetKey(UserName, "rcptMessage", _TableClass.MyMessage, KeyType.Text);

                if (!_TableClass.IsError)
                {
                    return RedirectToPage(0);
                }
            }

            return Page();

        }
        #endregion

    }

    public class Parameter
    {
        public int ID { get; set; }
        public string Vou_No { get; set; } = "New";
        public DateTime Vou_Date { get; set; }
        public string Ref_No { get; set; } = string.Empty;
        public int COACash { get; set; }
        public int COA { get; set; }
        public int Project { get; set; }
        public int Employee { get; set; }
        public int Payer { get; set; }
        public decimal Amount { get; set; } = 0.00M;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

    }
}
