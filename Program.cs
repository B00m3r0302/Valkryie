using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Net.NetworkInformation;
using Valkryie;

namespace Valkryie
{
    public class Program
    {
        static void Main(string[] args)
        {
            /// <summary>
            /// Introduction and description of the program
            /// Also the initialization point where the display will continue until "exit" is sent to the program
            /// </summary>
            bool exitProgram = false;
            string exitProgramString = "exit";
            do
            {
                Console.WriteLine("\nStarting Valkryie\n");
                Console.WriteLine("Description:\n");
                Console.WriteLine("An automated incident detection and response system for use on autonomous vehicles with limited connectivity. This is a compiled executable that will run in the background on a base operating system of Windows to monitor for attacks across the spectrum of the MITRE ATT&CK framework. Specific features that are currently supported can be found in the Implemented.md file in the Planning/ directory. At this time the tool can simply detect these attacks and automated response is slowly being added to the capability suite.");
                Console.WriteLine("\n");
                Console.WriteLine("Type exit to Exit the program...");

                /// <summary>
                /// Initializing the database to store the executable file details 
                /// </summary>
                DataAccess.InitializeDatabase();
                Console.WriteLine("SQLite database initialized with the BaseLineExecutables and CurrentExecutables tables");
                
                /// <summary>
                /// Starting to monitor for internet connections and saving them to the class
                /// </summary>
                InternetConnectionsDetails[] activeConnections = InternetConnectionsDetails.GetActiveConnections();

                foreach (var connection in activeConnections)
                {
                    Console.WriteLine($"Protocol: {connection.Protocol}");
                    Console.WriteLine($"Local Address: {connection.LocalAddress}");
                    Console.WriteLine($"Foreign Address: {connection.ForeignAddress}");
                    Console.WriteLine($"Port: {connection.Port}");
                    Console.WriteLine("\n");
                }

                // Disconnnecting from unknown (right now non-local) connections
                // REENABLE BEFORE RELEASE
                // REENABLE BEFORE RELEASE
                // ConnectionRemediation.DisconnectUnknownConnections();

                /// <summary>
                /// Starting to monitor accounts on the system and saving them to the class
                /// This starts with finding the executables and saving the name, path and hash to the Valyrie database
                /// </summary>
                ExecutableScanner.ScanForExeFilesAndStoreInDatabase();
                Console.WriteLine("Scanning and storing completed. Now printing the results to the console. \n")
                BaseLineAccounts accountScanner = new BaseLineAccounts();

                try
                {
                    accountScanner.ScanForAccounts();
                    Console.WriteLine("Account Names:");
                    foreach (string accountName in accountScanner.AccountNames)
                    {
                        Console.WriteLine(accountName);
                    }
                }

                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Error {ex.Message}");
                }

                /// <summary>
                /// Starting the creation of the baseline executables list and saving it to the class
                /// This will later be stored in a sqlite database 
                /// </summary>
                Dictionary<string, BaseLineExecutables> exeFiles = ExecutableScanner.ScanForExeFiles();

                foreach (var kvp in exeFiles)
                {
                    string fileName = kvp.Value.fileName;
                    string filePath = kvp.Value.filePath;
                    string fileHash = kvp.Value.fileHash;

                    Console.WriteLine($"File Name: {fileName}");
                    Console.WriteLine($"File Path: {filePath}");
                    Console.WriteLine($"File Hash: {fileHash}");
                    Console.WriteLine("\n");
                }


                /// <summary>
                /// Monitoring for user input and the criteria to exit the program
                /// </summary>
                string input = Console.ReadLine();
                if (input.ToLower() == exitProgramString)
                {
                    exitProgram = true;
                }
            } while (exitProgram == false);
        }
    }
}