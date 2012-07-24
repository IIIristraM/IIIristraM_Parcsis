using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace DomainModel.Entities
{
    [Table(Name = "Persones")]
    [DataContract(Namespace = "http://Person")]
    public class Person
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        [DataMember]
        public int PersonID {get; set;}
        [Column]
        [DataMember]
        public string PasportNumber { get; set; }
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
        public string Adresse { get; set; }
    }

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

    [DataContract]
    public class Relative
    {
        [DataMember]
        public Person Person { get; set; }
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
