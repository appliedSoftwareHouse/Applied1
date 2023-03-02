//using GemBox.Spreadsheet;
using System.Data;
using System.Linq;
using System.Security.Claims;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelClass
{
    public class AppExcel
    {
        public ClaimsPrincipal AppUser { get; set; }
        public string ExcelFilePath { get; set; }
        public string ExcelFileName { get; set; }
        public DataTable DataFile { get; set; }
        public string CompanyName => GetUserClaim(AppUser, "CompanyName");




        public void COAList()
        {
            // If using Professional version, put your serial key below.


            // Create a new empty workbook.
            var ExcelApp = new Excel.Application();
            var Workbook = ExcelApp.Workbooks.Add();
            var WorkSheet = ExcelApp.Worksheets.Add("Sheet1");

            var ExcelSaveFile = string.Concat(ExcelFilePath, ExcelFileName);

            if (System.IO.File.Exists(ExcelSaveFile)) { System.IO.File.Delete(ExcelSaveFile); }

            object misValue = System.Reflection.Missing.Value;
            // Add a worksheet to it.
            var ws = Workbook;
            var Headings = new string[] { "Code", "Title of Accounts" };

            
            

            // Write to the first cell.
            //ws.Cells[1, "A"].Value = CompanyName;
            //ws.Cells["A2"].Value = "Chart of Accounts";
            //ws.Cells["A3"].Value = Headings[0];
            //ws.Cells["A3"].Value = Headings[1];


            int i = 4;

            if (DataFile != null)
            {
                foreach (DataRow row in DataFile.Rows)
                {

                    //ws.Cells[string.Concat("A", i.ToString())].Value = row["Code"].ToString();
                    //ws.Cells[string.Concat("B", i.ToString())].Value = row["Title"].ToString();

                    i++;
                }
            }


            // Save the workbook as Excel's XLSX file.
            //ws.Save(ExcelSaveFile);

        }

        #region Get Claim

        public static string GetUserClaim(ClaimsPrincipal _ClaimPrincipal, string Key)
        {
            // Get user claim value.
            string Result = string.Empty;
            foreach (Claim _Claim in _ClaimPrincipal.Claims)
            {
                if (_Claim.Type == Key)
                {
                    Result = _Claim.Value;
                    break;
                }
            }
            return Result;
        }

       
       
        #endregion

        //=========================================================== END
    }

}


