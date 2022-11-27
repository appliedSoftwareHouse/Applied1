using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Entity;

namespace Applied_WebApplication.Pages.Accounts
{
    public class COA_EditModel : PageModel
    {
        private DataTableClass COA = new DataTableClass(Tables.COA.ToString());
        public double MyID;

        public async Task<IActionResult> OnGetAsync(double? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MyID = (double)id;

            //Movie = await _context.Movie.FirstOrDefaultAsync(m => m.ID == id);

            //if (Movie == null)
            //{
            //    return NotFound();
            //}
            return Page();
        }

        public void OnGet()
        {
        }
    }
}
