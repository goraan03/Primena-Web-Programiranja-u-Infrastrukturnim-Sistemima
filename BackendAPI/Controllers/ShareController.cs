using QRCoder;
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
            var result = await _travelService.CreateShareTokenAsync(dto);

            var shareUrl = $"http://localhost:5173/share/{result.Token}";

            using var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(shareUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var qrBase64 = Convert.ToBase64String(qrCode.GetGraphic(20));

            return Ok(new { token = result.Token, accessType = result.AccessType, expiresAt = result.ExpiresAt, shareUrl, qrCode = qrBase64 });
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> GetByToken(string token)
        {
            try
            {
                var travel = await _travelService.GetTravelByShareTokenAsync(token);
                var info = await _travelService.GetShareTokenInfoAsync(token);
                return Ok(new { accessType = info.AccessType, travel });
            }
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