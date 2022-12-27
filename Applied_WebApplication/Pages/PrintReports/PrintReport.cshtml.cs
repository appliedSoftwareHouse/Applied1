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
            MyReport.ReportFilter.TableName = Tables.COA;
            MyReport.ReportFilter.dt_From = new DateTime(2022, 12, 1);
            MyReport.ReportFilter.dt_To = new DateTime(2022, 12, 31);
            MyReport.ReportFilter.n_ID = 2;
            MyReport.ReportFilter.n_COA = 0;
            MyReport.ReportFilter.n_Customer = 0;
            MyReport.ReportFilter.n_Project = 0;
            MyReport.ReportFilter.n_Employee = 0;
            MyReport.ReportFilter.n_Inventory = 0;
            MyReport.ReportFilter.n_InvSubCategory = 0;
            MyReport.ReportFilter.n_InvCategory = 0;
            MyReport.UserName = UserName;
            MyReport.MyReportPath = string.Concat(MyReport.MyReportPath, "Reports\\", "Report1.rdlc");
            MyReport.MyPrintReportPath = String.Concat(RootPath, "\\MyReport\\PrintReports\\", UserName, "\\");
            MyReport.RenderFileType = ReportClass.FileType.pdf;
            MyReport.DataSourceName = "DataSet1";
            MyReport.CommandText = DataTableClass.GetQueryText(MyReport.ReportFilter); //  "SELECT ID, CODE, TITLE FROM [COA]";
            MyReport.Parameters.Add("UserName", UserName);
            MyReport.GetReport();
            return new FileContentResult(MyReport.MyBytes, "application/pdf");


        }

    }
}
