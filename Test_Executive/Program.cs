///////////////////////////////////////////////////////////////////////
// TestExecutive.cs - Demonstrating Requirements                     //
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
 * This package has been designed to demostrate that the requirements for the project 4
 * have been taken care. We have taken a requirement at a time and tried to demonstarte 
 * how we are taking care of it. We have created 4 function to help us in this task. This
 * package acts like a second client.
 *
 *Public Interfaces:
 *Requirement12   //Helps in demostrating requirement 1 ,2.
 *Requirement3         // Helps in demostrating requirement 3 ,4,5.
 *Requirement6     //Helps in demostrating requirement 6 ,7,8.
 *Requirement9           //Helps in demostrating requirement 9 ,10,11 and 12.
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

namespace Test_Executive
{
    using Client;
    using Message;
    class Program
    {
        private Client cl2;
        void printLine() {

            Console.WriteLine("=========================================================================================================");

        }

    public    Program(string url1, string url2, string url3) {
         cl2 = new Client("http://localhost:" + url1 + "/BasicService", "TestExecutiveClient");
            cl2.RepositoryUrl = "http://localhost:" + url2 + "/BasicService";
            cl2.TestHarnessUrl = "http://localhost:" + url3 + "/BasicService";
        }
        //Helps in demostrating requirement 1 ,2.
        void Requirement12() {
            Console.WriteLine("I have used the Test Executive as the second client to demonstarte the requirements for project 4 have been taken care. This also proves by Test Harness can handle two clients.");
           printLine();
            Console.WriteLine("Requirement 1. Implemented in C# using the facilities of the .Net Framework Class Library and Visual Studio 2015, as provided in the ECS clusters.");
            Console.WriteLine("Requirement 2. Implement a Test Harness Server that accepts one or more Test Requests, each in the form of an a message with XML body that specifies the test developer's identity and the names of a set of one or more test libraries to be tested. Each test driver and the code it will be testing is implemented as a dynamic link library (DLL) and sent by the client to the Repository server2 before sending the Test Request to the Test Harness. The Test Request XML body names one or more of these test DLLs to execute.");
            Console.WriteLine();
            Console.WriteLine("2.1 Sending files CodeToBeTested1.dll and TestDriverOne.dll. Please check RepoFolder/Repository for the folder with file name and timestamp");
           
            cl2.sendFile(@"..\..\..\SampleDrivers\CodeToBeTested1.dll");
            cl2.sendFile(@"..\..\..\SampleDrivers\TestDriverOne.dll");
            Console.WriteLine();

            Console.WriteLine("2.2 As the files in Repository have timestamp attached to it. We have used another folder in Repository to run Test and get result");
            List<string> listOfCode1 = new List<string>();
            string codename = @"..\..\..\RepoFolder\Repository\shishir_folder\CodeToBeTested1.dll";
            listOfCode1.Add(codename);
            cl2.sendDoTestRequest(@"..\..\..\RepoFolder\Repository\shishir_folder\TestDriverOne.dll", listOfCode1);
            Message Requirement2Msg = cl2.getIncomingMessage();
            Console.WriteLine();

            Console.WriteLine("Result Got Back is in below XML format : Please node we are parsing this it to display in GUI");
            Console.WriteLine(Requirement2Msg.body);
        }

        // Helps in demostrating requirement 3 ,4,5.
        void Requirement3()
        {
            printLine();
            Console.WriteLine("Requirement 3. If a Test Request specifies test DLLs not available from the Repository, the Test Harness server is sending back an error message to the client");
           

            List<string> listOfCode1 = new List<string>();
            Console.WriteLine(@"Driver Requested not prsent in Repository : ..\..\..\RepoFolder\Repository\shishir_folder\CodeToBeTested7.dll");
            string codename = @"..\..\..\RepoFolder\Repository\shishir_folder\CodeToBeTested7.dll";
            listOfCode1.Add(codename);
            cl2.sendDoTestRequest(@"..\..\..\RepoFolder\Repository\shishir_folder\TestDriver11.dll", listOfCode1);
            Message Requirement2Msg = cl2.getIncomingMessage();
            Console.WriteLine();

            Console.WriteLine("The message below tells the reason for the failure");
            Console.WriteLine(Requirement2Msg.body);
            printLine();
            Console.WriteLine("Requirement 4.  Test Requests from multiple concurrent clients and execute them by creating, for each Test Request, an AppDomain, running on its own thread. Once a child AppDomain is constructed, the Test Harness shall start the child processing the dequeued Test Request. The result is that Test Requests can be processed concurrently");
            Console.WriteLine(" ::Please check Test Harness console. We have printed the thread id for 2 test ran above. Please look for \"**Test Request Object de-queued in \" to get different thread id for both test ");
            Console.WriteLine(" ::The child app domain name also can be checked in Test Harness console. Please look for \"== App Domain Name\"");
            Console.WriteLine(" :: Test Harness can handle mutiple client as currently it has two client test Executive and GUI. Please use GUI to verify this.");
            Console.WriteLine("Requirement 5. Each test driver derives from an ITest interface that declares a method test() that takes no arguments and returns the test pass status, e.g., a boolean true or false value");
            Console.WriteLine(" :: Please Check ITest package line number 44 to verify it.");

        }
        //Helps in demostrating requirement 6 ,7,8.
        public void Requirement6()
        {
            printLine();
            Console.WriteLine("Requirement 6. Test libraries and Test Requests are sent to the Repository and Test Harness server, respectively, and results sent back to a requesting client, using an asynchronous message-passing communication channel3. The Test Harness receives test libraries from the Repository using the same communication processing. File transfer shall use streams or a chunking file transfer that does not depend on enqueuing messages4");
            Console.WriteLine(" :: Test Libraries are sent to Repository. Please Console for requirement 2.1 display. It is display file name and to whom it is sent");
            Console.WriteLine(" :: Test Request are sent to Test Harness. Please check console for requirement 2.2 display");
            Console.WriteLine(" :: To verify it has async communication. Please see that text(\" Result got back..\") is being printed message sent and message received from client.In case of Sync comm. it was not possible to print text(would have wait) ");
            Console.WriteLine(" :: To verify that chunks are being transferred please see file sending code sendFileFunction in clientSender package line 45.");
            printLine();
            Console.WriteLine("Requirement 7. At the end of test execution the Test Harness shall store the test results and logs in the Repository and send test results to the requesting client.");
            Console.WriteLine(" :: Results to Repository: Please find the messages received by Repository for both test drivers ran displayed in Repository console.");
            Console.WriteLine(" :: Results to Client: We have displayed above the messages with results received by client in requiredment 2 and 3");
            printLine();
            Console.WriteLine("Requirement 8. The Test Harness shall, in cooperation with the Repository, store test results and logs for all of the test executions using a key that combines the test developer identity and the current date-time ");
            Console.WriteLine(" :: Please find below the logs files created for two test we ran -");

            string[] str = Directory.GetFiles("../../../RepoFolder/Database", "*.xml");
            foreach (string s in str)
            {
                Console.WriteLine("Log File  name : " + s);
            }
            Console.WriteLine("** I had left few old xml to help me demonstrate requirement 9");

            Console.WriteLine(" :: Please find below the dll files present in repository -");

            str = Directory.GetFiles("../../../RepoFolder/Repository", "*.dll", SearchOption.AllDirectories);
            foreach (string s in str)
            {
                Console.WriteLine("DLL File  name : " + s);
            }
        }
        //Helps in demostrating requirement 9 ,10,11 and 12.
        public void Requirement9() {
            printLine();
            Console.WriteLine("Requirement 9.The Repository shall support client queries about Test Results from the Repository storage");
            Console.WriteLine("  ::Sending request to get file ../../../RepoFolder/Database/ForRequirement9_Demonstration_20161119221652930.xml");
            cl2.getLogFile(@"../../../RepoFolder/Database/ForRequirement9_Demonstration_20161119221652930.xml");
            Message msg=cl2.getIncomingMessage();
            Console.WriteLine("Body of Message is as below( In GUI we are parsing it and displaying results)");
            Console.WriteLine(msg.body);
            printLine();
            Console.WriteLine("Requirement 10: All communication between Clients, the Test Harness, and the Repository, shall be implemented using Windows Communication Foundation (WCF) channels, passing messages that contain requests, results, and notifications.");
            Console.WriteLine("  :: All communication above is happening using WCF. Please verify packages \n 1.Iservice \n 2. Client Sender, Test harness Sender , Repository Sender \n 3. Client/TestHarness/Repository Receiver ");
            printLine();
            Console.WriteLine("Requirement 11: Clients shall provide a Graphical User Interface constructed with Windows Presentation Foundation (WPF).");
            Console.WriteLine("  :: Please check the GUI which has opened along with this console.");
            Console.WriteLine("  :: In case of Repositor we pass its port number as command line argument");
            Console.WriteLine("  :: In case of TestHarness we pass its port number and Repository port number  as command line arguments");
            printLine();
            Console.WriteLine("Requirement 12: includes means to time test executions and communication latency. ");
            Console.WriteLine("  :: Test Harness side- we are printing communication latency for file transfer as \" ## communication latency for file\" . Please check Test Harness console");
            Console.WriteLine("  :: Client Side: We are displaying the time taken in display when user executes the test using GUI");
            printLine();
            Console.WriteLine("Requirement 13: Demonstarted all requirements from 2 to 10 ");
            printLine();
            Console.WriteLine("Requirement 14: Please find the file with name Requirement14 in project folder");

        }
        //Main method
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("===========This is Test Executive at port number " + args[0] + "===================");
                Console.WriteLine();
                Program p = new Program(args[0], args[1], args[2]);
                p.Requirement12();
                p.Requirement3();
                p.Requirement6();
                p.Requirement9();
                Console.ReadKey();
            }
            catch (Exception ex) {

                Console.WriteLine("Exception catched : " + ex.Message);
            }

        }
    }
}
