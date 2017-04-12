///////////////////////////////////////////////////////////////////////
// ClientSevice.cs -Implementation of Service Contract               //
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
 *This package is the implementation of the IService interface which is the service contract for 
 * the wcf communication happening in this project. With the help of the proxy object of the client
 * any other machine can call any of these function to communicate with the client. We can say it is
 * the concrete implementation of the service contract and is a service class for client.
 *
 *Public Interfaces:
 *getMessage   //function to get the imcoming message that was send by other machine.
 *OpenFileForWrite         // To open a file on local machine.
 *WriteFileBlock     //To write blocks in the file that was opened.
 *closeFile           //To close the file.
 *sendMessage      //This function is used by other machines to send message to client.
*
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
using System.ServiceModel.Activation;
using System.IO;
namespace ClientService
{
    using Iservice;
    using SWTools;
    using Message;
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
  public  class ClientService : Iservice
    {
        string filePath = "..\\..\\..\\LocalDownloadFolder";
        string fileSpec = "";
        static BlockingQueue<Message> IncomingMessageQueue = new BlockingQueue<Message>();
        FileStream fs = null;  

     // Function to close a file stream
        public bool CloseFile()
        {
            //   Console.WriteLine("File close called");

            try
            {
                fs.Close();
                Console.Write("\n  {0} closed", fileSpec);
                return true;
            }
            catch { return false; }
        }

        // Function to give the message that came in to client
        public Message getMessage()
        {
            return IncomingMessageQueue.deQ();
        }

        //Function to open and write a file
        public bool OpenFileForWrite(string fname)
        {

            string filename = new DirectoryInfo(fname).Name;

            //  Console.WriteLine("File open called");

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            fileSpec = filePath + "\\" + filename;
            try
            {
                fs = File.Open(fileSpec, FileMode.Create, FileAccess.Write);
                //    Console.Write("\n  {0} opened", fileSpec);
                return true;
            }
            catch
            {
                Console.Write("\n  {0} filed to open", fileSpec);
                return false;
            }
        }

        // Download function(Not getting used in project now)
        public bool downloadFile(string name, string requestingUrl)
        {

            return true;
        }

        //Function to write blocks in the file stream that was opened
        public bool WriteFileBlock(byte[] block)
        {
            //  Console.WriteLine("File write called");

            try
            {
                //  Console.Write("\n  writing block with {0} bytes", block.Length);
                fs.Write(block, 0, block.Length);
                fs.Flush();
                return true;
            }
            catch { return false; }
        }

        // Function which is used by other machine to send message to client
        public void sendMessage(Message msg)
        {
           
            IncomingMessageQueue.enQ(msg);
        }
//Main method
        static void Main(string[] args)
        {

            ClientService rs = new ClientService();
            string name = "check.txt";
            rs.OpenFileForWrite(name);
            string filePath = Directory.GetCurrentDirectory();
            FileStream fs = File.Open(name, FileMode.Open, FileAccess.Read);
            long blockSize = 512;

            int bytesRead = 0;
            while (true)
            {
                long remainder = (int)(fs.Length - fs.Position);
                if (remainder == 0)
                    break;
                long size = Math.Min(blockSize, remainder);
                byte[] block = new byte[size];
                bytesRead = fs.Read(block, 0, block.Length);
                rs.WriteFileBlock(block);
            }
            fs.Close();
            rs.CloseFile();


        }
    }
}
