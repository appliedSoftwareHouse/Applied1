using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Applied_WebApplication.Pages.Stock
{
    public class BOMProcessModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public string UserName => User.Identity.Name;
        public void OnGet(int? ID)
        {
            Variables = new();
            if (ID == null)
            {
                Variables.BOMProcess = 0;
            }
            else
            {
                DataTableClass TableClass = new(UserName, Tables.FinishedGoods, "ID=" + ID.ToString());
                if (TableClass.Count == 1)
                {
                    Variables.BOMProcess = (int)TableClass.CurrentRow["Process"];
                    DataTableClass BOMClass = new(UserName, Tables.BOMProfile2, "TranID=" + Variables.BOMProcess.ToString());


                }

            }




        }

        public class MyParameters
        {
            public int BOMProcess { get; set; }
             
        }
    }
}
