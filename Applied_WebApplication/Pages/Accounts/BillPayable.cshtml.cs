using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Accounts
{
    public class BillPayableModel : PageModel
    {

        public List<Message> ErrorMessages = new();
        public int ErrorCount { get=> ErrorMessages.Count;}


        public void OnGet()
        {
        }
    }
}
