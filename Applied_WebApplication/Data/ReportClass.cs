using AspNetCore.Reporting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Text;
using static Applied_WebApplication.Data.AppFunctions;

namespace Applied_WebApplication.Data
{
    public class ReportClass 
    {
        private string mimtype = "";
        private int extension = 1;
        public string UserName { get; set; }                                     // Current User Name
        public string MyReportPath { get; set; }                              // RDLC report name with path
        public string MyPrintReportPath { get; set; }                       // Path or location to print report.
        public string DataSourceName { get; set; }                          // Datasource DataSet name exact in RDLC
        public Dictionary<string, string> Parameters { get; set;  } = new Dictionary<string, string>();     // Reports Paramates
        public FileType RenderFileType { get; set; }                        // Rendered file type pdf, word, excel
        //public Tables DataTableName { get; set; }                         // Name of DataTable ontain from erum Table.
        public string CommandText { get; set; }                             // commad for factch date from DB
        public string MyMessage { get; set; }                                // Store message of the class
        public FileStream MyFileStream { get; set; }                      // File Stream Object
        public byte[] MyBytes { get; set; }                                     // Rendered file bytes for view or print report
        public Filters ReportFilter { get; set; }                               // SQL query Filters
        public Tables PrintDataTable { get; set; }                   // Data Table Set for print report.
        

        public ReportClass()
        {
            MyReportPath =   string.Concat(Directory.GetCurrentDirectory(),"\\wwwroot\\");
            ReportFilter = new Filters();

            ReportFilter.dt_From = DateTime.Now;
            ReportFilter.dt_To = DateTime.Now;
            ReportFilter.n_ID = 0;
            ReportFilter.n_COA = 0;
            ReportFilter.n_Customer = 0;
            ReportFilter.n_Project = 0;
            ReportFilter.n_Employee = 0;
            ReportFilter.n_Inventory = 0;
            ReportFilter.n_InvSubCategory = 0;
            ReportFilter.n_InvCategory = 0;


        }

        public void GetReport()
        {
            if (File.Exists(MyReportPath))
            {
                if(!Directory.Exists(MyPrintReportPath)) { Directory.CreateDirectory(MyPrintReportPath); }                                           // Create a Directory if not existed.

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding.GetEncoding("windows-1252");
                LocalReport _Report = new LocalReport(MyReportPath);                                                                                                // Create Report Object.
                _Report.AddDataSource(DataSourceName, ReportDataTable());                                                                                    // Create DataTabel and inject in report.
                var result = _Report.Execute(GetRenderType(RenderFileType.ToString()), extension, Parameters, mimtype);

                byte[] bytes = result.MainStream;
                MyBytes = bytes;

                if (RenderFileType == FileType.pdf) { MyPrintReportPath = string.Concat(MyPrintReportPath, ".pdf"); }
                if (RenderFileType == FileType.word) { MyPrintReportPath = string.Concat(MyPrintReportPath, ".doc"); }
                if (RenderFileType == FileType.excel) { MyPrintReportPath = string.Concat(MyPrintReportPath, ".xls"); }

                using (FileStream fs = new FileStream(MyPrintReportPath, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    MyFileStream = fs;
                }
                MyMessage = "Report generated." + MyPrintReportPath;
            }
            else
            { MyMessage = "Report not generated."; }

        }


        private DataTable ReportDataTable()
        {
            return GetAppliedTable(UserName, ReportFilter);
        }

        private RenderType GetRenderType(string reportType)
        {
            var renderType = RenderType.Pdf;
            switch (reportType.ToLower())
            {
                default:
                case "pdf":
                    renderType = RenderType.Pdf;
                    break;
                case "word":
                    renderType = RenderType.Word;
                    break;
                case "excel":
                    renderType = RenderType.Excel;
                    break;
            }

            return renderType;
        }

        public enum FileType
        {
            pdf,
            word,
            excel
        }


        public class Filters
        {
            public Tables TableName;
            public string Columns = "*";
            public DateTime dt_From;
            public DateTime dt_To;
            public int n_ID = 0;
            public int n_COA = 0;
            public int n_Customer = 0;
            public int n_Employee = 0;
            public int n_Project = 0;
            public int n_Inventory = 0;
            public int n_InvCategory = 0;
            public int n_InvSubCategory = 0;
        }


    }
}
