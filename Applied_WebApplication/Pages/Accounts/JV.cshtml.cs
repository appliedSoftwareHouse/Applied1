using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Accounts
{
    public class JVModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public List<Message> ErrorMessages { get; set; } = new();

        public string UserName => User.Identity.Name;
        public DataTable Voucher { get; set; }
        public Boolean IsBalance { get; set; }

        public void OnGetNew()
        {
            ErrorMessages = new();
            TempTableClass TempClass;
            Variables = new();              // Setup of Variables

            TempClass = new TempTableClass(UserName, Tables.Ledger);
            if (TempClass.TempTable.Rows.Count == 0)
            {
                TempClass.CurrentRow = TempClass.NewRecord();
                TempClass.CurrentRow["ID"] = 0;
                TempClass.CurrentRow["TranID"] = 0;
                TempClass.CurrentRow["Vou_No"] = "NEW";
                TempClass.CurrentRow["Vou_Date"] = DateTime.Now;
                TempClass.CurrentRow["Sr_No"] = 1;
                TempClass.CurrentRow["Status"] = VoucherStatus.Submitted;
                TempClass.CurrentRow["Vou_Type"] = VoucherType.JV;
            }
            Row2Variable(TempClass.CurrentRow);
            Voucher = TempClass.TempTable;

        }


        public void OnGet(string? Vou_No, int? Sr_No)
        {
            ErrorMessages = new();
            try
            {
                TempTableClass TempClass;
                Variables = new();              // Setup of Variables

                if (Vou_No == null || Vou_No == "New")
                {

                    TempClass = new TempTableClass(UserName, Tables.Ledger);
                    if (TempClass.TempTable.Rows.Count == 0)
                    {
                        TempClass.CurrentRow = TempClass.NewRecord();
                        TempClass.CurrentRow["ID"] = 0;
                        TempClass.CurrentRow["TranID"] = 0;
                        TempClass.CurrentRow["Vou_No"] = "NEW";
                        TempClass.CurrentRow["Vou_Date"] = DateTime.Now;
                        TempClass.CurrentRow["Sr_No"] = 1;
                        TempClass.CurrentRow["Status"] = VoucherStatus.Submitted;
                        TempClass.CurrentRow["Vou_Type"] = VoucherType.JV;
                    }
                    Row2Variable(TempClass.CurrentRow);
                }
                else
                {
                    TempClass = new TempTableClass(UserName, Tables.Ledger, Vou_No, true);
                    if (Sr_No == -1)
                    {
                        if (TempClass.TempTable.Rows.Count > 0)
                        {
                            TempClass.CurrentRow = TempClass.NewRecord();
                            TempClass.CurrentRow["ID"] = 0;
                            TempClass.CurrentRow["TranID"] = TempClass.TempTable.Rows[0]["TranID"];
                            TempClass.CurrentRow["Vou_Type"] = TempClass.TempTable.Rows[0]["Vou_Type"];
                            TempClass.CurrentRow["Vou_No"] = TempClass.TempTable.Rows[0]["Vou_No"];
                            TempClass.CurrentRow["Vou_Date"] = TempClass.TempTable.Rows[0]["Vou_Date"];
                            TempClass.CurrentRow["Sr_No"] = MaxSrNo(TempClass.TempTable);
                            TempClass.CurrentRow["Status"] = VoucherStatus.Submitted.ToString();
                            Row2Variable(TempClass.CurrentRow);
                        }
                    }
                    else
                    {
                        Sr_No ??= 1;
                        var _DataView = TempClass.TempView;
                        _DataView.RowFilter = string.Format("SR_No={0}", (int)Sr_No);
                        if (_DataView.Count == 1)
                        {
                            TempClass.CurrentRow = _DataView[0].Row;
                            Row2Variable(TempClass.CurrentRow);
                        }
                    }
                }

                Voucher = TempClass.TempTable;
                if (Voucher.Rows.Count > 0)
                {
                    var _DR = (decimal)Voucher.Compute("SUM(DR)", "");
                    var _CR = (decimal)Voucher.Compute("SUM(CR)", "");
                    if (_DR == _CR) { IsBalance = true; } else { IsBalance = false; }
                }

            }
            catch (Exception e)
            {
                ErrorMessages.Add(MessageClass.SetMessage(e.Message));
            }

        }


        #region Methods

        private void Row2Variable(DataRow Row)
        {
            Variables.ID = (int)Row["ID"];
            Variables.TranID = (int)Row["TranID"];
            Variables.Vou_Type = (string)Row["Vou_Type"];
            Variables.Vou_Date = (DateTime)Row["Vou_Date"];
            Variables.Vou_No = (string)Row["Vou_No"];
            Variables.Sr_No = (int)Row["Sr_No"];
            Variables.Ref_No = (string)Row["Ref_No"];
            Variables.COA = (int)Row["COA"];
            Variables.DR = (decimal)Row["DR"];
            Variables.CR = (decimal)Row["CR"];
            Variables.Customer = (int)Row["Customer"];
            Variables.Project = (int)Row["Project"];
            Variables.Employee = (int)Row["Employee"];
            Variables.Inventory = (int)Row["Inventory"];
            Variables.Description = (string)Row["Description"];
            Variables.Comments = (string)Row["Comments"];
            Variables.Status = (string)Row["Status"];

        }

        private static int MaxSrNo(DataTable _Table)
        {
            int _MaxNo = (int)_Table.Compute("MAX(Sr_No)", "");
            return _MaxNo + 1;
        }

        private string NewVoucherNo(DateTime _Date)
        {
            string NewNum;
            DataView VouMax = (DataTableClass.GetTable(UserName, Tables.VouMax_JV)).AsDataView();
            var _Year = _Date.ToString("yy");
            var _Month = _Date.ToString("MM");
            VouMax.RowFilter = string.Concat("Vou_No like '", "JV", _Year, _Month, "%'");
            DataTable _VouList = VouMax.ToTable();
            var MaxNum = _VouList.Compute("Max(num)", "");
            if (MaxNum == DBNull.Value)
            {
                NewNum = string.Concat("JV", _Year, _Month, "-", "0001");
            }
            else
            {
                var _MaxNum = int.Parse(MaxNum.ToString()) + 1;
                NewNum = string.Concat("JV", _Year, _Month, "-", _MaxNum.ToString("0000"));
            }
            return NewNum;
        }
        #endregion

        #region POST

        public IActionResult OnPostSave()
        {
            #region Variables Setup

            Variables.Ref_No ??= string.Empty;
            Variables.Description ??= string.Empty;
            Variables.Comments ??= string.Empty;
            Variables.Vou_Type ??= string.Empty;
            Variables.Vou_No ??= string.Empty;
            ErrorMessages = new();
            #endregion

            var TempClass = new TempTableClass(UserName, Tables.Ledger, Variables.Vou_No, true);
            var _View = TempClass.TempView;
            _View.RowFilter = string.Format("Sr_No={0}", Variables.Sr_No);
            if (_View.Count == 1) { TempClass.CurrentRow = _View[0].Row; }
            else
            {
                TempClass.CurrentRow = TempClass.NewRecord();
            }

            TempClass.CurrentRow["ID"] = Variables.ID;
            TempClass.CurrentRow["TranID"] = Variables.TranID;
            TempClass.CurrentRow["Vou_Type"] = Variables.Vou_Type;
            TempClass.CurrentRow["Vou_Date"] = Variables.Vou_Date;
            TempClass.CurrentRow["Vou_No"] = Variables.Vou_No;
            TempClass.CurrentRow["Sr_No"] = Variables.Sr_No;
            TempClass.CurrentRow["Ref_No"] = Variables.Ref_No;
            TempClass.CurrentRow["COA"] = Variables.COA;
            TempClass.CurrentRow["DR"] = Variables.DR;
            TempClass.CurrentRow["CR"] = Variables.CR;
            TempClass.CurrentRow["Customer"] = Variables.Customer;
            TempClass.CurrentRow["Employee"] = Variables.Employee;
            TempClass.CurrentRow["Project"] = Variables.Project;
            TempClass.CurrentRow["Inventory"] = Variables.Inventory;
            TempClass.CurrentRow["Description"] = Variables.Description;
            TempClass.CurrentRow["Comments"] = Variables.Comments;
            TempClass.CurrentRow["Status"] = Variables.Status;
            TempClass.Save();
            ErrorMessages.AddRange(TempClass.ErrorMessages);
            if (TempClass.ErrorMessages.Count > 0)
            {
                return Page();
            }

            var Vou_No = TempClass.CurrentRow["Vou_No"].ToString();
            var Sr_No = TempClass.CurrentRow["Sr_No"].ToString();


            return RedirectToPage("JV", routeValues: new { Vou_No, Sr_No });
        }
        public IActionResult OnPostAdd(string Vou_No, int Sr_No)
        {
            return RedirectToPage("JV", routeValues: new { Vou_No, Sr_No });
        }
        public IActionResult OnPostEdit(string Vou_No, int Sr_No)
        {

            return RedirectToPage("JV", routeValues: new { Vou_No, Sr_No });

        }
        public IActionResult OnPostBack()
        {
            return RedirectToPage("../Index");
        }
        public IActionResult OnPostSaveVoucher(string Vou_No)
        {

            #region Voucher is null
            ErrorMessages = new();
            if (Vou_No == null)
            {
                ErrorMessages.Add(MessageClass.SetMessage("Voucher Number is not define. Value is null.", Color.Red));
                ErrorMessages.Add(MessageClass.SetMessage("Voucher not saved,"));
            }
            #endregion


            TempTableClass TempClass = new TempTableClass(UserName, Tables.Ledger, Vou_No, true);
            DataTableClass SourceClass = new(UserName, Tables.Ledger);

            var Voucher = TempClass.TempTable;

            if (Voucher.Rows.Count > 0)
            {
                var _DR = (decimal)Voucher.Compute("SUM(DR)", "");
                var _CR = (decimal)Voucher.Compute("SUM(CR)", "");
                if (_DR.Equals(_CR))
                {
                    #region of Temp Voucher Validation (Pending)
                    { }
                    #endregion

                    #region Deletion of Source Voucher  Delete all existing voucher entries for update new entries from temp Voucher.
                    SourceClass.MyDataView.RowFilter = string.Format("Vou_No='{0}'", Vou_No);
                    if (SourceClass.MyDataView.Count > 0)
                    {
                        string CommandText = "DELETE FROM [Ledger] WHERE Vou_No=" + Vou_No;
                        SQLiteCommand Command = new(CommandText, SourceClass.MyConnection);
                        Command.ExecuteNonQuery();
                    }
                    #endregion

                    #region SAVE VOUCHER
                    string NewVouNo = Vou_No;
                    int NewTranID = 0;
                    if (Vou_No == "NEW")
                    {
                        NewVouNo = NewVoucherNo((DateTime)Voucher.Rows[0]["Vou_Date"]);
                        NewTranID = SourceClass.GetMaxTranID(VoucherType.JV);
                    }

                    foreach (DataRow Row in Voucher.Rows)
                    {
                        if ((string)Row["Vou_No"] == "NEW")
                        {
                            Row["ID"] = 0;
                            Row["Vou_No"] = NewVouNo;
                            Row["TranID"] = NewTranID;
                        }

                        SourceClass.CurrentRow = Row;
                        SourceClass.Save();
                        ErrorMessages.AddRange(SourceClass.TableValidation.MyMessages);
                    }

                    if (ErrorMessages.Count == 0)
                    {
                        TempClass = new TempTableClass(UserName, Tables.Ledger, Vou_No, true);

                        foreach (DataRow Row in TempClass.TempTable.Rows)
                        {
                            TempClass.CurrentRow = Row;
                            TempClass.Delete();
                        }
                    }
                    #endregion
                }
            }

            return RedirectToPage("JVList");
        }
        public IActionResult OnPostDelete(int ID)
        {
            int Sr_No;
            TempTableClass _TempTable = new(UserName, Tables.Ledger, Variables.Vou_No, true);
            _TempTable.TempSeek(ID);
            _TempTable.Delete();
            ErrorMessages.AddRange(_TempTable.ErrorMessages);
            if (ErrorMessages[0].ErrorID == 0)
            { Sr_No = Variables.Sr_No; }
            else
            { Sr_No = 1; }

            string Vou_No = Variables.Vou_No;

            return RedirectToPage("JV", new { Vou_No, Sr_No });
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
