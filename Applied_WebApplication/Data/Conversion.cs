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
    }
}
