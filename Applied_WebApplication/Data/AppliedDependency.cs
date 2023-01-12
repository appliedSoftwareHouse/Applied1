using Microsoft.VisualBasic;
using System.Globalization;

namespace Applied_WebApplication.Data
{
    public interface IAppliedDependency
    {
        string DefaultDB { get;  }
        string DefaultPath { get;  }
        string UserDBPath { get;  }
        CultureInfo AppCurture { get; }
        string CultureString { get; }
        string InputDatesFormat { get; }
        string DateFormat { get; set; }
        string CurrencyFormat { get; set; }
}

    public class AppliedDependency : IAppliedDependency
    {
        public string UserDBPath { get;  }
        public string DefaultDB { get; set; }
        public string DefaultPath { get; }
        public CultureInfo AppCurture { get; }
        public string CultureString { get; }
        public string InputDatesFormat { get; }
        public string DateFormat { get; set; }
        public string CurrencyFormat { get; set; }

        public AppliedDependency()
        {
            DefaultDB = ".\\wwwroot\\SQLiteDB\\";
            UserDBPath = string.Concat(DefaultPath, "AppliedUsers.db");
            DefaultDB = string.Concat(DefaultPath, "Applied.db");
            AppCurture = new CultureInfo(CultureString, false);
            CultureString = "en-EN";
            InputDatesFormat = "yyyy-MM-dd";
            DateFormat = "dd-MM-yyyy";
            CurrencyFormat = "#0.00";

        }
    }
}
