using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Valkryie
{
    internal class BaseLineExecutables
    {
        /// <summary>   
        /// Attributes for the baseline executables class
        /// </summary>
        public string fileName { get; set; }
        public string filePath { get; set; }
        public string fileHash { get; set; }

        public BaseLineExecutables(string fileName, string filePath, string fileHash)
        {
            fileName = fileName;
            filePath = filePath;
            fileHash = fileHash;
        }
    }
}
