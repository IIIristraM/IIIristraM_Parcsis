using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace DomainModel.Abstract
{
    public interface IContextDB
    {
        void SubmitChanges();
        IRepository<T> CreateRepository<T>() where T:class;
    }
}
