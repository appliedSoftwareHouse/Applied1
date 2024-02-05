using Microsoft.Reporting.NETCore;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Data;
using System.IO.Enumeration;

namespace AppReportClass
{
    public class ReportModel
    {
        public InputReport InputReport { get; set; }
        public OutputReport OutputReport { get; set; }
        public ReportData ReportData { get; set; }
        public byte[] ReportBytes { get; set; }

        public List<ReportParameter> ReportParameters { get; set; }
        public bool Render => ReportRender();

        public ReportModel() { }

        private bool ReportRender()
        {
            if(ReportParameters.Count == 0) { GetDefaultParameters(); }


            if (InputReport.IsFileExist)
            {

                var _ReportFile = InputReport.FileName;
                LocalReport report = new();
                var _ReportStream = new StreamReader(_ReportFile);
                report.LoadReportDefinition(_ReportStream);
                report.DataSources.Add(ReportData.DataSource);
                report.SetParameters(ReportParameters);
                ReportBytes = report.Render(OutputReport.ReportType.ToString());

                //MimeType = OutputReport.MimeType;
                //Variables.OutputFileExtention = GetReportExtention(OutputReport.ReportType);
                //Variables.OutputFileFullName = $"{Variables.OutputPath}{Variables.OutputFile}{Variables.OutputFileExtention}";
                //Variables.OutputFileName = $"{Variables.OutputFile}{Variables.OutputFileExtention}";

                //Variables.MyMessage = $"File length = {Variables.FileBytes.Length} ";
                if (OutputReport.ReportType == ReportType.Preview)
                {
                    SaveReport();
                }
            }

            if(ReportBytes.Length > 0) { return true; }
            return false;
        }

        private void GetDefaultParameters()
        {
            ReportParameters = new List<ReportParameter>
            {
                new ReportParameter("CompanyName", "Applied Software House"),
                new ReportParameter("Heading1", "Heading "),
                new ReportParameter("Heading2", "Sub Heading"),
                new ReportParameter("Footer", "Powered by Applied Software House")
            };
        }

        private string GetReportExtention(ReportType _ReportType)
        {
            if (_ReportType == ReportType.PDF) { return ".pdf"; }
            if (_ReportType == ReportType.HTML) { return ".html"; }
            if (_ReportType == ReportType.Word) { return ".docx"; }
            if (_ReportType == ReportType.Excel) { return ".xlsx"; }
            return "";
        }

        private string GetReportMime(ReportType _ReportType)
        {
            if (_ReportType == ReportType.PDF) { return "application/pdf"; }
            if (_ReportType == ReportType.HTML) { return "text/html"; }
            if (_ReportType == ReportType.Word) { return "application/vnd.openxmlformats-officedocument.wordprocessingml.doc.ument"; }
            if (_ReportType == ReportType.Excel) { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
            return "";
        }

        public bool SaveReport()
        {
            if (ReportBytes.Length == 0)
            {
                return false;
            }

            FileStream MyFileStream;
            var _FileName = OutputReport.FileFullName;
            if (_FileName.Length > 0)
            {
                if (File.Exists(_FileName)) { File.Delete(_FileName); }
                using (FileStream fstream = new FileStream(_FileName, FileMode.Create))
                {
                    fstream.Write(ReportBytes, 0, ReportBytes.Length);
                    MyFileStream = fstream;
                }
            }

            return false;
        }
    }
    public class InputReport
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileExtention { get; set; } = string.Empty;

        public string FileFullName => GetFullName();
        public bool IsFileExist => GetFileExist();

        private bool GetFileExist()
        {
            if (File.Exists(FileFullName)) { return true; }
            { return false; }
        }

        private string GetFullName()
        {
            if (FilePath.Length > 0 && FileName.Length > 0 && FileExtention.Length > 0)
            {
                return $"{FilePath}{FileName}.{FileExtention}";
            }
            return string.Empty;
        }
    }
    public class OutputReport
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileExtention { get; set; } = string.Empty;
        public ReportType ReportType { get; set; } = ReportType.Preview;
        public string MimeType { get; set; } = string.Empty;
        public bool IsFileExist => File.Exists(FileFullName);

        public string FileFullName => GetFullName();
        private string GetFullName()
        {
            if (FilePath.Length > 0 && FileName.Length > 0 && FileExtention.Length > 0)
            {
                return $"{FilePath}{FileName}.{FileExtention}";
            }
            return string.Empty;
        }
    }
    public class ReportData
    {
        public string SQLQuery { get; set; }
        public DataTable ReportTable { get; set; }
        public string DataSetName { get; set; }
        public ReportDataSource DataSource => GetReportDataSource();

        private ReportDataSource GetReportDataSource()
        {
            if (DataSetName.Length > 0 && ReportTable != null)
            {
                return new ReportDataSource(DataSetName, ReportTable);
            }
            return new ReportDataSource();
        }
    }

}
