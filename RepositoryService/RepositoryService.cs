///////////////////////////////////////////////////////////////////////
// RepositorySevice.cs -Implementation of Service Contract            //
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
 * the wcf communication happening in this project. With the help of the proxy object of the Repository
 * any other machine can call any of these function to communicate with the Repository. We can say it is
 * the concrete implementation of the service contract and is a service class for Repository.
 *
 *Public Interfaces:
 *getMessage   //function to get the incoming message that was send by other machine.
 *OpenFileForWrite         // To open a file on local machine.
 *WriteFileBlock     //To write blocks in the file that was opened.
 *closeFile           //To close the file.
 *sendMessage      //This function is used by other machines to send message to Repository.
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
using System.Threading;
namespace RepositoryService
{
    using Iservice;
    using SWTools;
    using Message;
    using RepositorySender;
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
  public  class RepositoryService : Iservice
    {
        string filePath = ".\\TempFolder";
        string Repositorypath = "..\\..\\..\\RepoFolder\\Repository";
        string fileSpec = "";
        static BlockingQueue<Message> IncomingMessageQueue= new BlockingQueue<Message>();
        FileStream fs = null; 
        string Filename;

//This function helps the other machine to download code file from Repository
        public bool downloadFile(string name, string requestingUrl) {
            int counter = 0;
            while (true) {
                counter++;
            RepositorySender rs = new RepositorySender(requestingUrl);

            try
            {
                rs.sendFileFunction(name);
                    return true;
            }
            catch (Exception ex) {
                    if (counter < 5) {
                        Console.WriteLine("Execption thrown " + ex.Message +" . Retrying attemp " +counter);
                        Thread.Sleep(800);
                       
                    }
                    else {
                        Console.WriteLine("Execption thrown " + ex.Message + " . Shutting off");

                        return false;
                    }
                
            }
            }
        }

// This function helps in closing the file stream that is currentry open
        public bool CloseFile()
        {

            try
            {
                fs.Close();
                Console.Write("\n  {0} closed", fileSpec);
                string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                string pathString = Repositorypath + "\\" + Filename+ time;

                System.IO.Directory.CreateDirectory(pathString);
                string source = filePath + "\\" + Filename;
                File.Copy(source, pathString+"\\"+Filename);
                return true;
            }
            catch { return false; }
        }

        //This Function is to give the incoming message 
        public Message getMessage()
        {
            return IncomingMessageQueue.deQ();
        }

        // This function is to create a file and attach it to a file stream
        public bool OpenFileForWrite(string name)
        {
            //  Console.WriteLine("File open called");
            Filename = name;
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            fileSpec = filePath + "\\" + name;
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


// This function is to write messages into the file stream
        public bool WriteFileBlock(byte[] block)
        {

            try
            {
                fs.Write(block, 0, block.Length);
                fs.Flush();
                return true;
            }
            catch { return false; }
        }
// This function is used to send message to Repository by other machines
        public void sendMessage(Message msg)
        {
           
            IncomingMessageQueue.enQ(msg);
        }
//Main method
        static void Main(string[] args)
        {

            RepositoryService rs = new RepositoryService();
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
