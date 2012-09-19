using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using DomainModel.Abstract;
using RelativesInfoService.Infrostructure;

namespace ConsoleHost
{
    public class ServiceHostForRIS : ServiceHost
    {
        private IContextCreator creator;

        public ServiceHostForRIS(Type serviceType, IContextCreator creator)
            : base(serviceType)
        {
            this.creator = creator;
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            Description.Behaviors.Add(new ServiceBehaviorForRIS(creator));
            base.OnOpen(timeout);
        }
    }
}
