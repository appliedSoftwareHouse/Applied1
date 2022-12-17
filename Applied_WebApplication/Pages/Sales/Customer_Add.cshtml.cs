using Applied_WebApplication.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Sales
{
    public class Customer_AddModel : PageModel
    {
        
        public Customer Record = new();

        public void OnGet()
        {
        }

        public void OnPostSave(Customer _Record, string UserName)
        {
            TableValidationClass TableValidation = new();
            DataTableClass Customers = new(UserName, Tables.Customers.ToString());
            TableValidation.MyDataTable = Customers.MyDataTable;

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
             _Validation =  Customers.Save();

            if(_Validation.MyMessages.Count > 0)
            {
                Page();

            }
            else
            {
                RedirectToPage("Customers");
            }


        }

    }


    public class Customer
    {
        public int ID { get; set; }
        public string Code { get; set; }
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
