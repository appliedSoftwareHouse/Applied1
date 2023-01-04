using System.Data;

public enum Tables
{
    COA = 101,
    COA_Nature = 102,
    COA_Class = 103,
    COA_Notes = 104,
    CashBook = 105,
    WriteCheques = 106,
    Taxes =107,

    Customers = 201,
    City = 202,
    Country = 203,
    Project = 204,
    Employees = 205,

    Inventory = 301,
    Inv_Category = 302,
    Inv_SubCategory = 303,
    Inv_Packing = 304,
    Inv_UOM = 305,

    Ledger = 401,
    view_Ledger = 402




}


public enum Messaages
{
    Record_Saved = 100,
    Record_not_Saved = 101

}


public enum CommandAction
{
    Insert,
    Update,
    Delete
}

public enum ChequeStatus
{
    Submitted,
    Clear,
    Bounced,
    Return,
    Lost
}