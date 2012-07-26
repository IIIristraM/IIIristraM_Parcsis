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
    public class Repository<T>: AbstractRepository<T> where T: class
    {

        public Repository(DataContext dc)
        {
            this.dc = dc;
            content = dc.GetTable<T>();
        }

        public IQueryable<T> Content
        {
            get { return content; }
        }

        public override void Insert(T row)
        {
            content.InsertOnSubmit(row);
        }

        public override void Delete(T row)
        {
            content.DeleteOnSubmit(row);
        }
    }
}
