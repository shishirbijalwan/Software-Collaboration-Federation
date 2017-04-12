/////////////////////////////////////////////////////////////////////
//  TestDriverTwo.cs - Test Driver for CodeToBeTestedTwo           //
//  ver 1.0                                                        //
//  Language:      Visual C#  2015                                 //
//  Platform:      HP Pallvilion, Windows 7                        //
//  Application:   TestHarness, FL16                              //
//  Author:        Shishir Bijalwan, Syracuse University           //
//                 (979) 587-6340, sbijalwa@syr.edu                //
/////////////////////////////////////////////////////////////////////

/*
Module Operations:
==================
This Package has been created to test the CodeToBeTested2 package. This package inherits from the
ITest interface. We have given the concrete implementation of doTest() and getLog() function of
ITest interface in this package.

Public Interface:
=================
getLogs                  // This method is receive the logs which are generated once the code has been tested.
doTest					 // In this function we create the object of CodeToBeTested2 and test it.
TestDriverTwo            //In the constructor we intialize the appDomain variable with th currentDomain.  

Build Process:
==============
Required files
- CodeToBeTested2.cs,TestDriverTwo.cs,ITest.cs
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
    using CodeNamespace;

    public class TestDriverTwo : MarshalByRefObject, ITest
    {
        private AppDomain ad = null;

        private CodeToBeTested2 codeT;
        private bool testResult = false;

        //In the constructor we intialize the appDomain variable with th currentDomain
        public TestDriverTwo()
        {
            ad = AppDomain.CurrentDomain;

        }


        //Function to Test the code present in CodeToBeTested2
        public bool test()
        {
            Console.WriteLine("Test Function of TestDriverTwo called in domain " + ad.FriendlyName);

            codeT = new CodeToBeTested2();
            int x = codeT.multiplyIntFunction(1, 2);
            if (x == 3)   //We have checked for wrong value to demonstarte the case of failed test
                testResult = true;
            return testResult;
        }

        //This function is to get the logs based on the test function output
        public string getLog()
        {
            if (testResult)
                return "The expected output received for CodeToBeTested2. Test passed!!!";
            else
                return "The expected output not received for CodeToBeTested2. Test failed!!!";

        }
        //Main Function
        static void Main(string[] args)
        {
            ITest TestDriver2 = new TestDriverTwo();
            TestDriver2.test();
            Console.WriteLine(TestDriver2.getLog());
        }
    }
}
