using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Stock
{
    public class OBStockModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public bool IsError = false;
        public List<Message> ErrorMessages = new();
        public DataTable OBStock;
        public string UserName => User.Identity.Name;

        public void OnGet(int? ID)
        {
            DataTableClass _Table = new(UserName, Tables.OBALStock);
            if (ID == null)
            {
                Variables = new()
                {
                    ID = 0,
                    Inventory = 0,
                    Batch = string.Empty,
                    Project = 0,
                    Qty = 0.00M,
                    Rate = 0,
                    Amount = 0.00M
                };
            }
            else
            {
                _Table.MyDataView.RowFilter = string.Concat("ID=", ID.ToString());
                if (_Table.Count == 1)
                {
                    _Table.SeekRecord((int)ID);
                    Variables = new()
                    {
                        ID = (int)_Table.CurrentRow["ID"],
                        Inventory = (int)_Table.CurrentRow["Inventory"],
                        Batch = _Table.CurrentRow["Batch"].ToString(),
                        Project = (int)_Table.CurrentRow["Project"],
                        Qty = (decimal)_Table.CurrentRow["Qty"],
                        Rate = (decimal)_Table.CurrentRow["Rate"],
                        Amount = (decimal)_Table.CurrentRow["Amount"]
                    };
                }
                else
                {
                    Variables = new()
                    {
                        ID = 0,
                        Inventory = 0,
                        Batch = string.Empty,
                        Project = 0,
                        Qty = 0.00M,
                        Rate = 0,
                        Amount = 0.00M
                    };
                }
            }

            OBStock = _Table.MyDataTable;

        }

        public async Task<IActionResult> OnPostSaveAsync(int ID)
        {
            DataTableClass OBStock = new(UserName, Tables.OBALStock);
            OBStock.MyDataView.RowFilter = string.Concat("ID=", ID.ToString());
            if (OBStock.Count == 1)
            { OBStock.SeekRecord(ID); }
            else { OBStock.NewRecord(); }
            OBStock.CurrentRow["ID"] = Variables.ID;
            OBStock.CurrentRow["Inventory"] = Variables.Inventory;
            OBStock.CurrentRow["Project"] = Variables.Project;
            OBStock.CurrentRow["Batch"] = Variables.Batch;
            OBStock.CurrentRow["Qty"] = Variables.Qty;
            OBStock.CurrentRow["Rate"] = Variables.Rate;
            OBStock.CurrentRow["Amount"] = (Variables.Qty * Variables.Rate);
            await Task.Run(() => OBStock.Save());
            ErrorMessages = OBStock.TableValidation.MyMessages;
            if (ErrorMessages.Count > 0) { return Page(); }
            return RedirectToPage("OBStock", routeValues: new { ID });
        }

        public IActionResult OnPostDelete(int ID)
        {
            DataTableClass OBStock = new(UserName, Tables.OBALStock);
            OBStock.MyDataView.RowFilter = string.Concat("ID=", ID.ToString());
            if (OBStock.Count == 1)
            {
                OBStock.SeekRecord(ID);
                OBStock.Delete();
                ErrorMessages.Add(MessageClass.SetMessage(OBStock.MyMessage));
            }

            return RedirectToPage("OBStock");
        }

        #region Back
        public IActionResult OnPostBack()
        {

            return RedirectToPage("../Index");
        }
        #endregion

        #region MyParameters Variables
        public class MyParameters
        {
            public int ID { get; set; }
            public int Inventory { get; set; }
            public string Batch { get; set; }
            public int Project { get; set; }
            public decimal Qty { get; set; }
            public decimal Rate { get; set; }
            public decimal Amount { get; set; }

        }
        #endregion
    }
}