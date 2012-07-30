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

        public override void CreateContext(string conStr)
        {
            context = new DataContext(conStr);
        }

        public override void SubmitChanges()
        {
            context.SubmitChanges();
        }

        public override IRepository<T> CreateRepository<T>()
        {
            return new Repository<T>(context);
        }
    }
}
