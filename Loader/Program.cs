///////////////////////////////////////////////////////////////////////
// Loader.cs -Is to facilitate running of test function of testDriver//
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
 * The purpose of this package is help the appDomain manager in loading all the required DLL in the child appDomain.
 * The appDomain manager creates the object of loader in the child appDomain and then use it load assembly function,
 * to load the assembly. Once all required assemblies have been loaded appDomain Manager uses the Execute function
 * to run test and getLog function of the test Driver, which returns result object to appDomain.
 *
 *Methods:
 *loadAssembly   //This function is to load assembly from a file.
 *ExecuteTest // The purpose of this function is to create testdriver class handler and execute test and getlog function of it.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Security.Policy;    // defines evidence needed for AppDomain construction
using System.Runtime.Remoting;   // provides remote communication between AppDomains
using System.IO;
namespace Loader
{
    using ITestDriver;
    using TestResult;

    public class Loader : MarshalByRefObject
    {
        private Assembly assembly;

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //This function is to load assembly from a file
        public void loadAssembly(string fileName)
        {
            assembly = Assembly.LoadFrom(fileName);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //This function is to craete ofbject of testDriver class and call test and getlog function to get results
        public TestResult ExecuteTest(string TestName, string Author, String testdriver, List<string> testcode, string fulllogs, string to, string from)
        {
            AppDomain ap = AppDomain.CurrentDomain;
            Console.WriteLine("==App Domain name " + ap.FriendlyName);
            Assembly[] a1 = ap.GetAssemblies(); //Loading all assemblies in child appDomain
            TestResult resultObject = new TestResult(TestName, Author, testdriver, testcode);
            resultObject.to = to;
            resultObject.from = from;
            foreach (Assembly asm in a1)
            {

                Type[] types = asm.GetExportedTypes();
                foreach (Type t in types)
                {
                    if (t.IsClass && typeof(ITest).IsAssignableFrom(t))  // does this type derive from ITest ?
                    {
                        try
                        {
                            ObjectHandle oh = ap.CreateInstance(asm.FullName, t.FullName); //Object handler creation
                            object ob = oh.Unwrap();
                            ITestDriver.ITest tdr = (ITestDriver.ITest)ob;
                            bool var = tdr.test();  //Test function of driver called
                            string str = tdr.getLog(); //getlog function called
                            resultObject.testPass = var;
                            resultObject.logs = str;
                            resultObject.fullLogs = fulllogs;
                        }
                        catch (Exception ex)
                        {
                            Console.Write("\n  Exception caught in child domain: {0}", ex.Message);
                            resultObject.testPass = false;
                            resultObject.logs = ex.Message;
                            resultObject.fullLogs = fulllogs;
                            return resultObject;
                        }
                    }

                }

            }
            return resultObject;

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Main
        static void Main(string[] args)
        {
            Loader load = new Loader();
            load.loadAssembly("../../../DLLFiles/TestDriverOne.dll");
            load.loadAssembly("../../../DLLFiles/CodeToBeTested1.dll");
            File.Copy("../../../DLLFiles/CodeToBeTested1.dll", "CodeToBeTested1.dll");
            File.Copy("../../../DLLFiles/TestDriverOne.dll", "TestDriverOne.dll");
            List<string> code = new List<string>();
            code.Add("CodeToBeTested1.dll");
            load.ExecuteTest("TestDriverOne.dll", "Shiihir", "TestDriverOne.dll", code, "true","sender","receiver");

        }
    }
}
