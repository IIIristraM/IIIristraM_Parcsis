using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Collections.ObjectModel;
using DomainModel.Abstract;

namespace RelativesInfoService.Infrostructure
{
    public class ServiceBehaviorForRIS : IServiceBehavior
    {
        private IContextDB context;

        public ServiceBehaviorForRIS(IContextDB context)
        {
            this.context = context;
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher ed in cd.Endpoints)
                {
                    if (!ed.IsSystemEndpoint)
                    {
                        ed.DispatchRuntime.InstanceProvider = new InstanceProviderForRIS(context);
                    }
                }
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, 
                                         ServiceHostBase serviceHostBase,  
                                         Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters)
        { }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { }
    }
}
