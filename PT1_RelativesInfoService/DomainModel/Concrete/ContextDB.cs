using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Abstract;
using System.Data.Linq;

namespace DomainModel.Concrete
{
    public class ContextDB : IContextDB
    {
        DataContext context;

        public ContextDB(string conStr)
        {
            context = new DataContext(conStr);
        }

        public void SubmitChanges()
        {
            context.SubmitChanges();
        }

        public IRepository<T> CreateRepository<T>() where T : class
        {
            return new Repository<T>(context);
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
