namespace AppliedReporting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string DBPath = "D:\\APPLIED\\Applied_WebApplication\\wwwroot\\SQLiteDB\\Applied.db";
        public System.Data.SQLite.SQLiteConnection MyConnection { get; set; }
        public string ConnectionStatus = "";

        private void Form1_Load(object sender, EventArgs e)
        {
            MyConnection = new(DBPath);
            try
            {
                MyConnection.Open();
                ConnectionStatus = "DB Connection is Open";

            }
            catch (Exception e)
            {
                ConnectionStatus = "DB Connection has error: " + e.Message;
                
            }
            
        }
    }
}