using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Stock
{
    public class OpeningCompanyModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public List<Message> ErrorMessages = new();
        public string UserName => User.Identity.Name;
        public void OnGet()
        {
            Variables = new()
            {
                OBDate = (DateTime)AppRegistry.GetKey(UserName, "OBDate", KeyType.Date),
                COA_CR = (int)AppRegistry.GetKey(UserName, "OBCompanyDR", KeyType.Number),
                COA_DR = (int)AppRegistry.GetKey(UserName, "OBCompanyCR", KeyType.Number)
            };
        }

        public IActionResult OnPostCompany()
        {
            AppRegistry.SetKey(UserName, "OBCompanyDR", Variables.COA_DR, KeyType.Number);
            AppRegistry.SetKey(UserName, "OBCompanyCR", Variables.COA_CR, KeyType.Number);
            if(Variables.COA_DR ==0 || Variables.COA_CR == 0)
            {
                ErrorMessages.Add(MessageClass.SetMessage("Chart of Accounts are not valid to post company opening balance.", ConsoleColor.Red));
            }
            else
            {
                ErrorMessages = PostingClass.PostOpeningBalanceCompany(UserName);
            }
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
