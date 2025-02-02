using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace Applied_WebApplication.Pages.Sales
{
    public class DeliveryChallanListModel : PageModel
    {
        
        [BindProperty] public string SearchText { get; set; } = string.Empty;
        public string UserName => User.Identity.Name;

        private DataTableClass _Table { get; set; }
      

        public void OnGet()
        {
            var _Filter = AppRegistry.GetText(UserName, "DC");
            _Table = new DataTableClass(UserName, SQLQuery.GetDeliveryChallans(_Filter));
        }

        public IActionResult OnPostSearch(string _Text)
        {
            return Page();
        }

        public IActionResult OnPostClear()
        {
            SearchText = string.Empty;
            return Page();
        }


    }

    
}
