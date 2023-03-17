using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Stock
{
    public class BOMProfileModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable BOMProfile { get; set; }
        public List<Message> ErrorMessages { get; set; }
        public string UserName => User.Identity.Name;
        public void OnGet(int? ID, int? TranID)
        {
            Variables = new(); ID ??= 0; ErrorMessages =  new();
            DataTableClass _Table = new(UserName, Tables.BOMProfile2);
            
            if (TranID != null)
            {
                _Table.MyDataView.RowFilter = string.Concat("TranID=", ID.ToString());
                if (_Table.Count > 1)
                {
                    BOMProfile = _Table.MyDataView.ToTable();

                    // Seek record in the sepcific Tran ID of BOM
                    _Table.MyDataView.RowFilter = string.Format("TranID={0} AND ID={1}", (int)TranID, (int)ID);
                    if(_Table.Count==1) { _Table.SeekRecord((int)ID); } else { _Table.NewRecord(); }
                    
                    Variables = new()
                    {
                        ID = (int)_Table.CurrentRow["ID"],
                        TranID = (int)_Table.CurrentRow["TranID"],
                        IN_OUT = (string)_Table.CurrentRow["IN_OUT"],
                        UOM = (int)_Table.CurrentRow["UOM"],
                        Qty = (int)_Table.CurrentRow["Qty"],
                        Rate = (int)_Table.CurrentRow["Rate"],
                        Westage = (int)_Table.CurrentRow["westage"]
                    };

                }
            }

        }


        #region Reset and Back
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
