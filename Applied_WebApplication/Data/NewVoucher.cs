using System.Data;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace Applied_WebApplication.Data
{
    public class NewVoucher
    {
        public static string GetNewVoucher(DataTable _Table, string _Prefix, DateTime _Date)
        {
            var _Year = _Date.ToString("yy");
            var _Month = _Date.ToString("MM");
            var _View = _Table.AsDataView();
            var _NewNum = $"{_Prefix}{_Year}{_Month}";
            _View.RowFilter = $"[Vou_No] like '{_NewNum}%'";

            if (_View.Count == 0) { return $"{_NewNum}-0001"; }
            else
            {
                string MaxVouNo = (string)_Table.Compute("MAX(Vou_No)", _View.RowFilter);
                string _Vou_No1 = MaxVouNo.Substring(0, 6);
                string _Vou_No2 = MaxVouNo.Substring(7, 4);
                int _Number = Conversion.ToInteger(_Vou_No2) + 1;
                string _MaxNum = Conversion.ToInteger(_Number).ToString("0000");
                return $"{_Vou_No1}-{_MaxNum}";
            }
        }


        public static string GetNewVoucher(DataTable _Table, string _Prefix)
        {
            return GetNewVoucher(_Table, _Prefix, DateTime.Now);
            
        }
    }
}
