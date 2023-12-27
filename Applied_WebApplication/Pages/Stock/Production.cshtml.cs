using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Stock
{
    [Authorize]
    public class ProductionModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public string UserName => User.Identity.Name;
        public DataTableClass Class_Products { get; set; }
        public DataTableClass Class_Products1 { get; set; }
        public DataTable tb_Products { get; set; }



        public void OnGet()
        {
            Class_Products = new DataTableClass(UserName, Tables.view_Production);
            tb_Products = Class_Products.MyDataTable;

            Variables = new Parameters();
            {
                Variables.Vou_No = "NEW";
                Variables.Vou_Date = DateTime.Now;
                Variables.Batch = string.Empty;
                Variables.Flow = string.Empty;
                Variables.StockID = 0;
                Variables.Qty = 0.00M;
                Variables.Rate = 0.00M;
                Variables.Amount = 0.00M;
            }
        }


        public void OnGetEdit(string Vou_No)
        {

            var _SQLQuery = $"SELECT * FROM [view_Production] WHERE [Vou_No] = '{Vou_No}'";
            Class_Products = new DataTableClass(UserName, _SQLQuery);
            
            if(Class_Products.CountTable ==0)
            {
                OnGet();                // show a New voucher if not found the specific voucher number in table.
                return;
            }

            tb_Products = Class_Products.MyDataTable;
        }

        public IActionResult OnPostSave()
        {
            Class_Products = new DataTableClass(UserName, Tables.Production);
            Class_Products1 = new DataTableClass(UserName, Tables.Production2);
            GetRow();

            Class_Products.Save();
            if(Class_Products.IsError) { }

            return RedirectToPage();
        }


        public IActionResult OnPostSaveAll() 
        {
            Class_Products = new DataTableClass(UserName, Tables.Production);
            Class_Products1 = new DataTableClass(UserName, Tables.Production2);
            GetRow();

            if (Class_Products.CurrentRow["Vou_no"].ToString().ToUpper() == "NEW") 
            {
                Class_Products.CurrentRow["Vou_No"] = NewVoucher.GetNewVoucher(Class_Products.MyDataTable, "PD");
            }
            return RedirectToPage();
        }

        private void GetRow()
        {
            Class_Products.NewRecord();
            Class_Products.CurrentRow["ID"] = Variables.ID1;
            Class_Products.CurrentRow["Vou_No"] = Variables.Vou_No;
            Class_Products.CurrentRow["Vou_Date"] = Variables.Vou_Date;
            Class_Products.CurrentRow["Batch"] = Variables.Batch;
            Class_Products.CurrentRow["Remarks"] = Variables.Remarks1;
            Class_Products.CurrentRow["Comments"] = Variables.Comments;

            Class_Products1.CurrentRow["ID"] = Variables.ID2;
            Class_Products1.CurrentRow["TranID"] = Variables.ID1;
            Class_Products1.CurrentRow["Stock"] = Variables.StockID;
            Class_Products1.CurrentRow["Flow"] = FlowtoBool(Variables.Flow);
            Class_Products1.CurrentRow["UOM"] = Variables.UOM;
            Class_Products1.CurrentRow["Rates"] = Variables.Rate;
            Class_Products1.CurrentRow["Remarks"] = Variables.Remarks2;
        }

        private bool FlowtoBool(string flow)
        {
            if(flow=="In") { return true; } return false;
        }

        
    }

    public class Parameters
    {
        public int ID1 {  get; set; }
        public int ID2 {  get; set; }
        public string Vou_No { get; set; }
        public DateTime Vou_Date { get; set; }
        public char Tran_ID { get; set; }
        public string Batch { get; set; }
        public string Remarks1 { get; set; }
        public string Comments { get; set; }
        public string Remarks2 { get; set; }
        public string Flow { get; set; }
        public int StockID {  get; set; }
        public decimal Qty {  get; set; }
        public int UOM { get; set; }
        public decimal Rate {  get; set; }
        public decimal Amount { get; set; }
        public List<DataRow> ListRows { get; set; }
        
    }

}
