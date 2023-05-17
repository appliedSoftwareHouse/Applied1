using Applied_WebApplication.Pages.Sales;
using NPOI.SS.Formula.Functions;
using System.Data;
using System.Text;
using static Applied_WebApplication.Pages.Stock.InventoryModel;

namespace Applied_WebApplication.Data
{
    public class SQLQuery
    {
        

        public static string SalesInvoice()
        {
            var Text = new StringBuilder();
            Text.Append("SELECT ");
            Text.Append("[B2].[TranID]", );
            Text.Append("[B1].[Vou_No], ");
            Text.Append("[B1].[Vou_Date], ");
            Text.Append("[C].[Title] AS [Company], ");
            Text.Append("[E].[Title] AS [Employee], ");
            Text.Append("[P].[Title] AS [Project], ");
            Text.Append("[B1].[Ref_No], ");
            Text.Append("[B1].[Inv_No], ");
            Text.Append("[B1].[Inv_Date], ");
            Text.Append("[B1].[Pay_Date], ");
            Text.Append("[B1].[Description], ");
            Text.Append("[B2].[Sr_No], ");
            Text.Append("[I].[Title] AS [Inventory], ");
            Text.Append("[B2].[Qty], ");
            Text.Append("[B2].[Rate], ");
            Text.Append("[B2].[Qty] * [B2].[Rate] AS [Amount], ");
            Text.Append("[T].[Rate] AS [Tax_Rate], ");
            Text.Append("([B2].[Qty] * [B2].[Rate]) * [T].[Rate] AS [Tax_Amount], ");
            Text.Append("([B2].[Qty] * [B2].[Rate]) + (([B2].[Qty] * [B2].[Rate]) * [T].[Rate]) AS [Net_Amount], ");
            Text.Append("[B2].[Description] AS [Remarks], ");
            Text.Append("[C].[Address1], ");
            Text.Append("[C].[Address2], ");
            Text.Append("[C].[City] || ' ' || [C].[State] || ' ' || [C].[Country] AS [Address3], ");
            Text.Append("[C].[Phone] ");
            Text.Append("FROM [BillReceivable] [B1] ");
            Text.Append("LEFT JOIN[BillReceivable2] [B2] ON[B2].[TranID] = [B1].[ID] ");
            Text.Append("LEFT JOIN[Customers] [C] ON[C].[ID] = [B1].[Company] ");
            Text.Append("LEFT JOIN[Employees] [E] ON[E].[ID] = [B1].[Employee] ");
            Text.Append("LEFT JOIN[Project] [P] ON[P].[ID] = [B2].[Project] ");
            Text.Append("LEFT JOIN[Inventory] [I] ON[I].[ID] = [B2].[Inventory] ");
            Text.Append("LEFT JOIN[Taxes] [T] ON[T].[ID] = [B2].[Tax] ");
            Text.Append("WHERE ID=@ID");

            return Text.ToString();
       
        }

    }
}
