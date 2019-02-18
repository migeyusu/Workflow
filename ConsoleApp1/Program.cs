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
            myClass1.O=new MyClass1();
            var x = myClass1.O;
            x = new MyClass1() {S = "sdfsdg"};
            Console.WriteLine(myClass1.O.S);
            Console.ReadKey();
        }


        
    }

    class MyClass
    {
        public string S { get; set; }

        public string S1 { get; set; }

        public MyClass1 O { get; set; }
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
