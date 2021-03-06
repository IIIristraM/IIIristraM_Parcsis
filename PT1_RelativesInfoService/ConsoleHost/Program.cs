﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using RISI = RelativesInfoService.Implementations;
using DomainModel.Concrete;
using System.Configuration;

namespace ConsoleHost
{
    /// <summary>
    ///хостинг сервиса
    ///при отладке проект должен быть усiтановлен как StartUp
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //устанавливается соеденение с БД и запуск сервиса
            string conStr = ConfigurationManager.ConnectionStrings["PT1_DB"].ConnectionString;
            ContextCreator creator = new ContextCreator(conStr); 

            using (ServiceHostForRIS host = new ServiceHostForRIS(typeof(RISI.RelativesInfoService), creator))
            {
                host.Open();
                Console.WriteLine("Service ready...\npress any key to shut it down");
                Console.ReadKey();
                host.Close();
            };

        }
    }
}
