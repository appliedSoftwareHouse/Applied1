using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Applied_WebApplication.Pages.Stock
{
    public class ProductionListModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public string UserName => User.Identity.Name;
        public DataTable tb_Products { get; set; }


        public void OnGet()
        {
            GetVariables();
            tb_Products = DataTableClass.GetTable(UserName, Tables.view_Production);
        }

        #region Set & Get Variables Values
        public void SetVariables()
        {
            AppRegistry.SetKey(UserName, "prdDate1", Variables.Date1, KeyType.Date);
            AppRegistry.SetKey(UserName, "prdDate2", Variables.Date2, KeyType.Date);
            AppRegistry.SetKey(UserName, "prdBatch", Variables.Batch, KeyType.Text);
            AppRegistry.SetKey(UserName, "prdInOut", Variables.InOut, KeyType.Text);
            AppRegistry.SetKey(UserName, "prdSearch", Variables.Search, KeyType.Text);
        }
        public void GetVariables()
        {
            Variables ??= new Parameters();
            Variables.Date1 = AppRegistry.GetDate(UserName, "prdDate1");
            Variables.Date2 = AppRegistry.GetDate(UserName, "prdDate2");
            Variables.Batch = AppRegistry.GetText(UserName, "prdBatch");
            Variables.InOut = AppRegistry.GetText(UserName, "prdInOut");
            Variables.Search = AppRegistry.GetText(UserName, "prdSearch");
        }
        #endregion


        #region POST Method
        public void OnPostFilter()
        {
            SetVariables();
            var Date1 = Variables.Date1.ToString(AppRegistry.DateYMD);
            var Date2 = Variables.Date2.ToString(AppRegistry.DateYMD);
            var _Filter = Variables.Filter;
        }

        public IActionResult OnPostNew()
        {
            return RedirectToPage("Production");
        }

        public IActionResult OnPostEdit()
        {
            return RedirectToPage("Production", "Edit", new { Vou_No = Variables.Vou_No });
        }

        public IActionResult OnPostDelete()
        {
            return Page();
        }

        public void OnPostPrint()
        {

        }

        #endregion

        #region Parameters
        public class Parameters
        {
            public int ID { get; set; }
            public string Vou_No { get; set; }
            public DateTime Date1 { get; set; }
            public DateTime Date2 { get; set; }
            public string Batch { get; set; }
            public int Inventory { get; set; }
            public string InOut { get; set; }
            public string Remarks { get; set; }
            public string Search { get; set; } = string.Empty;
            public string Filter => GetFilter();

            public string GetFilter()
            {
                var _Filter = string.Empty;
                var _Date1 = Date1.ToString(AppRegistry.DateYMD);
                var _Date2 = Date2.ToString(AppRegistry.DateYMD);
                int _Flow() { if (InOut == "In") { return 1; } else { return 0; } }

                _Filter = $"(Date(Vou_No) >= Date('{_Date1}') AND Date(Vou_No) <= Date('{_Date2}')) ";

                if (InOut != "Both") { _Filter += $" AND (Flow = {_Flow()}) "; }
                if (Search != null)
                {
                    _Filter += " ";
                    if (Search.Length > 0)
                    {
                        _Filter += $" AND (Remarks  like '%{Search.Trim()}%' ";
                        _Filter += $" OR Remarks2 like '%{Search.Trim()}%' ";
                        _Filter += $" OR Vou_No   like '%{Search.Trim()}%' ";
                        _Filter += ")";
                    }
                }
                return _Filter;
            }
        }
        #endregion

    }
}
