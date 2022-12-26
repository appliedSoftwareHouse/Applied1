using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.PrintReports
{
    public class COAListModel : PageModel
    {
        public readonly IWebHostEnvironment MyEnvironment;
        public COAListModel(IWebHostEnvironment _Environment)
        {
            MyEnvironment = _Environment;
        }


        public void OnGet()
        {
            
        }

        public void OnGetPrint()
        {


        }

        public IActionResult OnPostCOAList(string UserName)
        {
            var RootPath = MyEnvironment.WebRootPath;

            ReportClass MyReport = new ReportClass();
            MyReport.UserName = UserName;
            MyReport.MyReportPath = string.Concat(MyReport.MyReportPath, "Reports\\", "Report1.rdlc");
            MyReport.MyPrintReportPath = String.Concat(RootPath, "\\MyReport\\PrintReports\\", UserName, "\\");
            MyReport.RenderFileType = ReportClass.FileType.pdf;
            MyReport.DataSourceName = "DataSet1";
            MyReport.CommandText = "SELECT ID, CODE, TITLE FROM [COA]";
            MyReport.Parameters.Add("UserName", UserName);
            MyReport.GetReport();
            return new FileContentResult(MyReport.MyBytes, "application/pdf");


        }

    }
}
