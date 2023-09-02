using System.Data.Entity.ModelConfiguration.Conventions;
using System.Numerics;

namespace Applied_WebApplication.Data
{
    public class Conversion
    {

        public static string Row2Money(object _Value, string _Format)
        {
            return ToDecimal(_Value).ToString(_Format);
        }

        public static string Row2Date(object _Value)
        {
            string _Result = string.Empty;
            if (_Value == null) { return _Result; }

            try
            {
                var _Date = (DateTime)_Value;
                _Result = _Date.ToString(AppRegistry.FormatDate);
            }
            catch (Exception)
            {
                _Result = "No Date";
            }
            return _Result;
        }

        public static int ToInteger(object _Value)
        {
            try
            {
                var type = _Value.GetType();

                if (type == typeof(string)) { return int.Parse((string)_Value); }
                if (type == typeof(decimal)) { return int.Parse(_Value.ToString()); }
                if (type == typeof(long)) { return int.Parse(_Value.ToString()); }
                if (type == typeof(float)) { return int.Parse(_Value.ToString()); }
                if (type == typeof(double)) { return int.Parse(_Value.ToString()); }
                if (type == typeof(int)) { return (int)_Value; }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static decimal ToDecimal(object _Value)
        {
            try
            {
                var type = _Value.GetType();

                if (type == typeof(string)) { return decimal.Parse((string)_Value); }
                if (type == typeof(int)) { return decimal.Parse(_Value.ToString()); }
                if (type == typeof(long)) { return decimal.Parse(_Value.ToString()); }
                if (type == typeof(float)) { return decimal.Parse(_Value.ToString()); }
                if (type == typeof(double)) { return decimal.Parse(_Value.ToString()); }
                if (type == typeof(decimal)) { return (decimal)_Value; }
                return 0.00M;
            }
            catch (Exception)
            {
                return 0.00M;
            }
        }

    }
}
