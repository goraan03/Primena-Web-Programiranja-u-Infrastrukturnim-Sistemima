using System.Collections.Generic;
using System.Fabric;
using Microsoft.EntityFrameworkCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using TravelService.Data;
using TravelService.Services;

namespace TravelService
{
    internal sealed class TravelService : StatelessService
    {
        public TravelService(StatelessServiceContext context) : base(context) { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(context =>
                    new FabricTransportServiceRemotingListener(
                        context,
                        CreateTravelManager(context),
                        serializationProvider: new ServiceRemotingDataContractSerializationProvider()),
                    "V2_1Listener")
            };
        }

        private TravelManager CreateTravelManager(StatelessServiceContext context)
        {
            var configPackage = context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var connectionString = configPackage.Settings.Sections["ConnectionStrings"]
                .Parameters["DefaultConnection"].Value;

            var optionsBuilder = new DbContextOptionsBuilder<TravelDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());

            return new TravelManager(optionsBuilder.Options);
        }
    }
}