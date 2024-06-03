using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Applied_WebApplication.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public string UserName => User.Identity.Name;

        public void OnGet()
        {
            Variables = new();
        }

        public IActionResult OnPostUpdatePwd()
        {
            var _Psw1 = Variables.Psw1;
            var _Psw2 = Variables.Psw2;

            if (_Psw1 is null || _Psw2 is null) { return Page(); }
            if (_Psw1.Length == 0 || _Psw2.Length == 0) { return Page(); }


            if (Variables.Psw1.Trim().Equals(Variables.Psw2.Trim()))
            {
                var _UserClass = new AppliedUsersClass();
                var _UserRow = _UserClass.UserRecord(UserName);
                var _RowID = (int)_UserRow["ID"];
                var _Text = $"UPDATE [Users] SET password='{_Psw1}' WHERE ID={_RowID};";

                _UserClass.AppliedUserCommand.CommandText = _Text;
                var _RecNo = _UserClass.AppliedUserCommand.ExecuteNonQuery();
                if(_RecNo > 0)
                {
                    return RedirectToPage("../Account/PasswordSaved");
                }


            }
            return Page();
        }
    }

    public class Parameters
    {
        public string Psw1 { get; set; } = string.Empty;
        public string Psw2 { get; set; } = string.Empty;
    }
}
