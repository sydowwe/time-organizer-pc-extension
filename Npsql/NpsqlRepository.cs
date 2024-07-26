using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ActivityLookUp.DTO;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace ActivityLookUp.Npsql
{
    public class NpsqlRepository : DbContext
    {
        private readonly string _connectionString;
        private readonly ILogger<NpsqlRepository> _logger;

        // Constructor with DI
        public NpsqlRepository(IServiceProvider Services, ILogger<NpsqlRepository> logger)
        {
            _logger = logger;
            _connectionString = Services.GetRequiredService<AppSettings>().ConnectionString;

            if (string.IsNullOrEmpty(_connectionString))
            {
                this._logger.LogError("Connection string is not provided.");
                throw new Exception("Connection string is not provided.");
            }
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<PCActivity> PCActivities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }

        public async Task<List<PCActivity>> GetPCActivitiesAsync()
        {
            return await PCActivities.ToListAsync();
        }

        public async Task AddPCActivityAsync(PCActivity activity)
        {
            await PCActivities.AddAsync(activity);
            await SaveChangesAsync();
        }

        public async Task UpdatePCActivityAsync(PCActivity activity)
        {
            PCActivities.Update(activity);
            await SaveChangesAsync();
        }

        public async Task<bool> IdExists(string? name, string? title, DateTime start)
        {
            return await PCActivities.AnyAsync(p => p.ProcessName == name && p.MainWindowTitle == title && p.StartTimestamp.Equals(start));
        }

        public async Task<PCActivity> GetId(string? name, string? title, DateTime start)
        {
            return await PCActivities
                .Where(p => p.ProcessName == name
                         && p.MainWindowTitle == title
                         && p.StartTimestamp == start)
                .FirstAsync(); 
        }



        public async Task DeletePCActivityAsync(int id)
        {
            var activity = await PCActivities.FindAsync(id);
            if (activity != null)
            {
                PCActivities.Remove(activity);
                await SaveChangesAsync();
            }
        }
    }
}
