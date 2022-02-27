using System.Data;

namespace Applied_WebApplication.Data
{
    public class UserProfile
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }

        public UserProfile(DataRow _Row)
        {
            if(_Row != null)
            {
                UserID = _Row["UserID"].ToString();
                UserName = _Row["DisplayName"].ToString();
                Password = _Row["Password"].ToString();
                Email = _Row["UserEmail"].ToString();
                Role = int.Parse(_Row["Role"].ToString());
            }



        }

    }
}
