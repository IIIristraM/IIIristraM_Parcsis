using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainModel.Abstract
{
    public interface IContextCreator
    {
        IContextDB CreateContext();
    }
}
