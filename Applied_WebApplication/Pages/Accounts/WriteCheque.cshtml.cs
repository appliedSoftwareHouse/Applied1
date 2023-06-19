using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using static Applied_WebApplication.Data.AppFunctions;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Accounts
{
    public class WriteChequeModel : PageModel
    {
        #region Start
        [BindProperty]
        public Chequeinfo Cheque { get; set; } = new();
        public List<DataRow> ChequeList = new();
        public List<Message> ErrorMessages = new List<Message>();
        public bool IsLoad { get; set; } = false;
        public bool IsSaved { get; private set; } = false;
        public bool IsExist { get; private set; } = false;

        public DateTime FiscalYearDate1 = new DateTime(2022, 7, 1);                     // In future, has to get from user profile table
        public DateTime FiscalYearDate2 = new DateTime(2023, 6, 30);                   // In future, has to get from user profile table

        #endregion

        public void OnGet(string ChqCode)
        {
            string UserName = HttpContext.User.Identity.Name;

            if (UserName == null)
            {
                ErrorMessages.Add(new Message { Success = false, ErrorID = 10, Msg = "User Name is not define...." });
                return;
            }

            if (ChqCode == "New Code" || string.IsNullOrEmpty(ChqCode))
            {
                Cheque = new Chequeinfo();
                Cheque.Code = ChqCode;
            }
            else
            {
                IsSaved = true;
                Cheque = GetChequeInfo(UserName, ChqCode);
                // Fatch a record from DataBase.
            }

            if (Cheque.TaxID1 != 0)
            {
                decimal _TaxRate1 = (decimal.Parse(GetColumnValue(UserName, Tables.Taxes, "Rate", Cheque.TaxID1)));
                Cheque.TaxAmount1 = Cheque.TaxableAmount1 * (_TaxRate1 / 100.00M);
            }
            if (Cheque.TaxID2 != 0)
            {
                decimal _TaxRate2 = (decimal.Parse(GetColumnValue(UserName, Tables.Taxes, "Rate", Cheque.TaxID2)));
                Cheque.TaxAmount2 = Cheque.TaxableAmount2 * (_TaxRate2 / 100.00M);
            }
        }


        public IActionResult OnPostSave()
        {
            string UserName = User.Identity.Name;            // Get User Name from Identity                  

            bool IsValadated = RowValidate(Cheque);
            if (IsValadated)
            {
                DataTableClass Table = new(UserName, Tables.WriteCheques);
                Table.MyDataView.RowFilter = String.Concat("Code='", Cheque.Code, "'");

                if (Table.MyDataView.Count > 0) { IsExist = true; } else { IsExist = false; }

                DataRow Row1, Row2, Row3;

                if (IsExist)
                {
                    DataRow Row;

                    Row = Table.MyDataView[0].Row;
                    if (Row != null) { Row1 = Row; } else { Row1 = Table.NewRecord(); }

                    Row = Table.MyDataView[1].Row;
                    if (Row != null) { Row2 = Row; } else { Row2 = Table.NewRecord(); }

                    Row = Table.MyDataView[2].Row;
                    if (Row != null) { Row3 = Row; } else { Row3 = Table.NewRecord(); }
                }
                else
                {
                    Row1 = Table.NewRecord();
                    Row2 = Table.NewRecord();
                    Row3 = Table.NewRecord();

                    Row1["ID"] = 0;
                    Row2["ID"] = 0;
                    Row3["ID"] = 0;

                    // check either code is alresady exisit or not
                }

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
                Row1["TaxID"] = 0;
                Row1["TaxableAmount"] = 0;
                Row1["TaxRate"] = 0;
                Row1["TaxAmount"] = 0;

                if (Cheque.TaxID1 != 0)
                {

                    Row2["Code"] = Cheque.Code;
                    Row2["TranType"] = int.Parse(GetColumnValue(UserName, Tables.Taxes, "TaxType", Cheque.TaxID1));
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
                    Row2["TaxID"] = Cheque.TaxID1;
                    Row2["TaxableAmount"] = Cheque.TaxableAmount1;
                    Row2["TaxRate"] = (decimal.Parse(GetColumnValue(UserName, Tables.Taxes, "Rate", Cheque.TaxID1)));
                    Row2["TaxAmount"] = (decimal)Row2["TaxableAmount"] * ((decimal)Row2["TaxRate"] / 100);
                }

                if (Cheque.TaxID2 != 0)
                {

                    Row3["Code"] = Cheque.Code;
                    Row3["TranType"] = (int.Parse(GetColumnValue(UserName, Tables.Taxes, "TaxType", Cheque.TaxID2)));
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
                    Row3["TaxID"] = Cheque.TaxID2;
                    Row3["TaxableAmount"] = Cheque.TaxableAmount2;
                    Row3["TaxRate"] = (decimal.Parse(GetColumnValue(UserName, Tables.Taxes, "Rate", Cheque.TaxID2)));
                    Row3["TaxAmount"] = (decimal)Row3["TaxableAmount"] * ((decimal)Row3["TaxRate"] / 100);
                }

                Table.CurrentRow = Row1;
                Table.Save();

                //Table.CurrentRow = Row2; 
                Table = new(UserName, Tables.WriteCheques)
                {
                    CurrentRow = Row2
                };
                Table.Save();

                //Table.CurrentRow = Row3; 
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

        public IActionResult OnPostBack()
        {
            return RedirectToPage("/Index");
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
            public int TaxID1 { get; set; }
            public decimal TaxRate1 { get; set; }
            public decimal TaxAmount1 { get; set; }

            // Sales Tax
            public decimal TaxableAmount2 { get; set; }
            public int TaxID2 { get; set; }
            public decimal TaxRate2 { get; set; }
            public decimal TaxAmount2 { get; set; }

        }
    }
}
