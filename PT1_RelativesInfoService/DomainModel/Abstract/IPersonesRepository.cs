using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Entities;
using System.Data.Linq;

namespace DomainModel.Abstract
{
    public interface IPersonesRepository
    {
        IQueryable<Person> Persones { get; }
        IQueryable<Relationship> Relationships { get; }
        DataContext DC { get; }
    }
}
