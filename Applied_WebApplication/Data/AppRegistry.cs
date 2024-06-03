using Microsoft.Win32;
using System.Data;
using System.Data.SQLite;

namespace Applied_WebApplication.Data
{
    public interface IAppRegistry
    {
        public static readonly string DateYMD;
        public static readonly string FormatCurrency;
        public static readonly string FormatCurrency1;
        public static readonly string FormatCurrency2;
        public static readonly string FormatDate;
        public static readonly string FormatDateY2;
        public static readonly string FormatDateM2;
        public static readonly string Two_digits;

    }


    public class AppRegistry : IAppRegistry
    {
        private string UserName { get; set; }

        public AppRegistry(string _UserName)
        {
            UserName = _UserName;

            DataTableClass tb_Registry = new(UserName, Tables.Registry);


            //Create Default Application Registry Keys.
            if (!tb_Registry.Seek("FiscalFrom")) { SetKey(UserName, "FiscalFrom", new DateTime(2022, 1, 1), KeyType.Date); }
            if (!tb_Registry.Seek("FiscalTo")) { SetKey(UserName, "FiscalTo", new DateTime(2025, 12, 31), KeyType.Date); }
            if (!tb_Registry.Seek("TBSort")) { SetKey(UserName, "TBSort", "Code", KeyType.Text); }
        }

        public static readonly string DateYMD = "yyyy-MM-dd";
        public static readonly string Two_digits = "#,##0.00";
        public static readonly string FormatCurrency1 = "#,##0.00";
        public static readonly string FormatCurrency2 = "#,##0";
        public static readonly string FormatDate = "dd-MMM-yyyy";
        public static readonly string FormatDateY2 = "dd-MMM-yy";
        public static readonly string FormatDateM2 = "dd-MM-yy";
        public static readonly DateTime MinDate = new DateTime(2020, 01, 01);

        public static DateTime GetFiscalFrom()
        { return new DateTime(2022, 07, 01); }           // In future addign value from App Registry
        public static DateTime GetFiscalTo()
        { return new DateTime(2024, 06, 30); }

        public static DateTime GetFiscalFrom(string UserName)
        {
            if (UserName == null || UserName == string.Empty) { return DateTime.Today; }
            return GetDate(UserName, "FiscalStart");
        }

        public static DateTime GetFiscalTo(string UserName)
        {
            if (UserName == null || UserName == string.Empty) { return DateTime.Today; }
            return GetDate(UserName, "FiscalEnd");
        }


        public static string GetFormatCurrency(string UserName)
        {
            if (UserName == null || UserName == string.Empty) { return string.Empty; }
            var _Format = GetText(UserName, "FMTCurrency");
            if(_Format.Length==0) { _Format = FormatCurrency1; }
            return _Format;
        }
        public static string GetFormatDate(string UserName)
        {
            if (UserName == null || UserName == string.Empty) { return string.Empty; }
            return GetText(UserName, "FMTDate");
        }

        public static string Currency(string UserName, object Amount)
        {
            if (UserName == null || UserName == string.Empty) { return string.Empty; }
            var _Format = GetText(UserName, "FMTCurrency");
            var _Sign = GetCurrencySign(UserName);
            var _Amount = ((decimal)Amount).ToString(_Format);
            return string.Concat(_Amount, " ", _Sign);
        }
        public static string Amount(string UserName, object Amount)
        {
            if (UserName == null || UserName == string.Empty) { return string.Empty; }
            var _Format = GetText(UserName, "FMTCurrency");
            return ((decimal)Amount).ToString(_Format);

        }

        public static string Date(string UserName, DateTime Date)
        {
            if (UserName == null || UserName == string.Empty) { return string.Empty; }
            var _Format = GetText(UserName, "FMTDate");
            return Date.ToString(_Format);
        }

        public static string YMD(DateTime Date)
        {
            return Date.ToString(DateYMD);
        }


        public static string GetCurrencySign(string UserName)
        {
            if (UserName == null || UserName == string.Empty) { return string.Empty; }
            var sign = GetText(UserName, "CurrencySign");
            if (sign.Length > 0) { return sign; }
            return "Rs.";
        }

        public static object GetKey(string UserName, string Key, KeyType keytype)
        {
            if (UserName == null || UserName == string.Empty) { return null; }
            object ReturnValue;
            DataTableClass tb_Registry = new(UserName, Tables.Registry);
            tb_Registry.MyDataView.RowFilter = string.Concat("Code='", Key, "'");
            if (tb_Registry.MyDataView.Count == 1)
            {
                ReturnValue = keytype switch
                {
                    KeyType.Number => tb_Registry.MyDataView[0]["nValue"],
                    KeyType.Currency => tb_Registry.MyDataView[0]["mValue"],
                    KeyType.Boolean => tb_Registry.MyDataView[0]["bValue"],
                    KeyType.Date => tb_Registry.MyDataView[0]["dValue"],
                    KeyType.Text => tb_Registry.MyDataView[0]["cValue"],
                    KeyType.UserName => tb_Registry.MyDataView[0]["UserName"],
                    _ => string.Empty
                };
            }
            else
            {
                ReturnValue = keytype switch
                {
                    KeyType.Number => 0,
                    KeyType.Currency => 0.00,
                    KeyType.Boolean => false,
                    KeyType.Date => DateTime.Now,
                    KeyType.Text => string.Empty,
                    KeyType.UserName => UserName,
                    _ => string.Empty
                };
            }
            return ReturnValue;
        }

        public static DateTime GetDate(string UserName, string Key)
        {
            if (UserName == null || UserName == string.Empty) { return DateTime.Now; }
            DataTableClass tb_Registry = new(UserName, Tables.Registry);
            tb_Registry.MyDataView.RowFilter = string.Concat("Code='", Key, "'");
            if (tb_Registry.MyDataView.Count == 1)
            {
                return (DateTime)tb_Registry.MyDataView[0]["dValue"];
            }
            else
            {
                return DateTime.Now;
            }
        }

        public static int GetNumber(string UserName, string Key)
        {
            if (UserName == null || UserName == string.Empty) { return 0; }
            DataTableClass tb_Registry = new(UserName, Tables.Registry);
            tb_Registry.MyDataView.RowFilter = string.Concat("Code='", Key, "'");
            if (tb_Registry.MyDataView.Count == 1)
            {
                return (int)tb_Registry.MyDataView[0]["nValue"];
            }
            else
            {
                return 0;
            }
        }

        public static decimal GetCurrency(string UserName, string Key)
        {
            if (UserName == null || UserName == string.Empty) { return 0.00M; }
            DataTableClass tb_Registry = new(UserName, Tables.Registry);
            tb_Registry.MyDataView.RowFilter = string.Concat("Code='", Key, "'");
            if (tb_Registry.MyDataView.Count == 1)
            {
                return (decimal)tb_Registry.MyDataView[0]["mValue"];
            }
            else
            {
                return 0.00M;
            }
        }

        public static string GetText(string UserName, string Key)
        {
            if (UserName == null || UserName == string.Empty) { return string.Empty; }
            DataTableClass tb_Registry = new(UserName, Tables.Registry);
            tb_Registry.MyDataView.RowFilter = $"Code='{Key}'";
            if (tb_Registry.MyDataView.Count == 1)
            {
                var value = tb_Registry.MyDataView[0]["cValue"];
                if (value == DBNull.Value) { return ""; }
                return (string)value;
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool GetBool(string UserName, string Key)
        {
            if (UserName == null || UserName == string.Empty) { return false; }
            DataTableClass tb_Registry = new(UserName, Tables.Registry);
            tb_Registry.MyDataView.RowFilter = string.Concat("Code='", Key, "'");
            if (tb_Registry.MyDataView.Count == 1)
            {
                var value = tb_Registry.MyDataView[0]["bValue"];
                if (value == DBNull.Value) { return false; }
                return (bool)value;
            }
            else
            {
                return false;
            }
        }

        public static DateTime[] GetDates(string UserName, string Key)
        {
            if (UserName == null || UserName == string.Empty) { return new DateTime[2]; }
            DateTime[] Dates = new DateTime[2];
            DataTableClass tb_Registry = new(UserName, Tables.Registry);
            tb_Registry.MyDataView.RowFilter = string.Concat($"Code='{Key}'");
            if (tb_Registry.MyDataView.Count == 1)
            {
                Dates[0] = (DateTime)tb_Registry.MyDataView[0]["From"];
                Dates[1] = (DateTime)tb_Registry.MyDataView[0]["To"];
            }
            return Dates;
        }

        public static bool SetKey(string UserName, string _Key, object KeyValue, KeyType _KeyType)
        {
            if (UserName == null || UserName == string.Empty) { return false; ; }
            return SetKey(UserName, _Key, KeyValue, _KeyType, "");
        }

        public static bool SetKey(string UserName, string Key, object KeyValue, KeyType keytype, string _Title)
        {

            if (UserName == null || UserName == string.Empty) { return false; }
            DataTableClass tb_Registry = new(UserName, Tables.Registry);
            tb_Registry.MyDataView.RowFilter = string.Concat("Code='", Key, "'");
            if (tb_Registry.MyDataView.Count == 1)
            {
                tb_Registry.CurrentRow = tb_Registry.MyDataView[0].Row;
            }
            else
            {
                tb_Registry.CurrentRow = tb_Registry.NewRecord();
                tb_Registry.CurrentRow["ID"] = 0;
            }

            tb_Registry.CurrentRow["Code"] = Key;
            tb_Registry.CurrentRow["Title"] = _Title;
            tb_Registry.CurrentRow["UserName"] = UserName;
            switch (keytype)
            {
                case KeyType.Number:
                    tb_Registry.CurrentRow["nValue"] = KeyValue;
                    break;
                case KeyType.Currency:
                    tb_Registry.CurrentRow["mValue"] = KeyValue;
                    break;
                case KeyType.Date:
                    tb_Registry.CurrentRow["dValue"] = KeyValue;
                    break;
                case KeyType.Boolean:
                    tb_Registry.CurrentRow["bValue"] = KeyValue;
                    break;
                case KeyType.Text:
                    tb_Registry.CurrentRow["cValue"] = KeyValue;
                    break;
                case KeyType.From:
                    tb_Registry.CurrentRow["From"] = KeyValue;
                    break;
                case KeyType.To:
                    tb_Registry.CurrentRow["To"] = KeyValue;
                    break;
                default:
                    break;
            }

            tb_Registry.Save();
            return false;
        }

        public static int ExpDays(string UserName)
        {
            if (UserName == null || UserName == string.Empty) { return 0; }
            int Days = (int)GetKey(UserName, "StockExpiry", KeyType.Number);
            return Days;   // One Year of Expiry Date
        }

        public static string GetReportFooter(string UserName)
        {
            var _Footer = GetText(UserName, "ReportFooter");
            if (_Footer != null && _Footer.Length == 0)
            {
                return "Power by Applied Software House";
            }
            return _Footer;

        }

        #region Registry Table
        public static object Get(string UserName, object Key, KeyType KeyType)
        {
            var _Text = string.Format("SELECT * FROM [Registry] WHERE Code='{0}'", Key.ToString());
            SQLiteConnection _Connection = ConnectionClass.AppConnection(UserName);
            SQLiteCommand _Command = new(_Text, _Connection);
            SQLiteDataAdapter _Adapter = new(_Command);
            DataSet _DataSet = new();
            _Adapter.Fill(_DataSet, "Registry");
            if (_DataSet.Tables.Count == 1)
            {
                if (_DataSet.Tables[0].Rows.Count == 1)
                {
                    if (KeyType == KeyType.Text) { return _DataSet.Tables[0].Rows[0]["cValue"]; }
                    if (KeyType == KeyType.Date) { return _DataSet.Tables[0].Rows[0]["dValue"]; }
                    if (KeyType == KeyType.Number) { return _DataSet.Tables[0].Rows[0]["nValue"]; }
                    if (KeyType == KeyType.Currency) { return _DataSet.Tables[0].Rows[0]["mValue"]; }
                    if (KeyType == KeyType.Boolean) { return _DataSet.Tables[0].Rows[0]["bValue"]; }
                }
            }

            return null;
        }
        #endregion

    }
}
