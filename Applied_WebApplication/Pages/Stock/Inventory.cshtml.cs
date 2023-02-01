using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Applied_WebApplication.Data;
using static Applied_WebApplication.Data.TableValidationClass;

namespace Applied_WebApplication.Pages.Stock
{
    public class InventoryModel : PageModel
    {
        [BindProperty]
        public Inventory Record { get; set; } 
        public bool IsError { get; set; }
        public List<Message> ErrorMessages = new();
        public int ErrorCount { get => ErrorMessages.Count; }

        public void OnGet()
        {
            Record = new();
        }

        public void OnGetEdit(int id)
        {
            string UserName = User.Identity.Name;
            ErrorMessages = new();
            DataTableClass Inventory = new DataTableClass(UserName, Tables.Inventory);
            Inventory.SeekRecord(id);
            Record = new()
            {
                ID = (int)Inventory.CurrentRow["ID"],
                Code = (string)Inventory.CurrentRow["Code"],
                Title = (string)Inventory.CurrentRow["Title"],
                SubCategory = (int)Inventory.CurrentRow["SubCategory"],
                UOM = (int)Inventory.CurrentRow["UOM"],
                Notes = (string)Inventory.CurrentRow["Notes"]
            };

        }

        public IActionResult OnGetAdd(string UserName)
        {
            DataTableClass Inventory = new DataTableClass(UserName, Tables.Inventory);
            Inventory.NewRecord();

            Record = new()
            {
                ID = 0,
                Code = string.Empty,
                Title = string.Empty,
                SubCategory = 0,
                UOM = 0,
                Notes = string.Empty,
            };

            return Page();
        }


        public IActionResult OnPostSave(int id)
        {
            string UserName = User.Identity.Name;
            DataTableClass Inventory = new(UserName, Tables.Inventory);

            if (Inventory.Seek(id)) {Inventory.SeekRecord(id);}
            else {Inventory.NewRecord(); }

            Inventory.CurrentRow["ID"] = Record.ID; ;
            Inventory.CurrentRow["Code"] = Record.Code;
            Inventory.CurrentRow["Title"] = Record.Title;
            Inventory.CurrentRow["SubCategory"] = Record.SubCategory;
            Inventory.CurrentRow["UOM"] = Record.UOM;
            Inventory.CurrentRow["Notes"] = Record.Notes;
            Inventory.Save();
            ErrorMessages = Inventory.TableValidation.MyMessages;

            if(ErrorCount>0) { return Page(); }
            else { return RedirectToPage("./Inventories"); }


        }


        public class Inventory
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string Title { get; set; }
            public int SubCategory { get; set; }
            public int UOM { get; set; }
            public string Notes { get; set; }
        }
    }
}
