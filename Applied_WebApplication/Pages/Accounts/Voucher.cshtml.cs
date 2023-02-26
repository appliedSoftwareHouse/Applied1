using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text;

namespace Applied_WebApplication.Pages.Accounts
{
    public class VoucherModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }

        public DataTable Tb_Voucher = new DataTable();
        public bool IsTable { get; set; } = false;
        public List<Message> ErrorMessages { get; set; } = new();

        public void OnGet()
        {
        }

        public void OnGetEdit(int TranID, string VouType)
        {
            string UserName = User.Identity.Name;
            Variables = new MyParameters();
            DataTableClass _Table = new(UserName, Tables.Ledger);
            _Table.MyDataView.RowFilter = string.Concat("TranID=", TranID.ToString(), " AND Vou_Type='", VouType, "'");
            if (_Table.MyDataView.Count > 0)
            {
                Tb_Voucher = GetTempLedger(_Table.MyDataView.ToTable());
                SetVariables((int)Tb_Voucher.Rows[0]["ID"]);
            }


        }

        public IActionResult OnPostEdit(int id)
        {
            SetVariables(id);
            return Page();
        }

        public IActionResult OnPostBack(int id)
        {
            return RedirectToPage("./VouchersList");
        }

        public HttpRequest GetRequest()
        {
            return Request;
        }

        public IActionResult OnPostSave(int id, HttpRequest request)
        {
            string UserName = User.Identity.Name;
            DataTableClass _TempLedger = new(UserName, Tables.TempLedger);
            _TempLedger.MyDataView.RowFilter = string.Concat("Created='", Variables.Created, "'");
            Tb_Voucher = _TempLedger.MyDataView.ToTable();

            foreach (DataRow Row in Tb_Voucher.Rows)
            {
                _TempLedger.SeekRecord((int)Row["ID2"]);

                _TempLedger.CurrentRow["Vou_Date"] = DateTime.Parse(request.Form["Vou_Date"].ToString());
                _TempLedger.CurrentRow["Vou_No"] = request.Form["Vou_No"].ToString();
                _TempLedger.CurrentRow["Vou_Type"] = request.Form["Vou_Type"].ToString();
                _TempLedger.CurrentRow["Ref_No"] = request.Form["Ref_No"].ToString();
                _TempLedger.CurrentRow["BookID"] = int.Parse(request.Form["BookID"].ToString());

                if ((int)Row["ID2"] == id)
                {
                    _TempLedger.CurrentRow["DR"] = request.Form["DR"];
                    _TempLedger.CurrentRow["CR"] = request.Form["CR"].ToString();
                    _TempLedger.CurrentRow["Description"] = request.Form["Description"].ToString();
                    _TempLedger.CurrentRow["Comments"] = request.Form["Comments"].ToString();
                    _TempLedger.CurrentRow["COA"] = request.Form["COA"].ToString();
                    _TempLedger.CurrentRow["Customer"] = request.Form["Customer"].ToString();
                    _TempLedger.CurrentRow["Employee"] = request.Form["Employee"].ToString();
                    _TempLedger.CurrentRow["Project"] = request.Form["Project"].ToString();
                }

                _TempLedger.Save();
                if (_TempLedger.TableValidation.MyMessages.Count > 0)

                {
                    ErrorMessages.AddRange(_TempLedger.TableValidation.MyMessages);
                }

            }



            // Refresh Data after save changes.  ========================================================
            _TempLedger = new(UserName, Tables.TempLedger);
            _TempLedger.MyDataView.RowFilter = string.Concat("Created='", Variables.Created, "'");
            Tb_Voucher = _TempLedger.MyDataView.ToTable();

            return Page();
        }

        private void SetVariables(int id)
        {
            string UserName = User.Identity.Name;
            DataTableClass _TempLedger = new(UserName, Tables.TempLedger);
            _TempLedger.MyDataView.RowFilter = string.Concat("Created='", Variables.Created, "'");
            Tb_Voucher = _TempLedger.MyDataView.ToTable();
            DataView vw_Voucher = new DataView(_TempLedger.MyDataTable, "ID2=" + id.ToString(), "SR_NO", DataViewRowState.OriginalRows);

            if (vw_Voucher.Count > 0)
            {

                Variables.ID = (int)vw_Voucher[0]["ID2"];
                Variables.TranID = (int)vw_Voucher[0]["TranID"];
                Variables.VouType = vw_Voucher[0]["Vou_Type"].ToString();
                Variables.VouNo = vw_Voucher[0]["Vou_No"].ToString();
                Variables.VouDate = DateTime.Parse(vw_Voucher[0]["Vou_Date"].ToString());
                Variables.SR_No = (int)vw_Voucher[0]["SR_No"];
                Variables.RefNo = vw_Voucher[0]["Ref_No"].ToString();
                Variables.BookID = (int)vw_Voucher[0]["BookID"];
                Variables.Debit = (decimal)vw_Voucher[0]["DR"];
                Variables.Credit = (decimal)vw_Voucher[0]["CR"];
                Variables.Description = vw_Voucher[0]["Description"].ToString();
                Variables.Comments = vw_Voucher[0]["Comments"].ToString();

                Variables.COA = (int)vw_Voucher[0]["COA"];
                Variables.Employee = (int)vw_Voucher[0]["Employee"];
                Variables.Company = (int)vw_Voucher[0]["Customer"];
                Variables.Project = (int)vw_Voucher[0]["Project"];
            }
        }


        private DataTable GetTempLedger(DataTable _Temp)
        {
            string UserName = User.Identity.Name;
            DataTableClass _TempTable = new(UserName, Tables.TempLedger);
            Variables.Created = UserProfile.GetUserClaim(User, "AppSession");
            StringBuilder _Filter = new();

            // cfbc2411-8f8a-4dfa-b7ce-3b5f4912e780

            _Filter.Append("ID2=" + (int)_Temp.Rows[0]["ID"]);
            _Filter.Append(" AND TranID=" + (int)_Temp.Rows[0]["TranID"]);
            _Filter.Append(" AND Created='" + Variables.Created + "'");


            _TempTable.MyDataView.RowFilter = _Filter.ToString();

            if (_TempTable.MyDataView.Count > 0)
            {
                // Delete previous Voucher
                foreach (DataRow Row in _TempTable.MyDataView.ToTable().Rows)
                {
                    _TempTable.SeekRecord((int)Row["ID"]);                  // Search Record
                    _TempTable.Delete();                                                // Delete Record.
                }
            }

            foreach (DataRow Row in _Temp.Rows)
            {
                _TempTable.NewRecord();
                _TempTable.CurrentRow["ID"] = 0;
                _TempTable.CurrentRow["ID2"] = Row["ID"];
                _TempTable.CurrentRow["TranID"] = Row["TranID"];
                _TempTable.CurrentRow["Vou_Type"] = Row["Vou_Type"];
                _TempTable.CurrentRow["Vou_Date"] = Row["Vou_Date"];
                _TempTable.CurrentRow["Vou_No"] = Row["Vou_No"];
                _TempTable.CurrentRow["SR_NO"] = Row["SR_NO"];
                _TempTable.CurrentRow["Ref_No"] = Row["Ref_No"];
                _TempTable.CurrentRow["BookID"] = Row["BookID"];
                _TempTable.CurrentRow["COA"] = Row["COA"];
                _TempTable.CurrentRow["DR"] = Row["DR"];
                _TempTable.CurrentRow["CR"] = Row["CR"];
                _TempTable.CurrentRow["Customer"] = Row["Customer"];
                _TempTable.CurrentRow["Employee"] = Row["Employee"];
                _TempTable.CurrentRow["Inventory"] = Row["Inventory"];
                _TempTable.CurrentRow["Project"] = Row["Project"];
                _TempTable.CurrentRow["Description"] = Row["Description"];
                _TempTable.CurrentRow["Comments"] = Row["Comments"];
                _TempTable.CurrentRow["Created"] = Variables.Created;
                _TempTable.Save();
            }

            _TempTable.MyDataView.RowFilter = string.Concat("Created='", Variables.Created, "'");
            return _TempTable.MyDataView.ToTable();
        }

        public class MyParameters
        {
            public int ID { get; set; }
            public int ID2 { get; set; }
            public int TranID { get; set; }
            public string VouType { get; set; }
            public string VouNo { get; set; }
            public DateTime VouDate { get; set; }
            public int SR_No { get; set; }
            public string RefNo { get; set; }
            public int BookID { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
            public string Description { get; set; }
            public string Comments { get; set; }

            public int COA { get; set; }
            public int Company { get; set; }
            public int Employee { get; set; }
            public int Project { get; set; }
            public string Created { get; set; }
        }
    }
}
