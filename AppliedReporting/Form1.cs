using System.Data.SQLite;

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
                MyConnection = new SQLiteConnection("Data Source="+DBPath);
                MyConnection.Open();
                ConnectionStatus = "DB Connection is Open";
                lblConnection.Text = MyConnection.State.ToString();

            }
            catch (Exception ee)
            {
                ConnectionStatus = "DB Connection has error: " + ee.Message;
                lblConnection.Text = "DB Connection not establised";
            }
            
        }
    }
}