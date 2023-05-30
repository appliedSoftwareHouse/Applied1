using Microsoft.Reporting.NETCore;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppReportClass
{
    public class ExportReport
    {
        
        #region GET Report Vales

        public static string GetRenderFormat(ReportType _ReportType)
        {
            if (_ReportType == ReportType.PDF) { return "PDF"; }
            if (_ReportType == ReportType.HTML) { return "HTML5"; }
            if (_ReportType == ReportType.Word) { return "WORDOPENXML"; }
            if (_ReportType == ReportType.Excel) { return "EXCELOPENXML"; }
            return "";
        }

        public static string GetReportExtention(ReportType _ReportType)
        {
            if (_ReportType == ReportType.PDF) { return ".pdf"; }
            if (_ReportType == ReportType.HTML) { return ".html"; }
            if (_ReportType == ReportType.Word) { return ".docx"; }
            if (_ReportType == ReportType.Excel) { return ".xlsx"; }
            return "";
        }

        public static string GetReportMime(ReportType _ReportType)
        {
            if (_ReportType == ReportType.PDF) { return "application/pdf"; }
            if (_ReportType == ReportType.HTML) { return "text/html"; }
            if (_ReportType == ReportType.Word) { return "application/vnd.openxmlformats-officedocument.wordprocessingml.doc.ument"; }
            if (_ReportType == ReportType.Excel) { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
            return "";
        }

        #endregion

        #region ReportExport

        public static byte[] Render(ReportParameters _ReportParameter)
        {
            var _ReportFile = string.Concat(_ReportParameter.ReportPath, _ReportParameter.ReportFile);
            ReportDataSource _DataSource = new(_ReportParameter.DataSetName, _ReportParameter.ReportData);
            LocalReport report = new();
            var _ReportStream = new StreamReader(_ReportFile);
            report.LoadReportDefinition(_ReportStream);
            report.DataSources.Add(_DataSource);
            report.SetParameters(_ReportParameter.DataParameters);
            var _RenderFormat = GetRenderFormat(_ReportParameter.ReportType);
            var reportData = report.Render(_RenderFormat);
            return reportData;
        }


        #endregion

    }
}
