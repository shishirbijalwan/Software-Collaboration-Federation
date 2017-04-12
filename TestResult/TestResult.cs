///////////////////////////////////////////////////////////////////////
// TestResult.cs -For saving Test Result as Data structure           //
// ver 1.0                                                           //
// Language:    C#, Visual Studio 2015                              //
// Application: Test Harness, CSE681 - SMA                           //
//  Platform:      HP Pavilion dv6/ Window 7 Service Pack 1          //
//Author:       Shishir Bijalwan, Syracuse University                //
//              sbijalwa@syr.edu, 9795876340                         //
// Source: Jim Fawcett, CSE687 - Object Oriented Design, Spring 2016 //
// CST 4-187, Syracuse University, 315 443-3948, jfawcett@twcny.rr.com //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package is to help the test harness in storing the results in form of a object. The test harness
 * creates objects of this class and uses it for information transfer between the child appDomain and
 * the main domain as well as back to the user. It is quite similar to Test Request with extra column
 * stating test passed or failed and to save extra logs transfered by child appDomain.
 *
 *Methods:
 *TestResult   //Construtor to set the basic information into the Test Result object.
 *show         // To show the data stored in Test Result.
 *testName    //setter and getter method for variable
 *author      //setter and getter method for variable
 *timeStamp   //setter and getter method for variable
 *testDriver  //setter and getter method for variable
 *testCode    //setter and getter method for variable
 *logs        //setter and getter method for variable
 *testPass    //setter and getter method for variable
 *fullLogs    //setter and getter method for variable
 *
 * Build Process:
 * --------------
 * Required Files: TestResult.cs
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

namespace TestResult
{
    [Serializable]
    public class TestResult
    {

        public string testName { get; set; }
        public string author { get; set; }
        public DateTime timeStamp { get; set; }
        public String testDriver { get; set; }
        public List<string> testCode { get; set; }
        public bool testPass { get; set; }
        public String logs { get; set; }
        public String fullLogs { get; set; }
        public string to { get; set; }
        public string from { get; set; }  
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Constructor to assign basic informtion to Test Result object
        public TestResult(string TestName, string Author, String testdriver, List<string> testcode)
        {

            testName = TestName;
            author = Author;
            testDriver = testdriver;
            testCode = testcode;
            timeStamp = DateTime.Now;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Function to display data inside Test Result object
        public void show()
        {
            Console.Write("\n  {0,-12} : {1}", "To ", to);
            Console.Write("\n  {0,-12} : {1}", "From ", from);

            Console.Write("\n  {0,-12} : {1}", "test name", testName);
            Console.Write("\n  {0,12} : {1}", "author", author);
            Console.Write("\n  {0,12} : {1}", "time stamp", timeStamp);
            Console.Write("\n  {0,12} : {1}", "test driver", testDriver);
            foreach (string library in testCode)
            {
                Console.Write("\n  {0,12} : {1}", "library", library);
            }
            Console.Write("\n {0,12} :{1}", "TestPass", testPass);
            if (fullLogs == "true")
                Console.Write("\n {0,12} :{1}", "Logs", logs);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Main function
        static void Main(string[] args)
        {
            List<string> str = new List<string>();
            str.Add("code1");
            str.Add("code2");

            TestResult t = new TestResult("TestDriver1", "Shishir", "GAME", str);
            t.show();
        }
    }
}
