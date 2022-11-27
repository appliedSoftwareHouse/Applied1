using Applied_WebApplication.Data;
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
        public double MyID;
        public string MyTitle;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            COA = new DataTableClass(Tables.COA.ToString(),  Convert.ToDouble(id));
            
            MyID = (double)COA.CurrentRow["ID"];
            MyTitle = COA.CurrentRow["Title"].ToString();

            return Page();
        }

        
        
    }
}
