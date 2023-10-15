using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Stock
{
    [Authorize]
    public class BOMProcessModel : PageModel
    {
        [BindProperty]
        public MyParameters Variables { get; set; }
        public DataTable Profile { get; set; }
        public string UserName => User.Identity.Name;
        public void OnGet(int? ID)
        {
            ID ??= 0;
            Variables = new();
            var FG_Table = DataTableClass.GetTable(UserName, Tables.FinishedGoods, $"ID={ID}");
            if (FG_Table.Rows.Count > 0)
            {
                Variables = new()
                {
                    Inventory = (int)ID,
                    Qty = 0.00M,
                    Rate = 0.00M,
                    Amount = Variables.Qty * Variables.Rate

                };
            }

            #region Temp

            //Variables = new();
            //if (ID == null)
            //{
            //    Variables.BOMProcess = 0;
            //}
            //else
            //{
            //    DataTableClass TableClass = new(UserName, Tables.FinishedGoods, "ID=" + ID.ToString());
            //    if (TableClass.Count == 1)
            //    {
            //        Variables.BOMProcess = (int)TableClass.CurrentRow["Process"];
            //        DataTableClass BOMClass = new(UserName, Tables.BOMProfile2, "TranID=" + Variables.BOMProcess.ToString());
            //        if (BOMClass.CountTable > 0)
            //        {
            //            Profile = BOMClass.MyDataTable;
            //        }
            //    }
            #endregion
        }
        public class MyParameters
        {
            public int BOMProcess { get; set; }
            public int BOMProfile { get; set; }
            public int Inventory { get; set; }
            public string StockTitle { get; set; }
            public decimal Qty { get; set; }
            public decimal Rate { get; set; }
            public decimal Amount { get; set; }

        }
    }
}
