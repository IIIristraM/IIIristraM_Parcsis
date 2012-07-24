using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using RISI = RelativesInfoService.Implementations;

namespace ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(RISI.RelativesInfoService));
            host.Open();
            Console.WriteLine("Service ready...\npress any key to shut it down");
            Console.ReadKey();
            host.Close();
        }
    }
}
