using System.IO;
using AppReportClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class HTMLViewerModel : PageModel
    {
        public string HTMLFile {  get; set; }
        public List<Message> Messages { get; set; }
        public ReportModel Model { get; set; }

        public void OnGet()
        {
        }


        //public HTMLViewerModel(ReportModel PrintModel)
        //{
        //    Model = PrintModel;


        //}


        public void OnGetHTMLView(string Linkfile)
        {
            Messages = new List<Message>();
            if (System.IO.File.Exists(Linkfile))
            {
                HTMLFile = Linkfile;
            }
            else
            {
                Messages.Add(MessageClass.SetMessage($"File {Linkfile} is not Exist"));
            }
        }
    }
}
