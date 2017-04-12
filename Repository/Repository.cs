///////////////////////////////////////////////////////////////////////
// Repository.cs - Repository                                        //
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
 * This package helps in managing all the resources required for the functioning of the Repository.  It uses packages Repository sender,
 * Repository Receiver , Repository Service and Iservice package for communicating with the client and the Test Harness.
 * Every Repository has its own identity (uri) which the other machines use to contact Repository.
 *
 *Public Interfaces:
 *getIncomingMessage   //This function helps in getting incoming messages for Repository.
 *XmlResultCreationHelper         // Helps in creation of log file.
 *sendCodeListHelper     //Helps in creation of list of dll file at Repository.
 *sendLogFile           //Sends the data in the log file to the user.
 *sendLogListHelper      //Helps in creation of list of xml file at Repository.
 *connectionHelper    // Helps in managing client proxies.
 *sendFile           // Function to send file to client and Test harness

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
using System.Threading;
using System.IO;

using System.Threading.Tasks;
using System.ServiceModel;

namespace Repository
{
    using RepositoryReceiver;
    using RepositoryService;
    using SWTools;
    using Message;
    using RepositorySender;
    using LogRequestHandler;
    using CodeRequestHandler;
 public   class Repository
    {
        
        RepositoryService rServices;
        Dictionary<string, RepositorySender> Connection_List;
        protected LogRequestHandler logHandler;
        protected CodeRequestHandler codeHandler;

        //Repository constructor so start Repository listener 
        public Repository(string url) {
            ServiceHost host = RepositoryReceiver.CreateChannel(url);
            host.Open();
            Console.Write("\n  Press key to terminate service:\n");
            rServices = new RepositoryService();
            Connection_List = new Dictionary<string, RepositorySender>();
            logHandler = new LogRequestHandler();
            codeHandler = new CodeRequestHandler();
            Thread th = new Thread(() => { getIncomingMessage(); });
            th.Start();
        }

        // This functions runs on a seperate thread which continuosly tries to dequeue receive message queue and then act based on type of message
        public void getIncomingMessage() {
            while (true) {
               
                Message msg = rServices.getMessage();
                Console.WriteLine();
                Console.WriteLine("Message dequeued at Repository");
                Console.WriteLine(msg.ToString());
                if (msg.type == "Get_Log_List")
                    sendLogListHelper(msg);
                else if (msg.type == "Log_File_Request")
                    sendLogFile(msg);
                else if (msg.type == "Get_Code_List")
                    sendCodeListHelper(msg);
                else if (msg.type == "Test_Result")
                    XmlResultCreationHelper(msg);
            }
            
        }
        // Function to help in creation of log file
        void XmlResultCreationHelper(Message msg) {

            logHandler.createLogFile(msg);
        }
        // Gives the list of dll files to user
        void sendCodeListHelper(Message msg) {
            Console.WriteLine("Helper function called");
            RepositorySender repoSender = connectionHelper(msg.from);
            Message replyMsg = codeHandler.sendCodeListHelper(msg);
            repoSender.SendMessage(replyMsg);
        }
        // To send data in the log file to user
        void sendLogFile(Message msg) {

            RepositorySender repoSender = connectionHelper(msg.from);
            Message replyMsg = logHandler.sendLogFile(msg);
            repoSender.SendMessage(replyMsg);

        }
        //Gives the list of log files to user
        void sendLogListHelper(Message msg) {
            RepositorySender repoSender = connectionHelper(msg.from);
            Message replyMsg = logHandler.sendLogListHelper(msg);
           repoSender.SendMessage(replyMsg);
        }

        // Function to send messages using to information
        void sendMsg(Message msg)
        {
            RepositorySender repoSender = connectionHelper(msg.to);
            repoSender.SendMessage(msg);
        }

        // This function helps the Repository in managing the proxy objects
        RepositorySender connectionHelper(string url)
        {
            if (!Connection_List.ContainsKey(url))
            {
                RepositorySender repoSender = new RepositorySender(url);
                Connection_List.Add(url, repoSender);
                return repoSender;
            }

            return Connection_List[url];
        }
        // Function to send files (not getting used now)
        void sendFile(string filename)
        {
            string RepositoryUrl = "http://localhost:8085/BasicService";
            RepositorySender clientproxy = connectionHelper(RepositoryUrl);
            clientproxy.sendFileFunction(filename);

        }
        //Main method
        static void Main(string[] args)
        {
            Console.WriteLine("=======================This is Repository" +args[0] +"===========================");
            try
            {
                Repository rp = new Repository("http://localhost:"+ args[0] + "/BasicService");
            Message msg = new Message();
            Console.ReadKey();

            // rp.sendCodeListHelper(msg);
            /* Console.WriteLine("This is Repository");
            // Console.ReadKey();
             Message msg = new Message();
             msg.to = "http://localhost:8085/BasicService";
             msg.body = "\n Message from Repository";
          //  rp.sendMsg(msg);
          //  rp.sendFile("FileNew.txt");

             // th.Start();*/
        }
            catch (Exception ex) {

                Console.WriteLine("Exception catched : " + ex.Message);


            }

}
    }
}
