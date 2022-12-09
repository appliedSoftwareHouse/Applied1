namespace Applied_WebApplication.Data
{
    public class CreateTablesClass
    {
       

        //public string tb_Users = "CREATE TABLE [Users](" +
        //                                        "[ID] INTEGER PRIMARY KEY NOT NULL UNIQUE," +
        //                                        "[UserID] NVARCHAR(100) NOT NULL UNIQUE," +
        //                                        "[Password] NVARCHAR(100) NOT NULL," +
        //                                        "[DisplayName] NVARCHAR(100) NOT NULL," +
        //                                        "[UserEmail] NVARCHAR NOT NULL UNIQUE," +
        //                                        "[Role] INTEGER NOT NULL," +
        //                                        "[LastLogin] DATETEXT," +
        //                                        "[Session] NVARCHAR)";

        //public string AddAdmin = "INSERT INTO [Users] ([ID], [UserID], [Password], [DisplayName], [UserEmail],[Role]) VALUES (1,'Admin','Password','Administrator','Admin@jahangir.com',1)";

        //public string tb_Profile = "CREATE TABLE [Profile](" +
        //                                        "[ID] INTEGER PRIMARY KEY NOT NULL UNIQUE," +
        //                                        "[Tag] NVARCHAR(100) NOT NULL UNIQUE," +
        //                                        "[Title] NVARCHAR NOT NULL UNIQUE," +
        //                                        "[Description] NVARCHAR NOT NULL," +
        //                                        "[LastLogin] DATETEXT," +
        //                                        "[Session] NVARCHAR)";

        //public string tb_Roles = "CREATE TABLE [Role](" +
        //                                        "[ID] INTEGER PRIMARY KEY NOT NULL UNIQUE," +
        //                                        "[Tag] NVARCHAR(100) NOT NULL UNIQUE," +
        //                                        "[Title] NVARCHAR NOT NULL UNIQUE," +
        //                                        "[Description] NVARCHAR NOT NULL)";

        //public string AddRole1 = "INSERT INTO [Role] ([ID], [Tag], [Title], [Description]) VALUES (1,'Admin','Admin','Administration')";


        //public string up_Applied = "CREATE TABLE[Applied] (" +
        //                                        "[ID] integer NOT NULL PRIMARY KEY, " +
        //                                        " [Key] [Text(30)]     NOT NULL," +
        //                                        "[nValue] Integer, " +
        //                                        "[sValue] [NVARCHAR(100)], " +
        //                                        "[bValue] BOOLEAN DEFAULT 0);";

        //public string up_COA = "CREATE TABLE[COA] (" +
        //                                        "[ID] Integer PRIMARY KEY NOT NULL, " +
        //                                        "[SCode] Text, " +
        //                                        "[Code] Text NOT NULL, " +
        //                                        "[Title] Text NOT NULL, " +
        //                                        "[IsCashBook] BOOLEAN NOT NULL DEFAULT 0," +
        //                                        "[IsBankBook] BOOLEAN NOT NULL DEFAULT 0," +
        //                                        "[Notes] INT64 NOT NULL DEFAULT 0 CONSTRAINT[COA_Notes] REFERENCES[Notes]([ID]), " +
        //                                        "[OBal] Integer NOT NULL DEFAULT 0," +
        //                                        "[Active] BOOLEAN NOT NULL DEFAULT 1," +
        //                                        "[Nature] INTEGER);";

        //public string up_Suppliers = "CREATE TABLE[Suppliers] (" +
        //                                            "[ID] INTEGER PRIMARY KEY NOT NULL DEFAULT(- 1), " +
        //                                            "[Code] [CHAR(10)] NOT NULL DEFAULT 0, " +
        //                                            "[SCode] [CHAR(10)] DEFAULT 0, " +
        //                                            "[Title] [NVARCHAR(300)] NOT NULL DEFAULT 'Supplier Title', " +
        //                                            "[Person] [NVARCHAR(30)], " +
        //                                            "[Contact] [NVARCHAR(30)], " +
        //                                            "[Address] [NVARCHAR(200)], " +
        //                                            "[City] [NVARCHAR(30)], " +
        //                                            "[Country] [NVARCHAR(30)], " +
        //                                            "[Email] [NVARCHAR(100)]," +
        //                                            "[Fax] [NVARCHAR(30)], " +
        //                                            "[NTN] [NVARCHAR(10)], " +
        //                                            "[STN] [NVARCHAR(15)], " +
        //                                            "[CNIC] [NVARCHAR(13)], " +
        //                                            "[BusinessTitle] [NVARCHAR(300)], " +
        //                                            "[Nature] [NUMBER(1)], " +
        //                                            "[Active] [BOOLEAN(1)] NOT NULL DEFAULT 1);";

        //public string up_Projects = "CREATE TABLE[Projects] (" +
        //                                            "[ID] Integer NOT NULL PRIMARY KEY DEFAULT(-1), " +
        //                                            " [Code] [CHAR(10)] NOT NULL DEFAULT 0, " +
        //                                            " [SCode] [CHAR(10)] DEFAULT 0, " +
        //                                            " [Title] [NVARCHAR(300)] NOT NULL DEFAULT 'Title of project', " +
        //                                            " [Location] NVARCHAR, " +
        //                                            " [City] [NVARCHAR(30)], " +
        //                                            " [Client]  INTEGER DEFAULT 0," +
        //                                            "[Cost] INTEGER DEFAULT 0," +
        //                                            "[Nature] [NVARCHAR(100)], " +
        //                                            "[Remarks] NVARCHAR, " +
        //                                            "[Active] BOOLEAN NOT NULL DEFAULT 1); ";

        //public string up_Employees = "CREATE TABLE[Employees] (" +
        //                                                "[ID] INTEGER NOT NULL PRIMARY KEY, " +
        //                                                "[Code]  [Text(10)]  NOT NULL," +
        //                                                "[SCode] [Text(10)], " +
        //                                                "[Title]  [Text(100)]  NOT NULL,m " +
        //                                                "[Designation] [Text(30)]  NOT NULL," +
        //                                                "[Grade] [Text(30)]    NOT NULL," +
        //                                                "[Department] [Text(30)] NOT NULL," +
        //                                                "[Address] [Text(150)], " +
        //                                                "[City] [Text(30)], " +
        //                                                "[CNIC] [Text(15)], " +
        //                                                "[Remarks] [Text(300)], " +
        //                                                "[Active] BOOLEAN NOT NULL DEFAULT 1);";
        //public string up_Notes = "CREATE TABLE[Notes]( " +
        //                                        "[ID] INT64 PRIMARY KEY NOT NULL, " +
        //                                        "[Code] [TEXT(10)] NOT NULL DEFAULT 0, " +
        //                                        "[Scode] [TEXT(10)] DEFAULT 0, " +
        //                                        "[Title] [Text(50)] NOT NULL," +
        //                                        "[COA_Type] Integer NOT NULL DEFAULT 0 CONSTRAINT[COA_Type] REFERENCES[COA_Type]([ID]), " +
        //                                        "[Active] BOOLEAN NOT NULL DEFAULT 1);";

        //public string upNature = "CREATE TABLE[Nature] (" +
        //                                        "[ID] INT64 PRIMARY KEY, " +
        //                                        "[Code] TEXT NOT NULL, " +
        //                                        "[SCode] TEXT, " +
        //                                        "[Title] NOT NULL UNIQUE);";

        //public string up_COAType = "CREATE TABLE[COA_Type] (" +
        //                                              "[ID] Integer NOT NULL PRIMARY KEY DEFAULT 1," +
        //                                              "[Code] Text NOT NULL DEFAULT 0," +
        //                                              "[Title] Text NOT NULL DEFAULT 'COA_Type'); ";

        //public string up_Ledgers = "CREATE TABLE[Ledger] (" +
        //                                            "[ID] INT64 CONSTRAINT[sqlite_autoindex_Ledger_1] PRIMARY KEY NOT NULL DEFAULT 0, " +
        //                                            "[Vou_No] [Text(12)] NOT NULL, " +
        //                                            "[Vou_Date] DATE NOT NULL, " +
        //                                            "[Vou_Type] [Text(10)] NOT NULL," +
        //                                             "[SRNO] Integer NOT NULL DEFAULT 0," +
        //                                            "[COA] Integer NOT NULL DEFAULT 0 CONSTRAINT[FK_COA] REFERENCES[COA]([ID]), " +
        //                                             "[Supplier] Integer DEFAULT 0 CONSTRAINT[FK_Supplier] REFERENCES[Suppliers]([ID]), " +
        //                                            "[Project] Integer DEFAULT 0 CONSTRAINT[FK_Project] REFERENCES[Projects]([ID]), " +
        //                                            "[Stock] Integer DEFAULT 0 CONSTRAINT[FK_Stock] REFERENCES[Stock]([ID]), " +
        //                                            "[Unit] Integer DEFAULT 0 CONSTRAINT[FK_Unit] REFERENCES[Units]([ID]), " +
        //                                            "[Employee] Integer DEFAULT 0 CONSTRAINT[FK_Employee] REFERENCES[Employees]([ID]), " +
        //                                            "[RefNo] [Text(20)], " +
        //                                            "[Chq_No] [Text(20)], " +
        //                                            "[Chq_Date] DATE, " +
        //                                            "[DR] [DECIMAL(15, 2)] NOT NULL DEFAULT 0, " +
        //                                            "[CR] [DECIMAL(15, 2)] NOT NULL DEFAULT 0, " +
        //                                            "[Description] [Text(200)]  NOT NULL," +
        //                                            "[Remarks] [Text(500)], " +
        //                                            "[POrder] INTEGER);";

        //public string up_Inventory = "CREATE TABLE[Inventory] ( " +
        //"[ID] INT64 PRIMARY KEY, " +
        //"[Vou_No] TEXT(12) NOT NULL," +
        //"[Vou_ID] INTEGER NOT NULL DEFAULT 0," +
        //"[Vou_Amount] DECIMAL(15, 2) DEFAULT 0, " +
        //"[SRNO] INT64, " +
        //"[Stock] INT64 NOT NULL DEFAULT 0 REFERENCES[Stock] ([ID]), " +
        //"[QTY] DECIMAL(15, 2) NOT NULL DEFAULT 0, " +
        //"[UOM] TEXT(10) NOT NULL," +
        //"[Size] TEXT(10), " +
        //"[Rate] DECIMAL(15, 2) DEFAULT 0, " +
        //"[Amount] DECIMAL(15, 2) DEFAULT 0, " +
        //"[Description] VARCHAR(300), " +
        //"[Comments] NVARCHAR(300), " +
        //"[Batch] INT64 REFERENCES[Batches]([ID]), " +
        //"[Status] TEXT(10));";

        //public string up_Stock = "CREATE TABLE[Stock] (" +
        //"[ID] INT64 PRIMARY KEY NOT NULL, " +
        //"[Code] [Text(10)] NOT NULL," +
        //"[Scode] [Text(10)] NOT NULL," +
        //"[Title] [Text(100)] NOT NULL," +
        //"[UOM] [Text(10)]  NOT NULL," +
        //"[Remarks] [Text(100)], " +
        //"[Qty] DECIMAL NOT NULL," +
        //"[Avg_Rate] DECIMAL NOT NULL, " +
        //"[Adv_Cost] DECIMAL NOT NULL, " +
        //"[Active] BOOLEAN NOT NULL DEFAULT 1);";

        //public string up_Unit = "CREATE TABLE[Units] (" +
        //"[ID] INTEGER PRIMARY KEY NOT NULL, " +
        //"[Code] [Text(10)] NOT NULL," +
        //"[SCode] [Text(10)],  " +
        //"[Title] [Text(50)]  NOT NULL," +
        //"[UType] [Text(50)], " +
        //"[ULocation] [Text(50)], " +
        //"[USize] [Text(50)], " +
        //"[Remarks] [Text(100)], " +
        //"[Active] BOOLEAN NOT NULL DEFAULT 1);";

        //==================================================================================
    }
}
