namespace Applied_WebApplication.Data
{
    public class UnpostClass
    {

        public static bool Unpost_CashBook(string UserName, int ID)
        {
            DataTableClass CashBook = new(UserName, Tables.CashBook);
            DataTableClass Ledger = new(UserName, Tables.Ledger);
            CashBook.MyDataView.RowFilter = "ID=" + ID.ToString();



            return true;
        }


    }
}
