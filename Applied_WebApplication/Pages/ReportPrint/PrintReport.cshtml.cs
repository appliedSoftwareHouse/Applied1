using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Applied_WebApplication.Data.AppFunctions;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class COAListModel : PageModel
    {
        public readonly IWebHostEnvironment MyEnvironment;
        public ReportClass MyReport = new ReportClass();
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

        public IActionResult OnGetCOAList(string UserName)
        {
            var RootPath = MyEnvironment.WebRootPath;
            MyReport = new ReportClass();
            MyReport.ReportFilter.TableName = Tables.COA;
            //MyReport.ReportFilter.dt_From = new DateTime(2022, 12, 1);
            //MyReport.ReportFilter.dt_To = new DateTime(2022, 12, 31);
            MyReport.UserName = UserName;
            MyReport.MyReportPath = string.Concat(MyReport.MyReportPath, "Reports\\", "Report1.rdlc");
            MyReport.MyPrintReportPath = string.Concat(RootPath, "\\MyReport\\PrintReports\\", UserName, "\\");
            MyReport.RenderFileType = ReportClass.FileType.pdf;
            MyReport.DataSourceName = "DataSet1";
            MyReport.CommandText = GetQueryText(MyReport.ReportFilter); //  "SELECT ID, CODE, TITLE FROM [COA]";
            MyReport.Parameters.Add("UserName", UserName);
            MyReport.GetReport();
            return new FileContentResult(MyReport.MyBytes, "application/pdf");
        }
    }
}
