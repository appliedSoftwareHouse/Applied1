using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace Applied_WebApplication.Pages.Stock
{
    public class OpeningStockModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public string UserName => User.Identity.Name;
        public void OnGet()
        {
            Variables = new()
            {
                OBDate = (DateTime)AppRegistry.GetKey(UserName, "OBDate", KeyType.Date),
                COA_CR = (int)AppRegistry.GetKey(UserName, "OBStockDR", KeyType.Number),
                COA_DR = (int)AppRegistry.GetKey(UserName, "OBStockCR", KeyType.Number)
            };
        }


        public IActionResult OnPostStock()
        {
            AppRegistry.SetKey(UserName, "OBStockDR", Variables.COA_DR, KeyType.Number);
            AppRegistry.SetKey(UserName, "OBStockCR", Variables.COA_CR, KeyType.Number);
            PostingClass.PostOpeningBalanceStock(UserName);
            return Page();
        }


        public class MyParameters
        {
            public int COA_DR { get; set; }
            public int COA_CR { get; set; }
            public DateTime OBDate { get; set; }
        }
    }
}
