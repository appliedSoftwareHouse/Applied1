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
        public DataTableClass COA; 
        public Record _Record { get; set; } = new Record();

        public async Task<IActionResult> OnGetAsync(int? id)

        {
            if (id == null)
            {
                return NotFound();
            }

            COA = new DataTableClass(Tables.COA.ToString(), Convert.ToDouble(id));

            _Record.ID = (int)COA.CurrentRow["ID"];
            _Record.Title = (string)COA.CurrentRow["Title"];
            _Record.Nature = (int)COA.CurrentRow["Nature"];
            _Record.Class = (int)COA.CurrentRow["Class"];
            _Record.Notes = (int)COA.CurrentRow["Notes"];
            return Page();
        }

        public async Task<IActionResult> OnPostSubmit(Record? _FillRecord)
        {
            if (ModelState.IsValid)
            {

                string exisitTitle = (string)COA.CurrentRow["Title"];

                string resultTitle = _FillRecord.Title;


                // do something
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