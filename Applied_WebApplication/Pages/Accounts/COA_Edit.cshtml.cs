using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace Applied_WebApplication.Pages.Accounts
{
    public class COA_EditModel : PageModel
    {
        private DataTableClass COA = new DataTableClass(Tables.COA.ToString());
        public double MyID;
        public string MyTitle;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DataTableClass _Table = new DataTableClass(Tables.COA.ToString(),  Convert.ToDouble(id));

            MyID = (double)_Table.CurrentRow["ID"];
            MyTitle = (string)_Table.CurrentRow["Title"];

            return Page();
        }
    }
}
