using Microsoft.VisualBasic;
using System.Globalization;

namespace Applied_WebApplication.Data
{
    public interface IAppliedDependency
    {
        string AppRoot { get; }
        string ReportRoot { get; }
        string PrintedReportPath { get; }
        string PrintedReportPathLink { get; }
        string DefaultDB { get;  }
        string DefaultPath { get;  }
        string UserDBPath { get;  }
        string GuestDBPath { get; }
        CultureInfo AppCurture { get; }
        string CultureString { get; }
        string InputDatesFormat { get; }
        string DateFormat { get; set; }
        string CurrencyFormat { get; set; }
}

    public class AppliedDependency : IAppliedDependency
    {
        public string AppRoot { get; }
        public string ReportRoot { get; }
        public string PrintedReportPath { get; }
        public string PrintedReportPathLink { get; }
        public string UserDBPath { get;  }
        public string GuestDBPath { get; }
        public string DefaultDB { get; set; }
        public string DefaultPath { get; }
        public CultureInfo AppCurture { get; }
        public string CultureString { get; }
        public string InputDatesFormat { get; }
        public string DateFormat { get; set; }
        public string CurrencyFormat { get; set; }

        public AppliedDependency()
        {
            AppRoot = ".\\wwwroot\\";
            ReportRoot = string.Concat(AppRoot,"Reports\\");
            PrintedReportPath = string.Concat(AppRoot, "PrintedReports\\");
            PrintedReportPathLink = "~/PrintedReports/";
            DefaultDB = string.Concat(AppRoot, "SQLiteDB\\");
            UserDBPath = string.Concat(DefaultPath, "AppliedUsers.db");
            GuestDBPath = string.Concat(DefaultPath, "Applied.db");
            CultureString = "en-EN";
            AppCurture = new CultureInfo(CultureString, false);
            InputDatesFormat = "yyyy-MM-dd";
            DateFormat = "dd-MM-yyyy";
            CurrencyFormat = "#0.00";

        }
    }
}
