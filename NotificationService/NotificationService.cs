using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using NotificationService.Services;

namespace NotificationService
{
    internal sealed class NotificationService : StatefulService
    {
        public NotificationService(StatefulServiceContext context) : base(context) { }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(context =>
                    new FabricTransportServiceRemotingListener(
                        context,
                        new NotificationManager(this.StateManager),
                        serializationProvider: new ServiceRemotingDataContractSerializationProvider()),
                    "V2_1Listener")
            };
        }
    }
}