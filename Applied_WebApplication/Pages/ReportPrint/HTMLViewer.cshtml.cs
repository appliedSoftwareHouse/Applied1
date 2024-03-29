using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class HTMLViewerModel : PageModel
    {
        public string HTMLFile {  get; set; }
        public List<Message> Messages { get; set; }

        public void OnGet()
        {
        }

        public void OnGetHTMLView(string _HTMLFile)
        {
            Messages = new List<Message>();
            if (System.IO.File.Exists(_HTMLFile)) 
            {
                HTMLFile = _HTMLFile;
            }
            else
            {
                Messages.Add(MessageClass.SetMessage($"File {_HTMLFile} is not Exist"));
            }
        }
    }
}
