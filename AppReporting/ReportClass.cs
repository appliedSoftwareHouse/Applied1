using Microsoft.Reporting.NETCore;
using Microsoft.ReportingServices.Interfaces;
using System.Data;
using System.Security.Claims;
using static AppReporting.ReportClass;

namespace AppReporting
{
    public class ReportClass
    {
        public ClaimsPrincipal AppUser { get; set; }                                // Current User Profile
        public string OutputFile { get; set; }                                            // Path + file name of printed report PDF/Doc/xls.
        public string OutputFilePath { get; set; }                                    // Path where to printed report store.
        public string OutputFileLinkPath { get; set; }                             // Location to printed report PDF.
        public string OutputFileLink { get; set; }                                    // Location to printed report PDF.
       
        public string ReportFilePath { get; set; }   // RDLC report path
        public string ReportFile { get; set; }   // RDLC report path + FileName
        public string ReportFileName { get; set; }                                       // Output File .pdf, .doc or .xls
        public string RecordSort { get; set; }                                                // Sorting of recport records.
        public DataTable ReportData { get; set; }                                       // DataTable to be perint in RDLC Report.
        public string ReportDataSet { get; set; }                                         // Datasource DataSet name exact in RDLC

        public Dictionary<string, string> ReportParameters { get; set; } = new Dictionary<string, string>();     // Reports Paramates
        public string MyMessage { get; set; }                                          // Store message of the class
        public FileStream MyFileStream { get; set; }                                // File Stream Object
        public byte[] MyBytes { get; set; }                                               // Rendered file bytes for view or print report

        public bool IsError { get; set; }

        public string GetReportLink()
        {
            MyBytes = GetReportbytes();

            if (MyBytes.Length > 1)
            {
                string FileName = string.Concat(OutputFilePath, OutputFile, ".pdf");
                string OutPutFile = string.Concat(OutputFileLinkPath, OutputFile, ".pdf");

                try
                {
                    if(File.Exists(FileName)) { File.Delete(FileName); }
                    
                    using (FileStream fstream = new FileStream(FileName, FileMode.Create))
                    {
                        fstream.Write(MyBytes, 0, MyBytes.Length);
                        MyFileStream = fstream;
                    }

                    if (File.Exists(FileName)) { IsError = false; } else { IsError = true; }
                    OutputFileLink = OutPutFile;                                                                                // Supply File link if file save sucessfully.
                    MyMessage = "File has been created sucessfully.";
                    //IsError = false;
                }
                catch (Exception e) { MyMessage = e.Message; IsError = true; }

            }

            if (IsError) { OutputFileLink = ""; }
            return OutputFileLink;
        }

        public byte[] GetReportbytes()
        {
            try
            {
                var _ReportFile = string.Concat(ReportFilePath, ReportFile);
                StreamReader ReportStream = new StreamReader(_ReportFile);
                LocalReport RDLCreport = new LocalReport();
                RDLCreport.LoadReportDefinition(ReportStream);
                RDLCreport.DataSources.Add(new ReportDataSource(ReportDataSet, ReportData));
                foreach (KeyValuePair<string, string> Item in ReportParameters)
                {
                    RDLCreport.SetParameters(new ReportParameter(Item.Key, Item.Value));
                }
                MyBytes = RDLCreport.Render("PDF");
            }
            
            catch (Exception e)
            {
                MyMessage = e.Message;
                IsError = true;
                MyBytes = new byte[] { 0 };
            }

            return MyBytes;
        }

        

    private static DataTable GetPreview(DataTable reportData)
        {
            // Show the Ledger if pdf fail to display ...............

            DataTable PreviewTable = new();
            PreviewTable.Columns.Add("Vou_Date");
            PreviewTable.Columns.Add("Vou_No");
            PreviewTable.Columns.Add("Description");
            PreviewTable.Columns.Add("DR");
            PreviewTable.Columns.Add("CR");
            PreviewTable.Columns.Add("Balance");

            if (reportData.Rows.Count > 0)
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

        public class ReportFilters
        {
            public string TableName { get; set; }
            public string Columns { get; set; }
            public DateTime Dt_From { get; set; }
            public DateTime Dt_To { get; set; }
            public int N_ID { get; set; }
            public int N_COA { get; set; }
            public int N_Customer { get; set; }
            public int N_Employee { get; set; }
            public int N_Project { get; set; }
            public int N_Inventory { get; set; }
            public int N_InvCategory { get; set; }
            public int N_InvSubCategory { get; set; }

            public bool All_COA { get; set; }
            public bool All_Customer { get; set; }

        }

        //=============================================================== end.
    }


}