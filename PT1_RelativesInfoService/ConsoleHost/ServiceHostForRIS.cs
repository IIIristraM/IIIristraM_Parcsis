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
        private IContextDB context;

        public ServiceHostForRIS(Type serviceType, IContextDB context) : base(serviceType)
        {
            this.context = context;
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            Description.Behaviors.Add(new ServiceBehaviorForRIS(context));
            base.OnOpen(timeout);
        }
    }
}
