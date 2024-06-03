using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedTemp
{
    public class UserProfileModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }            // Not a use now.
        public string TempFolder { get; set; }          // Assign a Temp Folder for Create database
        public string TempFile { get; set; }              // Assign a Temp File store inb Temp Directory
        public string UserRole { get; set; }
    }
}
