using Microsoft.EntityFrameworkCore;
using TravelService.Data;
using TravelService.Data.Entities;
using TravelPlanner.Contracts.Interfaces;
using TravelPlanner.Contracts.Models;

namespace TravelService.Services
{
    public class TravelManager : ITravelService
    {
        private readonly DbContextOptions<TravelDbContext> _dbOptions;

        public TravelManager(DbContextOptions<TravelDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        // ---------- Travel ----------

        public async Task<List<TravelDto>> GetAllTravelsAsync(int userId)
        {
            using var db = new TravelDbContext(_dbOptions);
            return await db.Travels
                .Where(t => t.UserId == userId)
                .Select(t => ToDto(t))
                .ToListAsync();
        }

        public async Task<TravelDto> GetTravelByIdAsync(int travelId)
        {
            using var db = new TravelDbContext(_dbOptions);
            var travel = await db.Travels.FindAsync(travelId);
            if (travel == null) throw new KeyNotFoundException("Putovanje nije pronađeno.");
            return ToDto(travel);
        }

        public async Task<TravelDto> CreateTravelAsync(TravelDto dto)
        {
            Validate(dto);

            using var db = new TravelDbContext(_dbOptions);
            var travel = new Travel
            {
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Budget = dto.Budget,
                Notes = dto.Notes,
                UserId = dto.UserId
            };

            db.Travels.Add(travel);
            await db.SaveChangesAsync();
            return ToDto(travel);
        }

        public async Task<TravelDto> UpdateTravelAsync(TravelDto dto)
        {
            Validate(dto);

            using var db = new TravelDbContext(_dbOptions);
            var travel = await db.Travels.FindAsync(dto.Id);
            if (travel == null) throw new KeyNotFoundException("Putovanje nije pronađeno.");

            travel.Name = dto.Name;
            travel.Description = dto.Description;
            travel.StartDate = dto.StartDate;
            travel.EndDate = dto.EndDate;
            travel.Budget = dto.Budget;
            travel.Notes = dto.Notes;

            await db.SaveChangesAsync();
            return ToDto(travel);
        }

        public async Task<bool> DeleteTravelAsync(int travelId)
        {
            using var db = new TravelDbContext(_dbOptions);
            var travel = await db.Travels.FindAsync(travelId);
            if (travel == null) return false;

            db.Travels.Remove(travel); // cascade briše Destinations/Activities/Expenses u bazi
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<int> DeleteTravelsByUserIdAsync(int userId)
        {
            using var db = new TravelDbContext(_dbOptions);
            var travels = await db.Travels.Where(t => t.UserId == userId).ToListAsync();
            db.Travels.RemoveRange(travels); // SQL cascade i dalje briše Destinations/Activities/Expenses/Checklist/ShareTokens
            await db.SaveChangesAsync();
            return travels.Count;
        }

        // ---------- Destination ----------

        public async Task<List<DestinationDto>> GetDestinationsAsync(int travelId)
        {
            using var db = new TravelDbContext(_dbOptions);
            return await db.Destinations.Where(d => d.TravelId == travelId).Select(d => ToDto(d)).ToListAsync();
        }

        public async Task<DestinationDto> CreateDestinationAsync(DestinationDto dto)
        {
            using var db = new TravelDbContext(_dbOptions);
            var entity = new Destination
            {
                Name = dto.Name,
                Location = dto.Location,
                ArrivalDate = dto.ArrivalDate,
                DepartureDate = dto.DepartureDate,
                Description = dto.Description,
                TravelId = dto.TravelId
            };
            db.Destinations.Add(entity);
            await db.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<DestinationDto> UpdateDestinationAsync(DestinationDto dto)
        {
            using var db = new TravelDbContext(_dbOptions);
            var entity = await db.Destinations.FindAsync(dto.Id);
            if (entity == null) throw new KeyNotFoundException("Destinacija nije pronađena.");

            entity.Name = dto.Name; entity.Location = dto.Location;
            entity.ArrivalDate = dto.ArrivalDate; entity.DepartureDate = dto.DepartureDate;
            entity.Description = dto.Description;

            await db.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<bool> DeleteDestinationAsync(int destinationId)
        {
            using var db = new TravelDbContext(_dbOptions);
            var entity = await db.Destinations.FindAsync(destinationId);
            if (entity == null) return false;
            db.Destinations.Remove(entity);
            await db.SaveChangesAsync();
            return true;
        }

        // ---------- Activity ----------

        public async Task<List<ActivityDto>> GetActivitiesAsync(int travelId)
        {
            using var db = new TravelDbContext(_dbOptions);
            return await db.Activities.Where(a => a.TravelId == travelId).Select(a => ToDto(a)).ToListAsync();
        }

        public async Task<ActivityDto> CreateActivityAsync(ActivityDto dto)
        {
            using var db = new TravelDbContext(_dbOptions);
            var entity = new Activity
            {
                Name = dto.Name,
                Date = dto.Date,
                Time = dto.Time,
                Location = dto.Location,
                Description = dto.Description,
                EstimatedCost = dto.EstimatedCost,
                Status = dto.Status ?? "PLANNED",
                TravelId = dto.TravelId
            };
            db.Activities.Add(entity);
            await db.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<ActivityDto> UpdateActivityAsync(ActivityDto dto)
        {
            using var db = new TravelDbContext(_dbOptions);
            var entity = await db.Activities.FindAsync(dto.Id);
            if (entity == null) throw new KeyNotFoundException("Aktivnost nije pronađena.");

            entity.Name = dto.Name; entity.Date = dto.Date; entity.Time = dto.Time;
            entity.Location = dto.Location; entity.Description = dto.Description;
            entity.EstimatedCost = dto.EstimatedCost; entity.Status = dto.Status;

            await db.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<bool> DeleteActivityAsync(int activityId)
        {
            using var db = new TravelDbContext(_dbOptions);
            var entity = await db.Activities.FindAsync(activityId);
            if (entity == null) return false;
            db.Activities.Remove(entity);
            await db.SaveChangesAsync();
            return true;
        }

        // ---------- Expense ----------

        public async Task<List<ExpenseDto>> GetExpensesAsync(int travelId)
        {
            using var db = new TravelDbContext(_dbOptions);
            return await db.Expenses.Where(e => e.TravelId == travelId).Select(e => ToDto(e)).ToListAsync();
        }

        public async Task<ExpenseDto> CreateExpenseAsync(ExpenseDto dto)
        {
            using var db = new TravelDbContext(_dbOptions);
            var entity = new Expense
            {
                Name = dto.Name,
                Category = dto.Category,
                Amount = dto.Amount,
                Date = dto.Date,
                Description = dto.Description,
                TravelId = dto.TravelId
            };
            db.Expenses.Add(entity);
            await db.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<ExpenseDto> UpdateExpenseAsync(ExpenseDto dto)
        {
            using var db = new TravelDbContext(_dbOptions);
            var entity = await db.Expenses.FindAsync(dto.Id);
            if (entity == null) throw new KeyNotFoundException("Trošak nije pronađen.");

            entity.Name = dto.Name; entity.Category = dto.Category; entity.Amount = dto.Amount;
            entity.Date = dto.Date; entity.Description = dto.Description;

            await db.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<bool> DeleteExpenseAsync(int expenseId)
        {
            using var db = new TravelDbContext(_dbOptions);
            var entity = await db.Expenses.FindAsync(expenseId);
            if (entity == null) return false;
            db.Expenses.Remove(entity);
            await db.SaveChangesAsync();
            return true;
        }

        // ---------- Validacija (zadatak: end >= start, budget >= 0) ----------

        private static void Validate(TravelDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                throw new ArgumentException("Krajnji datum ne može biti pre početnog datuma.");
            if (dto.Budget < 0)
                throw new ArgumentException("Budžet ne može biti negativan.");
        }

        // ---------- Mapiranje Entity -> DTO ----------

        private static TravelDto ToDto(Travel t) => new TravelDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            StartDate = t.StartDate,
            EndDate = t.EndDate,
            Budget = t.Budget,
            Notes = t.Notes,
            UserId = t.UserId
        };

        private static DestinationDto ToDto(Destination d) => new DestinationDto
        {
            Id = d.Id,
            Name = d.Name,
            Location = d.Location,
            ArrivalDate = d.ArrivalDate,
            DepartureDate = d.DepartureDate,
            Description = d.Description,
            TravelId = d.TravelId
        };

        private static ActivityDto ToDto(Activity a) => new ActivityDto
        {
            Id = a.Id,
            Name = a.Name,
            Date = a.Date,
            Time = a.Time,
            Location = a.Location,
            Description = a.Description,
            EstimatedCost = a.EstimatedCost,
            Status = a.Status,
            TravelId = a.TravelId
        };

        private static ExpenseDto ToDto(Expense e) => new ExpenseDto
        {
            Id = e.Id,
            Name = e.Name,
            Category = e.Category,
            Amount = e.Amount,
            Date = e.Date,
            Description = e.Description,
            TravelId = e.TravelId
        };

        public async Task<BudgetSummaryDto> GetBudgetSummaryAsync(int travelId)
        {
            using var db = new TravelDbContext(_dbOptions);
            var travel = await db.Travels.FindAsync(travelId);
            if (travel == null) throw new KeyNotFoundException("Putovanje nije pronađeno.");

            var expenses = await db.Expenses.Where(e => e.TravelId == travelId).ToListAsync();
            var totalSpent = expenses.Sum(e => e.Amount);

            return new BudgetSummaryDto
            {
                TravelId = travelId,
                TotalBudget = travel.Budget,
                TotalSpent = totalSpent,
                RemainingBudget = travel.Budget - totalSpent,
                ByCategory = expenses.GroupBy(e => e.Category)
                    .Select(g => new CategorySummaryDto { Category = g.Key, Total = g.Sum(e => e.Amount) })
                    .ToList()
            };
        }

        // ---------- Checklist CRUD ----------

        public async Task<List<ChecklistItemDto>> GetChecklistAsync(int travelId)
        {
            using var db = new TravelDbContext(_dbOptions);
            return await db.ChecklistItems.Where(c => c.TravelId == travelId)
                .Select(c => new ChecklistItemDto { Id = c.Id, Name = c.Name, IsCompleted = c.IsCompleted, TravelId = c.TravelId })
                .ToListAsync();
        }

        public async Task<ChecklistItemDto> CreateChecklistItemAsync(ChecklistItemDto dto)
        {
            using var db = new TravelDbContext(_dbOptions);
            var entity = new ChecklistItem { Name = dto.Name, TravelId = dto.TravelId, IsCompleted = false };
            db.ChecklistItems.Add(entity);
            await db.SaveChangesAsync();
            return new ChecklistItemDto { Id = entity.Id, Name = entity.Name, IsCompleted = entity.IsCompleted, TravelId = entity.TravelId };
        }

        public async Task<ChecklistItemDto> ToggleChecklistItemAsync(int id)
        {
            using var db = new TravelDbContext(_dbOptions);
            var entity = await db.ChecklistItems.FindAsync(id);
            if (entity == null) throw new KeyNotFoundException("Stavka nije pronađena.");
            entity.IsCompleted = !entity.IsCompleted;
            await db.SaveChangesAsync();
            return new ChecklistItemDto { Id = entity.Id, Name = entity.Name, IsCompleted = entity.IsCompleted, TravelId = entity.TravelId };
        }

        public async Task<bool> DeleteChecklistItemAsync(int id)
        {
            using var db = new TravelDbContext(_dbOptions);
            var entity = await db.ChecklistItems.FindAsync(id);
            if (entity == null) return false;
            db.ChecklistItems.Remove(entity);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<ShareTokenDto> CreateShareTokenAsync(CreateShareTokenDto dto)
        {
            using var db = new TravelDbContext(_dbOptions);
            var entity = new Data.Entities.ShareToken
            {
                Token = Guid.NewGuid().ToString("N"),
                TravelId = dto.TravelId,
                AccessType = dto.AccessType,
                ExpiresAt = DateTime.UtcNow.AddDays(dto.ExpiresInDays),
                IsActive = true
            };
            db.ShareTokens.Add(entity);
            await db.SaveChangesAsync();
            return ToShareDto(entity);
        }

        public async Task<TravelDto> GetTravelByShareTokenAsync(string token)
        {
            using var db = new TravelDbContext(_dbOptions);
            var share = await ValidTokenAsync(db, token);
            var travel = await db.Travels.FindAsync(share.TravelId);
            return ToDto(travel);
        }

        public async Task<TravelDto> UpdateTravelByShareTokenAsync(string token, TravelDto dto)
        {
            using var db = new TravelDbContext(_dbOptions);
            var share = await ValidTokenAsync(db, token);
            if (share.AccessType != "EDIT")
                throw new UnauthorizedAccessException("Ovaj link dozvoljava samo pregled.");

            var travel = await db.Travels.FindAsync(share.TravelId);
            Validate(dto);
            travel.Name = dto.Name; travel.Description = dto.Description;
            travel.StartDate = dto.StartDate; travel.EndDate = dto.EndDate;
            travel.Budget = dto.Budget; travel.Notes = dto.Notes;

            await db.SaveChangesAsync();
            return ToDto(travel);
        }

        public async Task<bool> RevokeShareTokenAsync(string token)
        {
            using var db = new TravelDbContext(_dbOptions);
            var share = await db.ShareTokens.SingleOrDefaultAsync(s => s.Token == token);
            if (share == null) return false;
            share.IsActive = false;
            await db.SaveChangesAsync();
            return true;
        }

        private static async Task<Data.Entities.ShareToken> ValidTokenAsync(TravelDbContext db, string token)
        {
            var share = await db.ShareTokens.SingleOrDefaultAsync(s => s.Token == token);
            if (share == null || !share.IsActive || share.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Link za deljenje nije važeći ili je istekao.");
            return share;
        }

        private static ShareTokenDto ToShareDto(Data.Entities.ShareToken s) => new ShareTokenDto
        {
            Id = s.Id,
            Token = s.Token,
            TravelId = s.TravelId,
            AccessType = s.AccessType,
            ExpiresAt = s.ExpiresAt,
            IsActive = s.IsActive
        };

        public async Task<ShareTokenDto> GetShareTokenInfoAsync(string token)
        {
            using var db = new TravelDbContext(_dbOptions);
            var share = await ValidTokenAsync(db, token);
            return ToShareDto(share);
        }

        // ---------- Checklist CRUD ----------
        public async Task<List<TravelDto>> GetAllTravelsForAdminAsync()
        {
            using var db = new TravelDbContext(_dbOptions);
            return await db.Travels.Select(t => ToDto(t)).ToListAsync();
        }

    }
}