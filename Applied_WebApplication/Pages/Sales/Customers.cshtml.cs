using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Drawing;

namespace Applied_WebApplication.Pages.Sales
{
    [Authorize]
    public class CustomersModel : PageModel
    {
        [BindProperty] public string SearchText { set; get; } 
        internal DataTable Customer { get; set; }
        internal DataTable MyTable { get; set; }

        public string UserName => User.Identity.Name;
        public void OnGet()
        {
            SearchText = AppRegistry.GetText(UserName,"ctmSearch");
            Customer = DataTableClass.GetTable(UserName, Tables.Customers, $"{GetFilter()}");
            
        }

        private string GetFilter()
        {
            var _Filter = string.Empty;

            _Filter += $"[Title] Like '%{SearchText}%' OR ";
            _Filter += $"[City] Like '%{SearchText}%' OR ";
            _Filter += $"[Phone] Like '%{SearchText}%' OR ";
            _Filter += $"[Mobile] Like '%{SearchText}%' OR ";
            _Filter += $"[Email] Like '%{SearchText}%'";

            return _Filter;
        }

        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "ctmSearch", SearchText, KeyType.Text);
            //Customer = DataTableClass.GetTable(UserName, Tables.Customers, $"{GetFilter()}");
            return RedirectToPage();
        }

        public IActionResult OnPostClear()
        {
            
            AppRegistry.SetKey(UserName, "ctmSearch", string.Empty, KeyType.Text);
            //Customer = DataTableClass.GetTable(UserName, Tables.Customers, $"{GetFilter()}");
            return RedirectToPage();
        }
    }
}
