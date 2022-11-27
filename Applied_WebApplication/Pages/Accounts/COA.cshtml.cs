using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Applied_WebApplication.Pages.Accounts
{
    public class COAModel : PageModel
    {

        public double ID;
        public string Title;
        

        public DataTableClass COA = new DataTableClass(Tables.COA.ToString());


        #region GET

        public void OnGet()
        {
        }

        #endregion


        #region POST

        public void OnPost(Record_COA Record)
            {
                //DataTableClass COA = new DataTableClass(Tables.COA.ToString());
                COA.CurrentRow = COA.NewRecord();
                COA.CurrentRow["ID"] = Record._ID;
                COA.CurrentRow["Title"] = Record._Title;
                COA.Save();

            }

        #endregion

            #region CLASS

        public class Record_COA
        {
            [BindProperty]
            public double _ID { get; set; }

            [BindProperty]
            public string _Title { get; set; }
        }

        #endregion

    }
}
