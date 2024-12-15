using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text;

namespace Applied_WebApplication.Pages.HR
{
    [Authorize]
    public class EmployeesModel : PageModel
    {
        [BindProperty] public MyParameters Variables { get; set; }
        [BindProperty] public string SearchText { get; set; }
        public DataTable MyTable { get; set; }
        public string UserName => User.Identity.Name;

        public void OnGet()
        {
            SearchText = AppRegistry.GetText(UserName, "empSearch");
            string _Filter = string.Empty;
            if (SearchText.Length > 0) { _Filter = GetFilter(); }
            MyTable = DataTableClass.GetTable(UserName, Tables.Employees, _Filter);
            Variables = new();

        }

        private string GetFilter()
        {
            var _Text = new StringBuilder();
            _Text.Append($"[Code] like '%{SearchText}%' OR ");
            _Text.Append($"[Title] like '%{SearchText}%' OR ");
            _Text.Append($"[Designation] like '%{SearchText}%' OR ");
            _Text.Append($"[Full_Name] like '%{SearchText}%'");

            return _Text.ToString();
        }

        public IActionResult OnPostEdit(int ID)
        {
            return RedirectToPage("Employee", routeValues: new { ID });
        }

        public IActionResult OnPostDelete(int ID)
        {
            DataTableClass Employee = new(UserName, Tables.Employees, $"ID={ID}");
            if (Employee.Count == 1)
            {
                Employee.CurrentRow = Employee.Rows[0];
                var Deleted = Employee.Delete();
                if (Deleted)
                {
                    return RedirectToPage("Employee");
                }
            }
            return Page();
        }

        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "empSearch", SearchText, KeyType.Text);
            return RedirectToPage();
        }

        public IActionResult OnPostClear()
        {
            AppRegistry.SetKey(UserName, "empSearch", string.Empty, KeyType.Text);
            return RedirectToPage();
        }



        public class MyParameters
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Title { get; set; }
            public string Designation { get; set; }
            public string FullName { get; set; }
            public string Contact { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public DateTime DOB { get; set; }
            public DateTime Join { get; set; }
            public DateTime left { get; set; }
            public string CNIC { get; set; }

        }

    }
}
