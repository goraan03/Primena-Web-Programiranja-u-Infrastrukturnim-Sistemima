using System.Security.Claims;
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
    [Route("api/[controller]")]
    [Authorize]
    public class TravelController : ControllerBase
    {
        private readonly ITravelService _travelService;
        private readonly INotificationService _notificationService;

        public TravelController()
        {
            var serializationProvider = new ServiceRemotingDataContractSerializationProvider();
            var clientFactory = new FabricTransportServiceRemotingClientFactory(serializationProvider: serializationProvider);
            var proxyFactory = new ServiceProxyFactory(c => clientFactory);

            _travelService = proxyFactory.CreateServiceProxy<ITravelService>(
                new Uri("fabric:/TravelPlannerBackend/TravelService"));
            _notificationService = proxyFactory.CreateServiceProxy<INotificationService>(new Uri("fabric:/TravelPlannerBackend/NotificationService"));
            _travelService = proxyFactory.CreateServiceProxy<ITravelService>(new Uri("fabric:/TravelPlannerBackend/TravelService"));
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var travels = await _travelService.GetAllTravelsAsync(CurrentUserId);
            return Ok(travels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try { return Ok(await _travelService.GetTravelByIdAsync(id)); }
            catch (Exception ex) { return NotFound(new { error = ex.InnerException?.Message ?? ex.Message }); }
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create(TravelDto dto)
        {
            dto.UserId = CurrentUserId;

            TravelDto created;
            try
            {
                created = await _travelService.CreateTravelAsync(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.InnerException?.Message ?? ex.Message });
            }

            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                await _notificationService.SendTravelCreatedEmailAsync(email, created.Name);
            }
            catch
            {
                
            }

            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TravelDto dto)
        {
            dto.Id = id;
            try { return Ok(await _travelService.UpdateTravelAsync(dto)); }
            catch (Exception ex) { return BadRequest(new { error = ex.InnerException?.Message ?? ex.Message }); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _travelService.DeleteTravelAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpGet("{travelId}/destinations")]
        public async Task<IActionResult> GetDestinations(int travelId) =>
            Ok(await _travelService.GetDestinationsAsync(travelId));

        [HttpPost("{travelId}/destinations")]
        public async Task<IActionResult> AddDestination(int travelId, DestinationDto dto)
        {
            dto.TravelId = travelId;
            return Ok(await _travelService.CreateDestinationAsync(dto));
        }

        [HttpGet("{travelId}/activities")]
        public async Task<IActionResult> GetActivities(int travelId) =>
            Ok(await _travelService.GetActivitiesAsync(travelId));

        [HttpPost("{travelId}/activities")]
        public async Task<IActionResult> AddActivity(int travelId, ActivityDto dto)
        {
            dto.TravelId = travelId;
            return Ok(await _travelService.CreateActivityAsync(dto));
        }

        [HttpGet("{travelId}/expenses")]
        public async Task<IActionResult> GetExpenses(int travelId) =>
            Ok(await _travelService.GetExpensesAsync(travelId));

        [HttpPost("{travelId}/expenses")]
        public async Task<IActionResult> AddExpense(int travelId, ExpenseDto dto)
        {
            dto.TravelId = travelId;
            return Ok(await _travelService.CreateExpenseAsync(dto));
        }

        [HttpPut("destinations/{id}")]
        public async Task<IActionResult> UpdateDestination(int id, DestinationDto dto)
        { 
            dto.Id = id; return Ok(await _travelService.UpdateDestinationAsync(dto)); 
        }

        [HttpDelete("destinations/{id}")]
        public async Task<IActionResult> DeleteDestination(int id)
        {
            return await _travelService.DeleteDestinationAsync(id) ? NoContent() : NotFound();
        }

        [HttpPut("activities/{id}")]
        public async Task<IActionResult> UpdateActivity(int id, ActivityDto dto)
        {
            dto.Id = id; return Ok(await _travelService.UpdateActivityAsync(dto));
        }

        [HttpDelete("activities/{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        { 
            return await _travelService.DeleteActivityAsync(id) ? NoContent() : NotFound();
        }

        [HttpPut("expenses/{id}")]
        public async Task<IActionResult> UpdateExpense(int id, ExpenseDto dto)
        {
            dto.Id = id; return Ok(await _travelService.UpdateExpenseAsync(dto));
        }

        [HttpDelete("expenses/{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            return await _travelService.DeleteExpenseAsync(id) ? NoContent() : NotFound();
        }

        [HttpGet("{travelId}/budget-summary")]
        public async Task<IActionResult> GetBudgetSummary(int travelId) => 
            Ok(await _travelService.GetBudgetSummaryAsync(travelId));

        [HttpGet("{travelId}/checklist")]
        public async Task<IActionResult> GetChecklist(int travelId) => Ok(await _travelService.GetChecklistAsync(travelId));

        [HttpPost("{travelId}/checklist")]
        public async Task<IActionResult> AddChecklistItem(int travelId, ChecklistItemDto dto)
        { dto.TravelId = travelId; return Ok(await _travelService.CreateChecklistItemAsync(dto)); }

        [HttpPut("checklist/{id}/toggle")]
        public async Task<IActionResult> ToggleChecklistItem(int id) => Ok(await _travelService.ToggleChecklistItemAsync(id));

        [HttpDelete("checklist/{id}")]
        public async Task<IActionResult> DeleteChecklistItem(int id) =>
            await _travelService.DeleteChecklistItemAsync(id) ? NoContent() : NotFound();

        [HttpGet("travel-plans")]
        public async Task<IActionResult> GetAllTravelPlans() => Ok(await _travelService.GetAllTravelsForAdminAsync());
    }
}