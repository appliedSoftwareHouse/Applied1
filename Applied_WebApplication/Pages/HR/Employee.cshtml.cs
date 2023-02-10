using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace Applied_WebApplication.Pages.HR
{
    public class EmployeeModel : PageModel
    {
        public MyParameters Variables { get; set; }


        public void OnGet(int? id)
        {
            id ??= 0;
            var UserName = User.Identity.Name;
            DataTableClass _Table = new(UserName, Tables.Employees);
            Variables = new();
            _Table.SeekRecord((int)id);
            Variables.ID = (int)_Table.CurrentRow["ID"];
            Variables.Code = _Table.CurrentRow["Code"].ToString();
            Variables.Title = _Table.CurrentRow["Title"].ToString(); ;
            Variables.Designation = _Table.CurrentRow["Designation"].ToString(); ;
            Variables.Contact = _Table.CurrentRow["Contact"].ToString(); ;
            Variables.Address = _Table.CurrentRow["Address"].ToString(); ;
            Variables.City = _Table.CurrentRow["City"].ToString(); ;
            Variables.CNIC = _Table.CurrentRow["CNIC"].ToString(); ;
            Variables.DOB = (DateTime)_Table.CurrentRow["DOB"] ;
            Variables.Join = (DateTime)_Table.CurrentRow["join"];
            Variables.left = (DateTime)_Table.CurrentRow["left"];
        }


        public IActionResult OnPostSave(int Id)
        {
            var UserName = User.Identity.Name;
            DataTableClass _Table = new(UserName, Tables.Employees);
            if(_Table.Seek(Id))
            {
                _Table.CurrentRow = _Table.SeekRecord(Id);
                _Table.CurrentRow["ID"] = Variables.ID;
                _Table.CurrentRow["Code"] = Variables.Code.ToString();
                _Table.CurrentRow["Title"] = Variables.Title.ToString();
                _Table.CurrentRow["Designation"] = Variables.Designation.ToString();
                _Table.CurrentRow["Full_Name"] = Variables.FullName.ToString();
                _Table.CurrentRow["Contact"] = Variables.Contact.ToString();
                _Table.CurrentRow["Address"] = Variables.Address.ToString();
                _Table.CurrentRow["City"] = Variables.City.ToString();
                _Table.CurrentRow["CNIC"] = Variables.CNIC.ToString();
                _Table.CurrentRow["DOB"] = Variables.DOB;
                _Table.CurrentRow["Join"] = Variables.Join;
                _Table.CurrentRow["Left"] = Variables.left;
            }
            return Page();
        }


        public IActionResult OnPostBack()
        {
            return RedirectToPage("./Employees");
        }

        public class MyParameters
        {
            public int ID { get; set; }
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
