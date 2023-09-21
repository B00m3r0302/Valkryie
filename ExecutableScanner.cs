using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Valkryie;

namespace Valkryie
{
    internal class ExecutableScanner
    {
        public static Dictionary<string, BaseLineExecutables> ScanForExeFiles()
        {
            if (!IsAdministrator())
            {
                throw new UnauthorizedAccessException("Administrator privileges required.");
            }

            var exeFiles = new Dictionary<string, BaseLineExecutables>();

            string rootDirectory = @"C:\";

            SearchForExeFiles(rootDirectory, exeFiles);

            return exeFiles;
        }

        private static void SearchForExeFiles(string directory, Dictionary<string, BaseLineExecutables> exeFiles)
        {
            try
            {
                string[] exeFilePaths = Directory.GetFiles(directory, "*.exe");

                foreach (string filePath in exeFilePaths)
                {
                    string fileName = Path.GetFileName(filePath);
                    string fileHash = CalculateMd5Hash(filePath);

                    exeFiles[fileName] = new BaseLineExecutables(fileName, filePath, fileHash)
                    {
                        fileName = fileName,
                        filePath = filePath,
                        fileHash = fileHash
                    };
                }

                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string subdirectory in subdirectories)
                {
                    SearchForExeFiles(subdirectory, exeFiles);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access to directories
            }
            catch (Exception ex)
            {
                // Handle other exceptions if needed
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static string CalculateMd5Hash(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = md5.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
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
