using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Abstract;
using DomainModel.Entities;
using System.Data.Linq;

namespace DomainModel.Concrete
{
    public class PersonesRepository : IPersonesRepository
    {
        private Table<Person> persones;
        private Table<Relationship> relationships;
        private DataContext dc;

        public PersonesRepository(string conectionnString)
        {
            dc = new DataContext(conectionnString);
            persones = DC.GetTable<Person>();
            relationships = DC.GetTable<Relationship>();
        }

        public IQueryable<Person> Persones
        {
            get { return persones; }
        }

        public DataContext DC 
        {
            get { return dc; } 
        }

        public IQueryable<Relationship> Relationships
        {
            get { return relationships; }
        }
        
    }
}
