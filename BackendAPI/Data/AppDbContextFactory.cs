using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BackendAPI.Data
{
    // Ova klasa se koristi SAMO u "design-time" trenutku (kad EF alat
    // treba da napravi/primeni migraciju). Ona zaobilazi Service Fabric
    // Program.cs (koji pokušava da se poveže na klaster) i daje EF alatu
    // AppDbContext direktno, sa istim connection stringom kao u appsettings.json.
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=localhost\\SQLEXPRESS;Database=TravelPlannerDB;Trusted_Connection=True;TrustServerCertificate=True;"
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}