///////////////////////////////////////////////////////////////////////
// RepositorySender.cs - Sending outgoing messages and Files         //
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
 * This package has been designed keeping in mind that the Repository will need to send messages and 
 * files to the client and the Test Harness. This class helps in creation of proxy of a service
 * which will be used to send files and messages to Repository. 
 *
 *Public Interfaces:
 *RepositorySender   //Constructor to create a proxy for a given url.
 *SendMessage     //To send messages to other machine.
 *sendFileFunction     //Function to send file to Test Harness and Client.

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

namespace RepositorySender
{
    using Iservice;
    using Message;
  public  class RepositorySender
    {
        private Iservice svc;

        //Createing proxy
        public RepositorySender(string url)
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

        //Function to send message
        public void SendMessage(Message str)
        {
            svc.sendMessage(str);
        }
        // This function is to send file to client or Test Harness depending upon proxy
        public void sendFileFunction(string name)
        {           

            //  while (true) { 
            long blockSize = 512;
            

                svc.OpenFileForWrite(name);
                string filePath = Directory.GetCurrentDirectory();
                FileStream fs = File.Open(name, FileMode.Open, FileAccess.Read);
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
        //Main method
        static void Main(string[] args)
        {

            string filePath = Directory.GetCurrentDirectory();
            Console.WriteLine(filePath);
            string url = "http://localhost:8080/BasicService";

            RepositorySender cl = new RepositorySender(url);

            cl.sendFileFunction("TestFile.txt");
            Message msg = new Message();
            msg.to = url;
            msg.body = "shishir msg body";
            cl.SendMessage(msg);
        }
    }
}
