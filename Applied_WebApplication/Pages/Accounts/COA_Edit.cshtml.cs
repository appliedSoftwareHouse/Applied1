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
        public int ID,Nature,Class,Notes;
        public string Title;
        public decimal OBal;

        public async Task<IActionResult> OnGetAsync(int? id)
        //public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            COA = new DataTableClass(Tables.COA.ToString(),  Convert.ToDouble(id));

            ID = (int)COA.CurrentRow["ID"];
            Title = (string)COA.CurrentRow["Title"];
            Nature = (int)COA.CurrentRow["Nature"];
            Class = (int)COA.CurrentRow["Class"];
            Notes = (int)COA.CurrentRow["Notes"];
            return Page();
        }

        public IActionResult OnPostSubmit(Record _Record)
        {
            if (ModelState.IsValid)
            {
                
                string resultTitle = Title;


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
            int ID { get; set; }

            [BindProperty]
            string Title { get; set; }

            int Nature { get; set; }

            int Class { get; set; }

            int Notes { get; set; }

            decimal OBal { get; set; }

        }

        
        

       

    }
}
