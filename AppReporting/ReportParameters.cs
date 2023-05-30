using Microsoft.Reporting.NETCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppReportClass
{
    public class ReportParameters
    {
        public string ReportPath { get; set; }
        public string ReportFile { get; set; }
        public string OutputPath { get; set; }
        public string OutputFile { get; set; }
        public string OutputFileExtention { get; set; }
        public DataTable ReportData { get; set; }
        public string DataSetName { get; set; }
        public List<ReportParameter>  DataParameters { get; set; }
        public ReportType ReportType { get; set; }
        public string MimeType { get; set; }
        public string RenderFormat { get; set; }
        
    }
}
