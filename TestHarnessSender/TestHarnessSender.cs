///////////////////////////////////////////////////////////////////////
// TestHarnessSender.cs - Sending outgoing messages                  //
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
 * This package has been designed keeping in mind that the TestHarness will need to send messages 
 * to the Repository and the client. This class helps in creation of proxy of a service
 * which will be used to send files and messages to it. This also has a function to request a file 
 * from a server which helps in download operation.
 *
 *Public Interfaces:
 *TestHarnessSender         //Constructor to create a proxy for a given url.
 *getCodeFile         // To download a code file .
 *SendMessage          //To send messages to other machine.
 *sendFileFunction    //Function to upload files in Repository .

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

namespace TestHarnessSender
{
    using Iservice;
    using Message;
 public   class TestHarnessSender
    {
        private Iservice svc;

        //Createing proxy
        public TestHarnessSender(string url)
        {
            svc = CreateProxy<Iservice>(url);
        }


        //Function to create proxy of the machine we want to connect
        static C CreateProxy<C>(string url)
        {
            WSHttpBinding binding = new WSHttpBinding();
            EndpointAddress address = new EndpointAddress(url);
            ChannelFactory<C> factory = new ChannelFactory<C>(binding, address);
            return factory.CreateChannel();
        }

        // This function is to get the code filr from the repository
        public bool getCodeFile(string filename, string senderUrl)
        {

            return svc.downloadFile(filename, senderUrl);

        }
        // This function is to send message to Repository and clients
        public void SendMessage(Message str)
        {

            svc.sendMessage(str);
        }

        //not required in Test Harness as we won't be uploading files. We need to download them only
        // Will remove in next release
        public void sendFileFunction(string CompleteFileName)
        {
            int count = 0;


            string filename = new DirectoryInfo(CompleteFileName).Name;
            long blockSize = 512;
            try
            {
                count++;

                svc.OpenFileForWrite(filename);
                string filePath = Directory.GetCurrentDirectory();
                FileStream fs = File.Open(CompleteFileName, FileMode.Open, FileAccess.Read);
                int bytesRead = 0;
                while (true)
                {
                    long remainder = (int)(fs.Length - fs.Position);
                    if (remainder == 0)
                        break;
                    long size = Math.Min(blockSize, remainder);
                    byte[] block = new byte[size];
                    bytesRead = fs.Read(block, 0, block.Length);
                    svc.WriteFileBlock(block);
                }
                fs.Close();
                svc.CloseFile();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception" + e.Message);

            }


        }
        //Main method
        static void Main(string[] args)
        {
            // string filePath = Directory.GetCurrentDirectory();
            // Console.WriteLine(filePath);
            Console.WriteLine("This is TestHarness");

            string url = "http://localhost:8080/BasicService";

            TestHarnessSender cl = new TestHarnessSender(url);

          //  cl.sendFileFunction("TestFile.txt");
            Message msg = new Message();
            msg.to = "http://localhost:8080/BasicService";
            msg.from = url;
            msg.type = "Test_Result";
            Console.Read();
            cl.SendMessage(msg);
        }
    }
}
