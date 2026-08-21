using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;
using TravelPlanner.Contracts.Models;

namespace TravelPlanner.Contracts.Interfaces
{
    public interface IAuthService : IService
    {
        Task<UserDto> RegisterAsync(RegisterRequestDto request);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int userId);
    }
}