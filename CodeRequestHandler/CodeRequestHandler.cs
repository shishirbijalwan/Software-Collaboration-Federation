///////////////////////////////////////////////////////////////////////
// CodeRequestHandler.cs - To handle code Request                    //
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
 * This package has been designed to help the Repository to handle to the request related to the code
 * file that are the dll files. In this class we helps the repository in sending the list of dll
 * files present in the repository.
 *
 *Public Interfaces:
 *sendCodeListHelper   //Helper function to create list of files present in Repository.

 *
 * Build Process:
 * --------------
 * Required Files: AppDomainMgr.cs, BlockingQueue.cs, Client.cs,ClientReceiver.cs,ClientSender.cs,ClientService.cs,FileManager.cs,ITest.cs,Loader.cs,
                   HiResTimer.cs,Iservice.cs,LogRequestHandler.cs,Message.cs,Repository.cs,RepositoryService.cs,RepositorySender.cs,RepositoryReceiver.cs,
                   CodeRequestHandler.cs,TestHatnessClietGUI.xaml.cs,TestHarness.cs,TestHarnessReceiver.cs,TestHarnessSender.cs,TestHarnessService.cs,
                   XmlParser.cs,TestResult.cs, XMLDatabase.cs
 * Build Command: devenv TestHarness.sln /rebuild debug
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 20 Nov 16
 * - first release
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace CodeRequestHandler
{
    using Message;

 public   class CodeRequestHandler
    {

        string basepath = "..\\..\\..\\RepoFolder\\Repository";

        // Function to create the list of dll files present in repository and create a message using file names
        public Message sendCodeListHelper(Message msg)
        {
            Console.WriteLine("Code List Helper Called");

            string[] tem = Directory.GetFiles(basepath, "*.dll*", SearchOption.AllDirectories);

            XDocument xmldoc = new XDocument();
            XElement root = new XElement("CodeFileNames");
            xmldoc.Add(root);
            foreach (string str in tem)
            {
                XElement fileadd = new XElement("CodeName", str);
                root.Add(fileadd);

            }
            Message reply = new Message();
            reply.to = msg.from;
            reply.from = msg.to;
            reply.type = "Code_List";
            reply.body = xmldoc.ToString();

            return reply;

        }
        static void Main(string[] args)
        {
            CodeRequestHandler crh = new CodeRequestHandler();
            Message msg1 = new Message();
            msg1.to = "http://localhost:8085/BasicService";
            msg1.from = "http://localhost:8080/BasicService";
            Message msg = crh.sendCodeListHelper(msg1);
            Console.WriteLine(msg.body);
        }
    }
}
