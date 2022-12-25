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
        public string UserName { get; set; }
        public string MyReportPath { get; set; }
        public string MyPrintReportPath { get; set; }
        public string DataSourceName { get; set; }
        public Dictionary<string, string> Parameters { get; set;  } = new Dictionary<string, string>();
        public FileType RenderFileType { get; set; }
        public Tables DataTableName { get; set; }
        public string CommandText { get; set; }
        public string MyMessage { get; set; }
        public FileStream MyFileStream { get; set; }
        public byte[] MyBytes { get; set; }

        public ReportClass()
        {
            MyReportPath = string.Concat(Directory.GetCurrentDirectory(),"\\wwwroot\\");
            
        }

        public void GetReport()
        {
            if (File.Exists(MyReportPath))
            {
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
