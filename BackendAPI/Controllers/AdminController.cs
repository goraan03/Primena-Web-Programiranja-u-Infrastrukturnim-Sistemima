using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using TravelPlanner.Contracts.Interfaces;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN")]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AdminController()
        {
            var serializationProvider = new ServiceRemotingDataContractSerializationProvider();
            var clientFactory = new FabricTransportServiceRemotingClientFactory(serializationProvider: serializationProvider);
            var proxyFactory = new ServiceProxyFactory(c => clientFactory);
            _authService = proxyFactory.CreateServiceProxy<IAuthService>(new Uri("fabric:/TravelPlannerBackend/AuthService"));
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers() => Ok(await _authService.GetAllUsersAsync());

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id) =>
            await _authService.DeleteUserAsync(id) ? NoContent() : NotFound();
    }
}