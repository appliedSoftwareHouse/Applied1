using AspNetCore.Reporting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Text;
using static Applied_WebApplication.Data.AppFunctions;

namespace Applied_WebApplication.Data
{
    public class ReportClass
    {
        private readonly string mimtype = "";
        private readonly int extension = 1;
        public string UserName { get; set; }                                             // Current User Name

        public string OutputFileName { get; set; }                                    // Output File .pdf, .doc or .xls
        public string OutputFile { get => GetOutputFile(); }                     // Path + file name of printed report PDF/Doc/xls.
        public string OutputPath { get => GetOutputPath(); }                  // Path where to printed report store.
        public string OutputFileLink { get; set; }                                       // Location to printed report PDF.
        public FileType OutputFileType { get; set; }                                  // Rendered file type pdf, word, excel

        public string RDLCFilePath { get => AppGlobals.ReportRoot; }    // RDLC report path
        public string RDLCFile { get => string.Concat(RDLCFilePath, RDLCFileName); }   // RDLC report path + FileName
        public string RDLCFileName { get; set; }                                       // Output File .pdf, .doc or .xls
        public Tables RDLCDataTable { get; set; }                                     // Data Table Set for print report.
        public string RDLCDataSet { get; set; }                                         // Datasource DataSet name exact in RDLC

        public string CommandText { get; set; }                                        // commad for factch date from DB
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();     // Reports Paramates
        public Dictionary<string, string> ReportParameters { get; set; } = new Dictionary<string, string>();     // Reports Paramates
        public string MyMessage { get; set; }                                          // Store message of the class
        public FileStream MyFileStream { get; set; }                                // File Stream Object
        public byte[] MyBytes { get; set; }                                               // Rendered file bytes for view or print report
        public ReportFilters ReportFilter { get; set; }                               // SQL query Filters
        public bool IsError { get; set; }

        public ReportClass()
        {
            ReportFilter = new ReportFilters
            {
                dt_From = DateTime.Now,
                dt_To = DateTime.Now,
                n_ID = 0,
                n_COA = 0,
                n_Customer = 0,
                n_Project = 0,
                n_Employee = 0,
                n_Inventory = 0,
                n_InvSubCategory = 0,
                n_InvCategory = 0
            };
        }

        public void GetReport()
        {
            if (!Directory.Exists(GetOutputPath())) { Directory.CreateDirectory(GetOutputPath()); }     // Create a Directory if not existed.
            if (File.Exists(GetOutputFile())) { File.Delete(GetOutputFile()); }                                          // Delete output file if exist.

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("windows-1252");
            LocalReport _Report = new LocalReport(RDLCFile);                                                                                                // Create Report Object.
            _Report.AddDataSource(RDLCDataSet, GetReportDataTable());                                                                   // Create DataTabel and inject in report.
            var result = _Report.Execute(GetRenderType(OutputFileType.ToString()), extension, ReportParameters, mimtype);

            byte[] bytes = result.MainStream;
            MyBytes = bytes;
            using (FileStream fstream = new FileStream(OutputFileName, FileMode.Create))
            {
                fstream.Write(bytes, 0, bytes.Length);
                MyFileStream = fstream;
            }
            IsError = false;
            MyMessage = "Report generated. " + OutputFileName;
        }

        private DataTable GetReportDataTable()
        {
            return GetAppliedTable(UserName, ReportFilter);                      // Get a DataTable from AppFuction 
        }

        private string GetOutputFile()
        {
            string output = "";
            if (OutputFileType == FileType.pdf) { output = string.Concat(GetOutputPath(), OutputFileName, ".pdf"); }
            if (OutputFileType == FileType.word) { output = string.Concat(GetOutputPath(), OutputFileName, ".doc"); }
            if (OutputFileType == FileType.excel) { output = string.Concat(GetOutputPath(), OutputFileName, ".xls"); }
            return output;
        }

        public string InputReportFile()
        {
            string inputfile = string.Concat(AppGlobals.PrintedReportPath, "\\", RDLCFileName);
            return inputfile;
        }

        public string GetOutputFileLink()
        {
            return string.Concat(AppGlobals.PrintedReportPathLink, OutputFileName);
        }


        public string GetOutputPath()
        {
            string outputpath = string.Concat(AppGlobals.PrintedReportPath, UserName, "\\");
            return outputpath;
        }

        private static RenderType GetRenderType(string reportType)
        {
            var renderType = reportType.ToLower() switch
            {
                "pdf" => RenderType.Pdf,
                "word" => RenderType.Word,
                "excel" => RenderType.Excel,
                _ => RenderType.Pdf,
            };
            return renderType;
        }

        public enum FileType
        {
            pdf,
            word,
            excel
        }


        public class ReportFilters
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
