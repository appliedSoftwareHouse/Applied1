using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Net;

namespace Applied_WebApplication.Pages.Account
{
    public class BackupModel : PageModel
    {
        public string FileName;
        public string Message { set; get; }
        public void OnGet()
        {
            FileName = UserProfile.GetUserClaim(User, "DBFilePath");
            Message = "Message.....";
        }

        public void OnPost()
        {
            UserProfile uprofile = new(User.Identity.Name);
            string BackupFileName = DateTime.Now.ToString("yyyyMMddhhmmss_") + uprofile.DBFileName;
            if (System.IO.File.Exists(uprofile.DBFilePath))
            {
                var BackupFilePath = Path.Combine(Path.GetTempPath(), BackupFileName);

                try
                {
                    System.IO.File.Copy(uprofile.DBFilePath, BackupFilePath);
                    AppRegistry.SetKey(User.Identity.Name, "Backup", BackupFileName, KeyType.Text, "Backup taken for save data");
                    AppRegistry.SetKey(User.Identity.Name, "Backup", DateTime.Now, KeyType.Date, "Backup taken for save data");
                    Message = "Backup has been successfully done at " + BackupFilePath;
                }
                catch (Exception)
                {
                    Message = "Backup process is fail. Contact to Administrator"; 
                    throw;
                }
            }
        }
    }
}
