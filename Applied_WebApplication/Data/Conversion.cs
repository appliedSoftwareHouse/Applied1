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
            var type = _Value.GetType();

            if (type == typeof(string)) { return int.Parse((string)_Value); }
            if (type == typeof(decimal)) { return int.Parse(_Value.ToString()); }
            if (type == typeof(long)) { return int.Parse(_Value.ToString()); }
            if (type == typeof(int)) { return (int)_Value; }
            return 0;
        }


        public static DateTime ToDateTime(object _Value)
        {
            var type = _Value.GetType();

            if (type == typeof(string)) { return DateTime.Parse((string)_Value); }
            if (type == typeof(decimal)) { return DateTime.Parse(_Value.ToString()); }
            if (type == typeof(long)) { return DateTime.Parse(_Value.ToString()); }
            if (type == typeof(DateTime)) { return (DateTime)_Value; }
            return new DateTime();
        }

    }
}
