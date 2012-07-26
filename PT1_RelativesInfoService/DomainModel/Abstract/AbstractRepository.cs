using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Entities;
using System.Data.Linq;

//интерфейс для работы с БД
namespace DomainModel.Abstract
{
    public abstract class AbstractRepository<T> where T:class
    {
        //переменная для доступа к контексту, необходимо для подтверждения изменений в БД
        protected DataContext dc;
        protected Table<T> content;
        //свойство для доступа к таблице БД
        public IQueryable<T> Content { get; }
        public abstract void Insert(T row);
        public abstract void Delete(T row);
    }
}
