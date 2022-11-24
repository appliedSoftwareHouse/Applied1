using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Accounts
{
    public class COAModel : PageModel
    {
        public int ID { get; set; }
        public string Title { get; set; }

        private bool IsPosted = false;

        public void OnGet()
        {
            if (!IsPosted) 
            {
                ID = 999;
                Title = "My Name type here";
            }
            else
            {
                ID = ID;
                Title =  Title;

            }
            

            
        }

        public void OnPost()
        {
            IsPosted = true;
            ID = ID+1;

        }

    }
}
