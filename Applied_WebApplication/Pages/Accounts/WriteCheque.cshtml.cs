using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using static Applied_WebApplication.Data.AppFunctions;
using System.Data;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Applied_WebApplication.Pages.Accounts
{
    public class WriteChequeModel : PageModel
    {
        [BindProperty]
        public Chequeinfo Cheque { get; set; } = new();
        public List<DataRow> ChequeList = new();
        public List<Message> ErrorMessages = new List<Message>();
        public bool IsLoad { get; set; } = false;
        public bool IsSaved { get; private set; } = false;
        public bool IsExist { get; private set; } = false;

        public DateTime FiscalYearDate1 = new DateTime(2022, 7, 1);                     // In future, has to get from user profile table
        public DateTime FiscalYearDate2 = new DateTime(2023, 6, 30);                   // In future, has to get from user profile table
        
        public void OnGet(string username)
        {
            string UserName = username;

            if (UserName == null)
            {
                ErrorMessages.Add(new Message { Success = false, ErrorID = 10, Msg = "User Name is not define...." });
                return;
            }
            if (Cheque == null) { Cheque = new Chequeinfo(); }

            Cheque.Code = "CHQ-000001";
            Cheque.Customer = 0;
            Cheque.Bank = 15;
            Cheque.ChqNo = "A-789456123";
            Cheque.ChqAmount = 125800;
            Cheque.Customer = 14;
            Cheque.TranDate = new DateTime(2023, 1, 12);
            Cheque.ChqDate = new DateTime(2023, 1, 15);
            Cheque.Description = "Paid to Mr. Ather for payment of stock purchased.";
            Cheque.Status = 1;
            Cheque.TaxableAmount1 = 125800;
            Cheque.TaxableAmount2 = 25600;
            Cheque.Employee = 2;
            Cheque.TaxRate1 = 4;
            Cheque.TaxRate2 = 1;

            if (Cheque.TaxRate1 != 0)
            {
                decimal _TaxRate1 = (int.Parse(GetColumnValue(UserName, Tables.Taxes, "Rate", Cheque.TaxRate1)));
                Cheque.TaxAmount1 = Cheque.TaxableAmount1 * (_TaxRate1 / 100.00M);
            }
            if (Cheque.TaxRate2 != 0)
            {
                decimal _TaxRate2 = (int.Parse(GetColumnValue(UserName, Tables.Taxes, "Rate", Cheque.TaxRate2)));
                Cheque.TaxAmount2 = Cheque.TaxableAmount2 * (_TaxRate2 / 100.00M);
            }
        }

        public IActionResult OnPostSave(string UserName)
        {

            bool IsValadated = RowValidate(Cheque);
            if (IsValadated)
            {
                DataTableClass Table = new(UserName, Tables.WriteCheques);
                List<DataRow> ChequeRows = new();
                DataRow Row1, Row2, Row3;
                Row1 = Table.NewRecord();
                Row2 = Table.NewRecord();
                Row3 = Table.NewRecord();

                Table.MyDataView.RowFilter = string.Concat("Code='", Cheque.Code, "'");

                if (Table.MyDataView.Count > 0)
                {
                    int i = 1;
                    foreach (DataRow Row in Table.MyDataView.ToTable().Rows)
                    {
                        if (i == 1) { Row1 = Row; }
                        if (i == 2) { Row2 = Row; }
                        if (i == 3) { Row3 = Row; }
                        i += 1;
                    }
                }

                Row1["ID"] = Cheque.ID;
                Row1["Code"] = Cheque.Code;
                Row1["TranType"] = 1;
                Row1["TranDate"] = Cheque.TranDate;
                Row1["Bank"] = Cheque.Bank;
                Row1["ChqNo"] = Cheque.ChqNo;
                Row1["ChqDate"] = Cheque.ChqDate;
                Row1["ChqAmount"] = Cheque.ChqAmount;
                Row1["Company"] = Cheque.Customer;
                Row1["Employee"] = Cheque.Employee;
                Row1["Project"] = Cheque.Project;
                Row1["Status"] = Cheque.Status;
                Row1["Description"] = Cheque.Description;
                Row1["TaxType"] = 0;
                Row1["TaxableAmount"] = 0;
                Row1["TaxRate"] = 0;
                Row1["TaxAmount"] = 0;

                if (Cheque.TaxRate1 != 0)
                {

                    Row2["ID"] = Cheque.ID;
                    Row2["Code"] = Cheque.Code;
                    Row2["TranType"] = int.Parse(GetColumnValue(UserName, Tables.Taxes, "TaxType", Cheque.TaxRate1));
                    Row2["TranDate"] = Cheque.TranDate;
                    Row2["Bank"] = Cheque.Bank;
                    Row2["ChqNo"] = Cheque.ChqNo;
                    Row2["ChqDate"] = Cheque.ChqDate;
                    Row2["ChqAmount"] = 0;
                    Row2["Company"] = Cheque.Customer;
                    Row2["Employee"] = Cheque.Employee;
                    Row2["Project"] = Cheque.Project;
                    Row2["Status"] = Cheque.Status;
                    Row2["Description"] = Cheque.Description;
                    Row2["TaxType"] = Cheque.TaxRate1;
                    Row2["TaxableAmount"] = Cheque.TaxableAmount1;
                    Row2["TaxRate"] = (int.Parse(GetColumnValue(UserName, Tables.Taxes, "Rate", Cheque.TaxRate1)));
                    Row2["TaxAmount"] = (decimal)Row2["TaxableAmount"] * ((decimal)Row2["TaxRate"] / 100);
                }

                if (Cheque.TaxRate2 != 0)
                {
                    Row3["ID"] = Cheque.ID;
                    Row3["Code"] = Cheque.Code;
                    Row3["TranType"] = (int.Parse(GetColumnValue(UserName, Tables.Taxes, "TaxType", Cheque.TaxRate2)));
                    Row3["TranDate"] = Cheque.TranDate;
                    Row3["Bank"] = Cheque.Bank;
                    Row3["ChqNo"] = Cheque.ChqNo;
                    Row3["ChqDate"] = Cheque.ChqDate;
                    Row3["ChqAmount"] = 0;
                    Row3["Company"] = Cheque.Customer;
                    Row3["Employee"] = Cheque.Employee;
                    Row3["Project"] = Cheque.Project;
                    Row3["Status"] = Cheque.Status;
                    Row3["Description"] = Cheque.Description;
                    Row3["TaxType"] = Cheque.TaxRate2;
                    Row3["TaxableAmount"] = Cheque.TaxableAmount2;
                    Row3["TaxRate"] = (int.Parse(GetColumnValue(UserName, Tables.Taxes, "Rate", Cheque.TaxRate2)));
                    Row3["TaxAmount"] = (decimal)Row3["TaxableAmount"] * ((decimal)Row3["TaxRate"] / 100);
                }

                Table.CurrentRow = Row1;
                Table.Save();
                Table = new(UserName, Tables.WriteCheques)
                {
                    CurrentRow = Row2
                };

                //Table.CurrentRow = Row2; 
                Table.Save();
                Table = new(UserName, Tables.WriteCheques)
                {
                    CurrentRow = Row3
                };

                Table.Save();

                if (Table.TableValidation.MyMessages.Count > 0)
                {
                    IsSaved = false;
                    ErrorMessages = Table.TableValidation.MyMessages;
                }
                else
                {
                    IsSaved = true;
                }
            }
            return Page();
        }
        private bool RowValidate(Chequeinfo cheque)
        {

            ErrorMessages = new();
            string Date1 = FiscalYearDate1.ToString("dd-MM-yyyy");
            string Date2 = FiscalYearDate2.ToString("dd-MM-yyyy");

            if (cheque.Code == "New Code") { ErrorMessages.Add(new Message() { Success = false, ErrorID = 11, Msg = "Cheque Transaction Code is not assigned." }); }
            if (cheque.TranDate <= FiscalYearDate1 || cheque.TranDate > FiscalYearDate2) { ErrorMessages.Add(new Message() { Success = false, ErrorID = 16, Msg = $"Transaction Date must be for current  fiscal year. From {Date1} to {Date2}" }); }
            if (cheque.ChqDate < cheque.TranDate) { ErrorMessages.Add(new Message() { Success = false, ErrorID = 16, Msg = "Cheque Date must be equal or greater than transaction date." }); }
            if (cheque.Bank == 0) { ErrorMessages.Add(new Message() { Success = false, ErrorID = 12, Msg = "Bank must be entered." }); }
            if (cheque.ChqAmount <= 0) { ErrorMessages.Add(new Message() { Success = false, ErrorID = 12, Msg = "Cheque Amount must be entered" }); }
            if (cheque.Customer == 0) { ErrorMessages.Add(new Message() { Success = false, ErrorID = 13, Msg = "Supplier or vendor must be entered." }); }
            if (string.IsNullOrEmpty(cheque.ChqNo)) { ErrorMessages.Add(new Message() { Success = false, ErrorID = 14, Msg = "Enter Cheque number, null value is not allowed." }); }
            if (string.IsNullOrEmpty(cheque.Description)) { ErrorMessages.Add(new Message() { Success = false, ErrorID = 15, Msg = "Enter some description, null value is not allowed." }); }
            //------------------------------------------------------------------------------- RESULT
            if (ErrorMessages.Count > 0) { return false; }
            return true;
        }

        public class Chequeinfo
        {
            public bool ChqArea { get; set; }
            public bool ListArea { get; set; }
            public bool ErrorArea { get; set; }

            [Required]
            public int ID { get; set; }
            [Required]
            public string Code { get; set; }
            public int TranType { get; set; }
            public DateTime TranDate { get; set; }
            [Required]
            public int Bank { get; set; }
            [Required]
            public int Customer { get; set; }
            public int Project { get; set; }
            public int Employee { get; set; }
            public DateTime ChqDate { get; set; }
            public string ChqNo { get; set; }
            [Required]
            public decimal ChqAmount { get; set; }
            public int TaxType { get; set; }
            public int Status { get; set; }


            [StringLength(200)]
            public string Description { get; set; }

            // WHT Income Tax

            public decimal TaxableAmount1 { get; set; }
            public int TaxRate1 { get; set; }
            public decimal TaxAmount1 { get; set; }

            // Sales Tax
            public decimal TaxableAmount2 { get; set; }
            public int TaxRate2 { get; set; }
            public decimal TaxAmount2 { get; set; }

        }
    }
}
