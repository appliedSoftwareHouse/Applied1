using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Drawing;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Stock
{
    public class BOMProfileModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable BOMProfile { get; set; }
        public List<Message> ErrorMessages { get; set; }
        public string UserName => User.Identity.Name;
        private int PageID, PageTranID;

        public void OnGet(int? ID, int? TranID)
        {
            Variables = new(); ID ??= 0; ErrorMessages = new();
            DataTableClass _Table = new(UserName, Tables.BOMProfile2);

            if (TranID != null)
            {
                _Table.MyDataView.RowFilter = string.Concat("TranID=", TranID.ToString());
                if (_Table.Count > 0)
                {
                    BOMProfile = _Table.MyDataView.ToTable();
                    if (ID == 0) { ID = (int)BOMProfile.Rows[0]["ID"]; }
                    if (ID == -1) { ID = 0; }                                                      // Parameter Values get from Method NEW()


                    // Seek record in the sepcific Tran ID of BOM
                    _Table.MyDataView.RowFilter = string.Format("TranID={0} AND ID={1}", (int)TranID, (int)ID);
                    if (_Table.Count == 1) { _Table.SeekRecord((int)ID); }
                    else
                    {
                        _Table.NewRecord();
                        _Table.CurrentRow["TranID"] = (int)TranID;
                    }

                    Variables = new()
                    {
                        ID = (int)_Table.CurrentRow["ID"],
                        TranID = (int)_Table.CurrentRow["TranID"],
                        IN_OUT = (string)_Table.CurrentRow["IN_OUT"],
                        Inventory = (int)_Table.CurrentRow["Inventory"],
                        UOM = (int)_Table.CurrentRow["UOM"],
                        Qty = (decimal)_Table.CurrentRow["Qty"],
                        Rate = (decimal)_Table.CurrentRow["Rate"],
                        Westage = (decimal)_Table.CurrentRow["Westage"]
                    };

                    if ((int)_Table.CurrentRow["TranID"] == 0)
                    {
                        ErrorMessages.Add(MessageClass.SetMessage("TranID is zero. Contact to Administrtor"));
                    }
                }
                else { Variables.TranID = (int)TranID; }
            }

        }

        public async Task<IActionResult> OnPostSaveAsync(int ID)
        {
            await Task.Run(() => Save(ID));
            if (ErrorMessages.Count > 0) { return Page(); }
            return RedirectToPage("BOMProfile", routeValues: new { ID = PageID , TranID = PageTranID });
        }


        private void Save(int ID)
        {
            DataTableClass _Table = new(UserName, Tables.BOMProfile2);
            if (_Table.Seek(ID)) { _Table.SeekRecord(ID); } else { _Table.NewRecord(); }
            _Table.CurrentRow["ID"] = Variables.ID;
            _Table.CurrentRow["TranID"] = Variables.TranID;
            _Table.CurrentRow["IN_OUT"] = Variables.IN_OUT;
            _Table.CurrentRow["Inventory"] = Variables.Inventory;
            _Table.CurrentRow["UOM"] = Variables.UOM;
            _Table.CurrentRow["Qty"] = Variables.Qty;
            _Table.CurrentRow["Rate"] = Variables.Rate;
            _Table.CurrentRow["Westage"] = Variables.Westage;
            _Table.Save();
            ErrorMessages = _Table.TableValidation.MyMessages;

            PageID = ID;
            PageTranID = Variables.TranID;

        }

        #region Reset and Back
        public IActionResult OnPostNew()
        {

            return RedirectToPage("BOMProfile", routeValues: new { ID = -1, TranID = Variables.TranID });
        }

        public IActionResult OnPostEdit(int ID)
        {

            return RedirectToPage("BOMProfile", routeValues: new { ID, TranID = Variables.TranID });
        }

        public IActionResult OnPostDelete(int ID)
        {
            ErrorMessages = new();
            DataTableClass _Table = new(UserName, Tables.BOMProfile2);
            _Table.MyDataView.RowFilter = "ID=" + ID.ToString();
            if (_Table.Count == 1)
            {
                try
                {
                    _Table.SeekRecord(ID);
                    _Table.Delete();
                    ErrorMessages.Add(MessageClass.SetMessage("Record has been deleted sucessfully.", Color.Yellow));
                }
                catch (Exception e)
                {
                    ErrorMessages.Add(MessageClass.SetMessage(e.Message, Color.Red));
                }
            }
            ErrorMessages = _Table.TableValidation.MyMessages;
            if (ErrorMessages.Count > 0) { return Page(); }

            return RedirectToPage("BOMProfile", routeValues: new { ID = 1, TranID = Variables.TranID });
        }


        public IActionResult OnPostReset()
        {
            return RedirectToPage(BOMProfile);
        }

        public IActionResult OnPostBack()
        {
            return RedirectToPage("BOMProfileList");
        }

        #endregion
        public class MyParameters
        {
            public int ID { get; set; }
            public int TranID { get; set; }
            public string IN_OUT { get; set; }
            public int Inventory { get; set; }
            public int UOM { get; set; }
            public decimal Qty { get; set; }
            public decimal Rate { get; set; }
            public decimal Westage { get; set; }
        }
    }
}
