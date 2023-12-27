using System.Data;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace Applied_WebApplication.Data
{
    public class NewVoucher
    {
        

        public static string GetNewVoucher(DataTable _Table, string _Prefix)
        {
            var _Date = DateTime.Now;
            var _Year = _Date.ToString("yy");
            var _Month = _Date.ToString("MM");
            var _View = _Table.AsDataView();
            var _NewNum = $"{_Prefix}{_Year}{_Month}";
            _View.RowFilter = $"[Vou_No] like '{_NewNum}%'";

            if(_View.Count == 0) { return $"{_NewNum}-0001"; }
            else
            {
                string MaxVouNo = (string)_Table.Compute("MAX(Vou_No)", _View.RowFilter);
                string Number = MaxVouNo.Substring(_View.RowFilter.Length, MaxVouNo.Length);
                string _MaxNum = (int.Parse(Number) + 1).ToString("0000");
                return $"{_NewNum}-{_MaxNum}";
            }
        }
    }
}
