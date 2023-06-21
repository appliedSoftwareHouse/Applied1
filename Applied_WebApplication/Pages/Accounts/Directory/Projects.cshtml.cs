using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Accounts.Directory
{
    public class ProjectsModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public DataTable Project { get; set; }
        public List<Message> MyMessages { get; set; }
        public string UserName => User.Identity.Name;

        public void OnGet(int? ID)
        {
            MyMessages = new();
            if(ID == null)
            {
                Variables = new();
                var _TableClass = new DataTableClass(UserName, Tables.Project);
                Project = _TableClass.MyDataTable;
            }
            else
            {
                var _TableClass = new DataTableClass(UserName, Tables.Project);
                Project = _TableClass.MyDataTable;
                _TableClass.SeekRecord((int)ID);
                Variables = new()
                {
                    ID = (int)_TableClass.CurrentRow["ID"],
                    Code = (string)_TableClass.CurrentRow["Code"],
                    Title = (string)_TableClass.CurrentRow["Title"],
                    Comments = (string)_TableClass.CurrentRow["Comments"],
                };
            }
            
        }

        public IActionResult OnPostEdit(int ID)
        {
            return RedirectToPage("Projects", routeValues: new {ID});
        }

        public IActionResult OnPostSave()
        {
            MyMessages = new();
            DataTableClass _Table = new(UserName, Tables.Project);
            _Table.CurrentRow = _Table.NewRecord();
            _Table.CurrentRow["ID"] = Variables.ID;
            _Table.CurrentRow["Code"] = Variables.Code;
            _Table.CurrentRow["Title"] = Variables.Title;
            _Table.CurrentRow["Comments"] = Variables.Comments;
            _Table.Save();
            if(_Table.IsError)
            {
                MyMessages = _Table.ErrorMessages;
            }
            else
            {
                MyMessages.Add(SetMessage("Record has been saved.", ConsoleColor.Green));
            }

            Project = _Table.MyDataTable;
            return Page();

        }

        public IActionResult OnPostNew()
        {
            MyMessages = new();
            Variables = new();
            var _Table = new DataTableClass(UserName, Tables.Project);
            Project = _Table.MyDataTable;
            return Page();
        }

        public IActionResult OnPostDelete(int ID)
        {
            MyMessages = new();
            var _Table = new DataTableClass(UserName, Tables.Project);
            _Table.MyDataView.RowFilter = $"ID={ID}";
            if(_Table.Count==1)
            {
                _Table.SeekRecord(ID);
                _Table.Delete();
                MyMessages.Add(SetMessage("Record has been deleted...."));
            }
            _Table = new DataTableClass(UserName, Tables.Project);
            Project = _Table.MyDataTable;
            return Page();
        }

        public IActionResult OnPostBack()
        {

            return RedirectToPage("/Accounts");
        }


        public class Parameters
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string Title { get; set; }
            public string Comments { get; set; }

        }
    }
}
