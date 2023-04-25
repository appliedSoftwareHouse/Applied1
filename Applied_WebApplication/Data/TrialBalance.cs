using AppReporting;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace Applied_WebApplication.Data
{
    public class TrialBalance
    {
        #region Setup
        public DataTable MyDataTable { get; set; }
        public DateTime TB_From { get; set; }
        public DateTime TB_To { get; set; }
        public DateTime OB_Date { get; set; }
        public string UserName { get; set; }
        public string ReportPath => AppFunctions.AppGlobals.ReportPath;
        public UserProfile uProfile { get; set; }
        public string CompanyName { get; set; }
        public ClaimsPrincipal UserPrincipal { get; set; }
        public string Heading1 { get; set; }
        public string Heading2 { get; set; }
        public string MyMessage { get; set; }
        public ReportClass MyReportClass { get; set; }

        #endregion

        public TrialBalance(ClaimsPrincipal _UserClaims)
        {
            UserPrincipal = _UserClaims;
            UserName = UserPrincipal.Identity.Name;
            uProfile = new(UserName);
            MyDataTable = new DataTable();
            TB_From = AppRegistry.GetFiscalFrom();
            TB_To = AppRegistry.GetFiscalTo();
            CompanyName = uProfile.Company;
            Heading1 = "Trial Balance";
            Heading2 = "---";
            //SetParameters();
        }


        public void SetParameters()
        {
            MyReportClass = new ReportClass
            {
                AppUser = UserPrincipal,
                ReportFilePath = ReportPath,
                ReportFile = "TB.rdlc",
                ReportDataSet = "dset_TB",
                ReportData = MyDataTable,
                RecordSort = "Code",

                OutputFilePath = AppFunctions.AppGlobals.PrintedReportPath,
                OutputFile = "TB",
                OutputFileLinkPath = AppFunctions.AppGlobals.PrintedReportPathLink

            };

            MyReportClass.ReportParameters.Add("CompanyName", CompanyName);
            MyReportClass.ReportParameters.Add("Heading1", Heading1);
            MyReportClass.ReportParameters.Add("Heading2", Heading2);
            MyReportClass.ReportParameters.Add("Footer", AppFunctions.AppGlobals.ReportFooter);

        }


        public DataTable TBOB_Data()
        {
            DataTable _Table;
            DateTime OBalDate = AppRegistry.GetDate(UserName, "OBDate");
            StringBuilder Text = new StringBuilder();
            Text.Append("SELECT [Ledger].[COA], [COA].[Code], [COA].[Title], ");
            Text.Append("SUM([Ledger].[DR]) AS[DR], ");
            Text.Append("SUM([Ledger].[CR]) AS[CR], ");
            Text.Append("SUM([Ledger].[DR] - [Ledger].[CR]) AS[BAL] ");
            Text.Append("FROM [Ledger] ");
            Text.Append("LEFT JOIN[COA] ON[COA].[ID] = [Ledger].[COA] ");
            Text.Append("WHERE Date([Ledger].[Vou_Date]) = Date('");
            Text.Append(OBalDate.ToString(AppRegistry.DateYMD));
            Text.Append("') GROUP BY[COA] ");

            _Table = DataTableClass.GetTable(UserName, Text.ToString(), "[COA].[Code]");

            SetParameters();
            MyReportClass.ReportData = _Table;
            
            return _Table;
        }

        public DataTable TB_Dates(DateTime Date1, DateTime Date2)
        {
            //DataTable _TableOB = TBOB_Data();
            DataTable _Table;
            
            StringBuilder Text = new StringBuilder();
            Text.Append("SELECT [Ledger].[COA], [COA].[Code], [COA].[Title], ");
            Text.Append("SUM([Ledger].[DR]) AS[DR], ");
            Text.Append("SUM([Ledger].[CR]) AS[CR], ");
            Text.Append("SUM([Ledger].[DR] - [Ledger].[CR]) AS[BAL] ");
            Text.Append("FROM [Ledger] ");
            Text.Append("LEFT JOIN[COA] ON[COA].[ID] = [Ledger].[COA] ");
            Text.Append("WHERE Date([Ledger].[Vou_Date]) >= Date('");
            Text.Append(Date1.ToString(AppRegistry.DateYMD));
            Text.Append("') AND Date([Ledger].[Vou_Date]) <= Date('");
            Text.Append(Date2.ToString(AppRegistry.DateYMD));
            Text.Append("') GROUP BY[COA] ");

            SetParameters();
            _Table = DataTableClass.GetTable(UserName, Text.ToString(), "[COA].[Code]");

          
            return _Table;
        }

        public DataTable TB_All()
        {
            DataTable _Table;

            StringBuilder Text = new StringBuilder();
            Text.Append("SELECT [Ledger].[COA], [COA].[Code], [COA].[Title], ");
            Text.Append("SUM([Ledger].[DR]) AS[DR], ");
            Text.Append("SUM([Ledger].[CR]) AS[CR], ");
            Text.Append("SUM([Ledger].[DR] - [Ledger].[CR]) AS[BAL] ");
            Text.Append("FROM [Ledger] ");
            Text.Append("LEFT JOIN[COA] ON[COA].[ID] = [Ledger].[COA] ");
            Text.Append("GROUP BY[COA] ");

            SetParameters();
            _Table = DataTableClass.GetTable(UserName, Text.ToString(), "[COA].[Code]");

            return _Table;
        }




    }
}
