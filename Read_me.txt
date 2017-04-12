Hello TA/ Instructor,

1.On executing the run.bat file. It will open 4 consoles and a GUI. Please check the Text Executive console. I have demostrated the requirement in it.

2. Please find below the main folders:
   Repository base folder : RepoFolder
   Log folder:             Repository/Database
   Code Files:             Repository/Repository
   

   Test Drivers which you can use to run is in : SampleDrivers
     TestDriverOne.dll   with  codeToBeTested1.ddl
     TestDriverTwo.dll   with  codeToBeTested2.ddl
     TestDriverThree.dll with  codeToBeTested1.ddl


3. Please note that the GUI first tab takes port numbers. Please don't change the entries in it if you are not changing the port numbers in run.bat file. Please find the sequence of port number as argument for each exe.
 Repository.exe <Repository_Port_Number>
 TestHarness.exe <TestHarnessPort_Number> <Repository_Port_Number>
 TestExecutive.exe <Test_excutive_Port> <Repository_Port_Number> <TestHarnessPort_Number>


4. In case you are using your own test driver. Please make sure it implements test() and getLog() function.

Thanks,
Shishir Bijalwan