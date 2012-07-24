using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DomainModel.Concrete;
using DomainModel.Abstract;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using DomainModel.Entities;
using System.Xml.Serialization;
using System.Xml;
using System.Threading;

//тесты сервиса для NUnit
namespace NUnit_Tests
{
    [TestFixture]
    public class Tests
    {
        //строка соединения с БД
        private static string ConStr = @"Server =.\SQLEXPRESS; Database = PT1_DB; Trusted_Connection = yes;";
        //экземпляр БД
        private static IPersonesRepository PersonesDB = new PersonesRepository(ConStr);
        //формат отправляемых сервису и возвращаемых сервисом данных
        private static string dataFormat = "json";
        //режим работы для методов AddRelative и UpdateRelationshipState
        private static string mode = "auto";

        //тесты для функции сервиса GetRelativesList
        public static void TestGetRelativesList()
        {
            //настройка сериализации типа DateTime в JSON
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            //адресс функции сервиса
            string operationUrl = "http://localhost:8732/Design_Time_Addresses/RESTService/GetRelativesList";
            //параметры для функции
            string pasportNumber = "3610123456";
            Person filter = new Person { Adresse = "", 
                                         DateOfBirth = new DateTime(), 
                                         FirstName = "Igor", 
                                         PasportNumber = "", 
                                         PersonID = 0, 
                                         SecondName = "", 
                                         Sex = "", 
                                         ThirdName = "" };

            //создание и отправка запроса сервису
            #region Send Request
            //описание Url запроса
            WebRequest req = WebRequest.Create(operationUrl + "?pasportNumber=" + pasportNumber);
            //описание метода запрса
            req.Method = "POST";
            req.Timeout = 12000;
            //описание формата обмена данными
            req.ContentType = "application/" + dataFormat;
            //конвертация параметров из С# в соответствующий формат
            #region Create params
            string filterStr = "";
            if (dataFormat == "json")
            {
                filterStr = "{\"filter\":"+ JsonConvert.SerializeObject(filter, settings) + "}";
            }
            else
            {
                filterStr = "<GetRelativesList>" + 
                            "<filter xmlns:a=\"http://Person\">" +
                            "<a:Adresse>" + filter.Adresse + "</a:Adresse>" +
                            "<a:DateOfBirth>" + filter.DateOfBirth.Value.ToString("o") + "</a:DateOfBirth>" +
                            "<a:FirstName>" + filter.FirstName + "</a:FirstName>" +
                            "<a:PasportNumber>" + filter.PasportNumber + "</a:PasportNumber>" +
                            "<a:PersonID>" + filter.PersonID + "</a:PersonID>" +
                            "<a:SecondName>" + filter.SecondName + "</a:SecondName>" +
                            "<a:Sex>" + filter.Sex + "</a:Sex>" +
                            "<a:ThirdName>" + filter.ThirdName + "</a:ThirdName>" +
                            "</filter>" +
                            "</GetRelativesList>";

                /*StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                XmlDocument doc = new XmlDocument();
                XmlElement operationEl = doc.CreateElement("GetRelativesList");
                XmlElement filterEl = doc.CreateElement("filter");
                filterEl.SetAttribute("xmlns:a", "http://Person");
                XmlElement adresseEl = doc.CreateElement("a", "Adresse", "http://Person");
                adresseEl.InnerText = filter.Adresse;
                XmlElement dobEl = doc.CreateElement("DateOfBirth");
                dobEl.InnerText = filter.DateOfBirth.ToUniversalTime().ToString();
                XmlElement firstNameEl = doc.CreateElement("a", "FirstName", "http://Person");
                firstNameEl.InnerText = filter.FirstName;
                XmlElement pasportNumberEl = doc.CreateElement("PasportNumber");
                pasportNumberEl.InnerText = filter.PasportNumber;
                XmlElement personIDEl = doc.CreateElement("PersonID");
                personIDEl.InnerText = filter.PersonID.ToString();
                XmlElement secondNameEl = doc.CreateElement("SecondName");
                secondNameEl.InnerText = filter.SecondName;
                XmlElement sexEl = doc.CreateElement("Sex");
                sexEl.InnerText = filter.Sex;
                XmlElement thirdNameEl = doc.CreateElement("ThirdName");
                thirdNameEl.InnerText = filter.ThirdName;
                filterEl.AppendChild(adresseEl);
                filterEl.AppendChild(dobEl);
                filterEl.AppendChild(firstNameEl);
                filterEl.AppendChild(pasportNumberEl);
                filterEl.AppendChild(personIDEl);
                filterEl.AppendChild(secondNameEl);
                filterEl.AppendChild(sexEl);
                filterEl.AppendChild(thirdNameEl);
                operationEl.AppendChild(filterEl);
                XmlTextWriter xtw = new XmlTextWriter(sw);
                doc.AppendChild(operationEl);
                doc.WriteContentTo(xtw);
                xtw.Close();
                filterStr = sb.ToString();*/
            }
            Console.WriteLine(filterStr);
            #endregion
            byte[] data = Encoding.GetEncoding(1251).GetBytes(filterStr);
            req.ContentLength = data.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(data, 0, data.Length);
            sendStream.Close();
            #endregion
            //получение и обработка ответа сервиса
            #region Get Response
            WebResponse resp = req.GetResponse();
            System.IO.Stream stream = resp.GetResponseStream();
            StreamReader sr = new System.IO.StreamReader(stream);
            List<Relative> result = new List<Relative>();
            //конвертация строки ответа в объект C# типа List<Relative>
            if (dataFormat == "json")
            {
                string s = sr.ReadToEnd();
                Console.WriteLine(s);
                result = JsonConvert.DeserializeObject<List<Relative>>(s, settings);
            }
            else
            {
                string s = sr.ReadToEnd();
                Console.WriteLine(s);
                StringReader strR = new StringReader(s);

                string root = "ArrayOfRelative";
                XmlRootAttribute xra = new XmlRootAttribute();
                xra.ElementName = root;
                xra.Namespace = "http://schemas.datacontract.org/2004/07/DomainModel.Entities";
                result = (List<Relative>)(new XmlSerializer(result.GetType(), xra).Deserialize(strR));
            }
            #endregion

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.First().RelationshipState == "father");
        }
        //тесты для функции сервиса AddRelative
        public static void TestAddRelative()
        {
            dataFormat = "json";
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;

            string operationUrl = "http://localhost:8732/Design_Time_Addresses/RESTService/AddRelative";
            string pasportNumber = "3610123456";
            Person person = new Person
            {
                Adresse = "",
                DateOfBirth = null,
                FirstName = "Julia",
                PasportNumber = "3610112233",
                PersonID = 0,
                SecondName = "Glazacheva",
                Sex = "female",
                ThirdName = "Igorevna"
            };
            string state = "sister";
            Relative relative = new Relative(person, state);

            
            #region Create params
            string relativeStr = "";
            relativeStr = "{\"relative\":" + JsonConvert.SerializeObject(relative, settings) + "}";
            Console.WriteLine(relativeStr);
            byte[] data = Encoding.GetEncoding(1251).GetBytes(relativeStr);          
            #endregion

            int i = 0;
            int[] personesCount = new int[2];
            int[] relationshipCount = new int[2];
            Stream stream = null;
            WebRequest req = null;
            WebResponse resp = null;
            StreamReader sr = null;
            while (i < 2)
            {
                #region Create Request
                req = WebRequest.Create(operationUrl + "/" + pasportNumber + "/" + mode);
                req.Method = "POST";
                req.Timeout = 12000;
                req.ContentType = "application/" + dataFormat;
                req.ContentLength = data.Length;
                #endregion

                #region Send Request
                Console.WriteLine("iter - " + i);
                stream = req.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
                #endregion

                #region Get Response
                resp = req.GetResponse();
                stream = resp.GetResponseStream();
                sr = new System.IO.StreamReader(stream);
                string s = sr.ReadToEnd();
                int result = Int32.Parse(s);
                #endregion

                Assert.IsTrue(result == 1);
                Console.WriteLine(result);

                var sqlP = from p in PersonesDB.Persones select p.PersonID;
                var sqlR = from r in PersonesDB.Relationships select r.RelationshipID;
                personesCount[i] = sqlP.Count();
                relationshipCount[i] = sqlR.Count();
                i++;
            }

            Assert.IsTrue(personesCount[0] == personesCount[1]);
            Assert.IsTrue(relationshipCount[0] == relationshipCount[1]);
        }
        //тесты для функции сервиса DeleteRelative
        public static void TestDeleteRelative()
        {
            dataFormat = "json";
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;

            string operationUrl = "http://localhost:8732/Design_Time_Addresses/RESTService/DeleteRelative";
            string pasportNumber = "3610123456";
            string relPasportNumber = "3610112233";

            int i = 0;
            int[] personesCount = new int[2];
            int[] relationshipCount = new int[2];
            Stream stream = null;
            WebRequest req = null;
            WebResponse resp = null;
            StreamReader sr = null;
            while (i < 2)
            {
                #region Create Request
                req = WebRequest.Create(operationUrl + "/" + pasportNumber + "/" + relPasportNumber);
                req.ContentType = "application/" + dataFormat;
                #endregion

                #region Send Request
                Console.WriteLine("iter - " + i);
                #endregion

                #region Get Response
                resp = req.GetResponse();
                stream = resp.GetResponseStream();
                sr = new System.IO.StreamReader(stream);
                string s = sr.ReadToEnd();
                Console.WriteLine(s);
                int result = Int32.Parse(s);
                #endregion

                if (i == 0) { Assert.IsTrue(result == 1); } else { Assert.IsTrue(result == 0); }

                var sqlP = from p in PersonesDB.Persones select p.PersonID;
                var sqlR = from r in PersonesDB.Relationships select r.RelationshipID;
                personesCount[i] = sqlP.Count();
                relationshipCount[i] = sqlR.Count();
                i++;
            }

            Assert.IsTrue(personesCount[0] == personesCount[1]);
            Assert.IsTrue(relationshipCount[0] == relationshipCount[1]);

        }
        //тесты для функции сервиса UpdateRelative
        public static void TestUpdateRelative()
        {
            dataFormat = "json";
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;

            string operationUrl = "http://localhost:8732/Design_Time_Addresses/RESTService/UpdateRelative";
            string pasportNumber = "3610123456";
            string relPasportNumber = "3215654321";
            Person updatedRelative = new Person
            {
                Adresse = "",
                DateOfBirth = null,
                FirstName = "Vasily",
                PasportNumber = "",
                PersonID = 0,
                SecondName = "",
                Sex = "",
                ThirdName = ""
            };

            int i = 0;
            Stream stream = null;
            WebRequest req = null;
            WebResponse resp = null;
            StreamReader sr = null;
            while (i < 2)
            {
                if (i > 0) updatedRelative.FirstName = "Igor";

                #region Create params
                string updatedRelativeStr = "";
                updatedRelativeStr = "{\"updatedRelative\":" + JsonConvert.SerializeObject(updatedRelative, settings) + "}";
                Console.WriteLine(updatedRelativeStr);
                byte[] data = Encoding.GetEncoding(1251).GetBytes(updatedRelativeStr);
                #endregion

                #region Create Request
                req = WebRequest.Create(operationUrl + "/" + pasportNumber + "/" + relPasportNumber);
                req.Method = "POST";
                req.Timeout = 12000;
                req.ContentType = "application/" + dataFormat;
                req.ContentLength = data.Length;
                #endregion

                #region Send Request
                Console.WriteLine("iter - " + i);
                stream = req.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
                #endregion

                #region Get Response
                resp = req.GetResponse();
                stream = resp.GetResponseStream();
                sr = new StreamReader(stream);
                string s = sr.ReadToEnd();
                int result = Int32.Parse(s);
                #endregion

                Assert.IsTrue(result == 1);

                var sql = from p in PersonesDB.Persones where p.PasportNumber == relPasportNumber select p.FirstName;
                Console.WriteLine(sql.First());
                Console.WriteLine(updatedRelative.FirstName);
                Assert.IsTrue(sql.First() == updatedRelative.FirstName);

                i++;
            }
        }
        //тесты для функции сервиса UpdateRelationshipState
        public static void TestUpdateRelationshipState()
        {
            dataFormat = "json";
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;

            string operationUrl = "http://localhost:8732/Design_Time_Addresses/RESTService/UpdateRelationshipState";
            string pasportNumber = "3610123456";
            string relPasportNumber = "3610112233";

            string updatedState = "aunt";

            int i = 0;
            int[] relationshipCount = new int[2];
            Stream stream = null;
            WebRequest req = null;
            WebResponse resp = null;
            StreamReader sr = null;

            while (i < 2)
            {
                if (i > 0) updatedState = "sister";

                #region Create Request
                req = WebRequest.Create(operationUrl + "/" + pasportNumber + "/" + relPasportNumber + "/" + updatedState + "/" + mode);
                req.ContentType = "application/" + dataFormat;
                #endregion

                #region Send Request
                Console.WriteLine("iter - " + i);
                #endregion

                #region Get Response
                resp = req.GetResponse();
                stream = resp.GetResponseStream();
                sr = new System.IO.StreamReader(stream);
                string s = sr.ReadToEnd();
                Console.WriteLine(s);
                int result = Int32.Parse(s);
                #endregion

                Assert.IsTrue(result == 1);

                var sql = from r in PersonesDB.Relationships
                          from p in PersonesDB.Persones
                          where ((p.PasportNumber == relPasportNumber) &&
                                ((p.PersonID == r.FirstPersonID) || (p.PersonID == r.SecondPersonID)))
                          select r.State;

                if (i == 0)
                {
                    Assert.IsTrue(sql.Count() == 1);
                    Assert.IsTrue(sql.First() == "aunt");
                }
                else
                {
                    Assert.IsTrue(sql.Count() == 6);
                }

                i++;
            }


        }

        //управление порядком тестирования
        [Test]
        public static void GlobalTest()
        {
            Console.WriteLine("Start TestGetRelativesList");
            Console.WriteLine("XML");
            dataFormat = "xml";
            TestGetRelativesList();
            Console.WriteLine("JSON");
            dataFormat = "json";
            TestGetRelativesList();
            Console.WriteLine();
            Console.WriteLine("Start TestAddRelative");
            TestAddRelative();
            Console.WriteLine();
            Console.WriteLine("Start TestUpdateRelative");
            TestUpdateRelative();
            Console.WriteLine();
            Console.WriteLine("Start TestUpdateRelationshipState");
            TestUpdateRelationshipState();
            Console.WriteLine();
            Console.WriteLine("Start TestDeleteRelative");
            TestDeleteRelative();
        }
    }
}
