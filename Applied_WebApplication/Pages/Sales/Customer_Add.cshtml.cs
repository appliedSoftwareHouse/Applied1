using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using static Applied_WebApplication.Data.TableValidationClass;

namespace Applied_WebApplication.Pages.Sales
{
    public class Customer_AddModel : PageModel
    {

        public Customer Record = new();
        public bool IsError = false;
        public List<Message> ErrorMessages;

        public void OnGet()
        {
        }

        public void OnPostSave(Customer _Record, string UserName)
        {

            DataTableClass Customers = new(UserName, Tables.Customers.ToString());

            Customers.NewRecord();
            Customers.CurrentRow["ID"] = 0;
            Customers.CurrentRow["Code"] = _Record.Code;
            Customers.CurrentRow["Title"] = _Record.Title;
            Customers.CurrentRow["Address1"] = _Record.Address1;
            Customers.CurrentRow["Address2"] = _Record.Address2;
            Customers.CurrentRow["City"] = _Record.City;
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
                Page();
            }
            else { RedirectToPage("Customers"); }

        }

    }


    public class Customer
    {
        public int ID { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Title { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string NTN { get; set; }
        public string CNIC { get; set; }
        public string Notes { get; set; }
    }

}
