using System.Globalization;
using System.Security.Claims;

namespace Applied_WebApplication.Data
{
    public interface IAppliedDependency
    {
        string AppPath { get; set; }
        string AppRoot { get; set; }
        string ReportPath { get; set; }
        string AppDBTempPath { get; set; }
        string PrintedReportPath { get; set; }
        string PrintedReportPathLink { get; set; }
        string DataBasePath { get; set; }
        string UserDBPath { get; set; }
        string GuestDBPath { get; set; }
        CultureInfo AppCurture { get; set; }
        string CultureString { get; set; }
        string InputDatesFormat { get; set; }
        string DateFormat { get; set; }
        string CurrencyFormat { get; set; }
        string ReportFooter { get; set; }

    }

    public class AppliedDependency : IAppliedDependency
    {
        public string AppPath { get; set; }
        public string AppRoot { get; set; }
        public string ReportPath { get; set; }
        public string AppDBTempPath { get; set; }
        public string PrintedReportPath { get; set; }
        public string PrintedReportPathLink { get; set; }
        public string DataBasePath { get; set; }
        public string UserDBPath { get; set; }
        public string GuestDBPath { get; set; }
        public CultureInfo AppCurture { get; set; }
        public string CultureString { get; set; }
        public string InputDatesFormat { get; set; }
        public string DateFormat { get; set; }
        public string CurrencyFormat { get; set; }
        public string ReportFooter { get; set; }
        public ClaimsPrincipal AppUser { get; set; }

        public AppliedDependency()
        {

            AppPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            AppRoot = ".\\wwwroot\\";
            AppDBTempPath = $"{AppRoot}\\DBTemp\\";
            ReportPath = string.Concat(AppRoot, "Reports\\");
            PrintedReportPath = string.Concat(AppRoot, "PrintedReports\\");
            PrintedReportPathLink = "~/PrintedReports/";
            DataBasePath = string.Concat(AppRoot, "SQLiteDB\\");
            UserDBPath = string.Concat(DataBasePath, "AppliedUsers.db");
            GuestDBPath = string.Concat(DataBasePath, "Applied.db");
            CultureString = "en-US";
            AppCurture = new CultureInfo(CultureString, false);
            InputDatesFormat = "yyyy-MM-dd";
            DateFormat = "dd-MM-yyyy";
            CurrencyFormat = "#0.00";
            ReportFooter = "Powered by Applied Software House, +92 336 2454 230";

            // If User is existing in class.
            if (AppUser != null)
            {
                string UserName = AppUser.Identity.Name;
                if (AppUser.Identity.Name.Length > 0)
                {
                    PrintedReportPath = string.Concat(PrintedReportPath, UserName, "\\");
                    PrintedReportPathLink = string.Concat(PrintedReportPath, UserName, "/");
                }
            }
        }
    }
}
