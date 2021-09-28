using ManagerThread;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TestManagerThread // Note: actual namespace depends on the project name.
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new TestTask(10).Run();

            Console.WriteLine(" ManagerThread  Hello, World!");


        }
    }
}

