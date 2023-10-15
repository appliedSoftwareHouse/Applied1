using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Drawing;

namespace Applied_WebApplication.Pages.Stock
{
    public class BOMProfileListModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable BOMProfileList { get; set; }
        public List<Message> ErrorMessages { get; set; }
        public string UserName => User.Identity.Name;


        public void OnGet()
        {

        }

        public void OnGet(int? ID)
        {
            DataTableClass _Table = new(UserName, Tables.BOMProfile);
            BOMProfileList = _Table.MyDataTable;
            ErrorMessages = new();
            Variables = new();

            if (ID != null)
            {
                _Table.MyDataView.RowFilter = "ID=" + ID.ToString();
                if (_Table.Count == 1)
                {
                    _Table.SeekRecord((int)ID);
                    Variables = new()
                    {
                        ID = (int)_Table.CurrentRow["ID"],
                        Code = (string)_Table.CurrentRow["Code"],
                        Title = (string)_Table.CurrentRow["Title"],
                        Status = (string)_Table.CurrentRow["Status"]
                    };

                }
            }
        }

        public IActionResult OnPostNew()
        {
            return RedirectToPage("BOMProfileList");

        }
        public async Task<IActionResult> OnPostSaveAsync(int ID)
        {
            //ErrorMessages = Save(int ID);
            await Task.Run(() => Save(ID));

            if(ErrorMessages.Count> 0) { return Page(); }
            return RedirectToPage("BOMProfileList", routeValues: new { ID });

        }
        public IActionResult OnPostEdit(int ID)
        {
            return RedirectToPage("BOMProfileList", new { ID });

        }
        public IActionResult OnPostDelete(int ID)
        {
            ErrorMessages = new();
            DataTableClass _Table = new(UserName, Tables.BOMProfile);
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

            return RedirectToPage("BOMProfileList");
        }
        public IActionResult OnPostBOM(int ID)
        {
            return RedirectToPage("BOMProfile", new { ID = 0, TranID = ID });
        }


        #region Save Method

        private void Save(int ID)
        {
            ErrorMessages = new();
            DataTableClass _Table = new(UserName, Tables.BOMProfile);
            _Table.MyDataView.RowFilter = "ID=" + ID.ToString();
            if (_Table.Count == 1) { _Table.SeekRecord(ID); } else { _Table.NewRecord(); }

            try
            {
                _Table.CurrentRow["ID"] = Variables.ID;
                _Table.CurrentRow["Code"] = Variables.Code;
                _Table.CurrentRow["Title"] = Variables.Title;
                _Table.CurrentRow["Status"] = Variables.Status;
                _Table.Save();
                ErrorMessages = _Table.TableValidation.MyMessages;

            }
            catch (Exception e)
            {
                ErrorMessages.Add(MessageClass.SetMessage(e.Message, Color.Red));
            }

        }

        #endregion


        public class MyParameters
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string Title { get; set; }
            public string Status { get; set; } = "Active";

        }
    }
}
