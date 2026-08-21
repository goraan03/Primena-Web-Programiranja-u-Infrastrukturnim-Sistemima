using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Data.Entities;
using TravelPlanner.Contracts.Interfaces;
using TravelPlanner.Contracts.Models;

namespace AuthService.Services
{
    public class AuthManager : IAuthService
    {
        private readonly DbContextOptions<AuthDbContext> _dbOptions;
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthManager(DbContextOptions<AuthDbContext> dbOptions, JwtSettings jwtSettings)
        {
            _dbOptions = dbOptions;
            _tokenGenerator = new JwtTokenGenerator(jwtSettings);
        }

        public async Task<UserDto> RegisterAsync(RegisterRequestDto request)
        {
            using var db = new AuthDbContext(_dbOptions);

            var exists = await db.Users.AnyAsync(u => u.Email == request.Email);
            if (exists)
                throw new InvalidOperationException("Email je već registrovan.");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "USER"
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return MapToDto(user);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            using var db = new AuthDbContext(_dbOptions);

            var user = await db.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Pogrešan email ili lozinka.");

            var token = _tokenGenerator.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                User = MapToDto(user)
            };
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            using var db = new AuthDbContext(_dbOptions);

            var user = await db.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            return MapToDto(user);
        }

        private static UserDto MapToDto(User user) => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            using var db = new AuthDbContext(_dbOptions);
            return await db.Users.Select(u => new UserDto { Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role }).ToListAsync();
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            using var db = new AuthDbContext(_dbOptions);
            var user = await db.Users.FindAsync(userId);
            if (user == null) return false;
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return true;
        }
    }
}