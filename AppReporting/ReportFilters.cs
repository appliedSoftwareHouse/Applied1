using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppReportClass
{
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
}
