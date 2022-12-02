using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace Applied_WebApplication.Pages.Accounts
{
    public class COA_EditModel : PageModel
    {
        public DataTableClass COA = new DataTableClass(Tables.COA.ToString());
        public DataTableClass COA_Nature = new DataTableClass(Tables.COA_Nature.ToString());
        public DataTableClass COA_Class = new DataTableClass(Tables.COA_Class.ToString());
        public DataTableClass COA_Notes = new DataTableClass(Tables.COA_Notes.ToString());
        
        public Record _Record = new Record();
        public int Counter = 0;
        public string Test = "";
        public string Title_Nature, Title_Class, Title_Notes;

        public async Task<IActionResult> OnGetAsync(int? id)

        {
            if (id == null)
            {
                return NotFound();
            }

            //COA = new DataTableClass(Tables.COA.ToString(), Convert.ToDouble(id));
            COA.SeekRecord((int)id);


            _Record.ID = (int)COA.CurrentRow["ID"];
            _Record.Title = (string)COA.CurrentRow["Title"];
            _Record.COA_Nature = (int)COA.CurrentRow["Nature"];
            _Record.COA_Class = (int)COA.CurrentRow["Class"];
            _Record.COA_Notes = (int)COA.CurrentRow["Notes"];

            Title_Class = COA_Class.Title(_Record.COA_Class);
            Title_Nature = COA_Nature.Title(_Record.COA_Nature);
            Title_Notes = COA_Notes.Title(_Record.COA_Notes);

            return Page();
        }

        public async Task<IActionResult> OnPostSubmit(Record? _FillRecord)
        {
            if (ModelState.IsValid)
            {
                var Test  = 

                COA.CurrentRow = COA.NewRecord();
                COA.CurrentRow["ID"] = _FillRecord.ID;
                COA.CurrentRow["Title"] = _FillRecord.Title;
                COA.CurrentRow["Nature"] = _FillRecord.COA_Nature;
                COA.CurrentRow["Class"] = _FillRecord.COA_Class;
                COA.CurrentRow["Notes"] = _FillRecord.COA_Notes;
                COA.CurrentRow["OPENING_BALANCE"] = _FillRecord.OBal;
                COA.Save();

                // message.
                // do something.
                return RedirectToPage("COA");
            }
            else
            {
                return Page();
            }
        }

        public class Record
        {
            [Required(ErrorMessage ="Enter Valid Code.")]
            [Range(100001,999999, ErrorMessage = "Must be 6 Charactors Code.")]
            public int ID { get; set; }

            [Required(ErrorMessage = "Enter Valid Title of Account")]
            
            public string Title { get; set; }

            public int COA_Nature { get; set; }

            public int COA_Class { get; set; }

            public int COA_Notes { get; set; }

            public decimal OBal { get; set; }

        }
    }
}