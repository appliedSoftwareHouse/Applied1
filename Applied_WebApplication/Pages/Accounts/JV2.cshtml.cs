using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SQLite;

namespace Applied_WebApplication.Pages.Accounts
{
    public class JV2Model : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; } = new();
        public List<Message> Messages { get; set; } = new();
        public string UserName => User.Identity.Name;
        public string UserRole => UserProfile.GetUserRole(User);
        public DataTable Voucher { get; set; }
        public TempDBClass2 TempVoucher { get; set; }


        #region Get
        public void OnGet()
        {
            var _Vou_No = AppRegistry.GetText(UserName, "JV2VouNo");
            var _Filter = $"Vou_No='{_Vou_No}'";
            var _VoucherTable = DataTableClass.GetTable(UserName, Tables.Ledger, _Filter, "Sr_No");
            
            TempVoucher = new TempDBClass2(User, _VoucherTable, "JV2Temp");
            Voucher = TempVoucher.TempTable;
            GetVariables();
        }
        public void OnGetEdit(int ID)
        {
            #region Record insert and show here.
            TempVoucher = new TempDBClass2(User, "JV2Temp");
            Voucher = TempVoucher.TempTable;

            Voucher.DefaultView.RowFilter = $"ID={ID}";
            if (Voucher.DefaultView.Count == 1)
            {
                TempVoucher.CurrentRow = Voucher.DefaultView[0].Row;
                GetVariables();
            }
            #endregion




        }
        public void OnGetAdd()
        {
            TempVoucher = new TempDBClass2(User, "JV2Temp");
            var _FirstRow = TempVoucher.CurrentRow;
            var _SrNo = (int)TempVoucher.TempTable.Compute("MAX(Sr_No)", "");

            TempVoucher.NewRow();
            TempVoucher.CurrentRow["ID"] = 0;
            TempVoucher.CurrentRow["Vou_No"] = _FirstRow["Vou_No"]; ;
            TempVoucher.CurrentRow["Vou_Date"] = _FirstRow["Vou_Date"]; ;
            TempVoucher.CurrentRow["Vou_Type"] = _FirstRow["Vou_Type"];
            TempVoucher.CurrentRow["TranID"] = _FirstRow["TranID"];
            TempVoucher.CurrentRow["Sr_No"] = _SrNo + 1;
            TempVoucher.CurrentRow["COA"] = 0;
            TempVoucher.CurrentRow["Customer"] = 0;
            TempVoucher.CurrentRow["Employee"] = 0;
            TempVoucher.CurrentRow["Inventory"] = 0;
            TempVoucher.CurrentRow["DR"] = 0.00M;
            TempVoucher.CurrentRow["CR"] = 0.00M;
            TempVoucher.CurrentRow["Description"] = string.Empty;
            TempVoucher.CurrentRow["Comments"] = string.Empty;
            TempVoucher.CurrentRow["Status"] = "Insert";
            GetVariables();

            Voucher = TempVoucher.TempTable;

        }
        #endregion

        private void GetVariables()
        {
            TempVoucher.RemoveDBNull();
            var Row = TempVoucher.CurrentRow;

            if (Row is not null)
            {

                Variables = new();
                {
                    Variables.ID = (int)Row["ID"];
                    Variables.TranID = (int)Row["TranID"];
                    Variables.Vou_Type = (string)Row["Vou_Type"];
                    Variables.Vou_Date = (DateTime)Row["Vou_Date"];
                    Variables.Vou_No = (string)Row["Vou_No"];
                    Variables.Sr_No = (int)Row["Sr_No"];
                    Variables.Ref_No = (string)Row["Ref_No"];
                    Variables.BookID = (int)Row["BookID"];
                    Variables.COA = (int)Row["COA"];
                    Variables.DR = (decimal)Row["DR"];
                    Variables.CR = (decimal)Row["CR"];
                    Variables.Customer = (int)Row["Customer"];
                    Variables.Project = (int)Row["Project"];
                    Variables.Employee = (int)Row["Employee"];
                    Variables.Description = (string)Row["Description"];
                    Variables.Comments = (string)Row["Comments"];
                    Variables.Status = (string)Row["Status"];

                };
                TempVoucher.CurrentRow = Row;
            }
        }

        public void GetCurrentRow()
        {
            if (TempVoucher is null) { return; }
            if (TempVoucher.CurrentRow is null) { return; }

            TempVoucher.CurrentRow = TempVoucher.TempTable.NewRow();
            TempVoucher.CurrentRow["ID"] = Variables.ID;
            TempVoucher.CurrentRow["TranID"] = Variables.TranID;
            TempVoucher.CurrentRow["Vou_Type"] = Variables.Vou_Type;
            TempVoucher.CurrentRow["Vou_Date"] = Variables.Vou_Date;
            TempVoucher.CurrentRow["Vou_No"] = Variables.Vou_No;
            TempVoucher.CurrentRow["Sr_No"] = Variables.Sr_No;
            TempVoucher.CurrentRow["Ref_No"] = Variables.Ref_No;
            TempVoucher.CurrentRow["BookID"] = Variables.BookID;
            TempVoucher.CurrentRow["COA"] = Variables.COA;
            TempVoucher.CurrentRow["DR"] = Variables.DR;
            TempVoucher.CurrentRow["CR"] = Variables.CR;
            TempVoucher.CurrentRow["Customer"] = Variables.Customer;
            TempVoucher.CurrentRow["Project"] = Variables.Project;
            TempVoucher.CurrentRow["Employee"] = Variables.Employee;
            TempVoucher.CurrentRow["Description"] = Variables.Description;
            TempVoucher.CurrentRow["Comments"] = Variables.Comments;
            TempVoucher.CurrentRow["Status"] = Variables.Status;
        }



        #region Post Insert / Edit / Delete / UnDelete
        public IActionResult OnPostInsert()
        {
            return RedirectToPage("JV2", "Add");
        }

        public IActionResult OnPostEdit(int ID)
        {
            return RedirectToPage("JV2", "Edit", new { ID });               // Redirect / Refresh a page 
        }

        public IActionResult OnPostDelete(int ID)
        {
            if(UserRole=="Viewer") { return Page(); }

            TempVoucher = new(User, "JV2Temp");
            GetCurrentRow();
            SQLiteCommand? _Command;
            var _GUID = TempVoucher.TempGUID.ToString();
            var _RowFound = TempVoucher.GetRow(ID);

            if (_RowFound)
            {
                var _Row = TempVoucher.CurrentRow;
                var _VouNo = _Row["Vou_No"].ToString();
                var _SR_No = _Row["Sr_No"].ToString();

                if (ID > 0)
                {
                    _Row["Status"] = "Deleted";

                    _Command = TempVoucher.CreateUpdateCommand(_Row, _GUID);
                    var _Effacted = _Command.ExecuteNonQuery();
                    if (_Effacted > 0)
                    {
                        Messages.Add(MessageClass.SetMessage($"{_VouNo} Sr. No. {_SR_No} has been marked deleted."));
                    }
                    else
                    {
                        Messages.Add(MessageClass.SetMessage($"{_VouNo} Sr. No. {_SR_No} NOT marked deleted."));
                        return Page();
                    }
                }

                if (ID <= 0)
                {
                    _Command = TempVoucher.CreateDeleteCommand(_Row, _GUID);
                    var _Effacted = _Command.ExecuteNonQuery();
                    if (_Effacted > 0)
                    {
                        Messages.Add(MessageClass.SetMessage($"{_VouNo} Sr. No. {_SR_No} has been marked deleted."));
                    }
                    else
                    {
                        Messages.Add(MessageClass.SetMessage($"{_VouNo} Sr. No. {_SR_No} NOT marked deleted."));
                        return Page();
                    }
                }
            }

            return RedirectToPage("JV2", "Edit", new { ID });
        }

        public IActionResult OnPostUnDelete(int ID)
        {
            if (UserRole == "Viewer") { return Page(); }

            TempVoucher = new(User, "JV2Temp");
            GetCurrentRow();
            SQLiteCommand? _Command;
            var _GUID = TempVoucher.TempGUID.ToString();
            var _RowFound = TempVoucher.GetRow(ID);
            if (_RowFound)
            {
                var _Row = TempVoucher.CurrentRow;
                var _VouNo = _Row["Vou_No"].ToString();
                var _SR_No = _Row["Sr_No"].ToString();
                _Row["Status"] = "Updated";

                _Command = TempVoucher.CreateUpdateCommand(_Row, _GUID);
                var _Effacted = _Command.ExecuteNonQuery();
                if (_Effacted > 0)
                {
                    Messages.Add(MessageClass.SetMessage($"{_VouNo} Sr. No. {_SR_No} has been Recovered."));
                }
                else
                {
                    Messages.Add(MessageClass.SetMessage($"{_VouNo} Sr. No. {_SR_No} NOT Recovered."));
                }

            }

            return RedirectToPage("JV2", "Edit", new { ID });

        }

        #endregion

        #region Save
        public IActionResult OnPostSave()
        {
            if (UserRole == "Viewer") { return Page(); }
            Messages = new();
            TempVoucher = new TempDBClass2(User, "JV2Temp");
            GetCurrentRow();

            var _Row = TempVoucher.CurrentRow;
            var _VouNo = _Row["Vou_No"].ToString();
            var _SR_No = _Row["Sr_No"].ToString();

            var _GUID = TempVoucher.TempGUID.ToString();
            if (_Row["Status"].ToString() == "Insert")
            {
                var _Command = TempVoucher.CreateInsertCommand(_Row, _GUID);
                var _Effacted = _Command.ExecuteNonQuery();
                if (_Effacted > 0)
                {
                    Messages.Add(MessageClass.SetMessage($"{_VouNo} Sr. No. {_SR_No} has been saved."));
                }
                else
                {
                    Messages.Add(MessageClass.SetMessage($"{_VouNo} Sr. No. {_SR_No} NOT saved."));
                }
            }

            if (_Row["Status"].ToString() == "Submitted" || _Row["Status"].ToString() == "Updated")
            {
                var _Command = TempVoucher.CreateUpdateCommand(_Row, _GUID);
                var _Effacted = _Command.ExecuteNonQuery();
                if (_Effacted > 0)
                {
                    Messages.Add(MessageClass.SetMessage($"{_VouNo} Sr. No. {_SR_No} has been Updated."));
                }
                else
                {
                    Messages.Add(MessageClass.SetMessage($"{_VouNo} Sr. No. {_SR_No} NOT Updated."));
                }
            }


            Voucher = TempVoucher.LoadTempTable();
            return Page();
        }

        public IActionResult OnPostSaveVoucher()
        {
            TempVoucher = new TempDBClass2(User, "JV2Temp");
            TempVoucher.SourceTableID = Tables.Ledger;


            Voucher = TempVoucher.TempTable;
            var IsSaved = TempVoucher.SaveToSource();

            return Page();
        }

        #endregion


        #region Back
        public IActionResult OnPostBack()
        {
            return RedirectToPage("./JVList");
        }
        #endregion

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
            public string Status { get; set; }

        }

    }
}
