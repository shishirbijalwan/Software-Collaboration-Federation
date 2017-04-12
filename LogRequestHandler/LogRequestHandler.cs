///////////////////////////////////////////////////////////////////////
// LogRequestHandler.cs - Handles log Request                        //
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
 * This package helps helps the Repository in handling the log requests. The log handler has two basic 
 * functionality one to create a log file from the results forwarded by the test harness and second is to 
 * give back the log data to the user if he wants to see any old log. This class can be threaded as a helper
 * class for the Repository to help in log related stuff.
 *
 *Public Interfaces:
 *sendLogListHelper   //This function is to create a message withthe list of log file names.
 *createLogFile         // Function to create log file from the results received from Test Harness.
 *sendLogFile          //Function to send the data in a log file.
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
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LogRequestHandler
{
    using Message;
    using XMLDatabase;
    using TestResult;
 public   class LogRequestHandler
    {
        string Databasepath = "..\\..\\..\\RepoFolder\\Database";

        //This function is to create a message with log file names in its body
     public   Message sendLogListHelper(Message msg)
        {
            Console.WriteLine("Log List Helper Called");

            string[] tem = Directory.GetFiles(Databasepath, "*.xml", SearchOption.AllDirectories);

            XDocument xmldoc = new XDocument();
            XElement root = new XElement("LogFileNames");
            xmldoc.Add(root);
            foreach (string str in tem)
            {
                XElement fileadd = new XElement("FileName", str);
                root.Add(fileadd);

            }
            Message reply = new Message();
            reply.to = msg.from;
            reply.from = msg.to;
            reply.type = "Log_List";
            reply.body = xmldoc.ToString();

            return reply;

        }

// This function helps in creating log filr from the results received from test harness. 
      public  void createLogFile(Message msg) {

            XDocument xdoc = XDocument.Parse(msg.body);

            string authorname = msg.author.Replace(" ", "_");
            string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            XMLDatabase xmld = new XMLDatabase();
            string driverName =xmld.parseStringXml(msg.body);
            var dirName = new DirectoryInfo(driverName).Name;

            string file = Databasepath + "\\"+ dirName + "_" + authorname + "_" + time + ".xml"; // Genrating name of file using driverName, author name and timestamp
            xdoc.Save(file);

        }
        // This function is to send the data inside the log file to user.
       public Message sendLogFile(Message msg) {
            string filename = new DirectoryInfo(msg.body).Name;

            string[] tem = Directory.GetFiles(Databasepath, "*.xml", SearchOption.AllDirectories);
            XDocument xdoc = XDocument.Load(msg.body);

            Message reply = new Message();
            reply.to = msg.from;
            reply.from = msg.to;
            reply.type = "Log_File";
            reply.body = xdoc.ToString();
            return reply;
        }
        //Main Method
        static void Main(string[] args)
        {
            string url = "http://localhost:8080/BasicService";

            LogRequestHandler lr = new LogRequestHandler();
            Message msg = new Message();
            msg.to = url;
            msg.from= "http://localhost:8085/BasicService";
            Message logListMessage = lr.sendLogListHelper(msg);
            Console.WriteLine("Log files in Repository are " + logListMessage.body);
        }
    }
}
