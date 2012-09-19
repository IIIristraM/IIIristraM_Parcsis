using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Abstract;

namespace DomainModel.Concrete
{
    public class ContextCreator: IContextCreator
    {
        private string conStr;

        public ContextCreator(string conStr)
        {
            this.conStr = conStr;
        }

        public IContextDB CreateContext()
        {
            return new ContextDB(conStr);
        }
    }
}
