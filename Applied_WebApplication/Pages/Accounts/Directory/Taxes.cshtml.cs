using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Accounts.Directory
{
    [Authorize]
    public class TaxesModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public DataTable TB_Taxes { get; set; }
        public string UserName => User.Identity.Name;
        public List<Message> MyMessages { get; set; }

        public void OnGet()
        {
            Variables = new();
            MyMessages = new();
            var TaxesClass = new DataTableClass(UserName, Tables.Taxes);
            TB_Taxes = TaxesClass.MyDataTable;
        }

        public void OnGetEdit(int ID)
        {
            MyMessages = new();
            var TaxesClass = new DataTableClass(UserName, Tables.Taxes);
            TB_Taxes = TaxesClass.MyDataTable;
            TaxesClass.SeekRecord(ID);
            Variables = new()
            {
                ID = (int)TaxesClass.CurrentRow["ID"],
                Code = (string)TaxesClass.CurrentRow["Code"],
                Title = (string)TaxesClass.CurrentRow["Title"],
                Rate = (decimal)TaxesClass.CurrentRow["Rate"],
                TaxType = (int)TaxesClass.CurrentRow["TaxType"],
                COA = (int)TaxesClass.CurrentRow["COA"]
            };
        }

        public IActionResult OnPostEdit(int ID)
        {
            return RedirectToPage("Taxes", "Edit", new { ID });
        }


        public IActionResult OnPostSave(int ID)
        {
            MyMessages = new();
            var TaxesClass = new DataTableClass(UserName, Tables.Taxes);
            TB_Taxes = TaxesClass.MyDataTable;
            TaxesClass.SeekRecord(ID);
            TaxesClass.CurrentRow["ID"] = Variables.ID;
            TaxesClass.CurrentRow["Code"] = Variables.Code;
            TaxesClass.CurrentRow["Title"] = Variables.Title;
            TaxesClass.CurrentRow["Rate"] = Variables.Rate;
            TaxesClass.CurrentRow["TaxType"] = Variables.TaxType;
            TaxesClass.CurrentRow["COA"] = Variables.COA;
            TaxesClass.Save();
            if(TaxesClass.IsError)
            {
                MyMessages.Add(SetMessage("Record not save. Contact to Administrator", ConsoleColor.Red));

            }
            else
            {
                MyMessages.Add(SetMessage($"Record has been saved: {Variables.Title}", ConsoleColor.Green));
                TaxesClass = new DataTableClass(UserName, Tables.Taxes);
                TB_Taxes = TaxesClass.MyDataTable;
            }
            return Page();
        }

        public IActionResult OnPostDelete(int ID)
        {
            MyMessages = new();
           

            MyMessages.Add(SetMessage("Record Deleted.", ConsoleColor.Red));
            return Page();
        }


        public class Parameters
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string Title { get; set; }
            public decimal Rate { get; set; }
            public int TaxType { get; set; }
            public int COA { get; set; }


        }
    }
}
