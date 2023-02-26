﻿using System.Globalization;

namespace Applied_WebApplication.Data
{
    public interface IAppliedDependency
    {
        string AppPath { get; set; }
        string AppRoot { get; }
        string ReportRoot { get; }
        string PrintedReportPath { get; }
        string PrintedReportPathLink { get; }
        string DefaultDB { get; }
        string LocalDB { get; }
        string DefaultPath { get; }
        string UserDBPath { get; }
        string GuestDBPath { get; }
        CultureInfo AppCurture { get; }
        string CultureString { get; }
        string InputDatesFormat { get; }
        string DateFormat { get; set; }
        string CurrencyFormat { get; set; }

    }

    public class AppliedDependency : IAppliedDependency
    {
        public string AppPath { get; set; }
        public string AppRoot { get; }
        public string ReportRoot { get; }
        public string PrintedReportPath { get; }
        public string PrintedReportPathLink { get; }
        public string UserDBPath { get; }
        public string GuestDBPath { get; }
        public string DefaultDB { get; set; }
        public string LocalDB { get; }
        public string DefaultPath { get; }
        public CultureInfo AppCurture { get; }
        public string CultureString { get; }
        public string InputDatesFormat { get; }
        public string DateFormat { get; set; }
        public string CurrencyFormat { get; set; }


        public AppliedDependency()
        {
            AppPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            AppRoot = ".\\wwwroot\\";
            ReportRoot = string.Concat(AppRoot, "Reports\\");
            PrintedReportPath = string.Concat(AppRoot, "PrintedReports\\");
            PrintedReportPathLink = "~/PrintedReports/";
            DefaultDB = string.Concat(AppRoot, "SQLiteDB\\");
            LocalDB = string.Concat(AppPath, "\\LocalDB\\");
            UserDBPath = string.Concat(DefaultDB, "AppliedUsers.db");
            GuestDBPath = string.Concat(DefaultDB, "Applied.db");
            CultureString = "en-US";
            AppCurture = new CultureInfo(CultureString, false);
            InputDatesFormat = "yyyy-MM-dd";
            DateFormat = "dd-MM-yyyy";
            CurrencyFormat = "#0.00";

        }
    }
}
