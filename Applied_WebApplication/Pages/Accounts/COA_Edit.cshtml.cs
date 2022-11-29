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
        //public Record _Record { get; set; } = new Record();
        public Record _Record = new Record();
        public int Counter = 0;
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
            _Record.Nature = (int)COA.CurrentRow["Nature"];
            _Record.Class = (int)COA.CurrentRow["Class"];
            _Record.Notes = (int)COA.CurrentRow["Notes"];

            Title_Class = COA_Class.Title(_Record.Class);
            Title_Nature = COA_Nature.Title(_Record.Nature);
            Title_Notes = COA_Notes.Title(_Record.Notes);

            return Page();
        }

        public async Task<IActionResult> OnPostSubmit(Record? _FillRecord)
        {
            if (ModelState.IsValid)
            {
                COA.CurrentRow = COA.NewRecord();
                COA.CurrentRow["ID"] = _FillRecord.ID;
                COA.CurrentRow["Title"] = _FillRecord.Title;
                COA.CurrentRow["Nature"] = _FillRecord.Nature;
                COA.CurrentRow["Class"] = _FillRecord.Class;
                COA.CurrentRow["Notes"] = _FillRecord.Notes;
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
            [BindProperty]
            public int ID { get; set; }

            [BindProperty]
            public string Title { get; set; }

            public int Nature { get; set; }

            public int Class { get; set; }

            public int Notes { get; set; }

            public decimal OBal { get; set; }

        }
    }
}