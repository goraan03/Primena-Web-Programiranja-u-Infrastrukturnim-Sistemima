using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using TravelPlanner.Contracts.Interfaces;
using TravelPlanner.Contracts.Models;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/share")]
    public class ShareController : ControllerBase
    {
        private readonly ITravelService _travelService;

        public ShareController()
        {
            var serializationProvider = new ServiceRemotingDataContractSerializationProvider();
            var clientFactory = new FabricTransportServiceRemotingClientFactory(serializationProvider: serializationProvider);
            var proxyFactory = new ServiceProxyFactory(c => clientFactory);
            _travelService = proxyFactory.CreateServiceProxy<ITravelService>(new Uri("fabric:/TravelPlannerBackend/TravelService"));
        }

        [Authorize]
        [HttpPost("{travelId}")]
        public async Task<IActionResult> CreateShareToken(int travelId, CreateShareTokenDto dto)
        {
            dto.TravelId = travelId;
            return Ok(await _travelService.CreateShareTokenAsync(dto));
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> GetByToken(string token)
        {
            try { return Ok(await _travelService.GetTravelByShareTokenAsync(token)); }
            catch (Exception ex) { return NotFound(new { error = ex.InnerException?.Message ?? ex.Message }); }
        }

        [HttpPut("{token}")]
        public async Task<IActionResult> UpdateByToken(string token, TravelDto dto)
        {
            try { return Ok(await _travelService.UpdateTravelByShareTokenAsync(token, dto)); }
            catch (Exception ex) { return BadRequest(new { error = ex.InnerException?.Message ?? ex.Message }); }
        }

        [Authorize]
        [HttpDelete("{token}")]
        public async Task<IActionResult> Revoke(string token) =>
            await _travelService.RevokeShareTokenAsync(token) ? NoContent() : NotFound();
    }
}