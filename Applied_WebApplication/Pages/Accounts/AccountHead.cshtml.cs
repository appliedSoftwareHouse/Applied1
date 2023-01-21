using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.Security;
using static Applied_WebApplication.Data.TableValidationClass;

namespace Applied_WebApplication.Pages.Accounts
{
    public class AccountHeadModel : PageModel
    {
        public AccounHead Record = new();
        public string MyPageAction { get; set; } = "Add";
        public int RecordID = 0;
        public bool IsError;
        public string UserName;
        public List<Message> ErrorMessages;

        public void OnPostAdd()
        {
            MyPageAction = "Add";
            Record = new AccounHead();
        }

        public void OnGetEdit(int id)
        {
            UserName = User.Identity.Name;
            MyPageAction = "Edit";
            RecordID = id;
            DataTableClass COA = new(UserName, Tables.COA);
            COA.SeekRecord(RecordID);
            Record.ID = (int)COA.CurrentRow["ID"];
            Record.Code = (string)COA.CurrentRow["Code"];
            Record.Title = (string)COA.CurrentRow["Title"];
            Record.Class = (int)COA.CurrentRow["Class"];
            Record.Nature = (int)COA.CurrentRow["Nature"];
            Record.Notes = (int)COA.CurrentRow["Notes"];
        }

        public void OnGetDelete(int id)
        {
            UserName = User.Identity.Name;
            MyPageAction = "Delete";
            RecordID = id;
            DataTableClass COA = new(UserName, Tables.COA);
            COA.SeekRecord(RecordID);
            Record.ID = (int)COA.CurrentRow["ID"];
            Record.Code = (string)COA.CurrentRow["Code"];
            Record.Title = (string)COA.CurrentRow["Title"];
            Record.Class = (int)COA.CurrentRow["Class"];
            Record.Nature = (int)COA.CurrentRow["Nature"];
            Record.Notes = (int)COA.CurrentRow["Notes"];

        }

        public IActionResult OnPostSave(AccounHead _Record)
        {
            RecordID = _Record.ID;
            UserName = User.Identity.Name;
            DataTableClass COA = new(UserName, Tables.COA);
            if (COA.Seek(RecordID))
            {
                COA.SeekRecord(RecordID);
            }
            else
            {
                COA.NewRecord();
                COA.CurrentRow["ID"] = 0;
            }

            COA.CurrentRow["Code"] = _Record.Code;
            COA.CurrentRow["Title"] = _Record.Title;
            COA.CurrentRow["Class"] = _Record.Class;
            COA.CurrentRow["Nature"] = _Record.Nature;
            COA.CurrentRow["Notes"] = _Record.Notes;
            COA.CurrentRow["Opening_Balance"] = _Record.OBal;
            COA.Save();
            ErrorMessages = COA.TableValidation.MyMessages;
            if (ErrorMessages.Count == 0) { return RedirectToPage("../Accounts/Directory/COA"); } else { IsError = true; }
            return Page();
        }

        public IActionResult OnPostDelete(AccounHead _Record)
        {
            RecordID = _Record.ID;
            DataTableClass COA = new(UserName, Tables.COA);
            if (COA.Seek(RecordID))
            {
                COA.SeekRecord(RecordID);                   // Assign a record for delete
                COA.Delete();                                           // Delete a record.
                return RedirectToPage("COA");
            }

            return Page();
        }


        public class AccounHead
        {
            public int ID { get; set; }
            [Required]
            public string Code { get; set; }
            [Required]
            public string Title { get; set; }
            public int Nature { get; set; }
            public int Class { get; set; }
            public int Notes { get; set; }
            public decimal OBal { get; set; }
        }
    }
}
