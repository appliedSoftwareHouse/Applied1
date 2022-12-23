using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Applied_WebApplication.Data;
using static Applied_WebApplication.Data.TableValidationClass;

namespace Applied_WebApplication.Pages.Stock
{
    public class InventoryModel : PageModel
    {
        public Inventory Record = new();
        public bool IsError { get; set; }
        public List<Message> ErrorMessages;
        public string MyPageAction { get; set; } = "Add";
        

        public void OnGet()
        {
        }

        public void OnGetEdit(string UserName, int id)
        {
            MyPageAction = "Edit";
            DataTableClass Inventory = new DataTableClass(UserName, Tables.Inventory);
            Inventory.SeekRecord(id);

            Record.ID = (int)Inventory.CurrentRow["ID"];
            Record.Code = (string)Inventory.CurrentRow["Code"];
            Record.Title = (string)Inventory.CurrentRow["Title"];
            Record.SubCategory = (int)Inventory.CurrentRow["SubCategory"];
            Record.UOM = (int)Inventory.CurrentRow["UOM"];
            Record.Notes = (string)Inventory.CurrentRow["Notes"];

        }

        public void OnGetAdd(string UserName)
        {
            MyPageAction = "Add";
            DataTableClass Inventory = new DataTableClass(UserName, Tables.Inventory);
            Inventory.NewRecord();

            Record.ID = (int)Inventory.CurrentRow["ID"];
            Record.Code = (string)Inventory.CurrentRow["Code"];
            Record.Title = (string)Inventory.CurrentRow["Title"];
            Record.SubCategory = (int)Inventory.CurrentRow["SubCategory"];
            Record.UOM = (int)Inventory.CurrentRow["UOM"];
            Record.Notes = (string)Inventory.CurrentRow["Notes"];

        }


        public void OnPostSave(Inventory _Record, string UserName)
        {
            DataTableClass Inventory = new(UserName, Tables.Inventory);

            if (Inventory.Seek(_Record.ID))
            {
                Inventory.SeekRecord(_Record.ID);
                Inventory.CurrentRow["ID"] = _Record.ID;
            }
            else
            {
                Inventory.NewRecord();
                Inventory.CurrentRow["ID"] = 0;
            }
            Inventory.CurrentRow["Code"] = _Record.Code;
            Inventory.CurrentRow["Title"] = _Record.Title;
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
