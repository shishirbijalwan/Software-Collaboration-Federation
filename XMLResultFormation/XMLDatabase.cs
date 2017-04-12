///////////////////////////////////////////////////////////////////////
// XMLDatase.cs -This package is a driver to connect to XML Database //
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
 * Any form of request either to read from the XML database or to write into the XML database has to 
 * go through the XMLDatabase package. It has two major functions one is to write the data which is in
 * testResult object into a xml file. It uses the user and timestamp to save the file name. Other operation
 * of this package is to read a xml file which was created to store the data. The folder it uses as database 
 * in this project is "Database". 
 *
 *Methods:
 *createXMLResult   //Creation of XML file with test result data.
 *getlogs           // This function is to get stored logs from database.
 *parse             // To extract information xml files.
 *
 * Build Process:
 * --------------
 * Required Files: XmlParser.cs,TestResult.cs, XMLDatabase.cs
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
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace XMLDatabase
{
    using TestResult;
    using TestRequestParser;
    using FileManager;
    public class XMLDatabase
    {
        private XDocument doc_;
        private Test currentTest;

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Function for creation of xml file with data from TestResult object
        public void createXMLResult(TestResult test)
        {
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XComment comment = new XComment("These are logs for " + test.testDriver);
            xml.Add(comment);
            XElement root = new XElement("Result"); // Xml file will have the root with name Result and all information will be inside it
            xml.Add(root);
            XElement child1 = new XElement("author", test.author);
            root.Add(child1);
            XElement child2 = new XElement("TestDriver", test.testDriver);
            root.Add(child2);
            XElement child3;
            foreach (string str in test.testCode)
            {
                child3 = new XElement("Library", str);
                root.Add(child3);
            }
            XElement child4 = new XElement("TestPassed", test.testPass);
            root.Add(child4);
            XElement child5 = new XElement("TestLogs", test.logs);
            root.Add(child5);
            XElement child6 = new XElement("Timestamp", test.timeStamp);
            root.Add(child6);
            var path = test.testDriver;
            var dirName = new DirectoryInfo(path).Name;
            string authorname = test.author.Replace(" ", "_");
            string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            string file = "../../../Database/" + dirName + "_" + authorname + "_" + time + ".xml"; // Genrating name of file using driverName, author name and timestamp
            xml.Save(file);
        }


        public string createXMLResultToString(TestResult test)
        {
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XComment comment = new XComment("These are logs for " + test.testDriver);
            xml.Add(comment);
            XElement root = new XElement("Result"); // Xml file will have the root with name Result and all information will be inside it
            xml.Add(root);
            XElement child1 = new XElement("author", test.author);
            root.Add(child1);
            XElement child2 = new XElement("TestDriver", test.testDriver);
            root.Add(child2);
            XElement child3;
            foreach (string str in test.testCode)
            {
                child3 = new XElement("Library", str);
                root.Add(child3);
            }
            XElement child4 = new XElement("TestPassed", test.testPass);
            root.Add(child4);
            XElement child5 = new XElement("TestLogs", test.logs);
            root.Add(child5);
            XElement child6 = new XElement("Timestamp", test.timeStamp);
            root.Add(child6);
            var path = test.testDriver;
            var dirName = new DirectoryInfo(path).Name;
            string authorname = test.author.Replace(" ", "_");
            string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");

           return xml.ToString();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //This function is to retieve old logs using the test request sent by user
        public List<TestResult> getlogs(Test test)
        {

            FileMgr fileManager = new FileMgr();

            var path = test.testDriver;
            var dirName = new DirectoryInfo(path).Name;
            string authorname = test.author.Replace(" ", "_");
            string search = dirName + "_" + authorname + "*.xml";
            List<string> files = fileManager.getDLLFileList("../../../Database/", search); // seaching for file in database
            currentTest = test;
            List<TestResult> testResult = new List<TestResult>();
            foreach (string str in files)
            {
                System.IO.FileStream xml = new System.IO.FileStream(str, System.IO.FileMode.Open);
                testResult.Add(parse(xml)); // calling helper function to parse
            }
            return testResult;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //This function is to  parse the XML file withe the help of the parser.
        private TestResult parse(System.IO.Stream xml)
        {
            doc_ = XDocument.Load(xml);
            if (doc_ == null)
                return null;
            XElement[] xtests = doc_.Descendants("Result").ToArray();

            int numTests = xtests.Count();
            TestResult testResult = new TestResult(currentTest.testDriver, currentTest.author, currentTest.testDriver, currentTest.testCode); ;
            testResult.fullLogs = currentTest.fullLogs;
            for (int i = 0; i < numTests; ++i)
            { // retrieving  test logs and test pass status
                testResult.logs = xtests[i].Element("TestLogs").Value;
                testResult.testPass = Convert.ToBoolean(xtests[i].Element("TestPassed").Value);
                testResult.timeStamp = Convert.ToDateTime(xtests[i].Element("Timestamp").Value);
            }
            return testResult;
        }


        public string parseStringXml(string xml)
        {
            doc_ = XDocument.Parse(xml);
            if (doc_ == null)
                return null;
        //    XElement[] xtests = doc_.Descendants("author").ToArray();
            IEnumerable < XElement > temp= doc_.Root.Elements();
            string TestDriver = "xyz";

            foreach (XElement xelem in temp) {
                if (xelem.Name == "TestDriver")
                    TestDriver = xelem.Value;
            }
           
            return TestDriver;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //Main Function
        static void Main(string[] args)
        {
            List<string> listString = new List<string>();
            listString.Add("code1");
            TestResult test = new TestResult("Test1", "Shishir", "test1.dll", listString);
            test.testPass = true;
            test.logs = "The test went fine";


            XMLDatabase xmldatabase = new XMLDatabase();
            xmldatabase.createXMLResult(test);
            Test test1 = new Test();

            test1.author = "Shishir";
            test1.testDriver = "test1.dll";
            test1.testCode = listString;
            test1.fullLogs = "true";
            List<TestResult> resultslist = xmldatabase.getlogs(test1);

            foreach (TestResult t in resultslist)
            {

                t.show();
            }


        }
    }
}
