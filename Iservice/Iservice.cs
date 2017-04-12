///////////////////////////////////////////////////////////////////////
// Iservice.cs -Service contract interface                           //
// ver 1.0                                                           //
// Language:    C#, Visual Studio 2015                               //
// Application: TestHarnessReceiver, CSE681 - SMA                    //
//  Platform:      HP Pavilion dv6/ Window 7 Service Pack 1          //
//Author:       Shishir Bijalwan, Syracuse University                //
//              sbijalwa@syr.edu, 9795876340                         //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 *This package is about the service contract which will be used by the Test Harness and
 * client class will use to communicate with each other. The operation contract sendMessage
 * will help them to post message to each other.
 *
 *Methods:
 *sendMessage        //For sending message.
 *getMessage   // For getting message.
 * Build Process:
 * --------------
 * Required Files: Iservice.cs
 * Build Command: devenv TestHarness.sln /rebuild debug
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 15 Oct 16
 * - first release
 *
 */
using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Iservice
{
    using Message;
    [ServiceContract(Namespace = "Iservice")]
    public interface Iservice
    {
        [OperationContract]
        void sendMessage(Message msg);

        Message getMessage();

        [OperationContract]
        bool OpenFileForWrite(string name);

        [OperationContract]
        bool WriteFileBlock(byte[] block);

        [OperationContract]
        bool CloseFile();

        [OperationContract]
        bool downloadFile(string name, string requestingUrl);
    }
}
