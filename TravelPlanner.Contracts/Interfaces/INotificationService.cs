using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace TravelPlanner.Contracts.Interfaces
{
    public interface INotificationService : IService
    {
        Task SendWelcomeEmailAsync(string email, string name);
        Task SendTravelCreatedEmailAsync(string email, string travelName);
    }
}