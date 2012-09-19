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
        private IContextCreator creator;

        public InstanceProviderForRIS(IContextCreator creator)
        {
            this.creator = creator;
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return new RISI.RelativesInfoService(creator);
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
