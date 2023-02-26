using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Applied_WebApplication.Pages.Account
{
    public class BackupModel : PageModel
    {
        [BindProperty]
        public string FileName { get; set; }
        public string BackFileName { get; set; }
        public string Message { set; get; }
        public void OnGet()
        {
            FileName = UserProfile.GetUserClaim(User, "DBFilePath");
            Message = "Message.....";
        }

        public void OnPost()
        {
            UserProfile uprofile = new(User.Identity.Name);
            string backupfilename = DateTime.Now.ToString("yyyymmddhhmmss_") + uprofile.DBFileName;
            if (System.IO.File.Exists(uprofile.DBFilePath))
            {
                Message = "Process is uder development.";


                var BackupTargetPath = Path.Combine("./wwwroot/backup/", backupfilename);
                try
                {
                    //System.IO.File.  Copy(uprofile.DBFilePath, BackupTargetPath);
                    //AppRegistry.SetKey(User.Identity.Name, "backup", backupfilename, KeyType.Text, "backup taken for save data");
                    //AppRegistry.SetKey(User.Identity.Name, "backup", DateTime.Now, KeyType.Date, "backup taken for save data");
                    //Message = "backup has been successfully done at " + BackupTargetPath;
                }
                catch (Exception)
                {
                    Message = "Process is uder development.";
                }
            }
        }



    }
}
