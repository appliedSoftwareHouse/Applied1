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
        public DataTableClass ProductionDBClass { get; set; }


        public void OnGet()
        {
        }

        public IActionResult OnPostSave() 
        { 
        
            return Page();
        }

    }

    public class Parameters
    {
        public int ID {  get; set; }
        public string Vou_No { get; set; }
        public DateTime Vou_Date { get; set; }
        public char Tran_ID { get; set; }
        public string Batch { get; set; }
        public string Remarks { get; set; }
        public string InOut { get; set; }
        public int StockID {  get; set; }
        public decimal Qty {  get; set; }
        public decimal Rate {  get; set; }
        public decimal Amount { get; set; }
        public List<DataRow> ListRows { get; set; }
    }

}
