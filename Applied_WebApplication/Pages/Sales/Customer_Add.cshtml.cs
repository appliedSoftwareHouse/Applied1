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
            DataTableClass Customers = new(UserName, Tables.Customers.ToString());



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
        public string Contact { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string NTN { get; set; }
        public string CNIC { get; set; }
        public string Notes { get; set; }
    }

}
