using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Entities;
using System.Data.Linq;

//интерфейс для работы с БД
namespace DomainModel.Abstract
{
    public interface IPersonesRepository
    {
        //свойство для доступа к аналогичной таблице БД
        IQueryable<Person> Persones { get; }
        //свойство для доступа к аналогичной таблице БД
        IQueryable<Relationship> Relationships { get; }
        //свойство для доступа к контексту, необходимо для подтверждения изменений в БД
        DataContext DC { get; }
    }
}
