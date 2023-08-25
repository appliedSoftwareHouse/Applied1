namespace Applied_WebApplication.Data;

public enum Tables
{
    Registry = 1,

    COA = 101,
    COA_Nature = 102,
    COA_Class = 103,
    COA_Notes = 104,
    CashBook = 105,
    WriteCheques = 106,
    Taxes = 107,
    ChequeTranType = 108,
    ChequeStatus = 109,
    TaxTypeTitle = 110,
    BillPayable = 111,
    BillPayable2 = 112,
    TB = 113,
    BillReceivable = 114,
    BillReceivable2 = 115,
    view_BillReceivable = 116,
    OBALCompany = 117,
    JVList = 118,
    ExpenseSheet = 119,
    SaleReturn = 120,
    BankBook = 121,
   

    Customers = 201,
    City = 202,
    Country = 203,
    Project = 204,
    Employees = 205,
    Directories = 206,

    Inventory = 301,
    Inv_Category = 302,
    Inv_SubCategory = 303,
    Inv_Packing = 304,
    Inv_UOM = 305,
    FinishedGoods = 306,
    SamiFinished = 307,
    OBALStock = 308,
    BOMProfile = 309,
    BOMProfile2 = 310,
    StockPositionData = 311,
    StockPosition = 312,
    StockPositionSUM = 313,
   
    Ledger = 401,
    view_Ledger = 402,
    CashBookTitles = 403,
    VouMax_JV = 404,
    VouMax = 405,

    PostCashBook = 501,
    PostBankBook = 502,
    PostWriteCheque = 503,
    PostBillReceivable = 504,
    PostBillPayable = 505,
    PostPayments = 506,
    PostReceipts = 507,
    UnpostCashBook = 508,
    UnpostBillPayable = 509,

    fun_BillPayableAmounts = 601,                    // Function of Bill Payable Amount and Tax Amount
    fun_BillPayableEntry = 602,

    Chk_BillReceivable1 = 701,
    Chk_BillReceivable2 = 702,

    TempLedger = 9999,

}
public enum CommandAction
{
    Insert,
    Update,
    Delete
}
public enum KeyType
{
    Number,
    Currency,
    Date,
    Boolean,
    Text,
    UserName,
    From,
    To,
    FromTo,
}
public enum PostType
{
    CashBook = 1,
    BankBook = 2,
    WriteCheque = 3,
    BillPayable = 4,
    BillReceivable = 5,
    Payment = 6,
    Receipt = 7,
    JV = 8,
    SaleReturn = 9,
    BOM =10
    
    
}
public enum VoucherStatus
{
    Submitted = 1,
    Approving = 2,
    Approved = 3,
    Posted = 4,
    Add = 5,
}
public enum VoucherType
{
    Cash = 1,
    Bank = 2,
    Cheque = 3,
    Payable = 4,
    Receivable = 5,
    Payment = 6,
    Receipt = 7,
    OBalance = 8,
    OBalCom = 9,
    OBalStock = 10,
    JV = 11,
    SaleReturn = 12
}

public enum PrintOption
{
    Preview = 0,
    PDF = 1,
    Excel=2,
    Word=3,
    HTML=4
}

public enum TBOption
{
    All=0,
    UptoDate=1,
    Monthly=2,
}