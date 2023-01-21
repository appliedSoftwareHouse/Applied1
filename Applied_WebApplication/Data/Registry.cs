using AspNetCore.Reporting;
using System.Diagnostics;
using System.Security.Permissions;

namespace Applied_WebApplication.Data
{
    public class Registry
    {
        public static object GetKey(string UserName, string Key, KeyType keytype)
        {
            object ReturnValue;
            DataTableClass tb_Registry = new(UserName, Tables.Registry);
            tb_Registry.MyDataView.RowFilter = string.Concat("Code='", Key, "'");
            if (tb_Registry.MyDataView.Count == 1)
            {
                ReturnValue = keytype switch
                {
                    KeyType.Number => tb_Registry.MyDataView[0]["nValue"],
                    KeyType.Currency => tb_Registry.MyDataView[0]["mValue"],
                    KeyType.Boolean => tb_Registry.MyDataView[0]["bValue"],
                    KeyType.Date => tb_Registry.MyDataView[0]["dValue"],
                    KeyType.Text => tb_Registry.MyDataView[0]["cValue"],
                    KeyType.UserName => tb_Registry.MyDataView[0]["UserName"],
                    _ => string.Empty
                };
            }
            else
            {
                ReturnValue = keytype switch
                {
                    KeyType.Number => 0,
                    KeyType.Currency => 0.00,
                    KeyType.Boolean => false,
                    KeyType.Date => DateTime.Now,
                    KeyType.Text => string.Empty,
                    KeyType.UserName => UserName,
                    _ => string.Empty
                };
            }
            return ReturnValue;
        }

        public static object[] GetDates(string UserName, string Key)
        {
            object[] Dates = new object[2];
            DataTableClass tb_Registry = new(UserName, Tables.Registry);
            tb_Registry.MyDataView.RowFilter = string.Concat("Code='", Key, "'");
            if (tb_Registry.MyDataView.Count == 1)
            {
                Dates[0] = tb_Registry.MyDataView[0]["From"];
                Dates[1] = tb_Registry.MyDataView[0]["To"];
            }
            return Dates;
        }

        public static bool SetKey(string UserName, string Key, object KeyValue, KeyType keytype)
        {
            
            if(KeyValue.GetType() != Type.GetType("System.DateTime")) { KeyValue = DateTime.Now; }          // Assign new date if proper date is not define.
            

            DataTableClass tb_Registry = new(UserName, Tables.Registry);
            tb_Registry.MyDataView.RowFilter = string.Concat("Code='", Key, "'");
            if (tb_Registry.MyDataView.Count == 1)
            {
                tb_Registry.CurrentRow = tb_Registry.SeekRecord((int)tb_Registry.MyDataView[0]["ID"]);
            }
            else
            {
                tb_Registry.CurrentRow = tb_Registry.NewRecord();

                tb_Registry.CurrentRow["ID"] = 0;
                tb_Registry.CurrentRow["Code"] = Key;
                tb_Registry.CurrentRow["Title"] = string.Empty;
                tb_Registry.CurrentRow["UserName"] = UserName;
                switch (keytype)
                {
                    case KeyType.Number:
                        tb_Registry.CurrentRow["nValue"] = KeyValue;
                        break;
                    case KeyType.Currency:
                        tb_Registry.CurrentRow["mValue"] = KeyValue;
                        break;
                    case KeyType.Date:
                        tb_Registry.CurrentRow["dValue"] = KeyValue;
                        break;
                    case KeyType.Boolean:
                        tb_Registry.CurrentRow["bValue"] = KeyValue;
                        break;
                    case KeyType.Text:
                        tb_Registry.CurrentRow["cValue"] = KeyValue;
                        break;
                    case KeyType.From:
                        tb_Registry.CurrentRow["From"] = KeyValue;
                        break;
                    case KeyType.To:
                        tb_Registry.CurrentRow["To"] = KeyValue;
                        break;
                    default:
                        break;
                }



            }

            tb_Registry.Save();

            return false;
        }

    }
}
