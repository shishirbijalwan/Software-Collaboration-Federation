///////////////////////////////////////////////////////////////////////
// ClientReceiver.cs - Client Listener                               //
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
 * The purpose of this package is to  create a listener for the client class. At the time of creation
 * of the client class. It creates the instance of the client receiver to help the client in listening.
 * The client receiver class needs the URI, service contract and service class for the creation of the
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
namespace ClientReceiver
{
    using Iservice;
    using ClientService;
  public  class ClientReceiver
    {
        // Function to create host for the client.
        public static ServiceHost CreateChannel(string url)
        {
            WSHttpBinding binding = new WSHttpBinding();
            Uri address = new Uri(url);
            Type service = typeof(ClientService);
            ServiceHost host = new ServiceHost(service, address);
            host.AddServiceEndpoint(typeof(Iservice), binding, address);

            return host;
        }
        //Main method
        static void Main(string[] args)
        {
            ServiceHost host = ClientReceiver.CreateChannel("http://localhost:8085/BasicService");
            host.Open();
        }
    }
}
