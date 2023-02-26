using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.AppFunctions;
using static Applied_WebApplication.Data.ReportClass;

namespace Applied_WebApplication.Pages.ReportPrint
{
    public class COAListModel : PageModel
    {
        [BindProperty]
        public string ReportLink { get; set; }
        public bool IsError { get; set; }
        public string MyMessage { get; set; }
        public DataTable Preview { get; set; }
        public string UserName => User.Identity.Name;

        public void OnGet()
        {
        }



        public IActionResult OnGetCOAList()
        {
          
            ReportClass MyReport = new ReportClass
            {
                OutputFileName = "PDFFile",
                RDLCFileName = "COAList.rdlc",
                OutputFileType = FileType.pdf,
                UserName = User.Identity.Name
            };
            //ReportFilter.dt_From = new DateTime(2022, 12, 1);
            //ReportFilter.dt_To = new DateTime(2022, 12, 31);
            MyReport.RDLCDataSet = "DataSet1";
            MyReport.ReportFilter.TableName = Tables.COA;
            MyReport.ReportFilter.Columns = " [ID],[Code],[Title] ";
            MyReport.CommandText = GetQueryText(MyReport.ReportFilter); //  "SELECT ID, CODE, TITLE FROM [COA]";
            //MyReport.Parameters.Add("UserName", UserName);
            MyReport.ReportParameters.Add("CompanyName", UserProfile.GetCompanyName(User));
            MyReport.ReportData = MyReport.GetReportDataTable();
            IsError = MyReport.GetReport();                                                                       // Report is ready to print.
            ReportLink = MyReport.OutputFileLink;

            Preview = GetPreview(MyReport.ReportData);
            return Page();

            //return new FileContentResult(MyReport.MyBytes, "application/pdf");            Do not delete it.

        }

        private DataTable GetPreview(DataTable reportData)
        {
            DataTable PreviewTable = new();
            PreviewTable.Columns.Add("Vou_Date");
            PreviewTable.Columns.Add("Vou_No");
            PreviewTable.Columns.Add("Description");
            PreviewTable.Columns.Add("DR");
            PreviewTable.Columns.Add("CR");
            PreviewTable.Columns.Add("Balance");

            if (reportData.TableName == Tables.Ledger.ToString())
            {

                decimal _Balance = 0;
                foreach (DataRow Row in reportData.Rows)
                {
                    decimal _Amount = (decimal)Row["DR"] - (decimal)Row["CR"];
                    _Balance += _Amount;

                    DataRow NewRow = PreviewTable.NewRow();
                    NewRow["Vou_Date"] = Row["Vou_Date"];
                    NewRow["Vou_No"] = Row["Vou_No"];
                    NewRow["Description"] = Row["Description"];
                    NewRow["DR"] = Row["DR"];
                    NewRow["CR"] = Row["CR"];
                    NewRow["Balance"] = _Balance;
                    PreviewTable.Rows.Add(NewRow);
                }
            }
            return PreviewTable;
        }

        public IActionResult OnGetGL()
        {
            
            //ReportClass MyReport = new();

            ReportFilters Filters = new ReportFilters
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };

            //var _FileType = (FileType)AppRegistry.GetKey(UserName, "ReportType", KeyType.Number);

            DataTable tb_Ledger = Ledger.GetGL(UserName, Filters);
            

            //string Heading1 = "<<--     GENERAL LEDGER     -->>";
            //string Heading2 = GetTitle(UserName, Tables.COA, Filters.N_COA);

            //ReportClass MyReport = new()
            //{
            //    UserName = User.Identity.Name,
            //    RDLCDataSet = "dsname_Ledger",
            //    RDLCFileName = "Ledger.rdlc",
            //    ReportData = tb_Ledger,
            //    OutputFileName = "GeneralLedger",
            //    OutputFileType = _FileType
            //};
            
            //MyReport.ReportParameters.Add("CompanyName", UserProfile.GetCompanyName(User));
            //MyReport.ReportParameters.Add("Heading1", Heading1);
            //MyReport.ReportParameters.Add("Heading2", Heading2);
            //IsError = MyReport.GetReport();
            //MyMessage = MyReport.MyMessage;
            //ReportLink = MyReport.OutputFileLink;

            Preview = GetPreview(tb_Ledger);
            return Page();
        }

        public IActionResult OnGetGLCompany()
        {
           
           // ReportClass MyReport = new();
            ReportFilters Filters = new ReportFilters
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                N_Customer = (int)AppRegistry.GetKey(UserName, "GL_Company", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };

            //var _Date1 = Filters.Dt_From.ToString(AppRegistry.FormatDate);
            //var _Date2 = Filters.Dt_To.ToString(AppRegistry.FormatDate);

            //var _FileType = (FileType)AppRegistry.GetKey(UserName, "ReportType", KeyType.Number);               // Get File Type from AppRegistry.
            var tb_Ledger = Ledger.GetGLCompany(UserName, Filters);
            //var Heading1 = "General Ledger - " + GetTitle(UserName, Tables.Customers, Filters.N_Customer);
            //var Heading2 = string.Concat("From ", _Date1, " To ", _Date2);

            //MyReport = new()
            //{
            //    UserName = User.Identity.Name,
            //    RDLCDataSet = "dsname_CompanyGL",
            //    RDLCFileName = "CompanyGL.rdlc",
            //    ReportData = tb_Ledger,
            //    OutputFileName = "CompanyGL",
            //    OutputFileType = _FileType
            //};
            ////MyReport.Parameters.Add("UserName", UserName);
            //MyReport.ReportParameters.Add("CompanyName", UserProfile.GetCompanyName(User));
            //MyReport.ReportParameters.Add("Heading1", Heading1);
            //MyReport.ReportParameters.Add("Heading2", Heading2);
            //IsError = MyReport.GetReport();
            //MyMessage = MyReport.MyMessage;
            //ReportLink = MyReport.OutputFileLink;

            Preview = GetPreview(tb_Ledger);
            return Page();
        }

        public IActionResult OnGetTB()
        {
            
            //ReportClass MyReport = new();
            ReportFilters Filters = new ReportFilters
            {
                N_COA = (int)AppRegistry.GetKey(UserName, "GL_COA", KeyType.Number),
                N_Customer = (int)AppRegistry.GetKey(UserName, "GL_Company", KeyType.Number),
                Dt_From = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_From", KeyType.Date),
                Dt_To = (DateTime)AppRegistry.GetKey(UserName, "GL_Dt_To", KeyType.Date),
            };

            //var _Date1 = Filters.Dt_From.ToString(AppRegistry.FormatDate);
            //var _Date2 = Filters.Dt_To.ToString(AppRegistry.FormatDate);

            //var _FileType = (FileType)AppRegistry.GetKey(UserName, "ReportType", KeyType.Number);               // Get File Type from AppRegistry.
            var tb_TB = Ledger.GetTB(UserName, Filters);
            //var Heading1 = "TRIAL BALANCE";
            //var Heading2 = string.Concat("Position as on ", _Date2);

            //MyReport = new()
            //{
            //    UserName = User.Identity.Name,
            //    RDLCDataSet = "dset_TB",
            //    RDLCFileName = "TB.rdlc",
            //    ReportData = tb_TB,
            //    OutputFileName = "TB",
            //    OutputFileType = _FileType
            //};
            ////MyReport.Parameters.Add("UserName", UserName);
            //MyReport.ReportParameters.Add("CompanyName", UserProfile.GetCompanyName(User));
            //MyReport.ReportParameters.Add("Heading1", Heading1);
            //MyReport.ReportParameters.Add("Heading2", Heading2);
            //IsError = MyReport.GetReport();
            //MyMessage = MyReport.MyMessage;

            //ReportLink = MyReport.OutputFileLink;

            Preview = GetPreview(tb_TB);
            return Page();
        }


    }
}
