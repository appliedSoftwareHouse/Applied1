using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NPOI.SS.Formula.Functions;
using System.Data;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class TBCustomersModel : PageModel
    {

        [BindProperty]
        public MyParameters Variables { get; set; }
        public string ReportLink { get; set; }
        public bool IsShowPdf { get; set; } = false;

        public DataTable TBCus = new();                                                        // Trial Balance Code class
        public decimal Tot_DR { get; set; } = 0.00M;
        public decimal Tot_CR { get; set; } = 0.00M;
        public string UserName => User.Identity.Name;
        public List<Message> ErrorMessages { get; set; }

        public void OnGet()
        {
            var _Date = (DateTime.Now).ToString(AppRegistry.DateYMD);
            var _Filter = $"Date(Vou_Date) <= Date('{_Date}')";
            TBCus = DataTableClass.GetTable(UserName, SQLQuery.TBCustomer(UserName, _Filter));
            

        }


        #region Variables
        public class MyParameters
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public string ReportType { get; set; }
            public string ReportOption { get; set; }
            public decimal Tot_DR { get; set; }
            public decimal Tot_CR { get; set; }
        }
        #endregion
    }
}
