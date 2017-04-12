/////////////////////////////////////////////////////////////////////
//  CodeToBeTested1.cs - It is the code that has to be tested      //
//  ver 1.0                                                        //
//  Language:      Visual C#  2015                                 //
//  Platform:      HP Pallvilion, Windows 7                        //
//  Application:   TestHarness , FL16                              //
//  Author:        Shishir Bijalwan, Syracuse University           //
//                 (979) 587-6340, sbijalwa@syr.edu                //
/////////////////////////////////////////////////////////////////////

/*
Module Operations:
==================
This package has be creted to demonstarte the working of the different test driver in isolated domains.
So this code has been developed for the TestDriver1. It has a addition function which take two 
integers as input and return the result.

Public Interface:
=================
addIntFunction                  // Function for the addition of two integers.


Build Process:
==============
Required files
- CodeToBeTested1.cs
 * Build Command: devenv TestHarness.sln /rebuild debug

Maintenance History:
====================
ver 1.0

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeTobeTested
{
    public class CodeToBeTested1
    {
        //This function is for the addition of two integers
        public int addIntFunction(int a, int b)
        {
            int x = a + b;
            return x;
        }
        //Main Function
        static void Main(string[] args)
        {
            CodeToBeTested1 codeToBeTested = new CodeToBeTested1();
            int x = codeToBeTested.addIntFunction(1, 2);
            Console.WriteLine(x);
        }
    }
}
