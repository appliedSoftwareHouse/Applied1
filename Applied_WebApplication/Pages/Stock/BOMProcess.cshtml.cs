using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Data;

namespace Applied_WebApplication.Pages.Stock
{
    public class BOMProcessModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable Profile { get; set; }
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
                    if (BOMClass.CountTable > 0)
                    {
                        Profile = BOMClass.MyDataTable;
                    }
                }
            }
        }



        public class MyParameters
        {
            public int BOMProcess { get; set; }

        }
    }
}
