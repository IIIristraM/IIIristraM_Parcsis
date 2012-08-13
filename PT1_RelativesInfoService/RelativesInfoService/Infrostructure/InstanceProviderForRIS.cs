using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.ServiceModel.Channels;
using RelativesInfoService.Implementations;
using DomainModel.Concrete;
using DomainModel.Abstract;
using RISI = RelativesInfoService.Implementations;

namespace RelativesInfoService.Infrostructure
{
    public class InstanceProviderForRIS : IInstanceProvider
    {
        private IContextDB context;

        public InstanceProviderForRIS(IContextDB context)
        {
            this.context = context;
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return new RISI.RelativesInfoService(context);
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return this.GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }
    }
}
