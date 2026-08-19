using Microsoft.ServiceFabric.Services.Remoting;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelPlanner.Contracts.Models;

namespace TravelPlanner.Contracts.Interfaces
{
    public interface ITravelService : IService
    {
        // Travel CRUD
        Task<List<TravelDto>> GetAllTravelsAsync(int userId);
        Task<TravelDto> GetTravelByIdAsync(int travelId);
        Task<TravelDto> CreateTravelAsync(TravelDto travel);
        Task<TravelDto> UpdateTravelAsync(TravelDto travel);
        Task<bool> DeleteTravelAsync(int travelId);

        // Destination CRUD
        Task<List<DestinationDto>> GetDestinationsAsync(int travelId);
        Task<DestinationDto> CreateDestinationAsync(DestinationDto destination);
        Task<DestinationDto> UpdateDestinationAsync(DestinationDto destination);
        Task<bool> DeleteDestinationAsync(int destinationId);

        // Activity CRUD
        Task<List<ActivityDto>> GetActivitiesAsync(int travelId);
        Task<ActivityDto> CreateActivityAsync(ActivityDto activity);
        Task<ActivityDto> UpdateActivityAsync(ActivityDto activity);
        Task<bool> DeleteActivityAsync(int activityId);

        // Expense CRUD
        Task<List<ExpenseDto>> GetExpensesAsync(int travelId);
        Task<ExpenseDto> CreateExpenseAsync(ExpenseDto expense);
        Task<ExpenseDto> UpdateExpenseAsync(ExpenseDto expense);
        Task<bool> DeleteExpenseAsync(int expenseId);
    }
}