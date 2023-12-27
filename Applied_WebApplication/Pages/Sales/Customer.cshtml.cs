using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Sales
{
    public class Customer_AddModel : PageModel
    {
        [BindProperty]
        public Customer Record { get; set; }
        public bool IsError = false;
        public List<Message> ErrorMessages;
        public string UserName => User.Identity.Name;
        public string MyPageAction { get; set; } = "Add";

        public void OnGetEdit(int id)
        {
            MyPageAction = "Edit";
            DataTableClass Customers = new(UserName, Tables.Customers);
            Customers.SeekRecord(id);
            Record = new()
            {
                ID = id,
                Code = Customers.CurrentRow["Code"].ToString(),
                Title = Customers.CurrentRow["Title"].ToString(),
                Address1 = Customers.CurrentRow["Address1"].ToString(),
                Address2 = Customers.CurrentRow["Address2"].ToString(),
                City = Customers.CurrentRow["City"].ToString(),
                State = Customers.CurrentRow["State"].ToString(),
                Country = Customers.CurrentRow["Country"].ToString(),
                Phone = Customers.CurrentRow["Phone"].ToString(),
                Mobile = Customers.CurrentRow["Mobile"].ToString(),
                NTN = Customers.CurrentRow["NTN"].ToString(),
                CNIC = Customers.CurrentRow["CNIC"].ToString(),
                Notes = Customers.CurrentRow["Notes"].ToString(),
                Email = Customers.CurrentRow["Email"].ToString()
            };

        }

        public void OnGetDelete(int id)
        {
            MyPageAction = "Delete";
            DataTableClass Customers = new(UserName, Tables.Customers);
            Customers.SeekRecord(id);
            Record.ID = id;
            Record.Code = Customers.CurrentRow["Code"].ToString();
            Record.Title = Customers.CurrentRow["Title"].ToString();
            Record.Address1 = Customers.CurrentRow["Address1"].ToString();
            Record.Address2 = Customers.CurrentRow["Address2"].ToString();
            Record.City = Customers.CurrentRow["City"].ToString();
            Record.State = Customers.CurrentRow["State"].ToString();
            Record.Country = Customers.CurrentRow["Country"].ToString();
            Record.Phone = Customers.CurrentRow["Phone"].ToString();
            Record.Mobile = Customers.CurrentRow["Mobile"].ToString();
            Record.NTN = Customers.CurrentRow["NTN"].ToString();
            Record.CNIC = Customers.CurrentRow["CNIC"].ToString();
            Record.Notes = Customers.CurrentRow["Notes"].ToString();
            Record.Email = Customers.CurrentRow["Email"].ToString();

        }

        public IActionResult OnPostSave(Customer _Record)
        {

            DataTableClass Customers = new(UserName, Tables.Customers);


            if (Customers.Seek(_Record.ID))
            {
                Customers.SeekRecord(_Record.ID);
                Customers.CurrentRow["ID"] = _Record.ID;
            }
            else
            {
                Customers.NewRecord();
                Customers.CurrentRow["ID"] = 0;
            }
            Customers.CurrentRow["Code"] = _Record.Code;
            Customers.CurrentRow["Title"] = _Record.Title;
            Customers.CurrentRow["Address1"] = _Record.Address1;
            Customers.CurrentRow["Address2"] = _Record.Address2;
            Customers.CurrentRow["City"] = _Record.City;
            Customers.CurrentRow["State"] = _Record.State;
            Customers.CurrentRow["Country"] = _Record.Country;
            Customers.CurrentRow["Phone"] = _Record.Phone;
            Customers.CurrentRow["Mobile"] = _Record.Mobile;
            Customers.CurrentRow["Email"] = _Record.Email;
            Customers.CurrentRow["NTN"] = _Record.NTN;
            Customers.CurrentRow["CNIC"] = _Record.CNIC;
            Customers.CurrentRow["Notes"] = _Record.Notes;
            Customers.Save();

            IsError = Customers.TableValidation.Success();

            if (IsError)
            {
                ErrorMessages = Customers.TableValidation.MyMessages;
                return Page();
            }
            else
            {
                return RedirectToPage("Customers");
            }

        }

        public IActionResult OnPostDelete(Customer _Record)
        {
            DataTableClass Customers = new(UserName, Tables.Customers);

            if (Customers.Seek(_Record.ID))
            {
                Customers.SeekRecord(_Record.ID);
                Customers.Delete();
            }
            return RedirectToPage("Customers");
        }

    }


    public class Customer
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Customer Code is required")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Customer Title or name is required")]
        public string Title { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string NTN { get; set; }
        public string CNIC { get; set; }
        public string Notes { get; set; }
        public string ContactPerson { get; set; }
    }

}
