using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Principal;
using System.Management;

namespace Valkryie
{
    public class BaseLineAccounts
    {
        public List<string> AccountNames { get; set; }
        
        public BaseLineAccounts()
        {
            AccountNames = new List<string>();
        }

        public void ScanForAccounts()
        {
            if (!IsAdministrator())
            {
                throw new UnauthorizedAccessException("Administrator rights required to run the program");
            }

            AccountNames.Clear(); // Clear the existing list of account names

            // Query for user accounts using WMI

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM WIN32_UserAccount"))
            {
                foreach (ManagementObject user in searcher.Get())
                {
                    string accountName = user["Name"].ToString();
                    AccountNames.Add(accountName);
                }
            }
        }

        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
