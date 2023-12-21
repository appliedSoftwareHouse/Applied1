using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Stock
{
    public class ProductionListModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }


        public void OnGet()
        {
        }


        public void OnPostFilter()
        {
            var Date1 = Variables.Date1.ToString(AppRegistry.DateYMD);
            var Date2 = Variables.Date2.ToString(AppRegistry.DateYMD);

            var _Filter = $"Date(Vou_Date) => '{Date1}' AND Date(Vou_no) =< '{Date2}'";


        }

        public class Parameters
        {
            public DateTime Date1 { get; set; }
            public DateTime Date2 { get; set; }
            public string Batch {  get; set; }
            public int Inventory {  get; set; }
            public string InOut { get; set; }
            public string Remarks { get; set; }
            public string Search {  get; set; }
            public string Filter => GetFilter();
            
            
            public string GetFilter()
            {
                var _Filter = string.Empty;
                var _Date1 = Date1.ToString(AppRegistry.DateYMD);
                var _Date2 = Date2.ToString(AppRegistry.DateYMD);
                int _Flow() { if (InOut == "In") { return 1; } else { return 0; } }

                _Filter = $"Date(Vou_No) >= '{_Date1}' AND Date(Vou_No) <= '{_Date2}' ";
                _Filter += "(";
                if(InOut != "Both") { _Filter += $" AND (Flow = {_Flow()}) "; }
                
                if(Search.Length>0) 
                { 
                    _Filter += $" OR (Remarks  like '%{Search.Trim()}% ";
                    _Filter += $" OR (Remarks2 like '%{Search.Trim()}% ";
                    _Filter += $" OR (Vou_No   like '%{Search.Trim()}% ";
                }

                _Filter += ")";

                return _Filter;
            }
        }

    }
}
