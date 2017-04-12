///////////////////////////////////////////////////////////////////////
// Client.cs - Manages client system                                  //
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
 * This package helps in managing all the resources required for the functioning of the client. This package can
 *also act as the console interface in case the GUI for client is not present.  It uses packages client sender,
 * client Receiver , client Service and Iservice package for communicating with the Repository and the Test Harness.
 * Every client has its own identity (uri) which the other machines use to contact client.
 *
 *Public Interfaces:
 *sendDoTestRequest   //This function helps in sending Test Request to Test Harness.
 *getCodeList         // To get dll file list from Repository.
 *getXMLFileList     //To get xml file list (logs) from Reposiory.
 *sendFile           //Function to upload a file to Repository.
 *connectionHelper      //Helps in managing the proxy object of services.
 *getIncomingMessage    // Function to get incoming messages from servcies.
 *getCodeFile           // To download a dll from Repository
 *DisplayCodeList       //Display files present in Repository
 *getLogFile            // To get the data in a log file at Repository.
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
using System.ServiceModel;
using System.Threading;
using System.IO;
using System.Xml.Linq;

namespace Client
{
    using Iservice;
    using Message;
    using ClientSender;
    using ClientService;
    using ClientReceiver;

  public  class Client
    {
        Dictionary<string, ClientSender> Connection_List;
        private ClientService clientservice;
        private string selfAddress;
        private string Clientauthor;
       public string RepositoryUrl { set; get; } = "http://localhost:8080/BasicService";
      public  string TestHarnessUrl { set; get; }  = "http://localhost:8090/BasicService";

        // The constructor of client is to create the listener for the client and set url value
        public Client(string url,string author) {
            Connection_List = new Dictionary<string, ClientSender>();

            ServiceHost host = ClientReceiver.CreateChannel(url);
            selfAddress = url;
            host.Open();
            clientservice = new ClientService();
            Clientauthor = author;

        }

        // Function to get the incoming messages from client service class
        public Message getIncomingMessage()
        {
            
             return  clientservice.getMessage();
               
        }
        // Function to display the results received. Not being used now as it is taken care by GUI
        void displayResults(Message msg) {

            XDocument xdoc = XDocument.Parse(msg.body);
            IEnumerable<XElement> elems = xdoc.Root.Elements();
            string testResult = "false";
            string testlogs = null;
            foreach (XElement xelem in elems) {
                if (xelem.Name == "TestPassed")
                    testResult = xelem.Value;
                if (xelem.Name == "TestLogs")
                    testlogs = xelem.Value;
            }
            Console.WriteLine(testResult);
            Console.WriteLine(testlogs);
        }

        //This function is to diplay the list of dll files present in Repository (Not used now)
        public void DisplayCodeList(Message msg)
        {

            XDocument xdoc = XDocument.Parse(msg.body);

            Console.WriteLine();
            Console.WriteLine(" code List Received at Client");

            IEnumerable<XElement> FileNames = xdoc.Root.Descendants();
            string checking_get_Log_file = null;
            foreach (XElement elem in FileNames)
            {
                Console.WriteLine(elem.Value);
                checking_get_Log_file = elem.Value;
            }
        }

        //Function to download a file from repository
        public void getCodeFile(string name) {
            ClientSender currentSender = connectionHelper(RepositoryUrl);
            currentSender.getCodeFile(name, selfAddress);
        }

        //Function to display log file received (Not used now)
        public void displayLogFile(Message msg)
        {
            Console.WriteLine("Log file came back is");
            Console.WriteLine(msg.body);


        }
        //Function to display all log files present in Repository
     public   void DisplayLogList(Message msg) {

            XDocument xdoc = XDocument.Parse(msg.body);

            Console.WriteLine();
            Console.WriteLine("List Received at Client");

            IEnumerable<XElement> FileNames = xdoc.Root.Descendants();
            string checking_get_Log_file = null;
            foreach (XElement elem in FileNames) {
                Console.WriteLine(elem.Value);
                checking_get_Log_file = elem.Value;
            }
        }

//This function creates message and sends to Repository to get a log file data.
      public  void getLogFile(string name) {

            Console.WriteLine("Sending request for log file from client");
            Message msg = new Message();
            msg.to = RepositoryUrl;
            msg.from = selfAddress;
            msg.type = "Log_File_Request";
            msg.author = Clientauthor;
            msg.body = name;
            ClientSender currentSender = connectionHelper(msg.to);
            currentSender.SendMessage(msg);

        }
        // This function is used to send messages to the respective machine
        void sendMsg(Message msg) {
            ClientSender currentSender = connectionHelper(msg.to);
         
            currentSender.SendMessage(msg);
        }

        //This function helps in managing proxy connections and give it to requeting function
        ClientSender connectionHelper(string url) {
            if (!Connection_List.ContainsKey(url)) {
                ClientSender clsender = new ClientSender(url);
                Connection_List.Add(url, clsender);
                return clsender;
            }

            return Connection_List[url];
        }
        // This function is to upload file to Repository
    public    void sendFile(string filename) {
            Console.WriteLine("Sending file to Repository(" + RepositoryUrl + ") names : " + filename);
            ClientSender clientSender = connectionHelper(RepositoryUrl);
            clientSender.sendFileFunction(filename);

        }
        //This function is to get the log list from Repository
      public  void getXMLFileList() {
            ClientSender clientSender = connectionHelper(RepositoryUrl);

            Message xmlListMsg = new Message();
            xmlListMsg.to = RepositoryUrl;
            xmlListMsg.from = selfAddress;
            xmlListMsg.type = "Get_Log_List";
            xmlListMsg.author = Clientauthor;
            xmlListMsg.body = "Request_For_Log_Files_list";
            clientSender.SendMessage(xmlListMsg);

        }
        // This function is to send request for dll files present in Repository
        public void getCodeList()
        {
            ClientSender clientSender = connectionHelper(RepositoryUrl);

            Message xmlListMsg = new Message();
            xmlListMsg.to = RepositoryUrl;
            xmlListMsg.from = selfAddress;
            xmlListMsg.type = "Get_Code_List";
            xmlListMsg.author = Clientauthor;
            Console.WriteLine("Message Sent from Client:");
            Console.WriteLine(xmlListMsg.ToString());
            clientSender.SendMessage(xmlListMsg);

        }
// This function taskes care of message creation and sending to Test harness to run test
     public   void sendDoTestRequest(string Drivename, List<string> codeToBeTested) {

            XDocument xml = new XDocument();
            XElement root = new XElement("TestRequest");
            xml.Add(root);
            XElement driverpart = new XElement("TestDriver", Drivename);
            root.Add(driverpart);
            foreach(string str in codeToBeTested)
            {
                root.Add(new XElement("TestCode", str));
            }
            ClientSender clientSender = connectionHelper(TestHarnessUrl);

            Message msg = new Message();
            msg.to = TestHarnessUrl;
            msg.body = xml.ToString();
            msg.type = "Do_Test";
            msg.author = Clientauthor;
            msg.from = selfAddress;
            msg.time = DateTime.Now;
            Console.WriteLine("Message Sent from Client:");
            Console.WriteLine(msg.ToString());
            clientSender.SendMessage(msg);

        }
//Main method
        static void Main(string[] args)
        {
            Console.WriteLine("This is client");
            Client cl = new Client("http://localhost:8085/BasicService","shishir");
            string filePath = Directory.GetCurrentDirectory();
            Console.WriteLine(filePath);
          //  string url = "http://localhost:8090/BasicService";
            List<string> listOfCode1=new List<string>();
            string codename = @"..\..\..\RepoFolder\Repository\shishir_folder\CodeToBeTested1.dll";
            listOfCode1.Add(codename);
            cl.sendDoTestRequest(@"..\..\..\RepoFolder\Repository\shishir_folder\TestDriverOne.dll", listOfCode1);


            List<string> listOfCode2 = new List<string>();
            string codename2 = @"..\..\..\RepoFolder\Repository\shishir_folder\CodeToBeTested2.dll";
            listOfCode2.Add(codename2);
            cl.sendDoTestRequest(@"..\..\..\RepoFolder\Repository\shishir_folder\TestDriverTwo.dll", listOfCode2);

            //   Message msg = new Message();
            //  msg.to = url;
            //  msg.body = "shishir msg body";
            //   msg.type = "Do_Test";
            //   Console.Read();
            //   cl.sendMsg(msg);
            //  cl.sendFile(@"C:\Users\Megha\Desktop\Fall_2016\ads\Practise_website.txt");
            //  cl.getXMLFileList();
            //  Console.ReadKey();

            cl.getCodeList();

            Console.ReadKey();

        }
    }
}
