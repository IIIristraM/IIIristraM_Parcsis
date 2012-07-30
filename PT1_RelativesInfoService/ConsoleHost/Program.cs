using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using RISI = RelativesInfoService.Implementations;
using DomainModel.Concrete;

namespace ConsoleHost
{
    /// <summary>
    ///хостинг сервиса
    ///при отладке проект должен быть установлен как StartUp
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(RISI.RelativesInfoService<ContextDB>));
            host.Open();
            Console.WriteLine("Service ready...\npress any key to shut it down");
            Console.ReadKey();
            host.Close();
        }
    }
}
