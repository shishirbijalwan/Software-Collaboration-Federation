/////////////////////////////////////////////////////////////////////
//  ITest.cs - Itest Interface                                     //
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
This package has been created to exploit the dynamic polymorphism behaviour of object oriented
languages. We will be assigning the ITest interface refernce variable with concrete class objects
at run time. It has two functions doTest() and getLogs()

Public Interface:
=================
getLogs                  // This method is receive the extra logs which are generated once the code has been tested.
doTest					 // In this function we create the object of Code To Be Tested and test it.


Build Process:
==============
Required Files: ITest.cs
Build Command: devenv TestHarness.sln /rebuild debug

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
    public interface ITest
    {
        bool test();
        string getLog();
    }
}
