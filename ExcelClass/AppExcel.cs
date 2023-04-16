//using GemBox.Spreadsheet;

//using Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.Security.Claims;
using DataTable = System.Data.DataTable;

namespace ExcelClass
{
    public class AppExcel
    {
        public ClaimsPrincipal AppUser { get; set; }
        public string ExcelFilePath { get; set; }
        public string ExcelFileName { get; set; }
        public DataTable DataFile { get; set; }
        //public string CompanyName => GetUserClaim(AppUser, "CompanyName");
        public string MyMessage = string.Empty;




        public string CreateExcel()
        {
            // If using Professional version, put your serial key below.
            // Create a new empty workbook.

            //MyMessage = "Start Excel sheet.";

            //var AppExcel = new Excel.Application();
            //Excel.Workbook AppBook = AppExcel.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet); ;
            //Excel.Worksheet AppSheet = (Excel.Worksheet)AppBook.Worksheets[1];

            //AppSheet.Cells[1, "A"] = 123;


            //AppExcel.Save("C:\\Test.xlxs");

            return "C:\\Test.xlxs";

            //=========================================================== END
        }
    }
}


