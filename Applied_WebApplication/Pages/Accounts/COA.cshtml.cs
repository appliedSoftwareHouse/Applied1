using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Accounts
{
    public class COAModel : PageModel
    {
        public double _ID { get; set; }
        public string _Title { get; set; }

        
        

        public void OnGet()
        {

            _ID = 999;
            _Title = "My Name type here";
         }

        public void OnPost()
        { 
            DataTableClass COA = new DataTableClass(Tables.COA.ToString());
            COA.CurrentRow = COA.MyDataTable.NewRow();
            COA.CurrentRow["ID"] = _ID;
            COA.CurrentRow["Title"] = _Title;
            COA.SaveCurrentRow();
                      

        }

    }
}
