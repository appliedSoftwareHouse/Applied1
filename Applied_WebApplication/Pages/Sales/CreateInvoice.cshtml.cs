using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Sales
{
    [Authorize]
    public class CreateInvoiceModel : PageModel
    {
        public Parameters Variables { get; set; }
        public DataTable Tb_Expense { get; set; }
        public DataTable Tb_Stock { get; set; }
        public string UserName => User.Identity.Name;
        public string UserRole => UserProfile.GetUserRole(User);
        public string ExpenseSheet { get; set; }
        private string Filter { get; set; }

        public void OnGet()
        {

            ExpenseSheet = "Sheet # 001";

            if (ExpenseSheet != null)
            {
                Filter = $"Sheet_No='{ExpenseSheet}'";
                var _Expense = new DataTableClass(UserName, SQLQuery.ExpenseSheet(Filter), "");
                Tb_Expense = _Expense.MyDataTable;

                Tb_Stock = DataTableClass.GetTable(UserName, Tables.Inventory);
            }
        }

        public class Parameters
        {
            public int ID { get; set; }
        }
    }
}
