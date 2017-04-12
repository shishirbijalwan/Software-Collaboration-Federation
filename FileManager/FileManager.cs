///////////////////////////////////////////////////////////////////////
// Program.cs - Provides dll files inside a root directory           //
// ver 1.0                                                           //
// Language:    C#, Visual Studio 2015                              //
// Application: Test Harness, CSE681 - SMA                           //
//  Platform:      HP Pavilion dv6/ Window 7 Service Pack 1          //
//Author:       Shishir Bijalwan, Syracuse University                //
//              sbijalwa@syr.edu, 9795876340                         //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This Package has been made to provide the DLL files inside any root directory. It
 * takes the path of the root folder as the command line argument.
 *
 *Methods:
 *getDLLFileList   //This function takes in path as the input argument and return all DLL in that root path.
 *
 * Build Process:
 * --------------
 * Required Files: FileManager.cs
 * Build Command: devenv TestHarness.sln /rebuild debug
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 09 Sep 16
 * - first release
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileManager
{
    public class FileMgr
    {

        //Function to get the all files with dll extension inside a root directory
        public List<string> getDLLFileList(string path, string extension)
        {
            List<String> filesList1 = new List<String>();
            try
            {

                string[] tem = Directory.GetFiles(path, extension, SearchOption.AllDirectories);

                foreach (string str in tem)
                {
                    filesList1.Add(str);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return filesList1;
        }



        //Test executive for the file Manager package
        static void Main(string[] args)
        {
            if (args.Count() == 0)
            {
                Console.Write("\n  Please enter path on command line\n\n");
                return;
            }
            FileMgr p = new FileMgr();
            List<string> FileList = p.getDLLFileList(args[0], "*.xml");
            foreach (string file in FileList)
            {
                Console.WriteLine(file);
            }

        }
    }
}
