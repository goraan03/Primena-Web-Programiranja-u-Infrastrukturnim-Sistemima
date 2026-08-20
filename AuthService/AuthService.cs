using AuthService.Data;
using AuthService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Collections.Generic;
using System.Fabric;

namespace AuthService
{
    internal sealed class AuthService : StatelessService
    {
        public AuthService(StatelessServiceContext context) : base(context) { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(context =>
                new FabricTransportServiceRemotingListener(
                context,
                CreateAuthManager(context),
                serializationProvider: new ServiceRemotingDataContractSerializationProvider()),
                "V2_1Listener")
            };
        }

        private AuthManager CreateAuthManager(StatelessServiceContext context)
        {
            var configPackage = context.CodePackageActivationContext.GetConfigurationPackageObject("Config");

            var connectionString = configPackage.Settings.Sections["ConnectionStrings"]
                .Parameters["DefaultConnection"].Value;

            var jwtSection = configPackage.Settings.Sections["JwtSettings"];
            var jwtSettings = new JwtSettings
            {
                Secret = jwtSection.Parameters["Secret"].Value,
                Issuer = jwtSection.Parameters["Issuer"].Value,
                Audience = jwtSection.Parameters["Audience"].Value,
                ExpirationHours = int.Parse(jwtSection.Parameters["ExpirationHours"].Value)
            };

            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());

            return new AuthManager(optionsBuilder.Options, jwtSettings);
        }
    }
}