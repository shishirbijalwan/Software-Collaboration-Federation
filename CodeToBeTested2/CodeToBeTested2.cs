/////////////////////////////////////////////////////////////////////
//  CodeToBeTested2.cs - It is the code that has to be tested      //
//  ver 1.0                                                        //
//  Language:      Visual C#  2015                                 //
//  Platform:      HP Pallvilion, Windows 7                        //
//  Application:   TestHarness, FL16                               //
//  Author:        Shishir Bijalwan, Syracuse University           //
//                 (979) 587-6340, sbijalwa@syr.edu                //
/////////////////////////////////////////////////////////////////////

/*
Module Operations:
==================
This package has be creted to demonstarte the working of the different test driver in isolated domains.
So this code has been developed for the TestDriver2. It has a multiplication function which take two 
integers as input and return the result.

Public Interface:
=================
multiplyIntFunction                  // Function for the multiplication of two integers.


Build Process:
==============
Required files
- CodeToBeTested2.cs
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

namespace CodeNamespace
{
    public class CodeToBeTested2
    {
        //Function for the multiplication of two integers
        public int multiplyIntFunction(int a, int b)
        {
            int x = a * b;
            return x;
        }
        //Main function
        static void Main(string[] args)
        {
            CodeToBeTested2 code = new CodeToBeTested2();
            Console.WriteLine(code.multiplyIntFunction(2, 2));
        }
    }
}
