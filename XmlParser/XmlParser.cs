///////////////////////////////////////////////////////////////////////
// XmlParser.cs - For parsing a xml file given by user               //
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
 * This package has been craeted to help the test harness to extract information from a
 * xml file. The parse function of this classs takes xml file as input and parse them and
 * save them as Test Resquest. This package also defines the a class Test which is used as 
 * a datastructure to save all the information extracted from the xml file. It has a function
 * to get the list of test Request.
 *
 *Methods:
 *XmlParser            //Constructor to create object of memeber variables.
 *getTestRequestList  // Returns all test object created from a xml file.
 *parse               // Function to extarct information from the xml file and creating test object.
 *show                //Displays all the data in the Test request 
 * Build Process:
 * --------------
 * Required Files: XmlParser.cs
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
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRequestParser
{

    public class Test
    {
        public string testName { get; set; }
        public string author { get; set; }
        public DateTime timeStamp { get; set; }
        public String testDriver { get; set; }
        public List<string> testCode { get; set; }
        public string fullLogs { get; set; }
        public bool QueryRequestOnly { get; set; }
        public bool AllFilesPresent { get; set; }
        public string to { get; set; }
        public string from { get; set; }
        public  string typeOfReuqest {get;set;}
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Function to show the content inside a test request object
        public void show()
        {
            Console.Write("\n  {0,-12} : {1}", "test name ", testName);
            Console.Write("\n  {0,12} : {1}", "author ", author);
            Console.Write("\n  {0,12} : {1}", "time stamp ", timeStamp);
            Console.Write("\n  {0,12} : {1}", "test driver ", testDriver);
            Console.Write("\n  {0,12} : {1}", "To ", to);
            Console.Write("\n  {0,12} : {1}", "from ", from);

            foreach (string library in testCode)
            {
                Console.Write("\n  {0,12} : {1}", "library", library);
            }
        }
        public Test()
        {
            AllFilesPresent = true;
            testCode = new List<string>();
        }
    }

    public class XmlParser
    {
        private XDocument doc_;
        private List<Test> testList_;

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Constructor to assign XDocumnet object and create new list
        public XmlParser()
        {
            doc_ = new XDocument();
            testList_ = new List<Test>();
        }
        public List<Test> getTestRequestList()
        {
            return testList_;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //This function is to extract information from ainput xml file
        public bool parse(System.IO.Stream xml)
        {
            doc_ = XDocument.Load(xml);
            if (doc_ == null)
                return false;
            string author = doc_.Descendants("author").First().Value;
            Test test = null;

            XElement[] xtests = doc_.Descendants("test").ToArray();
            int numTests = xtests.Count();

            for (int i = 0; i < numTests; ++i)
            {  // extracting data from each test request inside a xml file 
                test = new Test();
                test.testCode = new List<string>();
                test.author = author;
                test.timeStamp = DateTime.Now;
                test.testName = xtests[i].Attribute("name").Value;
                test.testDriver = xtests[i].Element("testDriver").Value;
                test.fullLogs = xtests[i].Element("fullLogs").Value;
                test.QueryRequestOnly = Convert.ToBoolean(xtests[i].Element("QueryRequestOnly").Value);
                IEnumerable<XElement> xtestCode = xtests[i].Elements("library");
                foreach (var xlibrary in xtestCode)
                {
                    test.testCode.Add(xlibrary.Value);
                }
                testList_.Add(test); // adding test object to the list
            }
            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Main Function
        static void Main(string[] args)
        {
            XmlParser demo = new XmlParser();
            try
            {
                string path = "../../../XMLRequestFolder/TestRequest.xml";
                System.IO.FileStream xml = new System.IO.FileStream(path, System.IO.FileMode.Open);
                demo.parse(xml);
                foreach (Test test in demo.testList_)
                {
                    test.show();
                }
            }
            catch (Exception ex)
            {
                Console.Write("\n\n  {0}", ex.Message);
            }
        }
    }
}

