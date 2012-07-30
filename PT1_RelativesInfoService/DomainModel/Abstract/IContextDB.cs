using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace DomainModel.Abstract
{
    public abstract class IContextDB
    {
        public abstract void CreateContext(string conStr);
        public abstract void SubmitChanges();
        public abstract IRepository<T> CreateRepository<T>() where T:class;
    }
}
