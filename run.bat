cd TestHarness\bin\Debug\
start .\TestHarness.exe 8090 8080
cd ../../../Repository\bin\Debug\
start .\Repository.exe 8080
cd ../../../Test_Executive\bin\Debug\
start ./Test_Executive.exe 8095 8080 8090
cd ../../../TestHarnessClientGUI/bin\Debug\
start ./TestHarnessClientGUI.exe
@pause

