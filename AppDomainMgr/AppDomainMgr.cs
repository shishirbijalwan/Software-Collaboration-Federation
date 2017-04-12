///////////////////////////////////////////////////////////////////////
// AppDomainMgr.cs - Manages child appDomain creation and unload     //
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
 * The purpose of this package in the Test Harness is to dequeue the test request one by one, which has been
 * placed into its blocking queue. The AppDomain Manager takes a request at a time do the task of child 
 * domain creation and loading of the dll files into the child appDomain. It uses the loader class in order to
 * achieve that objective. Once it has called the execute function the result will be saved in the Result blocking
 * queue of the AppDomainManager. Once all test request are over it will dequeue one Result object at a time from the
 * result blocking queue and create a xml file of it as well as save them in a list to return it back to the user.
 *
 *Public Interfaces:
 *copyFiles   //This function is for copying the file from repository to a temp location where harness can use it.
 *RemoveFiles // This function is to delete the file from temp folder once its driver has been executed.
 *runTest     // Its the most vital function of this class which take care of child app domain creation and driver execution
 *EnqueueTestRequest //This function helps in enqueuing test request in the blocking queue.
 *AppDomainMgr      //This the constructor which is used to create object for reference variables.
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
 * ver 1.0 : 05 Oct 16
 * - first release
 *
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;    // defines evidence needed for AppDomain construction
using System.Reflection;         // defines Assembly type
using System.Runtime.Remoting;   // provides remote communication between AppDomains
using System.Threading;
namespace AppDomainManager
{
    using SWTools;
    using TestRequestParser;
    using Loader;
    using TestResult;
    using XMLDatabase;
    using TestHarnessSender;
    using HiResTimer;
    public class AppDomainMgr
    {
        private BlockingQueue<Test> TestRequestQueue;
        private BlockingQueue<TestResult> TestResultQueue;
        private List<TestResult> ResultDatabase;
        private XMLDatabase xmlcreator;
        private string selfAddress;

        private string RepositoryUrl ;


        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //In the constructor we create objects for blocking queue, Result list and XMLDatabase to create xml files
        public AppDomainMgr(string url,string pRepositoryUrl)
        {
            TestRequestQueue = new BlockingQueue<Test>();
            TestResultQueue = new BlockingQueue<TestResult>();
            ResultDatabase = new List<TestResult>();
            xmlcreator = new XMLDatabase();
            selfAddress = url;
            RepositoryUrl = pRepositoryUrl;

        }
     public   TestResult getTestResultAppDomainManager() {

            return TestResultQueue.deQ();
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Function to enqueue test request in the TestRequestQueue blocking queue
        public void EnqueueTestRequest(Test test)
        {
            TestRequestQueue.enQ(test);
            Console.WriteLine("Test Request Object En-queued  in blocking queue is {0}. Blocking queue Length is " + TestRequestQueue.size(), test.testDriver);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Function to copy files to temp location from the Repository
        private bool copyFiles(Test testRequest, ref List<string> filemovedlist, ref BlockingQueue<TestResult> bq)
        {   HiResTimer hTimerChildDomain = new HiResTimer();
            hTimerChildDomain.Start();
        bool filecheck =   getFileHelper(testRequest.testDriver);
            if (filecheck == false)
            {   TestResult tResult = new TestResult( testRequest.testDriver,testRequest.author, testRequest.testDriver,testRequest.testCode);
                tResult.to = testRequest.from;
                tResult.from = testRequest.to;
                tResult.testPass = false;
                tResult.logs = " File not found in Repository " + testRequest.testDriver;
                bq.enQ(tResult);
                return false; }
            hTimerChildDomain.Stop();
            Console.WriteLine();
               Console.WriteLine( "communication latency for file " + testRequest.testDriver + " is " + hTimerChildDomain.ElapsedMicroseconds +"  micro seconds " );
            foreach (string str in testRequest.testCode) {hTimerChildDomain.Start();
                filecheck= getFileHelper(str);
                if (filecheck == false){
                    TestResult tResult = new TestResult(testRequest.testDriver, testRequest.author, testRequest.testDriver, testRequest.testCode);
                    tResult.to = testRequest.from;
                    tResult.from = testRequest.to;
                    tResult.testPass = false;
                    tResult.logs = " File not found in Repository " + testRequest.testDriver;
                    bq.enQ(tResult);
                    return false;}
                hTimerChildDomain.Stop();
                Console.WriteLine();
                Console.WriteLine("## communication latency for file " + str + " is " + hTimerChildDomain.ElapsedMicroseconds + "  micro seconds "); }
            var tempDir = "./DLLFiles/";
            var path = testRequest.testDriver;
            var testDriverTarget = new DirectoryInfo(path).Name;
            if (!File.Exists(tempDir + testDriverTarget)){
                File.Copy(testRequest.testDriver, tempDir + testDriverTarget);
                filemovedlist.Add(tempDir + testDriverTarget); }
            List<string> TestCode = testRequest.testCode;
            foreach (string str in TestCode) {
                path = str;
                var codename = new DirectoryInfo(path).Name;
                if (!File.Exists(tempDir + codename)) {
                    File.Copy(str, tempDir + codename);
                    filemovedlist.Add(tempDir + codename);}
            }
            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Function to remove files from temp location 
        private void RemoveFiles(List<string> files)
        {

            foreach (string str in files)
            {
                File.Delete(str);

            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Function for entertaining each test request enqueued 
        public void runTest()
        {
            AppDomainSetup domaininfo = new AppDomainSetup();
            string mainDirectory = System.Environment.CurrentDirectory.ToString();
            domaininfo.ApplicationBase = "file:///" + System.Environment.CurrentDirectory;
            domaininfo.PrivateBinPath = "DLLFiles";
            Evidence adevidence = AppDomain.CurrentDomain.Evidence;
        
                Test currentTest = TestRequestQueue.deQ();
            Console.WriteLine();
                Console.WriteLine(" **Test Request Object de-queued in blocking queue is {0}. Blocking queue Length is " + TestRequestQueue.size(), currentTest.testDriver);
            Console.WriteLine("Running on Thread id " + Thread.CurrentThread.ManagedThreadId);

            List<string> filesmoved = new List<string>();
            bool filestatus=    copyFiles(currentTest, ref filesmoved, ref TestResultQueue);

            if (filestatus == false) {
                return;
            }
                AppDomain ad = AppDomain.CreateDomain("Child Domain 1" , adevidence, domaininfo); //Creating child app Domain
                Loader load = (Loader)ad.CreateInstanceAndUnwrap(typeof(Loader).Assembly.FullName, typeof(Loader).FullName);
                List<string> TestCode = currentTest.testCode;
                load.loadAssembly(currentTest.testDriver);   //loading driver              
                foreach (string str in TestCode)
                    load.loadAssembly(str);           //loading dependent files for test driver    
                TestResult testResult = load.ExecuteTest(currentTest.testName, currentTest.author, currentTest.testDriver, currentTest.testCode, currentTest.fullLogs,currentTest.from,currentTest.to);
                TestResultQueue.enQ(testResult); //enequeuing results in Result blocking returned by child app domain
                AppDomain.Unload(ad);
                RemoveFiles(filesmoved);
             
          
           // int sizeOfResultQueue = TestResultQueue.size();
          
        }
        bool getFileHelper(string fileName) {
            TestHarnessSender ts = new TestHarnessSender(RepositoryUrl);

           return ts.getCodeFile(fileName,selfAddress);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Main Method
        static void Main(string[] args)
        {
            Test test1 = new Test();
            test1.author = "shishir";
            test1.fullLogs = "true";
            test1.testDriver = "../../../DLLFiles/TestDriverOne.dll";
            List<string> code = new List<string>();
            code.Add("../../../DLLFiles/CodeToBeTested1.dll");
            test1.testCode = code;
            test1.testName = "TestRun";
            test1.QueryRequestOnly = false;
            AppDomainMgr apm = new AppDomainMgr("http://localhost:8090/BasicService", "http://localhost:8080/BasicService");
            apm.EnqueueTestRequest(test1);
            apm.runTest();

        }
    }
}
