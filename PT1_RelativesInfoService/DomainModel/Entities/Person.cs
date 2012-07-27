using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;


//отражает основные сущности предметной области
namespace DomainModel.Entities
{
    //дублирует аналогичную таблицу БД
    [Table(Name = "Persones")]
    [DataContract(Namespace = "http://Person")]
    public class Person
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        [DataMember]
        public int PersonID {get; set;}
        [Column]
        [DataMember]
        public string PassportNumber { get; set; }
        [Column]
        [DataMember]
        public string FirstName { get; set; }
        [Column]
        [DataMember]
        public string SecondName { get; set; }
        [Column]
        [DataMember]
        public string ThirdName { get; set; }
        [Column]
        [DataMember]
        public string Sex { get; set; }
        [Column]
        [DataMember]
        public DateTime? DateOfBirth { get; set; }
        [Column]
        [DataMember]
        public string Address { get; set; }
    }

    //дублирует аналогичную таблицу БД
    [Table(Name = "Relationships")]
    [DataContract]
    public class Relationship
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        [DataMember]
        public int RelationshipID { get; set; }
        [Column]
        [DataMember]
        public int FirstPersonID { get; set; }
        [Column]
        [DataMember]
        public int SecondPersonID { get; set; }
        [Column]
        [DataMember]
        public string State { get; set; }
    }

    //вспомогательный класс, позволяет более понятно и удобно написать функционал
    [DataContract]
    public class Relative
    {
        //детализация по персоне родственника
        [DataMember]
        public Person Person { get; set; }
        //тип родственной связи
        [DataMember]
        public string RelationshipState { get; set; }

        public Relative()
        {
        }

        public Relative(Person p, string rs)
        {
            Person = p;
            RelationshipState = rs;
        }
    }
}
