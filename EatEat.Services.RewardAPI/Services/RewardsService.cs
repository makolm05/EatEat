using System.Text;
using Microsoft.EntityFrameworkCore;
using EatEat.Services.RewardsAPI.Data;
using EatEat.Services.RewardsAPI.Services.Interfaces;
using EatEat.Services.RewardAPI.Messages;
using EatEat.Services.RewardAPI.Models;

namespace EatEat.Services.RewardsAPI.Services
{
    public class RewardsService : IRewardsService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public RewardsService(DbContextOptions<AppDbContext> options)
        {
            _dbOptions = options;
        }

        public async Task UpdateRewards(RewardsMessage message)
        {
            var rewards = new Rewards()
            {
                OrderID = message.OrderID,
                UserID = message.UserID,
                RewardsActivity = message.RewardsActivity,
                RewardsDate = DateTime.Now
            };

            await using var _db = new AppDbContext(_dbOptions);
            _db.Rewards.Add(rewards);
            await _db.SaveChangesAsync();
        }
    }
}