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
            string BackupFileName = DateTime.Now.ToString("yyyymmddhhmmss_") + uprofile.DataBaseFile;
            if (System.IO.File.Exists(uprofile.DataBaseFile))
            {
                Message = "Process is uder development.";


                //var BackupTargetPath = Path.Combine("./wwwroot/backup/", BackpFileName);
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


        public IActionResult OnPostBackup() 
        {
            UserProfile uprofile = new(User.Identity.Name);
            var _DBFile = uprofile.DataBaseFile;
            var _DBBackup = _DBFile + ".Backup";   // ERROR File name sperate not assign, dot it by code to spereate the file name
            System.IO.File.Copy(_DBFile, _DBFile+".Backup",true);


            byte[] _FileBytes  = ReadFileToBytes(_DBBackup);
            return File(_FileBytes, "aapplication/x-sqlite3",$"{uprofile.Name}.Backup");
        }

        static byte[] ReadFileToBytes(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            using (MemoryStream memoryStream = new MemoryStream())
            {
                reader.BaseStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
