using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace DomainModel.Abstract
{
    public abstract class AbstractContextDB
    {
        protected Dictionary<string, DataContext> contexts;

        public abstract void AddContext(string contextName, string conStr);
        public abstract void SubmitChanges();

        public abstract AbstractRepository<T> CreateRepository<T>(string contextName) where T:class;
    }
}
