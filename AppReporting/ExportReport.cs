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
        //public enum ReportType
        //{
        //    PDF,
        //    HTML,
        //    Word,
        //    Excel
        //}

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
            if (_ReportType == ReportType.PDF) { return "pdf"; }
            if (_ReportType == ReportType.HTML) { return "html"; }
            if (_ReportType == ReportType.Word) { return "docx"; }
            if (_ReportType == ReportType.Excel) { return "xlsx"; }
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
    }
}
