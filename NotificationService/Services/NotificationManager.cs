using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using TravelPlanner.Contracts.Interfaces;

namespace NotificationService.Services
{
    public class NotificationManager : INotificationService
    {
        private readonly IReliableStateManager _stateManager;

        public NotificationManager(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public Task SendWelcomeEmailAsync(string email, string name)
        {
            return StoreNotificationAsync(email, $"Dobrodošli, {name}! Vaš nalog je uspešno kreiran.");
        }

        public Task SendTravelCreatedEmailAsync(string email, string travelName)
        {
            return StoreNotificationAsync(email, $"Vaše putovanje '{travelName}' je uspešno kreirano.");
        }

        private async Task StoreNotificationAsync(string email, string message)
        {
            var notifications = await _stateManager
                .GetOrAddAsync<IReliableDictionary<string, string>>("notifications");

            var key = $"{email}_{DateTime.UtcNow:O}_{Guid.NewGuid():N}";

            using var tx = _stateManager.CreateTransaction();
            await notifications.AddOrUpdateAsync(tx, key, message, (k, v) => message);
            await tx.CommitAsync();
        }
    }
}