using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Abstract;
using DomainModel.Entities;
using System.Data.Linq;

//реализация интерфейса доступа к БД
namespace DomainModel.Concrete
{
    public class Repository<T>: IRepository<T> where T: class
    {
        //переменная для доступа к контексту, необходимо для подтверждения изменений в БД
        protected DataContext dc;
        protected Table<T> content;

        public Repository(DataContext dc)
        {
            this.dc = dc;
            content = dc.GetTable<T>();
        }

        public IQueryable<T> GetContent()
        {
            return content;
        }

        public void Insert(T row)
        {
            content.InsertOnSubmit(row);
        }

        public void Delete(T row)
        {
            content.DeleteOnSubmit(row);
        }
    }
}
