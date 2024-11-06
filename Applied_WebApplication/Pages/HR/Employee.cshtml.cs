using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.HR
{
    [Authorize]
    public class EmployeeModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public string UserName => User.Identity.Name;
        public List<Message> ErrorMessages { get; set; } = new();


        public void OnGet(int? ID)
        {
            try
            {
                ID ??= 0;
                var UserName = User.Identity.Name;
                DataTableClass Employees = new(UserName, Tables.Employees, $"ID={ID}");
                Variables = new()
                {
                    ID = 0,
                    Code = "",
                    Title = "",
                    FullName = "",
                    Designation = string.Empty,
                    Contact = string.Empty,
                    Address = string.Empty,
                    City = string.Empty,
                    CNIC = string.Empty,
                    DOB = new DateTime(2000, 1, 1),
                    Join = new DateTime(2000, 1, 1),
                    Left = new DateTime(2000, 1, 1)
                };
                if (Employees.Count > 0)
                {
                    Employees.RemoveNull();
                    Variables.ID = (int)Employees.CurrentRow["ID"];
                    Variables.Code = Employees.CurrentRow["Code"].ToString();
                    Variables.Title = Employees.CurrentRow["Title"].ToString();
                    Variables.FullName = Employees.CurrentRow["Full_Name"].ToString();
                    Variables.Designation = Employees.CurrentRow["Designation"].ToString();
                    Variables.Contact = Employees.CurrentRow["Contact"].ToString();
                    Variables.Address = Employees.CurrentRow["Address"].ToString();
                    Variables.City = Employees.CurrentRow["City"].ToString();
                    Variables.CNIC = Employees.CurrentRow["CNIC"].ToString();
                    Variables.DOB = (DateTime)Employees.CurrentRow["DOB"];
                    Variables.Join = (DateTime)Employees.CurrentRow["join"];
                    Variables.Left = (DateTime)Employees.CurrentRow["left"];
                }
            }
            catch (Exception e)
            {
                ErrorMessages.Add(MessageClass.SetMessage(e.Message));
            }
        }


        public IActionResult OnPostSave(int ID)
        {
            DataTableClass Employees = new(UserName, Tables.Employees);
            Employees.NewRecord();

            Employees.MyDataView.RowFilter = $"ID={ID}";
            if(Employees.MyDataView.Count == 1) { Employees.CurrentRow = Employees.MyDataView[0].Row; }

            RemoveNulls();
            Employees.CurrentRow["ID"] = Variables.ID;
            Employees.CurrentRow["Code"] = Variables.Code.ToString();
            Employees.CurrentRow["Title"] = Variables.Title.ToString();
            Employees.CurrentRow["Designation"] = Variables.Designation.ToString();
            Employees.CurrentRow["Full_Name"] = Variables.FullName.ToString();
            Employees.CurrentRow["Contact"] = Variables.Contact.ToString();
            Employees.CurrentRow["Address"] = Variables.Address.ToString();
            Employees.CurrentRow["City"] = Variables.City.ToString();
            Employees.CurrentRow["CNIC"] = Variables.CNIC.ToString();
            Employees.CurrentRow["DOB"] = Variables.DOB;
            Employees.CurrentRow["Join"] = Variables.Join;
            Employees.CurrentRow["Left"] = Variables.Left;

            Employees.Save();
            ErrorMessages = Employees.ErrorMessages;
            if (ErrorMessages.Count == 0)
            {
                ErrorMessages.Add(MessageClass.SetMessage("Employee's record has been saved.", Color.Green));
            }
                return Page(); 
        }

        private void RemoveNulls()
        {
            if (Variables.Code == null) { Variables.Code = ""; }
            if (Variables.Title == null) { Variables.Title = ""; }
            if (Variables.Designation == null) { Variables.Designation = ""; }
            if (Variables.FullName == null) { Variables.FullName = ""; }
            if (Variables.Contact == null) { Variables.Contact = ""; }
            if (Variables.Address == null) { Variables.Address = ""; }
            if (Variables.City == null) { Variables.City = ""; }
            if (Variables.CNIC == null) { Variables.CNIC = ""; }
            //if(Variables.DOB==null) { Variables.DOB = new DateTime(); }
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
            public DateTime Left { get; set; }
            public string CNIC { get; set; }
        }

    }
}
