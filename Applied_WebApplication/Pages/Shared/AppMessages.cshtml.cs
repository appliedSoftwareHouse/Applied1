using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Shared
{
    public class AppMessagesModel : PageModel
    {
        
        public List<Message> ThisMessages { get; set; } = new();

        public void OnGet(List<Message> MyMessages)
        {
            ThisMessages = MyMessages;
        }
    }
}
