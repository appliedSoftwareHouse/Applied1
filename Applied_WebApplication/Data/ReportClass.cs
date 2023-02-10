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
        #region Variables

        private readonly string mimtype = "";
        private int extension = 1;
        public string UserName { get; set; }                                             // Current User Name
        //private LocalReport? _Report { get; set; }

        public string OutputFileName { get; set; }                                    // Output File .pdf, .doc or .xls
        public string OutputFile { get => GetOutputFile(); }                     // Path + file name of printed report PDF/Doc/xls.
        public string OutputPath { get => GetOutputPath(); }                  // Path where to printed report store.
        public string OutputFileLink { get => GetOutputFileLink(); }                                       // Location to printed report PDF.
        public FileType OutputFileType { get; set; }                                  // Rendered file type pdf, word, excel

        public string RDLCFilePath { get => AppGlobals.ReportRoot; }    // RDLC report path
        public string RDLCFile { get => string.Concat(RDLCFilePath, RDLCFileName); }   // RDLC report path + FileName
        public string RDLCFileName { get; set; }                                       // Output File .pdf, .doc or .xls
        public Tables RDLCDataTable { get; set; }                                     // Data Table Set for print report.
        public DataTable ReportData { get; set; }                                       // DataTable to be perint in RDLC Report.
        public string RDLCDataSet { get; set; }                                         // Datasource DataSet name exact in RDLC
        public string CommandText { get; set; }                                        // commad for factch date from DB
        public string CommandFilter { get; set; }                                        // commad for factch date from DB

        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();     // Reports Paramates
        public Dictionary<string, string> ReportParameters { get; set; } = new Dictionary<string, string>();     // Reports Paramates
        public string MyMessage { get; set; }                                          // Store message of the class
        public FileStream MyFileStream { get; set; }                                // File Stream Object
        public byte[] MyBytes { get; set; }                                               // Rendered file bytes for view or print report
        public ReportFilters ReportFilter { get; set; }                               // SQL query Filters
        public bool IsError { get; set; }

        #endregion

        public void GetReport()
        {
            if (OutputFile.Length > 0)
            {

                if (!Directory.Exists(GetOutputPath())) { Directory.CreateDirectory(GetOutputPath()); }     // Create a Directory if not existed.
                if (File.Exists(GetOutputFile())) { File.Delete(GetOutputFile()); }                                          // Delete output file if exist.

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding.GetEncoding("windows-1252");

                LocalReport _Report = new LocalReport(RDLCFile);                                                                                 // Create Report Object.

                if (ReportData == null) { ReportData = GetReportDataTable(); }                                          // Get data from default source 
                _Report.AddDataSource(RDLCDataSet, ReportData);                                                           // Create DataTabel and inject in report.
                ReportResult reportResult;
                try
                {
                    reportResult = _Report.Execute(GetRenderType(OutputFileType.ToString()), 1, ReportParameters, mimtype);
                    byte[] bytes = reportResult.MainStream;
                    MyBytes = bytes;
                    using (FileStream fstream = new FileStream(OutputFile, FileMode.Create))
                    {
                        fstream.Write(bytes, 0, bytes.Length);
                        MyFileStream = fstream;
                    }
                    IsError = false;
                    MyMessage = "Report generated. " + OutputFile;
                }
                catch (Exception)
                {
                    reportResult = null;
                    _Report = null;
                }
            }
            else
            {
                MyMessage = "Output File name not define. " + OutputFile;
            };

            
        }


        public DataTable GetReportDataTable()
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
            string FileName = string.Concat(OutputFileName, ".", OutputFileType.ToString());
            return string.Concat(AppGlobals.PrintedReportPathLink, UserName, "/", FileName);
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
            pdf = 1,
            excel = 2,
            word = 3
        }
        public class ReportFilters
        {
            public Tables TableName { get; set; }
            public string Columns { get; set; }
            public DateTime Dt_From { get; set; }
            public DateTime Dt_To { get; set; }
            public int N_ID { get; set; }
            public int N_COA { get; set; }
            public int N_Customer { get; set; }
            public int N_Employee { get; set; }
            public int N_Project { get; set; }
            public int N_Inventory { get; set; }
            public int N_InvCategory { get; set; }
            public int N_InvSubCategory { get; set; }

            public bool All_COA { get; set; }
            public bool All_Customer { get; set; }

        }
        public ReportClass()
        {
            ReportFilter = new ReportFilters
            {
                Dt_From = DateTime.Now,
                Dt_To = DateTime.Now,
                N_ID = 0,
                N_COA = 0,
                N_Customer = 0,
                N_Project = 0,
                N_Employee = 0,
                N_Inventory = 0,
                N_InvSubCategory = 0,
                N_InvCategory = 0
            };
        }
    }
}
