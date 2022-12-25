using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AspNetCore.Reporting;
using System.Drawing.Text;
using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Hosting;

namespace Applied_WebApplication.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public void OnPOST()
        {

        }

        public void OnGetPrint()
        {

        }

        public IActionResult OnPostPrint(string UserName)
        {

            ReportClass MyReport = new ReportClass();
            MyReport.UserName = UserName;
            MyReport.MyReportPath = string.Concat(MyReport.MyReportPath,"Reports\\", "Report1.rdlc");
            MyReport.MyPrintReportPath = "G:\\MyReport"; 
            MyReport.RenderFileType = ReportClass.FileType.pdf;
            MyReport.DataSourceName = "DataSet1";
            MyReport.CommandText = "SELECT ID, CODE, TITLE FROM [COA]";
            MyReport.Parameters.Add("UserName", UserName);
            MyReport.GetReport();

            return  new  FileContentResult(MyReport.MyBytes, "application/pdf");

            //return new FileContentResult(MyReport.MyBytes, "application/pdf");
            //return Page();

        }
    }
}