using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var myClass1 = new MyClass1();
            Console.WriteLine(myClass1.S);
            Console.WriteLine(string.IsNullOrEmpty(myClass1.S1));
            Console.ReadKey();
        }


        
    }

    class MyClass
    {
        public string S { get; set; }

        public string S1 { get; set; }
        public MyClass()
        {
            S = "d";
            S1 = "dsfgd";
        }
    }

    class MyClass1:MyClass
    {
        public MyClass1():base()
        {
            S = "22";
        }
    }

}
