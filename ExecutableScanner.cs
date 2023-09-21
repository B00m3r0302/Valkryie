using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Valkryie;

namespace Valkryie
{
    internal class ExecutableScanner
    {
        // Define a list to store BaseLineExecutables objects
        private static List<BaseLineExecutables> exeFilesList = new List<BaseLineExecutables>();

        // Define a dictionary to store BaseLineExecutables objects
        private static Dictionary<string, BaseLineExecutables> exeFilesDictionary = new Dictionary<string, BaseLineExecutables>();

        public static void ScanForExeFilesAndStoreInDatabase()
        {
            if (!IsAdministrator())
            {
                throw new UnauthorizedAccessException("Administrator privileges required.");
            }

            string rootDirectory = @"C:\";
            
            // Clear the list before scanning for exe files
            exeFilesList.Clear();

            SearchForExeFiles(rootDirectory);

            // Store the data in the SQLite database
            StoreInSQLiteDatabase(exeFilesList);
        }

        private static void StoreInSQLiteDatabase(List<BaseLineExecutables> exeFiles)
        {
            string dbPath = "Valkryie.db";

            using (var dbConnection = new SQLiteConnection($"Data Source={dbPath};Version3;"))
            {
                dbConnection.Open();

                foreach (var executable in exeFiles)
                {
                    string insertCommand = "INSERT INTO BaseLineExecutables (fileName, filePath, fileHash) VALUES (@fileName, @filePath, @fileHash)";

                    using (var cmd = new SQLiteCommand(insertCommand, dbConnection))
                    {
                        cmd.Parameters.AddWithValue("@fileName", executable.fileName);
                        cmd.Parameters.AddWithValue("@filePath", executable.filePath);
                        cmd.Parameters.AddWithValue("@fileHash", executable.fileHash);
                        cmd.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Data stored in the SQLite database");
            }
        }

        public static Dictionary<string, BaseLineExecutables> ScanForExeFiles()
        {
            if (!IsAdministrator())
            {
                throw new UnauthorizedAccessException("Administrator privileges required.");
            }

            var exeFiles = new Dictionary<string, BaseLineExecutables>();

            string rootDirectory = @"C:\";

            SearchForExeFiles(rootDirectory);

            return exeFiles;
        }

        private static void SearchForExeFiles(string directory)
        {
            try
            {
                string[] exeFilePaths = Directory.GetFiles(directory, "*.exe");

                foreach (string filePath in exeFilePaths)
                {
                    string fileName = Path.GetFileName(filePath);
                    string fileHash = CalculateMd5Hash(filePath);

                    // Add to the list 
                    exeFilesList.Add(new BaseLineExecutables(fileName, filePath, fileHash));

                    // Add to the dictionary
                    exeFilesDictionary.Add(fileName, new BaseLineExecutables(fileName, filePath, fileHash));
                }

                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string subdirectory in subdirectories)
                {
                    SearchForExeFiles(subdirectory);
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
