using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Stock
{
    public class FinishedGoodsModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public List<Message> ErrorMessages { get; set; } = new();

        private string UserName => User.Identity.Name;
        private int ExpDays => AppRegistry.ExpDays(User.Identity.Name);
        public void OnGet(int? id)
        {

            if (id == null)
            {
                Variables = new()
                {
                    MFDate = DateTime.Now,
                    EXPDate = DateTime.Now.AddDays(ExpDays)
                };
            }
            else
            {
                DataTableClass _Table = new(UserName, Tables.FinishedGoods);
                DataRow Row = _Table.SeekRecord((int)id);
                Variables = new()
                {
                    ID = (int)Row["ID"],
                    Batch = Row["Batch"].ToString(),
                    MFDate = (DateTime)Row["MFDate"],
                    EXPDate = (DateTime)Row["EXPDate"],
                    Process = (int)Row["Process"],
                    Product = (int)Row["Product"],
                    Qty = (decimal)Row["Qty"],
                    Rate = (decimal)Row["Rate"],
                    Amount = (decimal)Row["Amount"],
                    Remarks = Row["Remarks"].ToString(),
                    Project = (int)Row["Project"],
                    Employee = (int)Row["Employee"]
                };
            }


        }

        public void OnPostSave()
        {
            DataTableClass _Table = new(UserName, Tables.FinishedGoods);
            _Table.SeekRecord(Variables.ID);
            _Table.CurrentRow["ID"] = Variables.ID;
            _Table.CurrentRow["Batch"] = Variables.Batch;
            _Table.CurrentRow["MFDate"] = Variables.MFDate;
            _Table.CurrentRow["EXPDate"] = Variables.EXPDate;
            _Table.CurrentRow["Process"] = Variables.Process;
            _Table.CurrentRow["Product"] = Variables.Product;
            _Table.CurrentRow["Qty"] = Variables.Qty;
            _Table.CurrentRow["Rate"] = Variables.Rate;
            _Table.CurrentRow["Amount"] = Variables.Amount;
            _Table.CurrentRow["Remarks"] = Variables.Remarks;
            _Table.CurrentRow["Project"] = Variables.Project;
            _Table.CurrentRow["Employee"] = Variables.Employee;
            _Table.Save();
            ErrorMessages = _Table.TableValidation.MyMessages;                      // Get Error Messages if any
            if (ErrorMessages.Count == 0)
            {
                ErrorMessages.Add(new Message { ErrorID = 0, Msg = "Record has been saved.", Success = true });
            }
        }

        public IActionResult OnPostBack()
        {
            return RedirectToPage("./FinishedGoodsList");
        }

        public class MyParameters
        {
            public int ID { get; set; }
            public string Batch { get; set; }
            public DateTime MFDate { get; set; }
            public DateTime EXPDate { get; set; }
            public int Process { get; set; }
            public int Product { get; set; }
            public decimal Qty { get; set; }
            public decimal Rate { get; set; }
            public decimal Amount { get; set; }
            public string Remarks { get; set; }
            public int Project { get; set; }
            public int Employee { get; set; }

        }

    }
}
