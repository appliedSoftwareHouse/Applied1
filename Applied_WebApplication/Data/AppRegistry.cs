using AspNetCore.Reporting;
using System.Diagnostics;
using System.Security.Permissions;

namespace Applied_WebApplication.Data
{
    public interface IAppRegistry
    {
        public static readonly string DateYMD; 
        public static readonly string FormatCurrency1; 
        public static readonly string FormatCurrency2; 
        public static readonly string FormatDate; 
        public static readonly string FormatDateY2; 
        public static readonly string FormatDateM2;
    }


    public class AppRegistry : IAppRegistry
    {
        public AppRegistry(string UserName)
        {
            DataTableClass tb_Registry = new(UserName, Tables.Registry);


            //Create Default Application Registry Keys.
            if (!tb_Registry.Seek("FiscalFrom")) { SetKey(UserName, "FiscalFrom", new DateTime(2022, 1, 1), KeyType.Date); }
            if (!tb_Registry.Seek("FiscalTo")) { SetKey(UserName, "FiscalTo", new DateTime(2025, 12, 31), KeyType.Date); }
            if (!tb_Registry.Seek("TBSort")) { SetKey(UserName, "TBSort", "Code", KeyType.Text); }
        }

        public static readonly string DateYMD = "yyyy-MM-dd";
        public static readonly string FormatCurrency1 = "#,##0.00";
        public static readonly string FormatCurrency2 = "#,##0";
        public static readonly string FormatDate = "dd-MMM-yyyy";
        public static readonly string FormatDateY2 = "dd-MMM-yy";
        public static readonly string FormatDateM2 = "dd-MM-yy";
        public static readonly DateTime MinDate = new DateTime(2020, 01, 01);

        public static DateTime GetFiscalFrom() { return new DateTime(2022, 07, 01); }           // In future addign value from App Registry
        public static DateTime GetFiscalTo() { return new DateTime(2023, 06, 30); }

        public static string GetFormatCurrency(string UserName)
        {
            return GetKey(UserName, "FMTCurrency", KeyType.Text).ToString();
        }

        public static object GetKey(string UserName, string Key, KeyType keytype)
        {
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

        public static DateTime[] GetDates(string UserName, string Key)
        {
            DateTime[] Dates = new DateTime[2];
            DataTableClass tb_Registry = new(UserName, Tables.Registry);
            tb_Registry.MyDataView.RowFilter = string.Concat("Code='", Key, "'");
            if (tb_Registry.MyDataView.Count == 1)
            {
                Dates[0] = (DateTime)tb_Registry.MyDataView[0]["From"];
                Dates[1] = (DateTime)tb_Registry.MyDataView[0]["To"];
            }
            return Dates;
        }


        public static bool SetKey(string UserName, string _Key, object KeyValue, KeyType _KeyType)
        {
            return SetKey(UserName, _Key, KeyValue, _KeyType, "");
        }

        public static bool SetKey(string UserName, string Key, object KeyValue, KeyType keytype, string _Title)
        {
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

        
    }
}
