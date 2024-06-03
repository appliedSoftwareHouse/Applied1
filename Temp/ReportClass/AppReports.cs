
using System;
using System.Data;
using System.IO;
using System.Security.Claims;

using System.Collections.Generic;

namespace ReportClass
{
    public class AppReports : IDisposable
    {
        #region Global Variables


        public ClaimsPrincipal AppUser { get; set; }
        public string Name { get; set; }
        public string ReportFile { get; set; }
        public string ReportDataSet { get; set; }
        public string ReportFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public string OutputFile { get; set; }
        public string OutputFileLinkPath { get; set; }
        public ReportType OutputFileType { get; set; }
        public DataTable ReportData { get; set; }
        public string MyMessage { get; set; }                                          // Store message of the class
        public FileStream MyFileStream { get; set; }                                // File Stream Object
        public byte[] MyBytes { get; set; }                                               // Rendered file bytes for view or print report
        public bool IsError { get; set; }
        public string Mimtype { get; set; } = "";
        public Dictionary<string, string> ReportParameters { get; set; } = new Dictionary<string, string>();     // Reports Paramates
        public string RecordSort { get; set; }
       
        #endregion

        private ReportResult reportResult;                                              // result of the report.



        public bool GetReport()
        {
            if (OutputFile.Length > 0)
            {

                if (!Directory.Exists(OutputFilePath)) { Directory.CreateDirectory(OutputFilePath); }     // Create a Directory if not existed.
                if (File.Exists(OutputFile)) { File.Delete(OutputFile); }                                          // Delete output file if exist.


                //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //Encoding.GetEncoding("windows-1252");

                string _ReportFile = string.Concat(ReportFilePath, ReportFile);

                LocalReport _Report = new LocalReport(_ReportFile);                                                                                 // Create Report Object.
               
                if (ReportData == null) { IsError = true; return IsError; }                                          // Get data from default source 
                _Report.AddDataSource(ReportDataSet, ReportData);                                                           // Create DataTabel and inject in report.

                try
                {
                    reportResult = _Report.Execute(GetRenderType(OutputFileType.ToString()), 1, null, Mimtype);
                    byte[] bytes = reportResult.MainStream;
                    MyBytes = bytes;
                    MyMessage = "Report generated. " + OutputFile;
                    IsError = false;
                }
                catch (Exception e)
                {
                    MyMessage = e.Message;
                    IsError = true;
                }
            }
            else
            {
                MyMessage = "Output File name not define. " + OutputFile;
            };

            return !IsError;

        }

        

        public string CreateReportFile()
        {
            string FileName = string.Concat(OutputFilePath, OutputFile, GetFileExtention());
            string OutPutFile = string.Concat(OutputFileLinkPath, OutputFile, GetFileExtention());

            var bytes = reportResult.MainStream;

            using (FileStream fstream = new FileStream(FileName, FileMode.Create))
            {
                fstream.Write(bytes, 0, bytes.Length);
                MyFileStream = fstream;
            }

            return OutPutFile;
        }


        #region Gets
        private string GetFileExtention()
        {
            if (OutputFileType == ReportType.pdf) { return ".pdf"; }
            if (OutputFileType == ReportType.word) { return ".word"; }
            if (OutputFileType == ReportType.excel) { return ".xls"; }
            return ".pdf";
        }

       

        private static RenderType GetRenderType(string reportType)
        {
            switch (reportType.ToLower())
            {
                case "pdf":
                    return RenderType.Pdf;
                case "Word":
                    return RenderType.Word;
                case "excel":
                    return RenderType.Excel;
                default:
                    return RenderType.Pdf;
            }
        }

        #endregion

        public void Dispose()
        {
            MyFileStream = null;
            MyBytes = null;
            ReportParameters = null;
            reportResult = null;  //Dispose();
        }

        public enum ReportType
        {
            pdf = 1,
            excel = 2,
            word = 3
        }
    }
}
