///////////////////////////////////////////////////////////////////////
// RepositoryReceiver.cs - Repository Listener                       //
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
 * The purpose of this package is to  create a listener for the Repository class. At the time of creation
 * of the Repository class. Repository creates the instance of the Repository receiver to help the Repository in listening.
 * The Repository receiver class needs the URI, service contract and service class for the creation of the
 * host. It also needs to know the binding to know the protocol which will be used.
 *
 *Public Interfaces:
 *CreateChannel   //This function helps in creating host that helps in getting incoming messages.
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

namespace RepositoryReceiver
{
    using RepositoryService;
    using Iservice;
  public  class RepositoryReceiver
    {
        //This function helps in creating host that helps in getting incoming messages.
        public static ServiceHost CreateChannel(string url)
        {
            WSHttpBinding binding = new WSHttpBinding();
            Uri address = new Uri(url);
            Type service = typeof(RepositoryService);
            ServiceHost host = new ServiceHost(service, address);
            host.AddServiceEndpoint(typeof(Iservice), binding, address);

            return host;
        }
        static void Main(string[] args)
        {


            Console.Write("\n  File Transfer Service running:");
            Console.Write("\n ================================\n");

            ServiceHost host = CreateChannel("http://localhost:8080/BasicService");
            host.Open();
            Console.Write("\n  Press key to terminate service:\n");
            Console.ReadKey();
            Console.Write("\n");
            host.Close();
        }
    }
}
