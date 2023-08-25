using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SQLite;

namespace Applied_WebApplication.Pages.Applied
{
    [Authorize]
    public class SQLExecuteModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public string UserName => User.Identity.Name;

        public void OnGet()
        {
            Variables = new()
            {
                QueryMessage = "Hi !!!"
            };

        }


       

        public IActionResult OnPostSQLExecute()
        {
            var _Connection = ConnectionClass.AppConnection(UserName);
            var _Query = Variables.SQLQuery;
            var _Command = new SQLiteCommand(_Query, _Connection);

            try
            {
                var num = _Command.ExecuteNonQuery();
               Variables.QueryMessage = $"{num} Number of rows effected";
            }
            catch (Exception e)
            {
                Variables.QueryMessage = e.Message; 
            }

            

            return Page();
        }

        public IActionResult OnPost()
        {
            return Page();
        }

        public class Parameters
        {
            public string SQLQuery { get; set; } = "UPDATE [Registry] SET ([nValue]=50) WHERE Code = 'Test'";
            public string QueryMessage { get; set; } = string.Empty;
        }
    }
}
