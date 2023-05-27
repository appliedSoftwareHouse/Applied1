using System.Data.Entity.ModelConfiguration.Conventions;

namespace Applied_WebApplication.Data
{
    public class Conversion
    {

        public static string Row2Money(object _Value)
        {
            string _Result = string.Empty;
            if (_Value == null) { return _Result; }

            try
            {
                var Amount = (decimal)_Value;
                _Result = Amount.ToString(AppRegistry.FormatCurrency1);

            }
            catch (Exception e)
            {
                _Result = e.Message;
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
