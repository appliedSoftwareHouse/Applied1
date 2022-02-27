namespace Applied_WebApplication.Data
{
    public class CreateTablesClass
    {
        public string tb_Users = "CREATE TABLE [Users](" +
                                                "[ID] INTEGER PRIMARY KEY NOT NULL UNIQUE," +
                                                "[UserID] NVARCHAR(100) NOT NULL UNIQUE," +
                                                "[Password] NVARCHAR(100) NOT NULL," +
                                                "[DisplayName] NVARCHAR(100) NOT NULL," +
                                                "[UserEmail] NVARCHAR NOT NULL UNIQUE," +
                                                "[Role] INTEGER NOT NULL," +
                                                "[LastLogin] DATETEXT," +
                                                "[Session] NVARCHAR)";

        public string AddAdmin = "INSERT INTO [Users] ([ID], [UserID], [Password], [DisplayName], [UserEmail],[Role]) VALUES (1,'Admin','Password','Administrator','Admin@jahangir.com',1)";

        public string tb_Profile =  "CREATE TABLE [Profile](" +
                                                "[ID] INTEGER PRIMARY KEY NOT NULL UNIQUE," +
                                                "[Tag] NVARCHAR(100) NOT NULL UNIQUE," +
                                                "[Title] NVARCHAR NOT NULL UNIQUE," +
                                                "[Description] NVARCHAR NOT NULL," +
                                                "[LastLogin] DATETEXT," +
                                                "[Session] NVARCHAR)";

        public string tb_Roles = "CREATE TABLE [Role](" +
                                                "[ID] INTEGER PRIMARY KEY NOT NULL UNIQUE," +
                                                "[Tag] NVARCHAR(100) NOT NULL UNIQUE," +
                                                "[Title] NVARCHAR NOT NULL UNIQUE," +
                                                "[Description] NVARCHAR NOT NULL)";
                                                
        public string AddRole1 = "INSERT INTO [Role] ([ID], [Tag], [Title], [Description]) VALUES (1,'Admin','Admin','Administration')";

    }
}
