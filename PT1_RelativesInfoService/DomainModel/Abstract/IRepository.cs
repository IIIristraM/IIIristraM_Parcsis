using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Entities;
using System.Data.Linq;

//интерфейс для работы с БД
namespace DomainModel.Abstract
{
    public interface IRepository<T> where T:class
    {
        //свойство для доступа к таблице БД
        IQueryable<T> GetContent();
        void Insert(T row);
        void Delete(T row);
    }
}
