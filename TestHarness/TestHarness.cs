///////////////////////////////////////////////////////////////////////
// TestHarness.cs - Manages TestHarness system                                  //
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
 * This package helps in managing all the resources required for the functioning of the TestHarness.It uses packages TestHarness sender,
 * TestHarness Receiver , TestHarness Service and Iservice package for communicating with the Repository and the client.
 * Every TestHarness has its own identity (uri) which the other machines use to contact TestHarness. It uses the app Domain
 * manager packages to run the test and give the results.
 *
 *Public Interfaces:
 *getIncomingMessage   //This function dequeues the incoming message queue and create a task for each message (child threads).
 *sendResult         // This function returns the test results to client and Repository.

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

namespace TestHarness
{
    using Iservice;
    using Message;
    using TestHarnessSender;
    using TestHarnessService;
    using TestHarnessReceiver;
    using AppDomainManager;
    using TestRequestParser;
    using TestResult;
    using XMLDatabase;
    class TestHarness
    {


        Dictionary<string, TestHarnessSender> Connection_List;
        private TestHarnessService testHarnessService;
        private string selfAddress;
        string RepositoryUrl { set; get; } = "http://localhost:8080/BasicService";
        AppDomainMgr apManager;

        // In the constructor of Test Harness we create a listener to listen to incoming messages
        // We start two threads one to dequeue messages and other to send results
        public TestHarness(string url, string pRepositoryUrl)
        {
            Connection_List = new Dictionary<string, TestHarnessSender>();

            ServiceHost host = TestHarnessReceiver.CreateChannel(url);
            selfAddress = url;
            host.Open();
            testHarnessService = new TestHarnessService();
            apManager = new AppDomainMgr(selfAddress, pRepositoryUrl);

            Thread th = new Thread(() => { getIncomingMessage(); });
            th.Start();
            Thread th2 = new Thread(() => { sendResult(); });
            th2.Start();
            RepositoryUrl = pRepositoryUrl;
        }

        // This function dequeue incoming message and calls a helper function which create task for each message
        public void getIncomingMessage()
        {
            while (true)
            {
                Message incomingMsg = testHarnessService.getMessage();
                if (incomingMsg.type == "Do_Test")
                    startTest(incomingMsg);
              
            }
        }

        //This function creates Test object and calls run test function each message using task( task creates child threads)
        //I have converted messages into Test object to incoperate my Project 2
        void startTest(Message msg) {

            Console.WriteLine("Message Received to start test");
            Console.WriteLine(msg.body);

            Test t = new Test();
            t.to = msg.to;
            t.from = msg.from;
            t.author = msg.author;
            t.timeStamp = msg.time;
          
            XDocument xdoc = XDocument.Parse(msg.body);
            IEnumerable<XElement> FileNames = xdoc.Root.Descendants();

            foreach (XElement x in FileNames) {

                if (x.Name == "TestDriver")
                    t.testDriver = x.Value;
                if (x.Name == "TestCode")
                    t.testCode.Add(x.Value);
            }
            t.testName = t.testDriver;
            apManager.EnqueueTestRequest(t);
            t.show();
            Task.Run(() => { apManager.runTest(); }); // starting child thread
        }

        // Function to send Result to result to client and  Repository
        void sendResult()
        {
            while (true) { 
            TestResult tResult = apManager.getTestResultAppDomainManager();
     tResult.show();
            XMLDatabase xmld = new XMLDatabase();
            Message msg = new Message();
            msg.to = tResult.to;
            msg.from = tResult.from;
            msg.author = tResult.author;
            xmld.createXMLResultToString(tResult);
            msg.body = xmld.createXMLResultToString(tResult);
            msg.time = tResult.timeStamp;
            msg.type = "Test_Result";
            TestHarnessSender clientproxy =new  TestHarnessSender(tResult.to);
            clientproxy.SendMessage(msg);
            msg.to = RepositoryUrl;
          clientproxy = new TestHarnessSender(RepositoryUrl);
            clientproxy.SendMessage(msg);
            }
        }

        //Main Method
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("==================Test Harness at  port number " + args[0] + "==========================");
                Console.WriteLine();
                string repositoryURL = "http://localhost:" + args[1] + "/BasicService";
                TestHarness test = new TestHarness("http://localhost:" + args[0] + "/BasicService", repositoryURL);
                string filePath = Directory.GetCurrentDirectory();
                Console.WriteLine(filePath);
                //    test.RepositoryUrl = "http://localhost:" + args[1] + "/BasicService";
                string url = "http://localhost:8080/BasicService";



                // test.sendMsg(msg);
                // test.sendFile(@"C:\Users\Megha\Desktop\Fall_2016\ads\Practise_website.txt");
                // test.getXMLFileList();
                Console.ReadKey();
            }
            catch (Exception ex) {

                Console.WriteLine("Exception catched : " + ex.Message);


            }
            //cl.getCodeList();

            Console.ReadKey();

        }
    }
}
