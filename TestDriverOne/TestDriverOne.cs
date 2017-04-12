/////////////////////////////////////////////////////////////////////
//  TestDriverOne.cs - Test Driver for CodeToBeTested1             //
//  ver 1.0                                                        //
//  Language:      Visual C#  2015                                 //
//  Platform:      HP Pallvilion, Windows 7                        //
//  Application:   TestHarness, FL16                          //
//  Author:        Shishir Bijalwan, Syracuse University           //
//                 (979) 587-6340, sbijalwa@syr.edu                //
/////////////////////////////////////////////////////////////////////

/*
Module Operations:
==================
This Package has been created to test the CodeToBeTested1 package. This package inherits from the
ITest interface. We have given the concrete implementation of doTest() and getLog() function of
ITest interface in this package.

Public Interface:
=================
getLogs                  // This method is receive the logs which are generated once the code has been tested.
doTest					 // In this function we create the object of CodeToBeTested1 and test it.
TestDriverTwo            //In the constructor we intialize the appDomain variable with th currentDomain.  

Build Process:
==============
Required files
 - Required Files: TestDriverOne.cs,CodeToBeTested1.cs,ITest.cs
- Build Command: devenv TestHarness.sln /rebuild debug
Maintenance History:
====================
ver 1.0

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITestDriver
{
    using CodeTobeTested;

    public class TestDriverOne : MarshalByRefObject, ITest
    {
        private AppDomain ad = null;
        private CodeToBeTested1 codeT;
        private bool testResult = false;

        //In the constructor we intialize the appDomain variable with th currentDomain
        public TestDriverOne()
        {
            ad = AppDomain.CurrentDomain;

        }

        //Function to Test the code present in CodeToBeTested1
        public bool test()
        {
            Console.WriteLine("Test Function of TestDriverOne called in domain " + ad.FriendlyName);

            codeT = new CodeToBeTested1();
            int x = codeT.addIntFunction(1, 2);
            if (x == 3)
                testResult = true;
            //   int exception_var1 = 10;
            //   int exception_var2 = 0;
            //  int var = exception_var1 / exception_var2;
            return testResult;
        }

        //This function is to get the logs based on the test function output
        public string getLog()
        {
            if (testResult)
                return "The expected output received for CodeToBeTested1. Test passed!!!";
            else
                return "The expected output not received for CodeToBeTested1. Test failed!!!";

        }

        //Main Function
        static void Main(string[] args)
        {
            ITest TestDriver1 = new TestDriverOne();
            TestDriver1.test();
            Console.WriteLine(TestDriver1.getLog());
        }
    }
}
