using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using static Applied_WebApplication.Data.MessageClass;

namespace Applied_WebApplication.Pages.Accounts
{
    [Authorize]
    public class BillReceivableListModel : PageModel
    {
        [BindProperty]
        public Parameters Variables { get; set; }
        public string UserName => User.Identity.Name;
        public DataTable BillReceivable { get; set; } = new();
        public List<Message> MyMessages = new();


        #region GET
        public void OnGet()
        {
            MyMessages = new();
            Variables = new()
            {
                MinDate = AppRegistry.GetDate(UserName, "Brec_Start"),
                MaxDate = AppRegistry.GetDate(UserName, "Brec_End"),
                Company = AppRegistry.GetNumber(UserName, "Brec_Company")
            };
            var Date1 = Variables.MinDate.AddDays(-1).ToString(AppRegistry.DateYMD);
            var Date2 = Variables.MaxDate.AddDays(1).ToString(AppRegistry.DateYMD);
            var _Filter = $"Date(Vou_Date) > Date('{Date1}') AND Date(Vou_Date) < Date('{Date2}')";
            if (Variables.Company > 0)
            {
                _Filter += $" AND Company={Variables.Company}";
            }
            var _Table = new DataTableClass(UserName, Tables.BillReceivable, _Filter);
            BillReceivable = _Table.MyDataTable;
        }
        #endregion GET

        #region Refresh
        public IActionResult OnPostRefresh()
        {
            AppRegistry.SetKey(UserName, "Brec_Start", Variables.MinDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "Brec_End", Variables.MaxDate, KeyType.Date);
            AppRegistry.SetKey(UserName, "Brec_Company", Variables.Company, KeyType.Number);

            return RedirectToPage();
        }

        #endregion

        #region Add Record
        public IActionResult OnPostAdd()
        {
            return RedirectToPage("../Sales/SaleInvoice", "New");
        }
        #endregion

        #region Show Record
        public IActionResult OnPostShow(int ID)
        {
            DataTableClass _Table = new(UserName, Tables.BillReceivable, $"ID={ID}");
            if (_Table.Count > 0)
            {
                var Vou_No = _Table.CurrentRow["Vou_No"].ToString();
                var Sr_No = 1;

                //if(Vou_No.ToUpper() == "NEW" )
                //{
                //    return RedirectToPage("../Sales/SaleInvoice", "New");
                //}
                //else
                //{
                return RedirectToPage("../Sales/SaleInvoice", routeValues: new { Vou_No, Sr_No, Refresh = true });
                //}


            }
            return Page(); ;
        }
        #endregion

        #region Delete
        public IActionResult OnPostDelete(int ID)
        {
            try
            {
                var _IsError = true;
                var _Filter1 = $"ID={ID}";

                DataTableClass _Receivable1 = new(UserName, Tables.BillReceivable, _Filter1);
                if (_Receivable1.Count > 0)
                {
                    var _Vou_No = _Receivable1.CurrentRow["Vou_No"].ToString();
                    var _Filter2 = $"TranID={_Receivable1.Rows[0]["ID"]}";
                    var _Receivable2 = new DataTableClass(UserName, Tables.BillReceivable2, _Filter2);
                    var _RecCount = _Receivable2.Count;

                    if (_Receivable2.Rows.Count > 0)
                    {
                        var _Record = 0;
                        foreach (DataRow Row in _Receivable2.Rows)
                        {
                            var _ID = (int)Row["ID"];
                            var _TranID = (int)Row["TranID"];

                            if (_TranID == ID)
                            {
                                _Receivable2.SeekRecord(_ID);
                                if (_Receivable2.Delete() == !_IsError) { _Record++; _IsError = true; };              // if Deleted has no error 
                            }
                        }
                        if (_Record == _RecCount)
                        {
                            _Receivable1.SeekRecord(ID);
                            if (_Receivable1.Delete())          // Delete return false means no error found in deletion of the record.
                            {
                                MyMessages.Add(SetMessage($"Sales Invoice Voucher No {_Vou_No} has NOT been deleted. Contact to Administrator."));
                            }
                            else
                            {
                                MyMessages.Add(SetMessage($"Sales Invoice Voucher No {_Vou_No} has been deleted sucessfully.", ConsoleColor.Yellow));
                            }
                        }
                    }
                    else
                    {
                        if (_Receivable1.Count > 0)
                        {
                            _Receivable1.SeekRecord(ID);
                            if (_Receivable1.Delete())          // Delete return false means no error found in deletion of the record.
                            {
                                MyMessages.Add(SetMessage($"Sales Invoice Voucher No {_Vou_No} has NOT been deleted. Contact to Administrator."));
                            }
                            else
                            {
                                MyMessages.Add(SetMessage($"Sales Invoice Voucher No {_Vou_No} has been deleted sucessfully.", ConsoleColor.Yellow));
                            }
                        }
                    }
                }
                


            }
            catch (Exception e)
            {
                MyMessages.Add(SetMessage($"Found System Error: {e.Message}"));
            }


            #region Temp
            //_Receivable1.SeekRecord(ID); 
            //var Vou_No = _Receivable1.CurrentRow["Vou_No"];

            //if (_Receivable1.Delete())
            //{
            //    MyMessages.Add(SetMessage($"Sales Invoice Voucher No {Vou_No} has been deleted sucessfully."));
            //    foreach (DataRow Row in _Receivable2.Rows)
            //    {
            //        var ID2 = (int)Row["ID"];
            //        _Receivable2.SeekRecord(ID2);
            //        if (_Receivable2.Delete())
            //        {
            //            MyMessages.Add(SetMessage($"Record ID {ID2} has been deleted sucessfully"));
            //        }
            //        else
            //        {
            //            MyMessages.Add(SetMessage($"Record ID {ID2} has NOT been deleted sucessfully. Contact to Administrator.", ConsoleColor.Red));
            //        }
            //    }
            //}
            //else
            //{
            //    MyMessages.Add(SetMessage($"Sales Invoice Voucher No {Vou_No} has NOT been deleted. Contact to Administrator."));
            //}

            #endregion

            return Page();
        }
        #endregion

        #region Print

        public IActionResult OnPostPrint(int ID)
        {
            var TranID = ID;
            return RedirectToPage("../ReportPrint/PrintReport", pageHandler: "SaleInvoice", routeValues: new { TranID });
        }
        #endregion

        #region Variables
        public class Parameters
        {
            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }
            public int Company { get; set; }
        }
        #endregion
    }
}
