using System.Data;
using System.Security.Claims;

namespace Applied_WebApplication.Data
{
    public class DirectoryClass
    {
        private string UserName;
        private DataTableClass Directories;
        public DataTable Directory;

        public DirectoryClass(ClaimsPrincipal _User)
        {
            UserName = _User.Identity.Name;
            Directories = new(UserName, Tables.Directories);
            Directory = GetDirectoryTable();
        }



        private static DataTable GetDirectoryTable()
        {
            DataTable _Table = new DataTable();
            _Table.Columns.Add("ID", typeof(int));
            _Table.Columns.Add("Title", typeof(string));
            return _Table;

        }

        public static string GetDirectoryValue(string UserName, string _Directory, int ID)
        {
            DataTable _Table = GetDirectory(UserName, _Directory);
            foreach (DataRow row in _Table.Rows)
            {
                if ((int)row["ID"] == ID) { return row["Title"].ToString(); }
            }
            return "";
        }


        public static DataTable GetDirectory(string UserName, string _Directory)
        {

            DataTableClass Directories = new(UserName, Tables.Directories);
            DataTable Directory = GetDirectoryTable();

            if (Directories != null)
            {
                if (_Directory != null)
                {
                    Directories.MyDataView.RowFilter = string.Concat("Directory='", _Directory, "'");
                    if (Directories.Count > 0)
                    {
                        foreach (DataRow SourceRow in Directories.MyDataView.ToTable().Rows)
                        {
                            DataRow TargetRow = Directory.NewRow();
                            TargetRow["ID"] = SourceRow["Key"];
                            TargetRow["Title"] = SourceRow["Value"];
                            Directory.Rows.Add(TargetRow);
                        }

                    }
                }
            }
            return Directory;
        }

    }
}
