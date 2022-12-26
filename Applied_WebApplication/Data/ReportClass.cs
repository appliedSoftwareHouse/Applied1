using AspNetCore.Reporting;
using Microsoft.AspNetCore;
using System.Data;
using System.Text;


namespace Applied_WebApplication.Data
{
    public class ReportClass 
    {
        private string mimtype = "";
        private int extension = 1;
        public string UserName { get; set; }                     // Current User Name
        public string MyReportPath { get; set; }               // RDLC report name with path
        public string MyPrintReportPath { get; set; }       // Path or location to print report.
        public string DataSourceName { get; set; }          // Datasource DataSet name exact in RDLC
        public Dictionary<string, string> Parameters { get; set;  } = new Dictionary<string, string>();     // Reports Paramates
        public FileType RenderFileType { get; set; }                        // Rendered file type pdf, word, excel
        public Tables DataTableName { get; set; }                         // Name of DataTable ontain from erum Table.
        public string CommandText { get; set; }                             // commad for factch date from DB
        public string MyMessage { get; set; }                                // Store message of the class
        public FileStream MyFileStream { get; set; }                      // File Stream Object
        public byte[] MyBytes { get; set; }                                     // Rendered file bytes for view or print report

        public ReportClass()
        {
            MyReportPath =   string.Concat(Directory.GetCurrentDirectory(),"\\wwwroot\\");
        }

        public void GetReport()
        {
            if (File.Exists(MyReportPath))
            {
                if(!Directory.Exists(MyPrintReportPath)) { Directory.CreateDirectory(MyPrintReportPath); }                                           // Create a Directory if not existed.

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding.GetEncoding("windows-1252");
                LocalReport _Report = new LocalReport(MyReportPath);
                _Report.AddDataSource(DataSourceName, ReportDataTable());
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


        public DataTable ReportDataTable()
        {
            return DataTableClass.GetAppliedTable(UserName, CommandText);
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


    }
}
