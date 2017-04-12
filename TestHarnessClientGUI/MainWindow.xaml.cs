///////////////////////////////////////////////////////////////////////
// MainWindow.xaml.cs - GUI backhand                                  //
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
 * This package has been designed to take care of all the actions that happens on the
 * GUI. It helps the client to fire all the task he wants with one click. This package
 * has been designed to support all 5 tabs of our GUI. Every button in those tab are
 * connected with a function here which will be called on clicking the button.
 *
 *Public Interfaces:
 *incomingMessages   //This function helps dequeue the incoming message queue.
 *displayLogFile         // To display data in a log file in GUI.
 *DisplayLogList     //To diplay log file list in GUI.
 *DisplayCodeList           //To display code file list in GUI.

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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Xml.Linq;
using System.Threading.Tasks;
namespace TestHarnessClientGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    using Client;
    using Message;
    using HiResTimer;
    public partial class MainWindow : Window
    {
        IAsyncResult cbResult;
        List<string> foundfiles = new List<string>();
        Client client;
        HiResTimer hTimer;
        bool fullLogsCheck=false;
        int counter = 0;
        //Constructor to incativate all button of UI apart from start harness
        public MainWindow()
        {
            InitializeComponent();
            textBox2.Text = "8085";
            textBox1.Text = "shishir";
            textBox3.Text = "8090";
            textBox4.Text = "8080";
            Browse.IsEnabled = false;
            Upload.IsEnabled = false;
            GetCodeList.IsEnabled = false;
            Download.IsEnabled = false;
            GetLogList.IsEnabled = false;
            GetLog.IsEnabled = false;
            getCodeList.IsEnabled = false;
            RunTest.IsEnabled = false;
            textBlock4.Text = "** Please Do keep the port number of Repository and Test Harness same as the one passed as their command line arguments in bat file(or dont change the current entry)";
        }

        //Dequeues the incoming message queue and acts based on type of message
        private void incomingMessages()
        {
            while (true)
            {
                Message incomingMsg = client.getIncomingMessage();
                if (incomingMsg.type == "Log_List")
                    DisplayLogList(incomingMsg);
                else if (incomingMsg.type == "Log_File")
                    displayLogFile(incomingMsg);
                else if ((incomingMsg.type == "Code_List"))
                    DisplayCodeList(incomingMsg);
                else if (incomingMsg.type == "Test_Result")
                    displayResults(incomingMsg);
            }
            
        }

        //To add file names in display
        void addResults(string file)
        {
            //listBox1.Items.Add(file);
            listBox_Copy.Items.Insert(0, file);
        }

        // To display data from log file in GUI
        public void displayLogFile(Message msg)
        {
            Console.WriteLine("Log file came back is");
            Console.WriteLine(msg.body);

         XDocument   doc_ = XDocument.Parse(msg.body);
            if (doc_ == null)
                return;

            IEnumerable<XElement> temp = doc_.Root.Elements();
          
            List<string> dataToDisplay = new List<string>();
            foreach (XElement xelem in temp)
            {
                if (xelem.Name == "TestDriver")
                    dataToDisplay.Add("Test Driver Name : "+ xelem.Value);

                if (xelem.Name == "TestPassed")
                    dataToDisplay.Add("Test Passes : " + xelem.Value);
                if(fullLogsCheck)
                if (xelem.Name == "TestLogs")
                    dataToDisplay.Add("Full Logs : " + xelem.Value);

            }

            foreach (string str in dataToDisplay) {

                if (Dispatcher.CheckAccess())
                    addResults(str);
                else
                    Dispatcher.Invoke(
                      new Action<string>(addResults),
                      System.Windows.Threading.DispatcherPriority.Background,
                      new string[] { str }
                    );

            }


        }

        // To display file name in GUI
        void addXMLFiles(string file)
        {
            //listBox1.Items.Add(file);
            listBox.Items.Insert(0, file);
        }
        //Function to display log list in GUI
        public void DisplayLogList(Message msg)
        {

            XDocument xdoc = XDocument.Parse(msg.body);

            Console.WriteLine();
            Console.WriteLine("List Received at Client");

            IEnumerable<XElement> FileNames = xdoc.Root.Descendants();
            foreach (XElement elem in FileNames)
            {
                if (Dispatcher.CheckAccess())
                    addXMLFiles(elem.Value);
                else
                    Dispatcher.Invoke(
                      new Action<string>(addXMLFiles),
                      System.Windows.Threading.DispatcherPriority.Background,
                      new string[] { elem.Value }
                    );
            }
        }
        //Displaying Result in Run Test Tab
        void addResultInRunTest(string file)
        {
            //listBox1.Items.Add(file);
            listBox5.Items.Insert(0, file);
        }
        // Function to help in displaying results
        void displayResults(Message msg)
        {
            // Console.WriteLine( msg.body);

            XDocument xdoc = XDocument.Parse(msg.body);
            IEnumerable<XElement> elems = xdoc.Root.Elements();
            string testResult = "false";
            string testlogs = null;
            List<string> dataToDisplay = new List<string>();
            foreach (XElement xelem in elems)
            {

                if (xelem.Name == "TestPassed")
                    dataToDisplay.Add("Test Passes : " + xelem.Value);
                   if (xelem.Name == "TestLogs")
                        dataToDisplay.Add("Full Logs : " + xelem.Value);
            }
            hTimer.Stop();
            dataToDisplay.Add("Time taken in Microseconds is " + hTimer.ElapsedMicroseconds);
            foreach (string str in dataToDisplay)
            {

                if (Dispatcher.CheckAccess())
                    addResultInRunTest(str);
                else
                    Dispatcher.Invoke(
                      new Action<string>(addResultInRunTest),
                      System.Windows.Threading.DispatcherPriority.Background,
                      new string[] { str }
                    );

            }
            Console.WriteLine(testResult);
            Console.WriteLine(testlogs);
        }

// Display files in GUI
        void addCodeFiles(string file)
        {
            //listBox1.Items.Add(file);
            listBox2.Items.Insert(0, file);
            listBox3.Items.Insert(0, file);
            listBox4.Items.Insert(0, file);

        }
        //Function to display code file list
        public void DisplayCodeList(Message msg)
        {

            XDocument xdoc = XDocument.Parse(msg.body);

            Console.WriteLine();
            Console.WriteLine(" code List Received at Client");

            IEnumerable<XElement> FileNames = xdoc.Root.Descendants();
            foreach (XElement elem in FileNames)
            {
                if (Dispatcher.CheckAccess())
                    addCodeFiles(elem.Value);
                else
                    Dispatcher.Invoke(
                      new Action<string>(addCodeFiles),
                      System.Windows.Threading.DispatcherPriority.Background,
                      new string[] { elem.Value }
                    );
            }
            
        }
        //Function to get xml file list
        private void button_Click(object sender, RoutedEventArgs e)
        {
            client.getXMLFileList();
        }

        //Function to transfer radio button value to class variable
        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            fullLogsCheck = sender == radioButton;
        }
        //Not using this 
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        // Function to show path in upload tab
        void showPath(string path)
        {
            textBox.Text = path;
        }
      
        // Add files in GUI display
        void addFile(string file)
        {
            listBox1.Items.Insert(0, file);
        }

        //Function to searcg files with a particular pattern
        void search(string path, string pattern)
        {
            /* called on asynch delegate's thread */
            if (Dispatcher.CheckAccess())
                showPath(path);
            else
                Dispatcher.Invoke(
                  new Action<string>(showPath),
                  System.Windows.Threading.DispatcherPriority.Background,
                  new string[] { path }
                );
            string[] files = System.IO.Directory.GetFiles(path, pattern);
            foreach (string file in files)
            {
                if (Dispatcher.CheckAccess())
                    addFile(file);
                else
                    Dispatcher.Invoke(
                      new Action<string>(addFile),
                      System.Windows.Threading.DispatcherPriority.Background,
                      new string[] { file }
                    );
            }
            string[] dirs = System.IO.Directory.GetDirectories(path);
            foreach (string dir in dirs)
                search(dir, pattern);
        }

        // Function to handle browse button in upload tab to select a folder
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Clear();
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            string path = AppDomain.CurrentDomain.BaseDirectory;
            dlg.SelectedPath = path;
            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                path = dlg.SelectedPath;
                string pattern = "*.dll";
                Action<string, string> proc = this.search;
                cbResult = proc.BeginInvoke(path, pattern, null, null);
            }


        }

        //Function to upload a file
        private void Upload_Click(object sender, RoutedEventArgs e)
        {

           client.sendFile( listBox1.SelectedItem.ToString());
            textBlock5.Text = "File uploaded.Please do check RepoFolder/Repository";

        }

        //Function to get code list
        private void GetCodeList_Click(object sender, RoutedEventArgs e)
        {
            listBox2.Items.Clear();
           
            client.getCodeList();
        }
        //Function to download file
        private void Download_Click(object sender, RoutedEventArgs e)
        {
            
            Console.WriteLine("File Download Request  went for" + listBox2.SelectedItem.ToString());
            string str = listBox2.SelectedItem.ToString();
            Task.Run(() => { client.getCodeFile(str); });
            textBlock7.Text = "File Downloaded.Please check LocalDownloadFolder";
        }
        // Not using this function
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //Function to get Log file data
        private void GetLog_Click(object sender, RoutedEventArgs e)
        {
            listBox_Copy.Items.Clear();
            client.getLogFile(listBox.SelectedItem.ToString());
        }

        //Not using the function
        private void listBox_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }
        //Function to get code list
        private void getCodeList_Click_1(object sender, RoutedEventArgs e)
        {
            listBox3.Items.Clear();
            listBox4.Items.Clear();

            client.getCodeList();

        }
        // Function to start test in test harness
        private void RunTest_Click(object sender, RoutedEventArgs e)
        {
            hTimer.Start();
            listBox5.Items.Clear();
            List<string> codeList = new List<string>();
            foreach (var item in listBox4.SelectedItems)
            {
                codeList.Add(item.ToString());
            }
            client.sendDoTestRequest(listBox3.SelectedItem.ToString(), codeList);

        }
// Function to start the  harness. Whole system will be in active till this function is called
        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            string author = textBox1.Text;
            string portNumber = textBox2.Text;
            string repoPortNumber = textBox4.Text;
            string TestHPortNumber = textBox3.Text;
            Console.WriteLine(String.IsNullOrEmpty(textBox4.Text));
            if (counter == 0 && !String.IsNullOrEmpty(textBox1.Text) && !String.IsNullOrEmpty(textBox2.Text) && !String.IsNullOrEmpty(textBox3.Text) && !String.IsNullOrEmpty(textBox4.Text)) {
                counter++;
          string url= "http://localhost:" + portNumber + "/BasicService";
            client = new Client(url, author);
                client.RepositoryUrl = "http://localhost:"+ textBox4.Text + "/BasicService";
                client.TestHarnessUrl= "http://localhost:" + textBox3.Text + "/BasicService";
                Thread th = new Thread(() => { incomingMessages(); });
            th.Start();
            hTimer = new HiResTimer();

            Browse.IsEnabled = true;
            Upload.IsEnabled = true;
            GetCodeList.IsEnabled = true;
            Download.IsEnabled = true;
            GetLogList.IsEnabled = true;
            GetLog.IsEnabled = true;
            getCodeList.IsEnabled = true;
            RunTest.IsEnabled = true;

            }
        }
    }
}
