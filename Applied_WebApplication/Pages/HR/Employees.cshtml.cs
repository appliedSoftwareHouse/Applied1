using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.HR
{
    public class EmployeesModel : PageModel
    {
        public MyParameters Variables { get; set; }
        public DataTable MyTable { get; set; }

        public void OnGet()
        {
            var UserName = User.Identity.Name;
            DataTableClass _Table = new(UserName, Tables.Employees);
            _Table.MyDataView.Sort = "Code";
            MyTable = _Table.MyDataView.ToTable();
            Variables = new();
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
