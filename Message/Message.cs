///////////////////////////////////////////////////////////////////////
// Message.cs - WCF com message                                      //
// ver 1.0                                                           //
// Language:    C#, Visual Studio 2015                              //
// Application: Test Harness, CSE681 - SMA                           //
//  Platform:      HP Pavilion dv6/ Window 7 Service Pack 1          //
//Author:       Shishir Bijalwan, Syracuse University                //
//              sbijalwa@syr.edu, 9795876340                         //
// Source:       Jim Fawcett, CST 2-187, Syracuse Univ.             ///
///                (315) 443-3948, jfawcett@twcny.rr.com            ///
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package has been designed to keep the a generic class for all types of messages that will
 * be sent over the wcf channel in this project. This message class can be used to send test request,
 * log request, Test Results by just changing the type and the body of this message class. We have made this
 * class serializable so that this can be serialized to be sent over wcf by windows serializer.
 *
 *Public Interfaces:
 *fromString   //This function is to construct a message object from a string.
 *ToString         // this function is to convert message class data to a string.
 *copy         //To create a copy of the message.

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

namespace Message
{
    [Serializable]
  public  class Message
    {
        public string to { get; set; }
        public string from { get; set; }
        public string type { get; set; }
        public string author { get; set; } = "";
        public DateTime time { get; set; } = DateTime.Now;
        public string body { get; set; } = "";

// This is the constructor of message class to set body and type propety of message class
        public Message(string bodyStr = "")
        {
            body = bodyStr;
            type = "undefined";
        }

// This function helps in creating a message object from  string 
        public Message fromString(string msgStr)
        {
            Message msg = new Message();
            try
            {
                string[] parts = msgStr.Split(',');
                for (int i = 0; i < parts.Count(); ++i)
                    parts[i] = parts[i].Trim();

                msg.to = parts[0].Substring(4);
                msg.from = parts[1].Substring(6);
                msg.type = parts[2].Substring(6);
                msg.author = parts[3].Substring(8);
                msg.time = DateTime.Parse(parts[4].Substring(6));
                if (parts[5].Count() > 6)
                    msg.body = parts[5].Substring(6);
            }
            catch
            {
                Console.Write("\n  string parsing failed in Message.fromString(string)");
                return null;
            }
            return msg;
        }

        // This function is to create a string from Message object. It overrides the toString function
        public override string ToString()
        {
            string temp = "to: " + to;
            temp += ", from: " + from;
            temp += ", type: " + type;
            if (author != "")
                temp += ", author: " + author;
            temp += ", time: " + time;
            temp += ", body:\n" + body;
            return temp;
        }

        //This function is create a copy of message
        public Message copy(Message msg)
        {
            Message temp = new Message();
            temp.to = msg.to;
            temp.from = msg.from;
            temp.type = msg.type;
            temp.author = msg.author;
            temp.time = DateTime.Now;
            temp.body = msg.body;
            return temp;
        }
        //main method
        static void Main(string[] args)
        {
            Message msg = new Message();
            string url = "http://localhost:8080/BasicService";
            msg.to = url;
            msg.from = "http://localhost:8085/BasicService";
            msg.body = "Demo Bosy";
            msg.type = "check";
            Message copyMsg = msg.copy(msg);
            Console.WriteLine("Body of copy is " + copyMsg.body);
        }
    }
}
