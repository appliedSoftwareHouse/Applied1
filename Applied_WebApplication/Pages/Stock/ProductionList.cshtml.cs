using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System.Data;

namespace Applied_WebApplication.Pages.Stock
{
    public class ProductionListModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public string UserName => User.Identity.Name;
        public DataTable tb_Production { get; set; }
        public List<Message> MyMessages { get; set; }


        public void OnGet()
        {
            GetVariables();
            tb_Production = DataTableClass.GetTable(UserName, SQLQuery.List_Production(""));
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
            //var Date1 = Variables.Date1.ToString(AppRegistry.DateYMD);
            //var Date2 = Variables.Date2.ToString(AppRegistry.DateYMD);
            var _Filter = GetFilter();
            tb_Production = DataTableClass.GetTable(UserName, SQLQuery.List_Production(_Filter));
        }

        public IActionResult OnPostNew()
        {
            return RedirectToPage("Production");
        }

        public IActionResult OnPostEdit(string Vou_No)
        {
            return RedirectToPage("Production", "Refresh", new { Vou_No, ID2=1 });
        }

        public IActionResult OnPostDelete(string Vou_No)
        {
            var view_Production = DataTableClass.GetTable(UserName, Tables.view_Production, $"Vou_No = '{Vou_No}'");
            if (view_Production.Rows.Count > 0)
            {
                var class_Production1 = new DataTableClass(UserName, Tables.Production);
                var class_Production2 = new DataTableClass(UserName, Tables.Production2);
                bool _IsDeleted = true;
                int _Records = 0;
                int _ID1 = (int)view_Production.Rows[0]["ID1"];
                MyMessages = new();

                foreach (DataRow Row in view_Production.Rows)
                {
                    int _ID2 = (int)Row["ID2"];
                    class_Production2.SeekRecord(_ID1);
                    class_Production2.Delete();
                    if (!class_Production2.IsError) { _Records++; } else { _IsDeleted = false; break; }
                }

                if (_IsDeleted)
                {
                    class_Production1.SeekRecord(_ID1);
                    class_Production1.Delete();
                    if (!class_Production1.IsError)
                    {
                        MyMessages.Add(new Message() { Success = true, ErrorID = 00, Msg = $"{Vou_No} has been deleted." });
                    }
                    else
                    {
                        { MyMessages.Add(new Message() { Success = true, ErrorID = 00, Msg = $"{Vou_No} did NOT delete completely." }); }
                    }
                }
            }

            return RedirectToPage();
        }

        public void OnPostPrint()
        {

        }

        #endregion


        public string GetFilter()
        {
            var _Filter = string.Empty;
            var _Date1 = Variables.Date1.ToString(AppRegistry.DateYMD);
            var _Date2 = Variables.Date2.ToString(AppRegistry.DateYMD);
            int _Flow() { if (Variables.InOut == "In") { return 1; } else { return 0; } }

            _Filter = $"(Date(Vou_Date) >= Date('{_Date1}') AND Date(Vou_Date) <= Date('{_Date2}')) ";

            if (Variables.InOut != "Both") { _Filter += $" AND (Flow = {_Flow()}) "; }
            Variables.Search ??= "";
            if (Variables.Search != null)
            {
                _Filter += " ";
                if (Variables.Search.Length > 0)
                {
                    _Filter += $" AND (Remarks  like '%{Variables.Search.Trim()}%' ";
                    _Filter += $" OR Remarks2 like '%{Variables.Search.Trim()}%' ";
                    _Filter += $" OR Vou_No   like '%{Variables.Search.Trim()}%' ";
                    _Filter += ")";
                }
            }
            return _Filter;
        }

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
            public string Filter { get; set; } = string.Empty;

            
        }
        #endregion

    }
}
