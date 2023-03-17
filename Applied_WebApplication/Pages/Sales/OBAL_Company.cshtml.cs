using Applied_WebApplication.Pages.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Sales
{
    public class OBAL_CompanyModel : PageModel
    {

        [BindProperty]
        public MyParameters Variables { get; set; }
        public bool IsError = false;
        public List<Message> ErrorMessages = new();
        public DataTable OBalance;
        public string UserName => User.Identity.Name;

        public void OnGet(int? ID)
        {
            DataTableClass OBalCompany = new(UserName, Tables.OBALCompany);
            OBalance = OBalCompany.MyDataTable;

            if (ID == null)
            {

                Variables = new()
                {
                    ID = 0,
                    Company = 0,
                    COA = 0,
                    Amount = 0.00M
                };

            }
            else
            {
                OBalCompany.MyDataView.RowFilter = "ID=" + ID.ToString();
                OBalCompany.SeekRecord((int)ID);
                Variables = new()
                {
                    ID = (int)OBalCompany.CurrentRow["ID"],
                    Company = (int)OBalCompany.CurrentRow["Company"],
                    COA = (int)OBalCompany.CurrentRow["COA"],
                    Amount = (decimal)OBalCompany.CurrentRow["Amount"]
                };

            }


        }

        public void OnPostSave()
        {
            DataTableClass OBalCompany = new(UserName, Tables.OBALCompany);
            OBalCompany.MyDataView.RowFilter = "ID=" + Variables.ID.ToString();
            if (OBalCompany.Count == 1) { OBalCompany.SeekRecord(Variables.ID); }
            else { OBalCompany.NewRecord(); }
            OBalCompany.CurrentRow["ID"] = Variables.ID;
            OBalCompany.CurrentRow["Company"] = Variables.Company;
            OBalCompany.CurrentRow["COA"] = Variables.COA;
            OBalCompany.CurrentRow["Amount"] = Variables.Amount;
            OBalCompany.Save();
            ErrorMessages = OBalCompany.TableValidation.MyMessages;

            // Refresh Data
            OBalCompany = new(UserName, Tables.OBALCompany);
            OBalance = OBalCompany.MyDataTable;

        }

        public IActionResult OnPostEdit(int ID)
        {
            return RedirectToPage("OBAL_Company", routeValues: new { ID });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int ID)
        {
            //var ID = Variables.ID;
            DataTableClass _Table = new(UserName, Tables.OBALCompany);
            _Table.MyDataView.RowFilter = "ID=" + ID.ToString();
            if (_Table.Count == 1)
            {
                _Table.SeekRecord(ID);
                await Task.Run(() => _Table.Delete());
                ErrorMessages = _Table.TableValidation.MyMessages;
            }

            // Refresh Data
            _Table = new(UserName, Tables.OBALCompany);
            OBalance = _Table.MyDataTable;

            return Page();

        }

        public IActionResult OnPostBack()
        {

            return RedirectToPage("../Index");
        }



        #region Parameters
        public class MyParameters
        {
            public int ID { get; set; }
            public int Company { get; set; }
            public int COA { get; set; }
            public int Project { get; set; }
            public decimal Amount { get; set; }
                    }
        #endregion
    }
}
