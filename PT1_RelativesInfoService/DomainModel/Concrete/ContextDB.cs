﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Abstract;
using System.Data.Linq;

namespace DomainModel.Concrete
{
    public class ContextDB : AbstractContextDB
    {
        public ContextDB()
        {
            contexts = new Dictionary<string, DataContext>();
        }

        public override void AddContext(string contextName, string conStr)
        {
            DataContext dc = new DataContext(conStr);
            contexts.Add(contextName, dc);
        }

        public override void SubmitChanges()
        {
            foreach (var c in contexts)
            {
                c.Value.SubmitChanges();
            }
        }

        public override AbstractRepository<T> CreateRepository<T>(string contextName)
        {
            DataContext dc;
            contexts.TryGetValue(contextName, out dc);
            return new Repository<T>(dc);
        }
    }
}
